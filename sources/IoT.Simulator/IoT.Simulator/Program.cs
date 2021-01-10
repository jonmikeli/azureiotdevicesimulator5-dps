using CommandLine;

using IoT.Simulator.Exceptions;
using IoT.Simulator.Services;
using IoT.Simulator.Settings;
using IoT.Simulator.Settings.DPS;
using IoT.Simulator.Tools;

using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IoT.Simulator
{
    class Program
    {
        // The device connection string to authenticate the device with your IoT hub.
        // Using the Azure CLI:
        // az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyDotnetDevice --output table
        private static string _environmentName;

        public static IConfiguration Configuration { get; set; }

        private static async Task Main(string[] args)
        {
            Console.WriteLine("=======================================================================");
            Console.WriteLine(AssemblyInformationHelper.HeaderMessage);
            Console.WriteLine("=======================================================================");
            Console.WriteLine(">> Loading configurations....");

            try
            {
                //Loading environment related settings
                _environmentName = Environment.GetEnvironmentVariable("ENVIRONMENT");

                // Environment device Id check
                await CheckEnvironmentDeviceId(_environmentName);

                //Configuration
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("devicesettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("modulessettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("dpssettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
                
                if (string.IsNullOrWhiteSpace(_environmentName))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("No environment platform has been found. Default setting: Development.");
                    _environmentName = "Development";
                    Console.ResetColor();
                }

                try
                {
                    ConfigurationHelpers.CheckEnvironmentConfigurationFiles(_environmentName);

                    builder.AddJsonFile($"appsettings.{_environmentName}.json", optional: true, reloadOnChange: true);
                    builder.AddJsonFile($"devicesettings.{_environmentName}.json", optional: true, reloadOnChange: true);
                    builder.AddJsonFile($"modulessettings.{_environmentName}.json", optional: true, reloadOnChange: true);
                    builder.AddJsonFile($"dpssettings.{_environmentName}.json", optional: true, reloadOnChange: true);

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"Environment related files loaded for: '{_environmentName}'.");
                    Console.ResetColor();
                }
                catch (MissingEnvironmentConfigurationFileException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                    Console.WriteLine("Execution will continue with default settings in appsettings.json, devicesettings.json and modulessettings.json.");
                }

                Configuration = builder.Build();

                //Service provider and DI
                IServiceCollection services = new ServiceCollection();
                ConfigureServices(services);

                //DPS and provisioning
                LoadDPSandProvisioningSettings(services, Configuration, _environmentName);

                //Device  related settings
                var deviceSettings = Configuration.Get<DeviceSettings>();
                if (deviceSettings == null)
                    throw new ArgumentException("No device settings have been configured.");

                if (string.IsNullOrEmpty(deviceSettings.DeviceId))
                    throw new ArgumentException("No device id has been found.");

                if (deviceSettings.SimulationSettings == null)
                    throw new ArgumentException("No device simulation settings have been configured.");

                if (deviceSettings.SimulationSettings.EnableDevice || deviceSettings.SimulationSettings.EnableModules)
                    //If any of the simulators is enabled, messaging services will be required to build the messages.
                    RegisterMessagingServices(services);

                if (deviceSettings.SimulationSettings.EnableDevice)
                    RegisterDeviceSimulators(services);

                if (deviceSettings.SimulationSettings.EnableModules)
                    RegisterModuleSimulators(deviceSettings, services);

                IServiceProvider serviceProvider = services.BuildServiceProvider();

                //Logger
                var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
                logger.LogDebug("PROGRAM::Settings, DI and logger configured and ready to use.");

                //Simulators
                if (!deviceSettings.SimulationSettings.EnableDevice && !deviceSettings.SimulationSettings.EnableModules)
                    logger.LogDebug("PROGRAM::No simulator has been configured.");
                else
                {
                    if (deviceSettings.SimulationSettings.EnableDevice)
                        await StartDevicesSimulatorsAsync(serviceProvider, logger);

                    if (deviceSettings.SimulationSettings.EnableModules)
                        StartModulesSimulators(serviceProvider, logger);
                }

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            finally
            {
                Console.ReadLine();
            }
        }


        #region Private methods
        #region DPS
        /// <summary>
        /// Checks and loads the DPS settings, as .NET IOptions.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="args"></param>
        /// <param name="_environmentName"></param>
        private static void LoadDPSandProvisioningSettings(IServiceCollection services, IConfiguration configuration, string _environmentName)
        {
            DPSSettings dpsEnvironmentSettings = LoadDPSOptionsFromEnvironmentVariables();
            DPSSettings dpsCommandSettings = LoadDPSOptionsFromCommandParameters();

            //Overwriting process in among environment, command and file settings
            DPSSettings dpsSettings = dpsEnvironmentSettings;
            if (dpsCommandSettings != null)
                dpsSettings = dpsCommandSettings;

            if (dpsSettings == null)
            {
                //WARNING: it seems that IOptions do not work properly with default deserializers
                string dpsSettingsJson = File.ReadAllText($"dpssettings.json");
                if (File.Exists($"dpssettings.{_environmentName}.json"))
                    dpsSettingsJson = File.ReadAllText($"dpssettings.{ _environmentName}.json");

                if (!string.IsNullOrEmpty(dpsSettingsJson))
                {
                    JObject jData = JObject.Parse(dpsSettingsJson);

                    if (jData != null && jData.ContainsKey(DPSSettings.DPSSettingsSection))
                    {
                        dpsSettings = jData[DPSSettings.DPSSettingsSection].ToObject<DPSSettings>();
                    }
                }
            }

            //IF any DPS setting is found, it is bound as .NET IOptions.
            if (dpsSettings != null)
            {
                var dpsSettingsOptions = Options.Create(dpsSettings);
                services.AddSingleton(dpsSettingsOptions);
                configuration.Bind(DPSSettings.DPSSettingsSection, dpsSettingsOptions);
            }
            else
                throw new Exception("No DPS settings have been provided (Environment variables, command parameters or settings files).");            
        }

        private static async Task CheckEnvironmentDeviceId(string environmentName)
        {
            //Overwrite the device Id if it is provided by the environment variable
            string deviceId = Environment.GetEnvironmentVariable("PROVISIONING_REGISTRATION_ID");
            if (!string.IsNullOrEmpty(deviceId))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"DeviceId set by environment variables: {deviceId}");


                List<Task> tasks = new List<Task>();
                tasks.Add(UpdateDeviceIdInDeviceSettings(deviceId, environmentName));
                tasks.Add(ClearDeviceIdInModulesSettings(environmentName));

                await Task.WhenAll(tasks);

                Console.ResetColor();
            }
        }

        private static async Task  UpdateDeviceIdInDeviceSettings(string deviceId, string environmentName)
        {
            //Get the device settings related Options
            string deviceSettingsFilePath = "devicesettings.json";
            if (!string.IsNullOrEmpty(environmentName))
                deviceSettingsFilePath = $"devicesettings.{environmentName}.json";

            if (!File.Exists(deviceSettingsFilePath))
                throw new Exception("Device settings path not found to update the device id coming from the environment settings.");

            string settingsData = File.ReadAllText(deviceSettingsFilePath);
            if (!string.IsNullOrEmpty(settingsData))
            {
                DeviceSettings settings = JsonConvert.DeserializeObject<DeviceSettings>(settingsData);

                if (settings != null)
                {
                    Console.WriteLine($"Updating the settings with the new device id: {deviceId}.");

                    settings.DeviceId = deviceId;
                    settings.ConnectionString = string.Empty;
                    await ConfigurationHelpers.WriteDeviceSettings(settings, environmentName);

                    Console.WriteLine($"Settings updated with the new device id: {deviceId}.");
                }
            }
        }

        private static async Task ClearDeviceIdInModulesSettings(string environmentName)
        {
            //Get the device settings related Options
            string modulesSettingsFilePath = "modulessettings.json";
            if (!string.IsNullOrEmpty(environmentName))
                modulesSettingsFilePath = $"modulessettings.{environmentName}.json";

            if (!File.Exists(modulesSettingsFilePath))
                throw new Exception("Modules settings path not found to clear the device id related data.");

            string settingsData = File.ReadAllText(modulesSettingsFilePath);
            if (!string.IsNullOrEmpty(settingsData))
            {
                ModulesSettings modulesSettings = JsonConvert.DeserializeObject<ModulesSettings>(settingsData);

                if (modulesSettings != null && modulesSettings.Modules != null && modulesSettings.Modules.Count > 0)
                {
                    foreach (var item in modulesSettings.Modules)
                    {
                        item.DeviceId = string.Empty;
                        item.ConnectionString = string.Empty;                        
                    }

                    Console.WriteLine($"Clearing modules settings configuration file.");
                    await ConfigurationHelpers.WriteModulesSettings(modulesSettings, environmentName);
                    Console.WriteLine($"Modules settings cleared.");
                }
            }
        }
        #endregion

        #region Console and environment parameters
        /// <summary>
        /// Parses console/command parameters.
        /// </summary>
        static DPSCommandParametersBase ParseCommandParameters(string[] args)
        {
            //Load the parameters and put them as environment variables (environment variables will always be of higher priority
            // Parse application parameters
            DPSCommandParametersBase parameters = null;

            if (args != null && args.Length > 4)
            {
                ParserResult<DPSCommandParametersBase> result = Parser.Default.ParseArguments<DPSCommandParametersBase>(args)
                    .WithParsed(parsedParams =>
                    {
                        parameters = parsedParams;
                    })
                    .WithNotParsed(errors =>
                    {
                        Environment.Exit(1);
                    });
            }

            return parameters;
        }

        /// <summary>
        /// Parses and types command parameters.
        /// </summary>
        /// <returns></returns>
        static DPSSettings LoadDPSOptionsFromCommandParameters()
        {
            DPSSettings settings = null;

            DPSCommandParametersBase parameters = ParseCommandParameters(Environment.GetCommandLineArgs());
            if (parameters != null && !string.IsNullOrEmpty(parameters.IdScope) && !string.IsNullOrEmpty(parameters.PrimaryKey))
            {
                settings = new DPSSettings();
                settings.EnrollmentType = EnrollmentType.Group;
                settings.GroupEnrollment = new GroupEnrollmentSettings();
                settings.GroupEnrollment.SecurityType = SecurityType.SymetricKey;

                settings.GroupEnrollment.SymetricKeySettings = new DPSSymmetricKeySettings();
                settings.GroupEnrollment.SymetricKeySettings.TransportType = TransportType.Mqtt;
                settings.GroupEnrollment.SymetricKeySettings.EnrollmentType = EnrollmentType.Group;

                settings.GroupEnrollment.SymetricKeySettings.IdScope = parameters.IdScope;
                settings.GroupEnrollment.SymetricKeySettings.PrimaryKey = parameters.PrimaryKey;
            }

            return settings;
        }

        /// <summary>
        /// Gets DPS parameters from environment variables.
        /// </summary>
        static DPSSettings LoadDPSOptionsFromEnvironmentVariables()
        {
            DPSSettings settings = null;

            var localVariables = Environment.GetEnvironmentVariables();

            if (localVariables != null && localVariables.Count >= 2)
            {
                string idScope = Environment.GetEnvironmentVariable("DPS_IDSCOPE");
                string primarySymmetricKey = Environment.GetEnvironmentVariable("PRIMARY_SYMMETRIC_KEY");

                if (!string.IsNullOrEmpty(idScope) && !string.IsNullOrEmpty(primarySymmetricKey))
                {
                    settings = new DPSSettings();
                    settings.EnrollmentType = EnrollmentType.Group;
                    settings.GroupEnrollment = new GroupEnrollmentSettings();
                    settings.GroupEnrollment.SecurityType = SecurityType.SymetricKey;

                    settings.GroupEnrollment.SymetricKeySettings = new DPSSymmetricKeySettings();
                    settings.GroupEnrollment.SymetricKeySettings.TransportType = TransportType.Mqtt;
                    settings.GroupEnrollment.SymetricKeySettings.EnrollmentType = EnrollmentType.Group;

                    settings.GroupEnrollment.SymetricKeySettings.IdScope = Environment.GetEnvironmentVariable("DPS_IDSCOPE");
                    settings.GroupEnrollment.SymetricKeySettings.PrimaryKey = Environment.GetEnvironmentVariable("PRIMARY_SYMMETRIC_KEY");
                }
            }

            return settings;
        }
        #endregion

        #region Simulation services
        //logging
        //https://andrewlock.net/using-dependency-injection-in-a-net-core-console-application/
        static void ConfigureServices(IServiceCollection services)
        {
            if (services != null)
            {
                services.AddLogging(
                    loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();

                        //log level configuration
                        var loggingConfiguration = Configuration.GetSection("Logging");
                        loggingBuilder.AddConfiguration(loggingConfiguration);

                        if (_environmentName != "Production")
                        {
                            loggingBuilder.AddConsole();
                            loggingBuilder.AddDebug();
                        }
                    }
                    );

                services.AddOptions();

                //services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
                services.Configure<AppSettings>(Configuration);
                services.Configure<DeviceSettings>(Configuration);               
                services.Configure<ModulesSettings>(Configuration);
                services.Configure<DPSSettings>(Configuration.GetSection(DPSSettings.DPSSettingsSection));
            }
        }

        static void RegisterDeviceSimulators(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IProvisioningService, DPSProvisioningService>();
            services.AddSingleton<ISimulationService, DeviceSimulationService>();
        }

        static void RegisterMessagingServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddTransient<ITelemetryMessageService, SimpleTelemetryMessageService>();
            services.AddTransient<IErrorMessageService, SimpleErrorMessageService>();
            services.AddTransient<ICommissioningMessageService, SimpleCommissioningMessageService>();
        }

        static void RegisterModuleSimulators(DeviceSettings deviceSettings, IServiceCollection services)
        {
            if (deviceSettings == null)
                throw new ArgumentNullException(nameof(deviceSettings));

            if (deviceSettings.SimulationSettings == null)
                throw new ArgumentNullException("No device simulation configuration has been configured.");

            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (deviceSettings.SimulationSettings.EnableModules)
            {
                var modules = Configuration.Get<ModulesSettings>();
                if (modules != null && modules.Modules != null && modules.Modules.Any())
                {
                    IServiceProvider serviceProvider = services.BuildServiceProvider();
                    if (serviceProvider == null)
                        throw new ApplicationException("IServiceProvider has not been resolved.");

                    ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();

                    if (loggerFactory == null)
                        throw new ApplicationException("ILoggerFactory has not been resolved.");

                    foreach (var item in modules.Modules)
                    {
                        var simulator = new ModuleSimulationService(
                            item,
                            serviceProvider.GetService<ITelemetryMessageService>(),
                            serviceProvider.GetService<IErrorMessageService>(),
                            serviceProvider.GetService<ICommissioningMessageService>(),
                            serviceProvider.GetService<IProvisioningService>(),
                            loggerFactory);

                        services.AddSingleton<IModuleSimulationService, ModuleSimulationService>(iServiceProvider => simulator);
                    }
                }
            }
        }

        static async Task StartDevicesSimulatorsAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            var simulators = serviceProvider.GetServices<ISimulationService>();
            if (simulators != null && simulators.Any())
            {
                List<Task> tasksToBeAwaited = new List<Task>();
                foreach (var item in simulators)
                {
                    tasksToBeAwaited.Add(item.InitiateSimulationAsync());
                }

                await Task.WhenAll(tasksToBeAwaited);

                logger.LogDebug($"DEVICES: {simulators.Count()} device simulator(s) initialized and running.");
            }
        }

        static void StartModulesSimulators(IServiceProvider serviceProvider, ILogger logger)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            var simulators = serviceProvider.GetServices<IModuleSimulationService>();
            if (simulators != null && simulators.Any())
            {
                foreach (var item in simulators)
                {
                    item.InitiateSimulationAsync();
                }

                logger.LogDebug($"MODULES: {simulators.Count()} module simulator(s) initialized and running.");
            }
        }
        #endregion
        #endregion
    }
}

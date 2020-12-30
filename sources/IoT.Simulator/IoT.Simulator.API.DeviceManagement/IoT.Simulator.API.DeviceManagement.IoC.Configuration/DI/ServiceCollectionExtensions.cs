using AutoMapper;

using IoT.Simulator.API.DeviceManagement.API.Common.Settings;
using IoT.Simulator.API.DeviceManagement.IoC.Configuration.AutoMapper;
using IoT.Simulator.API.DeviceManagement.Services;
using IoT.Simulator.API.DeviceManagement.Services.Contracts;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Reflection;


namespace IoT.Simulator.API.DeviceManagement.IoC.Configuration.DI
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (services != null)
            {
                var appSettingsSection = configuration.GetSection(nameof(AppSettings));
                if (appSettingsSection == null)
                    throw new System.Exception("No appsettings section has been found");

                var appSettings = appSettingsSection.Get<AppSettings>();

                if (!appSettings.IsValid())
                    throw new Exception("No valid settings.");

                services.Configure<AppSettings>(appSettingsSection);

                services.AddTransient<IDeviceManagementService, DeviceManagementService>();
                services.AddTransient<IIoTHubC2DOperationsService, IoTHubC2DOperationsService>();
            }
        }

        public static void ConfigureMappings(this IServiceCollection services)
        {
            if (services != null)
            {
                //Automap settings
                services.AddAutoMapper(Assembly.GetExecutingAssembly());
                MappingConfigurationsHelper.ConfigureMapper();
            }
        }
    }
}

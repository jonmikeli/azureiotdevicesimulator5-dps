using IoT.Simulator.Extensions;
using IoT.Simulator.Settings;
using IoT.Simulator.Settings.DPS;
using IoT.Simulator.Tools;

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Simulator.Services
{
    public class DPSProvisioningServiceX509CA : IProvisioningService
    {
        private AppSettings _appSettings;
        private DPSSettings _dpsSettings;
        private IOptionsMonitor<DeviceSettings> _deviceSettingsDelegate;
        private readonly ILogger<DPSProvisioningService> _logger;

        public DPSProvisioningServiceX509CA(
            IOptions<AppSettings> appSettings,
            IOptions<DPSSettings> dpsSettings,
            IOptionsMonitor<DeviceSettings> deviceSettingsDelegate,
            ILoggerFactory loggerFactory)
        {
            if (appSettings == null)
                throw new ArgumentNullException(nameof(appSettings));

            if (deviceSettingsDelegate == null)
                throw new ArgumentNullException(nameof(deviceSettingsDelegate));

            if (dpsSettings == null)
                throw new ArgumentNullException(nameof(dpsSettings));

            if (dpsSettings.Value == null)
                throw new ArgumentNullException("dpsSettings.Value", "No device configuration has been loaded.");

            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory), "No logger factory has been provided.");

            _appSettings = appSettings.Value;
            _dpsSettings = dpsSettings.Value;
            _deviceSettingsDelegate = deviceSettingsDelegate;

            _logger = loggerFactory.CreateLogger<DPSProvisioningService>();

            string logPrefix = "system.dps.provisioning".BuildLogPrefix();
            _logger.LogDebug($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::Logger created.");
            _logger.LogDebug($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::DPS Provisioning service created.");
        }

        public async Task<string> ProvisionDevice()
        {
            string result = string.Empty;
            string logPrefix = "DPSProvisioningService.ProvisionDevice".BuildLogPrefix();

            // When registering with a symmetric key using a group enrollment, the provided key will not
            // work for a specific device, rather it must be computed based on two values: the group enrollment
            // key and the desired device Id.
            if (_dpsSettings.EnrollmentType == EnrollmentType.Group)
            {
                if (_dpsSettings.GroupEnrollment == null)
                    throw new ArgumentNullException("_dpsSettings.GroupEnrollment", "No group enrollment settings have been found.");

                if (_dpsSettings.GroupEnrollment.SecurityType == SecurityType.SymetricKey)
                    _dpsSettings.GroupEnrollment.SymetricKeySettings.PrimaryKey = ProvisioningTools.ComputeDerivedSymmetricKey(_dpsSettings.GroupEnrollment.SymetricKeySettings.PrimaryKey, _deviceSettingsDelegate.CurrentValue.DeviceId);
            }

            _logger.LogDebug($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::Initializing the device provisioning client...");

            // X509 device leaf certificate
            X509Certificate2 deviceLeafCertificate = new X509Certificate2(@"./X509/new-device.cert.pfx", "1234");

            using (var security = new SecurityProviderX509Certificate(deviceLeafCertificate))
            {
                using (var transportHandler = ProvisioningTools.GetTransportHandler(_dpsSettings.GroupEnrollment.SymetricKeySettings.TransportType))
                {
                    ProvisioningDeviceClient provClient = ProvisioningDeviceClient.Create(
                        _dpsSettings.GroupEnrollment.SymetricKeySettings.GlobalDeviceEndpoint,
                        _dpsSettings.GroupEnrollment.SymetricKeySettings.IdScope,
                        security,
                        transportHandler);

                    _logger.LogDebug($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::Initialized for registration Id {security.GetRegistrationID()}.");
                    _logger.LogDebug($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::Registering with the device provisioning service...");

                    DeviceRegistrationResult deviceRegistrationResult = await provClient.RegisterAsync();

                    if (deviceRegistrationResult != null)
                    {
                        _logger.LogDebug($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::Registration status: {deviceRegistrationResult.Status}.");
                        if (deviceRegistrationResult.Status != ProvisioningRegistrationStatusType.Assigned)
                        {
                            _logger.LogWarning($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::Device registration process did not assign an IoT Hub.");
                        }
                        else
                        {
                            var deviceAuthentificationCertificate = security.GetAuthenticationCertificate();
                            var deviceAuthenticationCertificateChain = security.GetAuthenticationCertificateChain();

                            _logger.LogDebug($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::Device {deviceRegistrationResult.DeviceId} registered to {deviceRegistrationResult.AssignedHub}.");
                            _logger.LogDebug($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::Creating X509 leaf authentication for IoT Hub...");                         

                            IAuthenticationMethod auth = new DeviceAuthenticationWithX509Certificate(deviceRegistrationResult.DeviceId, deviceAuthentificationCertificate);

                            _logger.LogDebug($"{logPrefix}::{deviceRegistrationResult.DeviceId}::Testing the provisioned device with IoT Hub...");

                            using (DeviceClient iotClient = DeviceClient.Create(deviceRegistrationResult.AssignedHub, auth, _dpsSettings.GroupEnrollment.SymetricKeySettings.TransportType))
                            {
                                _logger.LogDebug($"{logPrefix}::{deviceRegistrationResult.DeviceId}::Sending a telemetry message after provisioning to test the process...");

                                using var message = new Message(Encoding.UTF8.GetBytes("TestMessage"));
                                await iotClient.SendEventAsync(message);

                                _logger.LogDebug($"{logPrefix}::{deviceRegistrationResult.DeviceId}::Finished.");
                            }

                            result = $"HostName={deviceRegistrationResult.AssignedHub};DeviceId={deviceRegistrationResult.DeviceId};SharedAccessKey={devicePrimaryKey}";
                        }
                    }
                    else
                        _logger.LogError($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::No provisioning result has been received.");
                }
            }

            return result;
        }

        public async Task<string> AddModuleIdentityToDevice(string moduleId)
        {
            if (_appSettings.DeviceManagementServiceSettings == null)
                throw new ArgumentNullException("_appSettings.DeviceManagementServiceSettings");

            if (string.IsNullOrEmpty(_appSettings.DeviceManagementServiceSettings.BaseUrl))
                throw new ArgumentNullException("_appSettings.DeviceManagementServiceSettings.BaseUrl");

            if (string.IsNullOrEmpty(_appSettings.DeviceManagementServiceSettings.AddModulesToDeviceRoute))
                throw new ArgumentNullException("_appSettings.DeviceManagementServiceSettings.AddModulesToDeviceRoute");

            if (string.IsNullOrEmpty(moduleId))
                throw new ArgumentNullException(nameof(moduleId));

            string result = string.Empty;
            string logPrefix = "DPSProvisioningService.AddModuleIdentityToDevice".BuildLogPrefix();

            try
            {

                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    if (_appSettings.DeviceManagementServiceSettings.AllowAutosignedSSLCertificates)
                    {
                        //handler.ServerCertificateCustomValidationCallback = CertificateValidation;
                        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    }

                    using (var client = new HttpClient(handler))
                    {
                        handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

                        client.BaseAddress = new Uri(_appSettings.DeviceManagementServiceSettings.BaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        string jsonContent = JsonConvert.SerializeObject(new
                        {
                            deviceId = _deviceSettingsDelegate.CurrentValue.DeviceId,
                            moduleId = moduleId
                        });

                        HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(_appSettings.DeviceManagementServiceSettings.AddModulesToDeviceRoute, content);
                       
                        if (response != null)
                        {
                            response.EnsureSuccessStatusCode();

                            _logger.LogDebug($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::{moduleId}::Adding the module entity to the device...");

                            string resultContent = await response.Content.ReadAsStringAsync();

                            if (!string.IsNullOrEmpty(resultContent))
                            {
                                _logger.LogDebug($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::{moduleId}::Device Management service called. Getting registered module identity keys...");

                                JObject jData = JObject.Parse(resultContent);
                                string primaryKey = jData.Value<string>("authenticationPrimaryKey");

                                _logger.LogWarning($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::{moduleId}::Keys provided by the Device Management service.");

                                if (!string.IsNullOrEmpty(primaryKey))
                                {
                                    _logger.LogDebug($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::{moduleId}::Module identity added to device.");

                                    result = $"HostName={_deviceSettingsDelegate.CurrentValue.HostName};DeviceId={_deviceSettingsDelegate.CurrentValue.DeviceId};ModuleId={moduleId};SharedAccessKey={primaryKey}";
                                }
                                else
                                    _logger.LogError($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::{moduleId}::An error has occurred when getting the keys from the Device Management service.");

                            }
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::{moduleId}::{ex.Message}->InnerException::{ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{logPrefix}::{_deviceSettingsDelegate.CurrentValue.ArtifactId}::{moduleId}::{ex.Message}->InnerException::{ex.InnerException?.Message}");
            }

            return result;
        }

        private bool CertificateValidation(HttpRequestMessage requestMessage, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}

using IoT.Simulator.Extensions;
using IoT.Simulator.Settings;
using IoT.Simulator.Settings.DPS;
using IoT.Simulator.Tools;

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Simulator.Services
{
    public class DPSProvisioningService : IProvisioningService
    {
        private DPSSettings _dpsSettings;
        private DeviceSettings _deviceSettings;
        private readonly ILogger<DPSProvisioningService> _logger;

        public DPSProvisioningService(IOptions<DPSSettings> dpsSettings,
            IOptions<DeviceSettings> deviceSettings,
            ILoggerFactory loggerFactory)
        {
            if (deviceSettings == null)
                throw new ArgumentNullException(nameof(deviceSettings));

            if (deviceSettings.Value == null)
                throw new ArgumentNullException("deviceSettings.Value", "No device configuration has been loaded.");

            if (dpsSettings == null)
                throw new ArgumentNullException(nameof(dpsSettings));

            if (dpsSettings.Value == null)
                throw new ArgumentNullException("dpsSettings.Value", "No device configuration has been loaded.");

            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory), "No logger factory has been provided.");

            _dpsSettings = dpsSettings.Value;
            _deviceSettings = deviceSettings.Value;

            _logger = loggerFactory.CreateLogger<DPSProvisioningService>();

            string logPrefix = "system.dps.provisioning".BuildLogPrefix();
            _logger.LogDebug($"{logPrefix}::{_deviceSettings.ArtifactId}::Logger created.");
            _logger.LogDebug($"{logPrefix}::{_deviceSettings.ArtifactId}::DPS Provisioning service created.");
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
                    _dpsSettings.GroupEnrollment.SymetricKeySettings.PrimaryKey = ProvisioningTools.ComputeDerivedSymmetricKey(_dpsSettings.GroupEnrollment.SymetricKeySettings.PrimaryKey, _deviceSettings.DeviceId);
            }

            _logger.LogDebug($"{logPrefix}::{_deviceSettings.ArtifactId}::Initializing the device provisioning client...");

            // For individual enrollments, the first parameter must be the registration Id, where in the enrollment
            // the device Id is already chosen. However, for group enrollments the device Id can be requested by
            // the device, as long as the key has been computed using that value.
            // Also, the secondary could could be included, but was left out for the simplicity of this sample.
            using (var security = new SecurityProviderSymmetricKey(_deviceSettings.DeviceId,_dpsSettings.GroupEnrollment.SymetricKeySettings.PrimaryKey,null))
            {
                using (var transportHandler = ProvisioningTools.GetTransportHandler(_dpsSettings.GroupEnrollment.SymetricKeySettings.TransportType))
                {
                    ProvisioningDeviceClient provClient = ProvisioningDeviceClient.Create(
                        _dpsSettings.GroupEnrollment.SymetricKeySettings.GlobalDeviceEndpoint,
                        _dpsSettings.GroupEnrollment.SymetricKeySettings.IdScope,
                        security,
                        transportHandler);

                    _logger.LogDebug($"{logPrefix}::{_deviceSettings.ArtifactId}::Initialized for registration Id {security.GetRegistrationID()}.");
                    _logger.LogDebug($"{logPrefix}::{_deviceSettings.ArtifactId}::Registering with the device provisioning service...");

                    DeviceRegistrationResult deviceRegistrationResult = await provClient.RegisterAsync();

                    if (deviceRegistrationResult != null)
                    {
                        _logger.LogDebug($"{logPrefix}::{_deviceSettings.ArtifactId}::Registration status: {deviceRegistrationResult.Status}.");
                        if (deviceRegistrationResult.Status != ProvisioningRegistrationStatusType.Assigned)
                        {
                            _logger.LogWarning($"{logPrefix}::{_deviceSettings.ArtifactId}::Registration status did not assign a hub, so exiting this sample.");
                        }
                        else
                        {
                            _logger.LogDebug($"{logPrefix}::{_deviceSettings.ArtifactId}::Device {deviceRegistrationResult.DeviceId} registered to {deviceRegistrationResult.AssignedHub}.");
                            _logger.LogDebug($"{logPrefix}::{_deviceSettings.ArtifactId}::Creating symmetric key authentication for IoT Hub...");

                            var devicePrimaryKey = security.GetPrimaryKey();
                            //IAuthenticationMethod auth = new DeviceAuthenticationWithRegistrySymmetricKey(deviceRegistrationResult.DeviceId, devicePrimaryKey);

                            //_logger.LogDebug($"{logPrefix}::{_deviceSettings.ArtifactId}::Testing the provisioned device with IoT Hub...");

                            //using (DeviceClient iotClient = DeviceClient.Create(deviceRegistrationResult.AssignedHub, auth, _dpsSettings.GroupEnrollment.SymetricKeySettings.TransportType))
                            //{
                            //    _logger.LogDebug($"{logPrefix}::{_deviceSettings.ArtifactId}::Sending a telemetry message after provisioning to test the process...");

                            //    using var message = new Message(Encoding.UTF8.GetBytes("TestMessage"));
                            //    await iotClient.SendEventAsync(message);

                            //    _logger.LogDebug($"{logPrefix}::{_deviceSettings.ArtifactId}::Finished.");
                            //}

                            result = $"HostName={deviceRegistrationResult.AssignedHub};DeviceId={deviceRegistrationResult.DeviceId};SharedAccessKey={devicePrimaryKey}";
                        }
                    }
                    else
                        _logger.LogError($"{logPrefix}::{_deviceSettings.ArtifactId}::No provisioning result has been received.");
                }
            }
           
            return result;
        }
    }
}

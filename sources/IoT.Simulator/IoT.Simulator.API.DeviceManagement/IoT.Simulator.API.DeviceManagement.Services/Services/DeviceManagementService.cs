using AutoMapper;

using IoT.Simulator.API.DeviceManagement.API.Common.Settings;
using IoT.Simulator.API.DeviceManagement.Services.Contracts;

using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using SIoT = IoT.Simulator.API.DeviceManagement.Services.Model.IoT;

namespace IoT.Simulator.API.DeviceManagement.Services
{
    public class DeviceManagementService : IDeviceManagementService
    {
        AppSettings _settings;
        RegistryManager _registryManager;
        private readonly IMapper _mapper;
        private ILogger<DeviceManagementService> _logger;

        public DeviceManagementService(IOptions<AppSettings> settings, IMapper mapper, ILogger<DeviceManagementService> logger)
        {
            _settings = settings?.Value;

            if (_settings != null && _settings.IsValid())
            {
                _registryManager = RegistryManager.CreateFromConnectionString(_settings.IoTHub?.ConnectionString);
            }
            else
                throw new Exception("AppSettings need to be reviewed");

            _mapper = mapper;
            _logger = logger;
        }

        ~DeviceManagementService()
        {
            if (_registryManager != null)
            {
                _registryManager.CloseAsync();
            }
        }

        #region Public methods
        #region Devices
        public async Task<SIoT.Device> GetDeviceAsync(string deviceId)
        {
            Device data = await GetIoTHubDeviceAsync(deviceId);

            if (data != null)
                return _mapper.Map<SIoT.Device>(data);
            else return null;
        }

        public async Task<SIoT.Device> AddDeviceAsync(string deviceId)
        {
            Device iotDevice = await AddDeviceToIoTHubAsync(deviceId);

            if (iotDevice != null)
                return _mapper.Map<SIoT.Device>(iotDevice);
            else
                return null;
        }

        public async Task<SIoT.Device> AddDeviceAsync(string deviceId, SIoT.DeviceIoTSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            Device device = await AddDeviceToIoTHubAsync(deviceId);

            bool twinUpdated = await UpdateDeviceSettings(deviceId, settings);

            if (device != null && twinUpdated)
                return _mapper.Map<SIoT.Device>(device);
            else
                throw new Exception("An error has occurred during the device creation or the twin updates.");
        }

        public async Task<SIoT.Device> AddDeviceWithTagsAsync(string deviceId, string jsonTwin)
        {
            Twin twin = new Twin(deviceId);
            twin.Tags = new TwinCollection(jsonTwin);

            BulkRegistryOperationResult result = await _registryManager.AddDeviceWithTwinAsync(new Device(deviceId), twin);

            if (result != null && result.IsSuccessful)
                return await GetDeviceAsync(deviceId);
            else
                throw new Exception(JsonConvert.SerializeObject(result.Errors, Formatting.Indented));
        }


        /// <summary>
        /// Add a device with strongly typed settings (could be usefull in some scenarios)
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task<SIoT.Device> AddDeviceWithTagsAsync(string deviceId, SIoT.DeviceIoTSettings settings)
        {
            TwinJsonConverter converter = new TwinJsonConverter();
            Twin twin = null;

            if (settings != null && settings.Twins != null)
            {
                twin = new Twin(deviceId);

                string jsonTags = JsonConvert.SerializeObject(settings.Twins, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                twin.Tags = new TwinCollection(jsonTags);
            }

            BulkRegistryOperationResult result = await _registryManager.AddDeviceWithTwinAsync(new Device(deviceId), twin);

            if (result != null && result.IsSuccessful)
                return await GetDeviceAsync(deviceId);
            else
                throw new Exception(JsonConvert.SerializeObject(result.Errors, Formatting.Indented));
        }

        public async Task<SIoT.Device> AddDeviceWithTwinAsync(string deviceId, Twin twin)
        {
            BulkRegistryOperationResult result = await _registryManager.AddDeviceWithTwinAsync(new Device(deviceId), twin);

            if (result != null && result.IsSuccessful)
                return await GetDeviceAsync(deviceId);
            else
                throw new Exception(JsonConvert.SerializeObject(result.Errors, Formatting.Indented));
        }

        public async Task<string> GetPrimaryKeyOrThumbprintFromDeviceAsync(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
                throw new ArgumentNullException("deviceId");

            string result = string.Empty;

            Device device = await _registryManager.GetDeviceAsync(deviceId);

            if (device != null)
            {
                switch (device.Authentication.Type)
                {
                    case AuthenticationType.Sas:
                        result = device.Authentication.SymmetricKey.PrimaryKey;
                        break;
                    case AuthenticationType.SelfSigned:
                        result = device.Authentication.X509Thumbprint.PrimaryThumbprint;
                        break;
                    case AuthenticationType.CertificateAuthority:
                        result = device.Authentication.X509Thumbprint.PrimaryThumbprint;
                        break;
                }
            }
            else
                return string.Empty;

            return result;
        }

        public async Task<string> GetSecondaryKeyOrThumbprintFromDeviceAsync(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
                throw new ArgumentNullException("deviceId");

            string result = string.Empty;

            Device device = await _registryManager.GetDeviceAsync(deviceId);

            if (device != null)
            {
                switch (device.Authentication.Type)
                {
                    case AuthenticationType.Sas:
                        result = device.Authentication.SymmetricKey.SecondaryKey;
                        break;
                    case AuthenticationType.SelfSigned:
                        result = device.Authentication.X509Thumbprint.SecondaryThumbprint;
                        break;
                    case AuthenticationType.CertificateAuthority:
                        result = device.Authentication.X509Thumbprint.SecondaryThumbprint;
                        break;
                }
            }
            else
                return string.Empty;

            return result;
        }

        public async Task<bool> RemoveDeviceAsync(string deviceId)
        {
            try
            {
                await _registryManager.RemoveDeviceAsync(deviceId);

                Device device = await _registryManager.GetDeviceAsync(deviceId);
                if (device != null)
                    return false;
                else
                    return true;
            }
            catch (DeviceNotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }

        }

        public async Task<SIoT.Device> DisableDeviceAsync(string deviceId)
        {
            Device device = await _registryManager.GetDeviceAsync(deviceId);

            if (device != null)
            {
                device.Status = DeviceStatus.Disabled;
                Device data = await _registryManager.UpdateDeviceAsync(device);
                if (data != null)
                    return _mapper.Map<SIoT.Device>(data);
                else
                    return null;
            }
            else
                return null;
        }

        public async Task<SIoT.Device> EnableDeviceAsync(string deviceId)
        {
            Device device = await _registryManager.GetDeviceAsync(deviceId);

            if (device != null)
            {
                device.Status = DeviceStatus.Enabled;
                Device data = await _registryManager.UpdateDeviceAsync(device);
                if (data != null)
                    return _mapper.Map<SIoT.Device>(data);
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// Usefull for updating Twins properties (either tags or desired/reported properties).
        /// This first version does not include desired/reported properties serialization/deserialization but it could be added easily (additional properties in settings classes)
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<bool> UpdateDeviceSettings(string deviceId, Model.IoT.DeviceIoTSettings options)
        {
            if (string.IsNullOrEmpty(deviceId))
                throw new ArgumentNullException("deviceId");

            if (options == null)
                throw new ArgumentNullException("options");

            if (options.Twins != null && options.Twins.Tags != null)
            {
                try
                {
                    Twin twin = await _registryManager.GetTwinAsync(deviceId);

                    if (twin != null)
                    {
                        string jsonTags = JsonConvert.SerializeObject(options.Twins, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented });
                        Twin updatedTwin = await _registryManager.UpdateTwinAsync(deviceId, jsonTags, twin.ETag);

                        return updatedTwin != null;
                    }
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw ex;
                }
            }
            else
                return false;
        }


        #region Get devices
        //JSON Improrvements:https://devblogs.microsoft.com/dotnet/try-the-new-system-text-json-apis/
        public async Task<JArray> GetDevicesAsync(int maxCount = 100)
        {
            return await GetDevicesAsync("select * from devices", maxCount);
        }

        public async Task<JArray> GetDevicesAsync(string query, int maxCount = 100)
        {
            return JArray.Parse(JsonConvert.SerializeObject(await GetDevicesAsJson(query, maxCount)));
        }

        private async Task<IEnumerable<JToken>> GetDevicesAsJson(string query, int maxCount = 100)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException(nameof(query));

            var iotQuery = _registryManager.CreateQuery(query, maxCount);

            List<JToken> data = new List<JToken>();

            IEnumerable<string> itemsToAdd = null;
            while (iotQuery.HasMoreResults)
            {
                itemsToAdd = await iotQuery.GetNextAsJsonAsync();
                if (itemsToAdd != null && itemsToAdd.Any())
                {
                    data.AddRange(itemsToAdd.Select(i => JToken.Parse(i)));
                    itemsToAdd = null;
                }
            }

            return data;
        }

        public async Task<JsonDocument> GetDevices2Async(int maxCount = 100)
        {
            return await GetDevices2Async("select * from devices", maxCount);
        }

        public async Task<JsonDocument> GetDevices2Async(string query, int maxCount = 100)
        {
            return JsonDocument.Parse(JsonConvert.SerializeObject(await GetDevicesAsJson(query, maxCount)));
        }
        #endregion
        #endregion

        #region Modules
        public async Task<SIoT.Module> AddModuleAsync(string deviceId, string moduleId)
        {
            Module iotModule = await AddModuleToDeviceAsync(deviceId, moduleId);

            if (iotModule != null)
                return _mapper.Map<SIoT.Module>(iotModule);
            else
                return null;
        }
        #endregion
        #endregion

        #region Private methods
        private async Task<Device> GetIoTHubDeviceAsync(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
                throw new ArgumentNullException("deviceId");

            return await _registryManager.GetDeviceAsync(deviceId);
        }

        private async Task<Device> AddDeviceToIoTHubAsync(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
                throw new ArgumentNullException("deviceId");

            Device device;
            try
            {
                device = await _registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                device = await _registryManager.GetDeviceAsync(deviceId);
            }

            return device;
        }

        private async Task<Module> AddModuleToDeviceAsync(string deviceId, string moduleId)
        {
            if (string.IsNullOrEmpty(deviceId))
                throw new ArgumentNullException(nameof(deviceId));

            if (string.IsNullOrEmpty(moduleId))
                throw new ArgumentNullException(nameof(moduleId));

            Module module;
            try
            {
                module = await _registryManager.AddModuleAsync(new Module(deviceId, moduleId));
            }
            catch (DeviceAlreadyExistsException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                module = await _registryManager.GetModuleAsync(deviceId, moduleId);
            }

            return module;
        }
        #endregion
    }
}

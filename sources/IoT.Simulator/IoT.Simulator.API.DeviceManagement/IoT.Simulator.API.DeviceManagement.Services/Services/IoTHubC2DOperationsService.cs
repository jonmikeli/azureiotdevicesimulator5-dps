using AutoMapper;

using IoT.Simulator.API.DeviceManagement.API.Common.Settings;
using IoT.Simulator.API.DeviceManagement.Services.Contracts;
using IoT.Simulator.API.DeviceManagement.Services.Model.IoT;

using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Options;

using System;
using System.Threading.Tasks;

using S = IoT.Simulator.API.DeviceManagement.Services.Model;

namespace IoT.Simulator.API.DeviceManagement.Services
{
    public class IoTHubC2DOperationsService : IIoTHubC2DOperationsService
    {
        RegistryManager _registryManager;
        ServiceClient _serviceClient;
        AppSettings _settings;

        readonly IMapper _mapper;

        public IoTHubC2DOperationsService(IOptions<AppSettings> settings, IMapper mapper)
        {
            _settings = settings?.Value;
            _mapper = mapper;

            if (_settings != null && _settings.IsValid())
            {
                _registryManager = RegistryManager.CreateFromConnectionString(_settings.IoTHub.ConnectionString);
                _serviceClient = ServiceClient.CreateFromConnectionString(_settings.IoTHub.ConnectionString);
            }
            else
                throw new Exception("AppSettings need to be reviewed");
        }

        ~IoTHubC2DOperationsService()
        {
            if (_registryManager != null)
            {
                _registryManager.CloseAsync();
            }

            if (_serviceClient != null)
            {
                _serviceClient.CloseAsync();
            }
        }

        #region Direct methods
        public async Task<string> InvokeDirectMethodAsync(string deviceId, string methodName, string payload)
        {
            try
            {
                var methodInvocation = new CloudToDeviceMethod(methodName) { ResponseTimeout = TimeSpan.FromSeconds((double)_settings.IoTHub?.DirectMethodTimeOut) };
                methodInvocation.SetPayloadJson(payload);

                // Invoke the direct method asynchronously and get the response from the simulated device.
                CloudToDeviceMethodResult response = await _serviceClient.InvokeDeviceMethodAsync(deviceId, methodInvocation);

                if (response != null)
                {
                    return response.GetPayloadAsJson();
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                //TODO: add trace
                return null;
            }
        }
        #endregion

        #region Twin
        public async Task<S.IoT.Twins> UpdateTwinsAsync(string deviceId, string patch)
        {
            Twin twin = await _registryManager.GetTwinAsync(deviceId);
            if (twin != null)
            {
                return await UpdateTwinsAsync(deviceId, twin.ETag, patch);
            }
            else
                return null;
        }

        public async Task<S.IoT.Twins> UpdateTwinsAsync(string deviceId, string etag, string patch)
        {
            var data = await _registryManager.UpdateTwinAsync(deviceId, patch, etag);

            if (data != null)
                return _mapper.Map<S.IoT.Twins>(data);
            else return null;
        }

        public async Task UpdateTwinsAsync(TwinsSearchRequest request, string patch)
        {
            var query = _registryManager.CreateQuery($"SELECT * FROM devices {request.WhereCondition}");

            if (query != null)
            {
                var twinsInScope = await query.GetNextAsTwinAsync();

                if (twinsInScope != null)
                {
                    foreach (var item in twinsInScope)
                    {
                        UpdateTwinsAsync(item.DeviceId, item.ETag, patch);
                    }
                }
            }
        }

        public async Task<S.IoT.Twins> GetTwinsAsync(string deviceId)
        {
            Twin twin = await _registryManager.GetTwinAsync(deviceId);

            if (twin != null)
                return _mapper.Map<S.IoT.Twins>(twin);
            else
                return null;

        }
        #endregion

        #region Jobs
        public async Task RunTwinUpdateJobAsync(string jobId, string queryCondition, S.IoT.Twins twin, DateTime startTime, long timeOut)
        {
            if (string.IsNullOrEmpty(jobId))
                throw new ArgumentNullException("jobId");

            if (string.IsNullOrEmpty(queryCondition))
                throw new ArgumentNullException("queryCondition");

            if (twin == null)
                throw new ArgumentNullException("twin");

            var iotHubTwin = _mapper.Map<Twin>(twin);

            if (iotHubTwin == null)
                throw new Exception("Twin mapping error.");


            using (JobClient jobClient = JobClient.CreateFromConnectionString(_settings.IoTHub.ConnectionString))
            {
                if (jobClient != null)
                {
                    JobResponse createJobResponse = await jobClient.ScheduleTwinUpdateAsync(
                        jobId,
                        queryCondition,
                        iotHubTwin,
                        startTime,
                        timeOut);
                }
            }

        }

        public async Task RunTwinUpdateJobAsync(string jobId, string queryCondition, S.IoT.Twins twin)
        {
            await RunTwinUpdateJobAsync(jobId, queryCondition, twin, DateTime.UtcNow, (long)TimeSpan.FromMinutes(2).TotalSeconds);
        }

        public async Task<JobResponse> MonitorJobWithDetailsAsync(string jobId)
        {
            if (string.IsNullOrEmpty(jobId))
                throw new ArgumentNullException("jobId");

            using (JobClient jobClient = JobClient.CreateFromConnectionString(_settings.IoTHub.ConnectionString))
            {

                if (jobClient != null)
                {
                    return await jobClient.GetJobAsync(jobId);
                }
                else
                    return null;
            }
        }
        #endregion
    }
}

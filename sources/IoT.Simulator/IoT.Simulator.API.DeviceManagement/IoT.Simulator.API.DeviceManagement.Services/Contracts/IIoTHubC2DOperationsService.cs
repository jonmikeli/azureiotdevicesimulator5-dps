using IoT.Simulator.API.DeviceManagement.Services.Model.IoT;

using Microsoft.Azure.Devices; //WARNING: this reference should not be done. It has been included only for JobResponse and save time for the project.

using System;
using System.Threading.Tasks;

namespace IoT.Simulator.API.DeviceManagement.Services.Contracts
{
    public interface IIoTHubC2DOperationsService
    {
        Task<string> InvokeDirectMethodAsync(string deviceId, string methodName, string payload);
        Task<Twins> UpdateTwinsAsync(string deviceId, string patch);
        Task UpdateTwinsAsync(TwinsSearchRequest request, string patch);
        Task<Twins> GetTwinsAsync(string deviceId);

        Task RunTwinUpdateJobAsync(string jobId, string queryCondition, Twins twin, DateTime startTime, long timeOut);
        Task RunTwinUpdateJobAsync(string jobId, string queryCondition, Twins twin);

        Task<JobResponse> MonitorJobWithDetailsAsync(string jobId);
    }
}

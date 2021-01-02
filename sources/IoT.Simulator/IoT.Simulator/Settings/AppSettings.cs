using Microsoft.Extensions.Logging;

namespace IoT.Simulator.Settings
{
    public class AppSettings
    {
        public Logging Logging { get; set; }
        public DeviceManagementService DeviceManagementServiceSettings { get; set; }
    }

    public class Logging
    {
        public LogLevel Default { get; set; }
    }

    public class DeviceManagementService
    {
        public string BaseUrl { get; set; }
        public string AddModulesToDeviceRoute { get; set; }
    }
}

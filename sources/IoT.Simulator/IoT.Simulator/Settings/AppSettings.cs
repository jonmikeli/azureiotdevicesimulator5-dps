using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace IoT.Simulator.Settings
{
    public class AppSettings
    {
        public Logging Logging { get; set; }

        public DeviceManagementServiceSettings DeviceManagementServiceSettings { get; set; }
    }

    public class Logging
    {
        public LogLevel Default { get; set; }
    }

    public class DeviceManagementServiceSettings
    {
        public string BaseUrl { get; set; }
        public string AddModulesToDeviceRoute { get; set; }
        public bool AllowAutosignedSSLCertificates { get; set; }
    }
}

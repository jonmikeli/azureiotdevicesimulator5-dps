using IoT.Simulator.Extensions;
using Newtonsoft.Json;

namespace IoT.Simulator.Settings
{
    public class SettingsBase
    {
        /// <summary>
        /// This property should be persisted in a secured location with monitoring and controlled access.
        /// The connection string is provided at the provisioning stage (either by the IoT Hub or the DPS, which relies in turn on IoT Hub to get the Connection String.
        /// </summary>
        [JsonProperty("connectionString")]
        public string ConnectionString { get; set; }

        public string DeviceId
        {
            get
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                    return ConnectionString.ExtractValue("DeviceId");
                else
                    return string.Empty;
            }
        }

        public string HostName
        {
            get
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                    return ConnectionString.ExtractValue("HostName");
                else
                    return string.Empty;
            }
        }
    }
}

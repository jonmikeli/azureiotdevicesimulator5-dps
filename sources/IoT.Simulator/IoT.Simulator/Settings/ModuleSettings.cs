using IoT.Simulator.Extensions;
using Newtonsoft.Json;

namespace IoT.Simulator.Settings
{
    public class ModuleSettings : SettingsBase
    {        
        [JsonProperty("moduleId", Required = Required.Always)]
        public string ModuleId { get; set; }

        public string ArtifactId
        {
            get
            {
                if (!string.IsNullOrEmpty(ConnectionString))
                    return $@"{DeviceId}/{ModuleId}";
                else
                    return string.Empty;
            }
        }

        [JsonProperty("simulationSettings")]
        public SimulationSettingsModule SimulationSettings
        {
            get; set;
        }
    }
}

using IoT.Simulator.Tools;
using Newtonsoft.Json;
using System;

namespace IoT.Simulator.Settings
{
    public class SimulationSettingsBase
    {
        [JsonProperty("enableTelemetryMessages")]
        public bool EnableTelemetryMessages { get; set; }
        [JsonProperty("telemetryFrecuency")]
        public int TelemetryFrecuency { get; set; }

        [JsonProperty("enableErrorMessages")]
        public bool EnableErrorMessages { get; set; }
        [JsonProperty("errorFrecuency")]
        public int ErrorFrecuency { get; set; }

        [JsonProperty("enableCommissioningMessages")]
        public bool EnableCommissioningMessages { get; set; }
        [JsonProperty("commissioningFrecuency")]
        public int CommissioningFrecuency { get; set; }


        [JsonProperty("enableTwinReportedMessages")]
        public bool EnableTwinReportedMessages { get; set; }

        [JsonProperty("twinReportedMessagesFrecuency")]
        public int TwinReportedMessagesFrecuency { get; set; }

        [JsonProperty("enableReadingTwinProperties")]
        public bool EnableReadingTwinProperties { get; set; }

        [JsonProperty("enableC2DDirectMethods")]
        public bool EnableC2DDirectMethods { get; set; }

        [JsonProperty("enableC2DMessages")]
        public bool EnableC2DMessages { get; set; }

        [JsonProperty("enableTwinPropertiesDesiredChangesNotifications")]
        public bool EnableTwinPropertiesDesiredChangesNotifications { get; set; }
    }
}

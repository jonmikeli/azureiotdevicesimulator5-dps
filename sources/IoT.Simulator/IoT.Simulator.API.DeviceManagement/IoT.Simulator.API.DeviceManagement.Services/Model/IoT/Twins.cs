using Newtonsoft.Json;

using System;

namespace IoT.Simulator.API.DeviceManagement.Services.Model.IoT
{
    public class Twins
    {
        [JsonProperty("tags")]
        public Tags Tags { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }

        [JsonProperty("etag")]
        public string ETag { get; set; }

        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("lastActivityTime")]
        public DateTime? LastActivityTime { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("statusReason")]
        public string StatusReason { get; set; }

        [JsonProperty("statusUpdatedTime")]
        public DateTime? StatusUpdatedTime { get; set; }
    }
}

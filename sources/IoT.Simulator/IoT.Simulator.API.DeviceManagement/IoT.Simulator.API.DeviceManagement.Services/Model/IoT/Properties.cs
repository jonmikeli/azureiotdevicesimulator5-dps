using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IoT.Simulator.API.DeviceManagement.Services.Model.IoT
{
    public class Properties
    {
        [JsonProperty("desired", NullValueHandling = NullValueHandling.Ignore)]
        public JObject Desired { get; set; }

        [JsonProperty("reported", NullValueHandling = NullValueHandling.Ignore)]
        public JObject Reported { get; set; }
    }
}

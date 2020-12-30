using Newtonsoft.Json;

namespace IoT.Simulator.API.DeviceManagement.Services.Model.IoT
{
    public class Location
    {
        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
    }
}

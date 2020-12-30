using Newtonsoft.Json.Linq;

namespace IoT.Simulator.API.DeviceManagement.API.DataContracts.IoT
{
    public class Properties
    {
        public JObject Desired { get; set; }

        public JObject Reported { get; set; }
    }
}

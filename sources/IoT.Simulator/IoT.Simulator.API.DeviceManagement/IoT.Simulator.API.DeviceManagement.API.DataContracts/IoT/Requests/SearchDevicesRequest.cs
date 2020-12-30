using System.ComponentModel.DataAnnotations;

namespace IoT.Simulator.API.DeviceManagement.API.DataContracts.IoT.Requests
{
    public class SearchDevicesRequest
    {
        [Required]
        public string Query { get; set; }

        public int MaxCount { get; set; }
    }
}

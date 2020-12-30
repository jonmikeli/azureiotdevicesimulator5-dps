using System.ComponentModel.DataAnnotations;

namespace IoT.Simulator.API.DeviceManagement.API.DataContracts.IoT.Requests
{
    public class ProvisionDeviceWithTagsRequest
    {
        [Required]
        public string DeviceId { get; set; }

        [Required]
        public Tags Tags { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace IoT.Simulator.API.DeviceManagement.API.DataContracts.IoT.Requests
{
    public class TwinUpdateRequest
    {
        [Required]
        public string DeviceId { get; set; }

        public Properties TwinProperties { get; set; }

        public Tags TwinTags { get; set; }

    }
}

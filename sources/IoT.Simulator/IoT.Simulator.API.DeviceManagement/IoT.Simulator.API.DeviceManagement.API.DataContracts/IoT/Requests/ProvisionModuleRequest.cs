using System.ComponentModel.DataAnnotations;

namespace IoT.Simulator.API.DeviceManagement.API.DataContracts.IoT.Requests
{
    public class ProvisionModuleRequest
    {
        [Required]
        public string DeviceId { get; set; }

        [Required]
        public string ModuleId { get; set; }
    }
}

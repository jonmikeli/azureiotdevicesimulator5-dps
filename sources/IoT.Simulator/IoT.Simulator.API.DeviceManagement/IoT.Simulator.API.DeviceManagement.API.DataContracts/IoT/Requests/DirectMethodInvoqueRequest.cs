using System.ComponentModel.DataAnnotations;

namespace IoT.Simulator.API.DeviceManagement.API.DataContracts.IoT.Requests
{
    public class DirectMethodInvoqueRequest
    {
        [Required]
        public string DeviceId { get; set; }

        [Required]
        public string MethodName { get; set; }

        public string Payload { get; set; }
    }
}

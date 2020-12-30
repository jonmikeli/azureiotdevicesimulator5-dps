using System.ComponentModel.DataAnnotations;

namespace IoT.Simulator.API.DeviceManagement.API.Common.Settings
{
    public class AppSettings
    {
        [Required]
        public ApiSettings API { get; set; }
        [Required]
        public Swagger Swagger { get; set; }

        public IoTHub IoTHub { get; set; }
    }

    public class ApiSettings
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public ApiContact Contact { get; set; }

        public string TermsOfServiceUrl { get; set; }

        public ApiLicense License { get; set; }
    }

    public class ApiContact
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public string Url { get; set; }
    }

    public class ApiLicense
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class Swagger
    {
        [Required]
        public bool Enabled { get; set; }
    }

    public class IoTHub
    {
        public string ConnectionString { get; set; }

        public int DirectMethodTimeOut { get; set; }
    }
}

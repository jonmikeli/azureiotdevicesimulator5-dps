
namespace IoT.Simulator.API.DeviceManagement.API.DataContracts.IoT
{
    public class Tags
    {
        #region System and manufacturer oriented
        public string DeviceType { get; set; }

        public string SerialNumber { get; set; }

        public string ManufacturerCode { get; set; }

        public string FirmwareVersion { get; set; }

        public string LastFirmwareUpdate { get; set; }
        #endregion

        #region Message and settings oriented
        public string MeasuredDataSchemaVersion { get; set; }
        #endregion

        #region Business oriented      
        public string Environment { get; set; }

        public string Status { get; set; }

        public Location Location { get; set; }

        public string RoomId { get; set; }
        #endregion
    }
}

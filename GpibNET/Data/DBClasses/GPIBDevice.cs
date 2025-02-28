namespace Gpib.Web.Data.DBClasses
{
    public class GPIBDevice : IDevice
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public string Model { get; set; } = "";
        public string ConnectionString { get; set; } = "";
        public EnumPrimaryDeviceTyp DeviceTyp { get; set; } = EnumPrimaryDeviceTyp.LabDevice;

        public string? ProfileDeviceId { get; set; } = "";
        public ProfileDevice? ProfileDevice { get; set; }

        public ICollection<FunctionDevice>? FunctionDevices { get; set; }
    }
}

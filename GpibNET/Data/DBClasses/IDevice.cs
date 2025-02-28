namespace Gpib.Web.Data.DBClasses
{
    public interface IDevice
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SerialNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string ConnectionString { get; set; }
        public EnumPrimaryDeviceTyp DeviceTyp { get; set; }
    }
    
}

namespace Gpib.Web.Data.DBClasses
{
    public class ProfileDevice
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public DateTime CreationDateTime { get; set; } = DateTime.UtcNow;

        public ICollection<FunctionDevice> FunctionDevices { get; set; }

        public ICollection<GPIBDevice> GPIBDevices { get; set; }
    }
}

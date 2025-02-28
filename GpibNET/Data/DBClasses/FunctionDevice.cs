namespace Gpib.Web.Data.DBClasses
{
    public class FunctionDevice
    {
        public string Id { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string Command { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public int ParameterCount { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public DateTime CreationDateTime { get; set; } = DateTime.UtcNow;
        public string Creator { get; set; }
        public string Profile { get; set; }
        public bool writeonly { get; set; } = false; // if true , the device is expected to listen only without any response
        public int ExpectedReturns { get; set; } = 0; // count of return values at one request
        public string ReturnUnit { get; set; }

        public string ProfileId { get; set; }
        public ProfileDevice ProfileDevice { get; set; }

        public int GPIBDeviceId { get; set; }
        public GPIBDevice GPIBDevice { get; set; }
    }
}

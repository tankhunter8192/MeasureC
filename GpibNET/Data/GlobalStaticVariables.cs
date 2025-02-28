using Gpib.Web.DeviceHandling;

namespace Gpib.Web.Data
{
     public static class GlobalStaticVariables
    {
        public static ApplicationDbContext DbContext { get; set; } = null!;
        public static PyVisaWrapper PyVisaWrapper { get; set; } = new();
        public static DateTime LastScanDateTime { get; set; } = DateTime.Now;
        public static DateTime StartTime { get; set; } = DateTime.Now; //for scanning devices to detect old devices that can be offline now after SW has restarted
        public static Logger Logger { get; set; } = new();
    }
}

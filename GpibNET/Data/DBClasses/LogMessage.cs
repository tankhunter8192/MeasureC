using System.ComponentModel.DataAnnotations;

namespace Gpib.Web.Data.DBClasses
{
    public enum LogSeverity
    {
        Info,
        Warning,
        Error
    }

    public class LogMessage
    {
        public string Id { get; set; }
        [Required]
        public string Message { get; set; } = "";
        [Required]
        public string Source { get; set; } = "";
        [Required]
        public DateTime CreationDateTime { get; set; } = DateTime.UtcNow;
        [Required]
        public LogSeverity Severity { get; set; }
    }
}

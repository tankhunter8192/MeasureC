

using System.ComponentModel.DataAnnotations;

namespace Gpib.Web.Models
{
    public class ProgramFileModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string Content { get; set; } = "";
        public string ContentNormalized { get; set; } = "";
        public string LastRunResult { get; set; } = "";
        public List<string> DependencyList { get; set; } = new List<string>();
        public DateTime CreationDateTime { get; set; } = DateTime.MinValue;

    }
}

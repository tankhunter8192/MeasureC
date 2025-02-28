namespace Gpib.Web.Data.DBClasses
{
    public class ProgramFile
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime CreationDateTime { get; set; } = DateTime.MinValue;
        public string CreationUserId { get; set; } = ""; //now username got problems with userId
        public DateTime LastChange { get; set; } = DateTime.MinValue;
        public string LastChangeUserId { get; set; } = ""; //now username got problems with userId
        public string LastRunResult { get; set; } = "";
        public DateTime LastRunDateTime { get; set; } = DateTime.MinValue;
        public string Content { get; set; } = "";
        public string ContentNormalized { get; set; } = "";
        public List<string> DependencyList { get; set; } = new List<string>();
    }
    
    
}

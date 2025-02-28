namespace Gpib.Web.Data.DBClasses
{
    public class User
    {
        public string Id { get; set; }
        private string _username;
        public string Username {
            get => _username;
            set => _username = value?[..Math.Min(value.Length, 20)];
        }
        public string Password { get; set; }
        public DateTime AddDateTime { get; set; }
        public DateTime LastLoginDateTime { get; set; }
        public bool IsAdmin { get; set; } //is root
        public bool IsVisitor { get; set; } // for users that have permission to view but not edit
    }
}

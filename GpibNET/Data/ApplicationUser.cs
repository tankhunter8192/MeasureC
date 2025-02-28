using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Gpib.Web.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        //public int Id { get => _id; set => _id = value; } //internally
        public string? UserId { get => _userId; set => _userId = value?[..Math.Min(value.Length, 2048)]; } //web usage
        private string? _userId;
        
        private string? _passwordHash;
        //private int _id;

        public override string? PasswordHash
        {
            get => _passwordHash; 
            set => _passwordHash = value?[..Math.Min(value.Length, 1023)];
        }
        public DateTime AddDateTime { get; set; }
        public DateTime LastLoginDateTime { get; set; }
        public bool IsAdmin { get; set; } //is root
        public bool IsVisitor { get; set; }
        public bool IsLocked { get; set; }
        public string? ConcurrencyStamp
        {
            get => _concurrencyStamp;
            set => _concurrencyStamp = value?[..Math.Min(value.Length, 1023)];
        }
        private string? _concurrencyStamp;
    }

}

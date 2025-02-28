using Azure.Identity;
using Gpib.Web.Data.DBClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace Gpib.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            GlobalStaticVariables.DbContext = this;
        }
        // Define your DbSets here
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<GPIBDevice> Devices { get; set; }
        public DbSet<ProgramFile> ProgramFiles { get; set; }
        public DbSet<LogMessage> LogMessages { get; set; }
        public DbSet<FunctionDevice> FunctionDevices { get; set; }
        public DbSet<ProfileDevice> ProfileDevices { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");
                //entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.UserId).ValueGeneratedOnAdd();
                entity.Property(e => e.UserName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.HasIndex(e => e.UserName).IsUnique();
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

            });

            modelBuilder.Entity<LogMessage>(entity =>
            {
                entity.ToTable("LogMessage");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.Source).HasMaxLength(1023);
                entity.Property(e => e.CreationDateTime).IsRequired();
            });

            modelBuilder.Entity<ProfileDevice>(entity =>
            {
                entity.ToTable("ProfileDevice");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Creator).HasMaxLength(1023);
                entity.Property(e => e.CreationDateTime).IsRequired();

            });

            modelBuilder.Entity<FunctionDevice>(entity =>
            {
                entity.ToTable("FunctionDevice");
                entity.Property(e => e.Id).HasMaxLength(255).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Command).HasMaxLength(1000).IsRequired();
                entity.Property(e => e.ParameterCount).IsRequired();
                entity.Property(e => e.CreationDateTime).IsRequired();
                entity.Property(e => e.Creator).HasMaxLength(1023).IsRequired();
                entity.Property(e => e.Profile).HasMaxLength(1023);
                entity.Property(e => e.writeonly).IsRequired();
                entity.Property(e => e.ExpectedReturns).IsRequired();
                entity.Property(e => e.ReturnUnit).HasMaxLength(100);

                entity.HasOne(d => d.ProfileDevice).WithMany(g => g.FunctionDevices).HasForeignKey(d => d.ProfileId);
                entity.HasOne(d => d.GPIBDevice).WithMany(g => g.FunctionDevices).HasForeignKey(d => d.GPIBDeviceId);
            });

            /*
            if (this.Users.IsNullOrEmpty())
            {
                var hasher = new PasswordHasher<ApplicationUser>();
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    IsAdmin = true,
                    IsVisitor = false,
                    IsLocked = false,
                    AddDateTime = DateTime.Now,
                    LastLoginDateTime = DateTime.MinValue,

                };
                adminUser.PasswordHash = hasher.HashPassword(adminUser, "admin");

                modelBuilder.Entity<ApplicationUser>().HasData(adminUser);
            }
            */

            modelBuilder.Entity<GPIBDevice>(entity =>
            {
                entity.ToTable("GPIBDevice");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.SerialNumber).HasMaxLength(100);
                entity.Property(e => e.Manufacturer).HasMaxLength(100);
                entity.Property(e => e.Model).HasMaxLength(100);
                entity.Property(e => e.ConnectionString).HasMaxLength(1000);
                entity.Property(e => e.DeviceTyp).IsRequired();

                entity.HasOne(d => d.ProfileDevice).WithMany(g => g.GPIBDevices).HasForeignKey(d => d.ProfileDeviceId);
                //entity.HasOne(d => d.ProfileDeviceId);
                entity.HasMany(d=> d.FunctionDevices).WithOne(g => g.GPIBDevice).HasForeignKey(d => d.GPIBDeviceId);
            });

            modelBuilder.Entity<ProgramFile>(entity =>
            {
                entity.ToTable("ProgramFile");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Content).HasMaxLength(-1);
                entity.Property(e => e.ContentNormalized).HasMaxLength(-1);
                entity.Property(e => e.LastRunResult);
                entity.Property(e => e.DependencyList).HasConversion(
                    v=>string.Join(',',v), 
                    v=> v.Split(',',StringSplitOptions.RemoveEmptyEntries).ToList())
                    .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
                entity.Property(e => e.CreationDateTime).IsRequired();
                entity.Property(e => e.CreationUserId).IsRequired();
                entity.Property(e=> e.LastChangeUserId).IsRequired();
                entity.Property(e => e.LastChange).IsRequired();
                entity.Property(e => e.LastRunDateTime).IsRequired();


            });
            
        }

        public async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Check if the admin user already exists
                var adminUser = await userManager.FindByNameAsync("admin");
                if (adminUser == null)
                {
                    // Create a new admin user, based on Microsoft SignIn -> Email required
                    var newAdminUser = new ApplicationUser
                    {
                        UserName = "admin",
                        Email = "admin@example.com",
                        IsAdmin = true,
                        IsVisitor = false,
                        IsLocked = false,
                        AddDateTime = DateTime.UtcNow,
                        LastLoginDateTime = DateTime.UtcNow,
                    };
                    // create default admin:admin
                    var result = await userManager.CreateAsync(newAdminUser, "admin");
                    if (!result.Succeeded)
                    {
                        throw new Exception("Could not create default admin user");
                    }
                }
            }
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, UserManager<ApplicationUser> userManager)
        {
            IdentityResult result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                GlobalStaticVariables.Logger.LogError("UserCreator: ", "Could not create user " + user.UserName);
                foreach (var res in result.Errors)
                {
                    GlobalStaticVariables.Logger.LogError("UserCreator: ", "Error: " + res.Description);
                }
            }
            else
            {
                await userManager.AddToRoleAsync(user, user.IsAdmin ? "Admin" : "User");
            }

            return result;
        }
    }
}

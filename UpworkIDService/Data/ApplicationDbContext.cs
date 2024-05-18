using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UpworkIDService.models;
using UpworkIDService.Models;

namespace UpworkIDService.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserProfileRole> UserProfileRole { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //builder.Entity<ApplicationUser>(entity =>
            //{
            //    // Define your entity properties and mappings here

            //    // Example of configuring a string property without specifying a column width
            //    entity.Property(e => e.Id)
            //          .HasColumnType("Id"); // Remove any column width specification
            //});
            //builder.Entity<Country>(entity =>
            //{
            //    // Define your entity properties and mappings here

            //    // Example of configuring a string property without specifying a column width
            //    entity.Property(e => e.Id)
            //          .HasColumnType("Id"); // Remove any column width specification
            //});
        }
    }
}

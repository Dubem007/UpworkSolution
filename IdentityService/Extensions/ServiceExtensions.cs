using Microsoft.AspNetCore.Identity;
using System.Data;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using IdentityService.models.Models;
using IdentityService.Services;
using IdentityService.IDConfigurations;
using IdentityService.models;
using IdentityService.Data;
using IdentityServerHost.Quickstart.UI;

namespace IdentityService.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration, string assembly)
        {
            var defaultConnString = configuration.GetValue<string>("ConnectionStrings:Default");
            services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(assembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(assembly));
                })
                .AddProfileService<CustomProfileService>()
                .AddDeveloperSigningCredential();
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<ApplicationUser, UserRole>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 8;
                opt.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<AspNetIdentityDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void ConfigureSQLserver(this IServiceCollection services, IConfiguration configuration, string assembly)
        {
            var connectionString = configuration.GetValue<string>("ConnectionStrings:Default");
            services.AddDbContext<AspNetIdentityDbContext>(options => options.UseSqlServer(connectionString,
             b => b.MigrationsAssembly(assembly)));
        }           
    }
}

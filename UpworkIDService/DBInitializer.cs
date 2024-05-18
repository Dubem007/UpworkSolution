using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UpworkIDService.Data;

namespace UpworkIDService
{
    public static class DBInitializer
    {
        public static async Task SeedRoleData(this IHost host)
        {
            var serviceProvider = host.Services.CreateScope().ServiceProvider;
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var roles = UserRoleData.GetRoles();
            var country = CountryData.GetCountries();

            if (!context.UserRoles.Any())
            {
                await context.UserRoles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }

            if (!context.Countries.Any())
            {
                await context.Countries.AddRangeAsync(country);
                await context.SaveChangesAsync();
            }
        }

    }


}

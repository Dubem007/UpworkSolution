 using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserServices.Persistense;

namespace Persistence.ModelBuilders
{
    public static class DBInitializer
    {
        public static async Task SeedRoleData(this IHost host)
        {
            var serviceProvider = host.Services.CreateScope().ServiceProvider;
            var context = serviceProvider.GetRequiredService<UserDbContext>();
            var roles = UserRoleData.GetRoles();
            var country = CountryData.GetCountries();
            var categories = CategoryData.GetCategories();
            var plans = MembershipPlanData.GetMembershipPlans();
            var langs = ProficiencyData.GetProficiencies();

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

            if (!context.Categories.Any())
            {
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            if (!context.MembershipPlans.Any())
            {
                await context.MembershipPlans.AddRangeAsync(plans);
                await context.SaveChangesAsync();
            }

            if (!context.Proficiencies.Any())
            {
                await context.Proficiencies.AddRangeAsync(langs);
                await context.SaveChangesAsync();
            }
        }

    }


}

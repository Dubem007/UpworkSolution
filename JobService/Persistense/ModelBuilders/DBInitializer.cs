
using Common.services.Caching;
using JobService.Persistense;

namespace JobService.Persistence.ModelBuilders
{
    public static class DBInitializer
    {

        public static async Task SeedRoleData(this IHost host)
        {
            var serviceProvider = host.Services.CreateScope().ServiceProvider;
            var context = serviceProvider.GetRequiredService<JobServiceDbContext>();
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

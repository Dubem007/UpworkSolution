using Common.Enums;
using Common.MessageQueue.Interfaces;
using Common.MessageQueue.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OnaxTools.Dto.Http;
using System.Text.Json;
using UserServices.AppCore.Repository.Interfaces;
using UserServices.Domain.DTOs;
using UserServices.Models.DTOs;
using UserServices.Persistense;

namespace UserServices.AppCore.Services.Core
{
    public class ProfileUpdateServices : BackgroundService
    {
    
        private readonly ILogger<ProfileUpdateServices> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppSettings _appsettings;

        public ProfileUpdateServices(ILogger<ProfileUpdateServices> logger, IServiceProvider serviceProvider, IOptions<AppSettings> appsettings)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _appsettings = appsettings.Value;
        }

        public async Task<GenResponse<bool>> UpdateAvalabilityBadge(UserDbContext dbcontext, CancellationToken ct = default)
        {
            GenResponse<bool> objResp = new();

           
            try
            {
                var badges = await dbcontext.Badges.Where(x => x.IsActive == true).ToListAsync();

                if (badges.Any()) 
                {
                    var Users = badges.Select(x => x.UserId).ToArray();
                    var connections = await dbcontext.Connections.Where(x => Users.Contains(x.UserId) && x.IsActive == true).ToListAsync();
                    var allbadges = badges.ToArray();

                    for (int i = 0; i < allbadges.Length; i++)
                    {
                        var theconnection = connections.FirstOrDefault(x => x.UserId == allbadges[i].UserId);
                        // Check if available date exceeds 7 days from today
                        bool exceedsSevenDays = ExceedsSevenDays(allbadges[i].AvailableDate);

                        // Print the result
                        if (exceedsSevenDays && theconnection.NewConnectsbalance == "0")
                        {
                            allbadges[i].AvailabileNow = false;
                            await dbcontext.SaveChangesAsync();
                        }
                        else
                        {
                            //Do nothing;
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return GenResponse<bool>.Failed(ex.Message);
            }
            return objResp;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                System.Diagnostics.Stopwatch stopwatch = new();
                stopwatch.Start();

                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                    DateTime thirtyDaysAgo = DateTime.Today.AddDays(-60);
                    await this.UpdateAvalabilityBadge(dbContext, stoppingToken);
                }
                

                Thread.Sleep(1000);
                stopwatch.Stop();
                Console.WriteLine("Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds);
            }
        }

        private static bool ExceedsSevenDays(DateTime availableDate)
        {
            // Calculate the difference in days between available date and today
            TimeSpan difference = availableDate - DateTime.Today;

            // Check if the difference exceeds 7 days
            return difference.Days > 7;
        }
    }
}

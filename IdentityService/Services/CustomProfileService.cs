using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityService.models.Models;
using IdentityService.Data;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace IdentityService.Services
{
    public class CustomProfileService : IProfileService
    {
        protected UserManager<ApplicationUser> userManager;
        private AspNetIdentityDbContext dbContext;

        public CustomProfileService(UserManager<ApplicationUser> userManager, AspNetIdentityDbContext dbContext)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var claims = new List<Claim>();
            var user = userManager.GetUserAsync(context.Subject).Result;
         
            if (user != null)
            {
                var subjectId = user.Id.ToString();

                claims = new List<Claim>
                {
                    new Claim("sub", subjectId),
                    new Claim(ClaimTypes.Name, $"{user.UserName}"),
                    new Claim(ClaimTypes.Email, user.UserName),
                    new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(user)),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.RoleId),
                };
            }
               

            var userClaims = dbContext.UserClaims.Where(m => m.UserId == user.Id).ToList();
            userClaims.ForEach(m => claims.Add(new Claim(m.ClaimType, m.ClaimValue)));
            context.IssuedClaims.AddRange(claims);

            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var user = userManager.GetUserAsync(context.Subject).Result;
            context.IsActive = user != null && user.LockoutEnd == null;

            return Task.FromResult(0);
        }
    }
}

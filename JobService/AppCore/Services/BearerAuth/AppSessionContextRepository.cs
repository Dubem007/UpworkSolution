using IdentityModel;
using JobService.Domain.Dtos;
using JobService.Extension.Middleware;
using JobService.Persistense;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using OnaxTools.Dto.Http;

namespace JobService.AppCore.Services.BearerAuth
{
    public interface IAppSessionContextRepository
    {
        Task<GenResponse<ClaimDetailsDto>> GetUserDetails();
        Dictionary<string, string> GetCurrentContextBearerAuth();
    }
    public class AppSessionContextRepositoryCommon : IAppSessionContextRepository
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly JobServiceDbContext _context;

        public AppSessionContextRepositoryCommon(IHttpContextAccessor contextAccessor, JobServiceDbContext context)
        {
            _contextAccessor = contextAccessor;
            _context = context;
        }
        public virtual async Task<GenResponse<ClaimDetailsDto>> GetUserDetails()
        {
            GenResponse<ClaimDetailsDto> objResp = new();
            try
            {
                var tokenUser = CommonHelpers.ValidateToken(_contextAccessor.HttpContext);
                var theid = Guid.Parse(tokenUser.Result.Result.UserId);
                var userdets = await _context.JobUsers.FirstOrDefaultAsync(x => x.Id == theid);
                var resp = new ClaimDetailsDto()
                {
                    UserId = tokenUser.Result.Result.UserId,
                    Email = tokenUser.Result.Result.Email,
                    Role = tokenUser.Result.Result.Role,
                    Country = tokenUser.Result.Result.Country,
                    FullName = tokenUser.Result.Result.FullName,
                };

                objResp.Result = resp;
                objResp.IsSuccess = true;

            }
            catch (Exception ex)
            {

                objResp.Result = new ClaimDetailsDto();
                objResp.IsSuccess = false;
            }
            return await Task.Run(() => objResp);
        }

        public virtual Dictionary<string, string> GetCurrentContextBearerAuth()
        {
            Dictionary<string, string> objResp = new();
            StringValues authValues = _contextAccessor.HttpContext.Request.Headers["Authorization"];
            if (StringValues.IsNullOrEmpty(authValues) || !authValues.Any(m => m.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase)))
            {
                return objResp;
            }
            var authHeader = authValues.FirstOrDefault(m => m != null && m.StartsWith("Bearer"));
            if (authHeader != null)
            {
                string[] authToken = !string.IsNullOrWhiteSpace(authHeader) ? authHeader.Split(" ") : new string[] {"",""};
                objResp.Add(Convert.ToString(authToken[0]), Convert.ToString(authToken[1]));
            }
            return objResp;
        }
    }
}

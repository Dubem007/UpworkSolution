
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using OnaxTools.Dto.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using JobService.Persistense;

namespace JobService.Extension.Middleware
{
    public class RoleAuthorizerAttribute : Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter
    {
        private readonly string[] allowedRoles;
        private readonly JobServiceDbContext _context;
        public RoleAuthorizerAttribute(JobServiceDbContext context, params string[] roles)
        {
            allowedRoles = roles;
            this._context = context;
        }


        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var tokenUser = CommonHelpers.ValidateToken(context.HttpContext);
            if (tokenUser.Result.IsSuccess)
            {
                string userEmail = tokenUser.Result.Result.Email;
                var allRoles = _context.JobUsers.FirstOrDefault(x=>x.UserId == tokenUser.Result.Result.UserId);
                if (allRoles != null)
                {
                    var role = _context.UserRoles.Where(x => x.RoleName.ToLower().Trim() == allRoles.Role.ToLower().Trim()).Select(x=>x.RoleName).ToList();
                    bool isAuthorized = role.Any(m => allowedRoles.Contains(m));
                    if (!isAuthorized)
                    {
                        context.Result = new CustomUnauthorizedResult("You are not authorized to access this resource.");
                    }
                }
                else
                {
                    context.Result = new CustomUnauthorizedResult("You are not authorized to access this resource.");
                }
            }
            else
            {
                context.Result = new CustomUnauthorizedResult("You are not authorized to access this resource.");
            }
        }
    }

    public class AuthAttribute : TypeFilterAttribute
    {
        public AuthAttribute(params string[] roles) : base(typeof(RoleAuthorizerAttribute))
        {
            Arguments = new object[] {
            roles
            };
        }
    }

    #region Helpers
    public class CustomUnauthorizedResult : JsonResult
    {
        public CustomUnauthorizedResult(string message) : base(new CustomError(message))
        {
            StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
    public class CustomError
    {
        public GenResponse<string> Error { get; }

        public CustomError(string message)
        {
            Error = new()
            {
                IsSuccess = false,
                Error = message,
                StatCode = 401
            };
        }
    }

    public class StringIEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
    #endregion
}

using Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnaxTools.Dto.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Models.DTOs;
using UserService.Services;
using UserServices.AppCore.CQRS.Commands;
using UserServices.Domain.DTOs;
using UserServices.Models;
using UserServices.Models.DTOs;
using UserServices.Services;

namespace UserServices.AppCore.CQRS.Handlers.CommandHandlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, GenResponse<TokenDTO>>
    {
        private readonly ILogger<LoginCommandHandler> logger;
        private readonly IUserServicess _UserServices;
        private readonly IMediator mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly AppSettings _appsettings;

        public LoginCommandHandler(ILogger<LoginCommandHandler> logger, IUserServicess userServices, IMediator mediator, UserManager<ApplicationUser> userManager, RoleManager<UserRole> roleManager, IOptions<AppSettings> appsettings)
        {
            this.logger = logger;
            _UserServices = userServices;
            this.mediator = mediator;
            _userManager = userManager;
            _roleManager = roleManager;
            _appsettings = appsettings.Value;
        }

        public async Task<GenResponse<TokenDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            GenResponse<TokenDTO> objResp = new();
            try
            {
                var resp = await _UserServices.GetLoginToken(request.Entity);
                if (resp.access_token != null)
                {
                    var response = await ConvertToBearerToken(resp.access_token);
                    resp.access_token = response;

                    objResp.IsSuccess = true;
                    objResp.Result = resp;
                    objResp.Message = AppConstants.LoginSuccessResponse;
                }
                else {
                    objResp.IsSuccess = false;
                    objResp.Result = resp;
                    objResp.Message = AppConstants.LoginFailedResponse;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(exception: ex, ex.Message);
                objResp.IsSuccess = false;
                objResp.Result = new TokenDTO();
                objResp.Message = $"{AppConstants.LoginFailedResponse} {ex.Message}";
            }
            return objResp;
        }

        private async Task<string> ConvertToBearerToken(string accessToken)
        {
            try
            {
                var role = new UserRole();
                // Parse the received access token
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;

                var userdetails = await _userManager.FindByIdAsync(token.Subject.ToString());
                if (userdetails != null)
                {
                    role = _roleManager.Roles.FirstOrDefault(X => X.Id == userdetails.RoleId);
                }
                // Create claims for the bearer token
                var claims = new List<Claim>();
                claims = new List<Claim>
                {
                    new(ClaimTypeHelper.Email, userdetails.Email),
                    new(ClaimTypeHelper.UserId, token.Subject.ToString()),
                    new(ClaimTypeHelper.FullName, $"{userdetails.FirstName} {userdetails.LastName}"),
                    new(ClaimTypeHelper.Role, role.Name)
                };
                claims.AddRange(claims); // Add existing claims from the access token

                // Create a JWT security token
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appsettings.SecretKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(30), // Set token expiration
                    Issuer = _appsettings.Issuer,
                    Audience = _appsettings.Audience,
                    SigningCredentials = credentials
                };

                var tokenResult = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(tokenResult);
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}

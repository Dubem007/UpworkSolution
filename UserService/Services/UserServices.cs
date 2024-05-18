using Azure.Core;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Models.DTOs;
using UserService.Services;
using UserServices.Domain.DTOs;
using UserServices.Models;
using UserServices.Models.DTOs;
using UserServices.Persistense;
using static IdentityModel.OidcConstants;

namespace UserServices.Services
{
    public class UserServicess: IUserServicess
    {
        private readonly UserDbContext _context;
        private readonly AppSettings _appsettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<UserRole> _roleManager;

        private DiscoveryDocumentResponse _discDocument { get; set; }

        public UserServicess(UserDbContext context, IOptions<AppSettings> appsettings, UserManager<ApplicationUser> userManager, RoleManager<UserRole> roleManager)
        {
            _context = context;
            _appsettings = appsettings.Value;
            _userManager = userManager;
            _roleManager = roleManager;
            using (var client = new HttpClient())
            {
                _discDocument = client.GetDiscoveryDocumentAsync("https://localhost:5001/.well-known/openid-configuration").Result;
            }
        }

        public async Task<TokenDTO> GetLoginToken(LoginInputDto entity)
        {
            var token = await generateToken(entity);
            return token;
        }
        public async Task<TokenDTO> generateToken(LoginInputDto entity)
        {
            try
            {
               
                var data = new[]
                {
                    new KeyValuePair<string, string>("grant_type", _appsettings.Granttype),
                    new KeyValuePair<string, string>("username", entity.Username),
                    new KeyValuePair<string, string>("password", entity.Password),
                    new KeyValuePair<string, string>("client_id", _appsettings.ClientId),
                    new KeyValuePair<string, string>("client_secret", _appsettings.ClientSecret),
                    new KeyValuePair<string, string>("scope", _appsettings.ClientScope),
                };

                var myHeader = "customHeader";
                using (var client = new HttpClient())
                {
                    string url = $"{_appsettings.IdentityBaseUrl}/connect/token";
                    using (var httpMessage = new HttpRequestMessage())
                    {
                        httpMessage.Method = HttpMethod.Post;
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("ContentType", "x-www.form-urlencoded");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", myHeader);
                        httpMessage.RequestUri = new Uri(url);

                        Task<HttpResponseMessage> httpResponse = client.PostAsync(url, new FormUrlEncodedContent(data));
                        using (HttpResponseMessage httpResponseMessage = httpResponse.Result)
                        {
                            var d = httpResponseMessage.ToString();
                            var sam = httpResponseMessage.Content.ReadAsStringAsync().Result;
                            var result = JsonConvert.DeserializeObject<TokenDTO>(sam);
                            return result;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The error in token generation: {ex.Message}");
                return null;
            }

        }


        public async Task<IdentityModel.Client.TokenResponse> GetToken(string scope, string Username, string password)
        {
            using (var client = new HttpClient())
            {
                var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = _discDocument.TokenEndpoint,
                    ClientId = "user.client",
                    ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
                    UserName = Username,
                    Password = password

                });
                if (tokenResponse.IsError)
                {
                    throw new Exception("Token Error");
                }
                return tokenResponse;
            }
        }

        public async Task<bool> AddUserToRole(string userId, string roleId)
        {
            try
            {
                var userrole = new UserProfileRole
                {
                    RoleId = roleId,
                    UserId = userId,
                };

                await _context.UserProfileRoles.AddAsync(userrole);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"The error in token generation: {ex.Message}");
                return false;
            }

        }

        public async Task<bool> ValidateEmail(string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(token);
                if (user == null)
                {
                    return false;
                }

                user.EmailConfirmed = true;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
            return true;
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

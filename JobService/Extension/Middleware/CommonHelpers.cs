using JobService.Domain.Dtos;
using JobService.Domain.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnaxTools.Dto.Http;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JobService.Extension.Middleware
{
    public class CommonHelpers
    {
        private readonly string _audience;
        private readonly string _issuer;
        private readonly byte[] _signingKey;
        private readonly AppSettings _appsettings;

        public CommonHelpers(string audience, string issuer, string signingKey,IOptions<AppSettings> appsettings)
        {
            _audience = audience;
            _issuer = issuer;
            _appsettings = appsettings.Value;
            _signingKey = Convert.FromBase64String(signingKey);
        }

        public static async Task<GenResponse<ClaimDetailsDto>> ValidateToken(HttpContext context)
        {
            var objResp = new GenResponse<ClaimDetailsDto>();
            try
            {
                var authHeader = context.Request.Headers.Authorization.FirstOrDefault(m => m != null && m.StartsWith("Bearer"));
                if (authHeader != null)
                {
                    
                    string authToken = !string.IsNullOrWhiteSpace(authHeader) ? authHeader.Split(" ")[1] : string.Empty;
                    var jwtoken = new JwtSecurityTokenHandler().ReadJwtToken(authToken);
                    Dictionary<string, string> tokenValues = new Dictionary<string, string>();
                    foreach (var claim in jwtoken.Claims)
                    {
                        if (!tokenValues.ContainsKey(claim.Type))
                        {
                            tokenValues.Add(claim.Type, claim.Value);
                        }
                        else
                        {
                            tokenValues[claim.Type] = tokenValues[claim.Type] + "," + claim.Value;
                        }
                    }
                    if (tokenValues.Any())
                    {

                        string[] emails = tokenValues["Email"].Split(',');
                        string[] roles = tokenValues["Role"].Split(',');
                        string[] userids = tokenValues["UserId"].Split(',');
                        string[] fullnames = tokenValues["FullName"].Split(',');

                        // Pick the first email address (if available)
                        string firstEmail = emails.FirstOrDefault();
                        string firstrole = roles.FirstOrDefault();
                        string firstuserid = userids.FirstOrDefault();
                        string firstfullname = fullnames.FirstOrDefault();
                        var appUser = new ClaimDetailsDto()
                        {
                            Email = firstEmail,
                            Role = firstrole,
                            UserId = firstuserid,
                            FullName = firstfullname
                        };

                        objResp = GenResponse<ClaimDetailsDto>.Success(appUser);
                    }
                    else
                    {
                        objResp = GenResponse<ClaimDetailsDto>.Failed("Invalid token credentials");
                    }
                }
                else { objResp = GenResponse<ClaimDetailsDto>.Failed("Invalid token credentials"); }
            }
            catch (Exception ex)
            {
                // Handle token validation exceptions
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }

            return objResp;
        }
    }
}

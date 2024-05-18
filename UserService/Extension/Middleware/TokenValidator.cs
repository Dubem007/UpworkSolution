using OnaxTools.Dto.Http;
using System.IdentityModel.Tokens.Jwt;
using UserServices.Models.DTOs;

namespace UserServices.Extension.Middleware
{
    public class TokenValidator
    {
        public static GenResponse<ClaimDetailsDto> ValidateJwt(HttpContext context)
        {
            var objResp = new GenResponse<ClaimDetailsDto>();
            try
            {
                var authHeader = context.Request.Headers.Authorization.FirstOrDefault(m => m != null && m.StartsWith("Bearer"));
                if (authHeader != null)
                {
                    string accountPermissions = string.Empty;
                    string fullName = string.Empty;
                    string UserId = string.Empty;
                    string userRoles = string.Empty;
                    var submodulePermissions = string.Empty;

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
                        var appUser = new ClaimDetailsDto
                        {
                            Email = tokenValues["Email"],
                        };
                       

                        if (tokenValues.ContainsKey("Name"))
                        {
                            fullName = tokenValues["Name"];
                            appUser.FullName = fullName;
                        }

                        if (tokenValues.ContainsKey("Role"))
                        {
                            userRoles = tokenValues["Role"];
                            appUser.Role = userRoles;
                        }

                        if (tokenValues.ContainsKey("NameIdentifier"))
                        {
                            UserId = tokenValues["NameIdentifier"];
                            appUser.UserId = UserId;
                        }

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
                objResp = GenResponse<ClaimDetailsDto>.Failed("Invalid token credentials");
            }
            return objResp;
        }
    }


}

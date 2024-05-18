using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotificationService.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NotificationService.Extension.Middleware
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

        public async Task<ClaimsPrincipal> ValidateToken(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _appsettings.Issuer,
            };

            try
            {
                var principal =  tokenHandler.ValidateToken(accessToken, validationParameters, out var validatedToken);
                var subClaim =  principal.Claims.FirstOrDefault(c => c.Type == "sub");

                if (subClaim != null)
                {
                    // Use subClaim.Value as the logged-in user
                    return principal;
                }
            }
            catch (Exception ex)
            {
                // Handle token validation exceptions
                Console.WriteLine($"Token validation failed: {ex.Message}");
            }

            return null;
        }
    }
}

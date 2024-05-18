using IdentityServer4.Models;

namespace IdentityServer.IdentityConfiguration
{
    public class Resources
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new[]
            {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource
            {
                Name = "role",
                UserClaims = new List<string> {"role"}
            }
        };
        }
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
            new ApiResource
            {
                Name = "userserviceApi",
                DisplayName = "User Service Api",
                Description = "Allow the application to access User Service Api on your behalf",
                Scopes = new List<string> { "userserviceApi.read", "userservicesApi.write"},
                ApiSecrets = new List<Secret> {new Secret("ProjectGuide".Sha256())},
                UserClaims = new List<string> {"role"}
            },
            new ApiResource
            {
                Name = "jobserviceApi",
                DisplayName = "Job Service Api",
                Description = "Allow the application to access Job Service Api on your behalf",
                Scopes = new List<string> { "jobserviceApi.read", "jobserviceApi.write"},
                ApiSecrets = new List<Secret> {new Secret("ProjectGuide".Sha256())},
                UserClaims = new List<string> {"role"}
            }
        };
        }
    }
}

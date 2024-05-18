using IdentityServer4.Models;

namespace IdentityServer.IdentityConfiguration
{
    public class Scopes
    {
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
                new ApiScope("userserviceApi.read"),
                new ApiScope("userserviceApi.write"),
                new ApiScope("jobserviceApi.read"),
                new ApiScope("jobserviceApi.write"),
            };
        }
    }
}

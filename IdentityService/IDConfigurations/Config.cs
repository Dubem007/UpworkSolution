using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityService.IDConfigurations
{
    public static class Config
    {
        //public static IEnumerable<Client> Clients => new Client[]
        //{
        //    new Client
        //    {
        //        ClientId = "ro.client",
        //        ClientName = "User Credentials Client",
        //        AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
        //        ClientSecrets = { new Secret("secret".Sha256()) },
        //        AllowedScopes = { "myApi.read", "myApi.write" }
        //    },

        //     new Client
        //    {
        //        ClientId = "xo.client",
        //        ClientName = "Client Credentials Client",
        //        AllowedGrantTypes = GrantTypes.ClientCredentials,
        //        ClientSecrets = { new Secret("supersecret".Sha256()) },
        //        AllowedScopes = { "myApi.read", "myApi.write" }
        //    },
        //};

        //public static IEnumerable<IdentityResource> GetIdentityResources => new List<IdentityResource>
        //{
        //    new IdentityResources.OpenId(),
        //    new IdentityResources.Profile()
        //};

        //public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
        //{
        //    new ApiScope("myApi.read"),
        //    new ApiScope("myApi.write"),
        //};
        //public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        //{
        //    new ApiResource("userApi")
        //    {
        //        Scopes = new List<string>{ "myApi.read","myApi.write" },
        //        ApiSecrets = new List<Secret>{ new Secret("secret".Sha256()) }
        //    },
        //    new ApiResource("paymentApi")
        //    {
        //        Scopes = new List<string>{ "myApi.read","myApi.write" },
        //        ApiSecrets = new List<Secret>{ new Secret("supersecret".Sha256()) }
        //    },
        //    new ApiResource("jobApi")
        //    {
        //        Scopes = new List<string>{ "myApi.read","myApi.write" },
        //        ApiSecrets = new List<Secret>{ new Secret("supersecret".Sha256()) }
        //    }
        //};

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiScope> ApiScopes =>
           new[] { new ApiScope("myApi.read"), new ApiScope("myApi.write") };


        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("userApi")
                {
                    Scopes = new List<string>{ "myApi.read","myApi.write" },
                    ApiSecrets = new List<Secret>{ new Secret("secret".Sha256()) }
                },
                new ApiResource("paymentApi")
                {
                    Scopes = new List<string>{ "myApi.read","myApi.write" },
                    ApiSecrets = new List<Secret>{ new Secret("supersecret".Sha256()) }
                },
                new ApiResource("jobApi")
                {
                    Scopes = new List<string>{ "myApi.read","myApi.write" },
                    ApiSecrets = new List<Secret>{ new Secret("supersecret".Sha256()) }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "ro.client",
                    ClientName = "User Credentials Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes = { "myApi.read", "myApi.write" }
                },

                new Client
                {
                    ClientId = "xo.client",
                    ClientName = "Client Credentials Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("supersecret".Sha256()) },
                    AllowedScopes = { "myApi.read", "myApi.write" }
                },
            };
        }
    }
}

// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[] {new IdentityResources.OpenId()};

        public static IEnumerable<ApiResource> GetApiResource()
        {
            return new List<ApiResource>
            {
                new("protected-api", "My API which is protected by JWT bearer tokens")
                {
                    Scopes = {"read:entity"}
                },
            };
        }

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[] { new("read:entity", "API"),};

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // SwaggerUI client
                new()
                {
                    ClientId = "client-app",
                    ClientSecrets = {new Secret("secret".Sha256())},
                    AllowedGrantTypes = new[] {GrantType.ClientCredentials},
                    AllowedScopes = new[] {"read:entity"}
                },
            };
    }
}

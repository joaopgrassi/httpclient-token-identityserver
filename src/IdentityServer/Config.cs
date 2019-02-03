// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId()
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource
                {
                    Name = "protected-api",
                    DisplayName = "My API which is protected by JWT bearer tokens",
                    Scopes = new []
                    {
                        new Scope("read:entity")
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new Client[] 
            {
                new Client
                {
                    ClientId = "client-app",
                    AllowedGrantTypes = new [] { GrantType.ClientCredentials },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = new [] { "read:entity" }
                }
            };
        }
    }
}
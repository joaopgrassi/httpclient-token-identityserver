using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ClientApi.ApiClient
{
    /// <summary>
    /// A Delegating Handler that gets an access_token with IdentityServer
    /// prior to calling our protected API.
    /// </summary>
    public class ProtectedApiBearerTokenHandler : DelegatingHandler
    {
        private readonly IIdentityServerClient _identityServerClient;
        private readonly ILogger<ProtectedApiBearerTokenHandler> _logger;

        public ProtectedApiBearerTokenHandler(
            IIdentityServerClient identityServerClient,
            ILogger<ProtectedApiBearerTokenHandler> logger)
        {
            _identityServerClient = identityServerClient ?? throw new ArgumentNullException(nameof(identityServerClient));
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // request the access token token
            // TODO: Add some sort of caching here. 
            var accessToken = await _identityServerClient.RequestClientCredentialsTokenAsync();

            // set the bearer token to the outgoing request
            request.SetBearerToken(accessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace ClientApi.ApiClient
{
    /// <summary>
    /// A Delegating Handler that gets an access_token with IdentityServer
    /// prior to calling our protected API.
    /// </summary>
    public class ProtectedApiBearerTokenHandler : DelegatingHandler
    {
        private readonly IIdentityServerClient _identityServerClient;

        public ProtectedApiBearerTokenHandler(
            IIdentityServerClient identityServerClient)
        {
            _identityServerClient = identityServerClient 
                ?? throw new ArgumentNullException(nameof(identityServerClient));
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            // TODO: In a real app, you would probably want to cache the access_token
            // and re-use it from there. If the request fails, you could then get a new token
            // from the Authorization Server, refresh the cache and re-try your request.

            // request the access token
            var accessToken = await _identityServerClient.RequestClientCredentialsTokenAsync();

            // set the bearer token to the outgoing request
            request.SetBearerToken(accessToken);

            // Proceed calling our "default" handler, that will actually send the request
            // to our protected api
            return await base.SendAsync(request, cancellationToken);
        }
    }
}

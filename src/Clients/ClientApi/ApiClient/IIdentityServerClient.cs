using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;

namespace ClientApi.ApiClient
{
    /// <summary>
    /// A handy client that knows how to get an access_token for a client_credentials flow.
    /// </summary>
    /// <remarks> It receives the ClientCredentialsTokenRequest which contains the client credentials
    /// </remarks>
    public interface IIdentityServerClient
    {
        /// <summary>
        ///  Requests an access_token for our client via the client_credentials OAuth flow.
        /// </summary>
        /// <returns></returns>
        Task<string> RequestClientCredentialsTokenAsync();
    }

    public class IdentityServerClient : IIdentityServerClient
    {
        private readonly HttpClient _httpClient;
        private readonly ClientCredentialsTokenRequest _tokenRequest;
        private readonly ILogger<IdentityServerClient> _logger;

        public IdentityServerClient(
            HttpClient httpClient, 
            ClientCredentialsTokenRequest tokenRequest,
            ILogger<IdentityServerClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _tokenRequest = tokenRequest ?? throw new ArgumentNullException(nameof(tokenRequest));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> RequestClientCredentialsTokenAsync()
        {
            // request the access token token
            var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(_tokenRequest);
            if (tokenResponse.IsError)
            {
                _logger.LogError(tokenResponse.Error);
                throw new HttpRequestException("Something went wrong while requesting the access token");
            }
            return tokenResponse.AccessToken;
        }
    }
}

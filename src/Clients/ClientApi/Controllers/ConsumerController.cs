using System;
using System.Net.Http;
using System.Threading.Tasks;
using ClientApi.ApiClient;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;

namespace ClientApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumerController : ControllerBase
    {
        private readonly IIdentityServerClient _identityServerClient;
        private readonly IProtectedApiClient _protectedApiClient;

        public ConsumerController(
            IIdentityServerClient identityServerClient,
            IProtectedApiClient protectedApiClient)
        {
            _identityServerClient = identityServerClient ?? throw new ArgumentNullException(nameof(identityServerClient));
            _protectedApiClient = protectedApiClient ?? throw new ArgumentNullException(nameof(protectedApiClient));
        }

        // GET api/values
        [HttpGet("version1")]
        public async Task<IActionResult> GetVersionOne()
        {
            // 1. "retrieve" our api credentials. This must be registered on Identity Server!
            var apiClientCredentials = new ClientCredentialsTokenRequest
            {
                Address = "http://httpclient-idsrv/connect/token",

                ClientId = "client-app",
                ClientSecret = "secret",

                // This is the scope our Protected API requires. 
                Scope = "read:entity",
            };

            // creates a new HttpClient to talk to our IdentityServer (localhost:5000)
            var client = new HttpClient();

            // just checks if we can reach the Discovery document. Not 100% needed but..
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "http://httpclient-idsrv",
                Policy = { RequireHttps = false }
            });
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return StatusCode(500);
            }

            // 2. Authenticates and get an access token from Identity Server
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(apiClientCredentials);
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return StatusCode(500);
            }

            // Another HttpClient for talking now with our Protected API
            var apiClient = new HttpClient();

            // 3. Set the access_token in the request Authorization: Bearer <token>
            client.SetBearerToken(tokenResponse.AccessToken);

            // 4. Send a request to our Protected API
            var response = await client.GetAsync("http://protected-api/api/protected");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return StatusCode(500);
            }

            var content = await response.Content.ReadAsStringAsync();

            // All good! We have the data here
            return Ok(content);
        }

        // Uses the IdentityServer HttpClient + the ClientCredentialsTokenRequest
        [HttpGet("version2")]
        public async Task<IActionResult> GetVersionThree()
        {
            // uses our typed HttpClient to get an access_token from identity server
            var accessToken = await _identityServerClient.RequestClientCredentialsTokenAsync();

            // the rest is the same as in version1
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(accessToken);

            var response = await apiClient.GetAsync("http://protected-api/api/protected");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return StatusCode(500);
            }
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        //Uses the typed HttpClient that implicitly gets the access_token from IdentityServer
        [HttpGet("version3")]
        public async Task<IActionResult> GetVersionFour()
        {
            var result = await _protectedApiClient.GetProtectedResources();
            return Ok(result);
        }
    }
}

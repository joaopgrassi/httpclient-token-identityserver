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
        private readonly ClientCredentialsTokenRequest _tokenRequest;
        private readonly IProtectedApiClient _protectedApiClient;
        private readonly HttpClient _identityServerClient;

        public ConsumerController(
            ClientCredentialsTokenRequest tokenRequest,
            IHttpClientFactory httpClientFactory,
            IProtectedApiClient protectedApiClient)
        {
            _tokenRequest = tokenRequest ?? throw new ArgumentNullException(nameof(tokenRequest));
            _protectedApiClient = protectedApiClient ?? throw new ArgumentNullException(nameof(protectedApiClient));
            _identityServerClient = httpClientFactory.CreateClient("IdentityServerClient");
        }

        // GET api/values
        [HttpGet("version1")]
        public async Task<IActionResult> GetVersionOne()
        {
            // discover endpoints from metadata
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return StatusCode(500);
            }

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = "http://localhost:5000/connect/token",

                ClientId = "client-app",
                ClientSecret = "secret",
                Scope = "read:entity"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return StatusCode(500);
            }

            var apiClient = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5002/api/protected");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return StatusCode(500);
            }
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        // Uses the Singleton ClientCredentialsTokenRequest
        [HttpGet("version2")]
        public async Task<IActionResult> GetVersionTwo()
        {
            // discover endpoints from metadata
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return StatusCode(500);
            }

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(_tokenRequest);
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return StatusCode(500);
            }

            var apiClient = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5002/api/protected");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return StatusCode(500);
            }
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        // Uses the IdentityServer HttpClient + the ClientCredentialsTokenRequest
        [HttpGet("version3")]
        public async Task<IActionResult> GetVersionThree()
        {
            // request token
            var tokenResponse = await _identityServerClient.RequestClientCredentialsTokenAsync(_tokenRequest);
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return StatusCode(500);
            }

            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("http://localhost:5002/api/protected");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return StatusCode(500);
            }
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        // Uses the typed HttpClient that implicitly gets the access_token from IdentityServer
        [HttpGet("version4")]
        public async Task<IActionResult> GetVersionFour()
        {
            var result = await _protectedApiClient.GetProtectedResources();            
            return Ok(result);
        }
    }
}

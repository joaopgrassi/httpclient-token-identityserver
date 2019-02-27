using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientApi.ApiClient
{
    public class ProtectedApiClient : IProtectedApiClient
    {
        private readonly HttpClient _httpClient;

        public ProtectedApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> GetProtectedResources()
        {
            // No more getting access_tokens code!

            var response = await _httpClient.GetAsync("/api/protected");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                throw new Exception("Failed to get protected resources.");
            }
            return await response.Content.ReadAsStringAsync();
        }
    }
}

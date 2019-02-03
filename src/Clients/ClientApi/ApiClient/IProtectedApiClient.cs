using System.Threading.Tasks;

namespace ClientApi.ApiClient
{
    /// <summary>
    /// A handy client to talk to our external protected API
    /// </summary>
    public interface IProtectedApiClient
    {
        /// <summary>
        /// Gets the protected resources from an external API
        /// </summary>
        /// <returns></returns>
        Task<string> GetProtectedResources();
    }
}

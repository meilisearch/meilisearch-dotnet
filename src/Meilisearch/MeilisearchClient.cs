using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Meilisearch
{
    /// <summary>
    /// Typed client for Meilisearch. 
    /// </summary>
    public class MeilisearchClient
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Typed client for Meilisearch API. Use it with proper Http Client Factory.
        /// </summary>
        /// <param name="client">Injects the reusable Httpclient </param>
        public MeilisearchClient(HttpClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Get the current version Meilisearch. For more details on response
        /// https://docs.meilisearch.com/references/version.html#get-version-of-meilisearch
        /// </summary>
        /// <returns>Returns the Meilisearch Version with commit and Build version.</returns>
        public async  Task<MeiliSearchVersion> GetVersion()
        {
           var response = await _client.GetAsync("/version");
           response.EnsureSuccessStatusCode();
           var content = await response.Content.ReadAsStringAsync();
           return JsonConvert.DeserializeObject<MeiliSearchVersion>(content);
        }
    }
}
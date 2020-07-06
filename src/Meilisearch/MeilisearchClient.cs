using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Meilisearch
{
    /// <summary>
    /// Typed client for MeiliSearch.
    /// </summary>
    public class MeilisearchClient
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Basic Meilisearch client with master Key.
        /// </summary>
        /// <param name="url">URL to connect to meilisearch client</param>
        /// <param name="masterKey">Master key for the usage</param>
        public MeilisearchClient(string url, string masterKey=default)
        {
            _client = new HttpClient {BaseAddress = new Uri(url)};
            if (!string.IsNullOrEmpty(masterKey))
            {
                _client.DefaultRequestHeaders.Add("X-Meili-API-Key", masterKey);
            }
        }

        /// <summary>
        /// Typed client for Meilisearch API. Use it with proper Http Client Factory.
        /// </summary>
        /// <param name="client">Injects the reusable Httpclient </param>
        /// <param name="masterKey">Master Key for Meilisearchclient. Best practice is to use Httpclientfactory default header rather than master Key.</param>
        public MeilisearchClient(HttpClient client,string masterKey=default)
        {
            _client = client;
            if (!string.IsNullOrEmpty(masterKey))
            {
                _client.DefaultRequestHeaders.Add("X-Meili-API-Key", masterKey);
            }
        }

        /// <summary>
        /// Get the current version MeiliSearch. For more details on response
        /// https://docs.meilisearch.com/references/version.html#get-version-of-meilisearch
        /// </summary>
        /// <returns>Returns the MeiliSearch Version with commit and Build version.</returns>
        public async Task<MeiliSearchVersion> GetVersion()
        {
            var response = await _client.GetAsync("/version");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MeiliSearchVersion>();
        }

        /// <summary>
        ///  Create Index with Unique name and Primary Key.
        /// BEWARE : Throws error If the Index already exist. Use GetIndex before using Create.
        /// </summary>
        /// <param name="uid">Unique Id</param>
        /// <param name="primaryKey">Primary key for Operation.</param>
        /// <returns>Index for the future operation.</returns>
        public async Task<Index> CreateIndex(string uid,string primaryKey=default)
        {
            Index index = new Index(uid, primaryKey);
            var response = await _client.PostAsJsonAsync("/indexes",index);
            // TODO : Revisit the Exception, We need to handle it better .
            return response.IsSuccessStatusCode? index.WithHttpClient(this._client) : throw new Exception("Not able to create index. May be Index already exist");
        }

        /// <summary>
        /// Get All the Indexes for the instance. Throws error if there is no indexes found.
        /// Need to handle an empty Enumerable.
        /// </summary>
        /// <returns>Return Enumerable of Index.</returns>
        public async Task<IEnumerable<Index>> GetAllIndexes()
        {
            var response = await _client.GetAsync("/indexes");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<IEnumerable<Index>>();
            return content
                .Select(p => p.WithHttpClient(_client));
        }

        /// <summary>
        /// Get Index for the unique ID.
        /// </summary>
        /// <param name="uid">Unique Id for the index.</param>
        /// <returns>returns Index or Null if the index is not found.</returns>
        public async Task<Index> GetIndex(string uid)
        {
            var response = await _client.GetAsync($"/indexes/{uid}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<Index>();
                return content.WithHttpClient(_client);
            }

            return null;  // TODO:  Yikes!! returning Null  Need to come back to solve this.
        }
    }
}

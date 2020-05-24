using System;
using System.Net.Http;
using System.Text;
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
        public async Task<MeiliSearchVersion> GetVersion()
        {
            var response = await _client.GetAsync("/version");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MeiliSearchVersion>(content);
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
            var content = JsonConvert.SerializeObject(index);
            var request = new HttpRequestMessage(HttpMethod.Post, "/indexes");
            request.Content = new StringContent(content,Encoding.UTF8,"application/json");
            var response = await _client.SendAsync(request);
            // TODO : Revisit the Exception, We need to handle it better .
            return response.IsSuccessStatusCode? index : throw new Exception("Not able to create index. May be Index already exist");
        }
    }
}
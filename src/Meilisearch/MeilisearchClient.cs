namespace Meilisearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// Typed client for MeiliSearch.
    /// </summary>
    public class MeilisearchClient
    {
        private readonly HttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchClient"/> class.
        /// Default client for Meilisearch API.
        /// </summary>
        /// <param name="url">URL to connect to meilisearch client.</param>
        /// <param name="apiKey">API key for the usage.</param>
        public MeilisearchClient(string url, string apiKey = default)
        {
            this.client = new HttpClient { BaseAddress = new Uri(url) };
            if (!string.IsNullOrEmpty(apiKey))
            {
                this.client.DefaultRequestHeaders.Add("X-Meili-API-Key", apiKey);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchClient"/> class.
        /// Custom client for Meilisearch API. Use it with proper Http Client Factory.
        /// </summary>
        /// <param name="client">Injects the reusable Httpclient.</param>
        /// <param name="apiKey">API Key for MeilisearchClient. Best practice is to use HttpClient default header rather than this parameter.</param>
        public MeilisearchClient(HttpClient client, string apiKey = default)
        {
            this.client = client;
            if (!string.IsNullOrEmpty(apiKey))
            {
                this.client.DefaultRequestHeaders.Add("X-Meili-API-Key", apiKey);
            }
        }

        /// <summary>
        /// Gets the current MeiliSearch version. For more details on response
        /// https://docs.meilisearch.com/reference/api/version.html#get-version-of-meilisearch.
        /// </summary>
        /// <returns>Returns the MeiliSearch version with commit and build version.</returns>
        public async Task<MeiliSearchVersion> GetVersion()
        {
            var response = await this.client.GetAsync("/version");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MeiliSearchVersion>();
        }

        /// <summary>
        /// Creates and index with an UID and a primary key.
        /// BEWARE : Throws error if the index already exist. Use GetIndex before using Create.
        /// </summary>
        /// <param name="uid">Unique Id.</param>
        /// <param name="primaryKey">Primary key for documents.</param>
        /// <returns>Returns Index.</returns>
        public async Task<Index> CreateIndex(string uid, string primaryKey = default)
        {
            Index index = new Index(uid, primaryKey);
            var response = await this.client.PostAsJsonAsync("/indexes", index);

            // TODO : Revisit the Exception, We need to handle it better.
            return response.IsSuccessStatusCode ? index.WithHttpClient(this.client) : throw new Exception("Not able to create index. May be Index already exist");
        }

        /// <summary>
        /// Gets all the Indexes for the instance. Throws error if the index does not exist.
        /// </summary>
        /// <returns>Return Enumerable of Index.</returns>
        public async Task<IEnumerable<Index>> GetAllIndexes()
        {
            var response = await this.client.GetAsync("/indexes");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<IEnumerable<Index>>();
            return content
                .Select(p => p.WithHttpClient(this.client));
        }

        /// <summary>
        /// Gets and index with the unique ID.
        /// </summary>
        /// <param name="uid">UID of the index.</param>
        /// <returns>Returns Index or Null if the index does not exist.</returns>
        public async Task<Index> GetIndex(string uid)
        {
            var response = await this.client.GetAsync($"/indexes/{uid}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<Index>();
                return content.WithHttpClient(this.client);
            }

            return null;  // TODO:  Yikes!! returning Null  Need to come back to solve this.
        }

        /// <summary>
        /// Gets the index instance or creates the index if it does not exist.
        ///
        /// /!\ Really basics. An error handler should be created to check the errorCode.
        /// cf https://docs.meilisearch.com/errors/#index_already_exists.
        /// </summary>
        /// <param name="uid">Unique Id.</param>
        /// <param name="primaryKey">Primary key for documents.</param>
        /// <returns>Returns Index.</returns>
        public async Task<Index> GetOrCreateIndex(string uid, string primaryKey = default)
        {
            try
            {
                return await this.CreateIndex(uid, primaryKey);
            }
            catch (Exception e)
            {
                if (e.Message == "Not able to create index. May be Index already exist")
                {
                    return await this.GetIndex(uid);
                }
                else
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Gets stats of all indexes.
        /// </summary>
        /// <returns>Returns stats of all indexes.</returns>
        public Task<Stats> GetStats()
        {
            return this.client.GetFromJsonAsync<Stats>("/stats");
        }

        /// <summary>
        /// Gets health state of the server.
        /// </summary>
        /// <returns>Returns whether server is healthy or throw an error.</returns>
        public async Task<MeiliSearchHealth> Health()
        {
            var response = await this.client.GetAsync("/health");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MeiliSearchHealth>();
        }

        /// <summary>
        /// Gets health state of the server.
        /// </summary>
        /// <returns>Returns whether server is healthy or not.</returns>
        public async Task<bool> IsHealthy()
        {
            try
            {
                await this.Health();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates Dump process.
        /// </summary>
        /// <returns>Returns dump creation status with uid and processing status.</returns>
        public async Task<DumpStatus> CreateDump()
        {
            var response = await this.client.PostAsync("/dumps", default, default);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<DumpStatus>()
                : throw new Exception("Another dump is already in progress");
        }

        /// <summary>
        /// Gets a dump creation status.
        /// </summary>
        /// <param name="uid">unique dump identifier.</param>
        /// <returns>Returns dump creation status with uid and processing status.</returns>
        public async Task<DumpStatus> GetDumpStatus(string uid)
        {
            var response = await this.client.GetAsync($"/dumps/{uid}/status");

            return await response.Content.ReadFromJsonAsync<DumpStatus>();
        }
    }
}

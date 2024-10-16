using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Meilisearch.Extensions;
using Meilisearch.QueryParameters;

namespace Meilisearch
{
    /// <summary>
    /// Typed client for Meilisearch.
    /// </summary>
    public class MeilisearchClient
    {
        private readonly HttpClient _http;
        private TaskEndpoint _taskEndpoint;
        public string ApiKey { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchClient"/> class.
        /// Default client for Meilisearch API.
        /// </summary>
        /// <param name="url">URL corresponding to Meilisearch server.</param>
        /// <param name="apiKey">API Key to connect to the Meilisearch server.</param>
        public MeilisearchClient(string url, string apiKey = default) : this(
            new HttpClient(new MeilisearchMessageHandler(new HttpClientHandler())) { BaseAddress = url.ToSafeUri() },
            apiKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchClient"/> class.
        /// Custom client for Meilisearch API. Use it with proper Http Client Factory.
        /// </summary>
        /// <param name="client">Injects the reusable HttpClient.</param>
        /// <param name="apiKey">API Key to connect to the Meilisearch server. Best practice is to use HttpClient default header rather than this parameter.</param>
        public MeilisearchClient(HttpClient client, string apiKey = default)
        {
            client.BaseAddress = client.BaseAddress.OriginalString.ToSafeUri();
            _http = client;
            _http.AddApiKeyToHeader(apiKey);
            _http.AddDefaultUserAgent();
            ApiKey = apiKey;
        }

        /// <summary>
        /// Gets the current Meilisearch version. For more details on response.
        /// https://www.meilisearch.com/docs/reference/api/version#get-version-of-meilisearch.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the Meilisearch version with commit and build version.</returns>
        public async Task<MeiliSearchVersion> GetVersionAsync(CancellationToken cancellationToken = default)
        {
            var response = await _http.GetAsync("version", cancellationToken).ConfigureAwait(false);

            return await response.Content.ReadFromJsonAsync<MeiliSearchVersion>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Create a local reference to an index identified by UID, without doing an HTTP call.
        /// Calling this method doesn't create an index in the Meilisearch instance, but grants access to all the other methods in the Index class.
        /// </summary>
        /// <param name="uid">Unique identifier of the index.</param>
        /// <returns>Returns an Index instance.</returns>
        public Index Index(string uid)
        {
            var index = new Index(uid);
            index.WithHttpClient(_http);
            return index;
        }

        /// <summary>
        /// Searches multiple indexes at once but get a aggregated result
        /// </summary>
        /// <param name="query">The queries to be executed (must have IndexUid set)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Aggregated results</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<ISearchable<T>> FederatedMultiSearchAsync<T>(FederatedMultiSearchQuery query,
            CancellationToken cancellationToken = default)
        {
            if (!query.Queries.TrueForAll(x => x.IndexUid != null))
            {
                throw new ArgumentNullException("IndexUid", "IndexUid should be provided for all search queries");
            }

            var responseMessage = await _http.PostAsJsonAsync("multi-search", query,
                Constants.FederatedSearchJsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken);
            return await responseMessage.Content
                .ReadFromJsonAsync<ISearchable<T>>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Searches multiple indexes at once
        /// </summary>
        /// <param name="query">The queries to be executed (must have IndexUid set)</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<MultiSearchResult> MultiSearchAsync(MultiSearchQuery query,
            CancellationToken cancellationToken = default)
        {
            if (!query.Queries.TrueForAll(x => x.IndexUid != null))
            {
                throw new ArgumentNullException("IndexUid", "IndexUid should be provided for all search queries");
            }

            var responseMessage = await _http.PostAsJsonAsync("multi-search", query,
                Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken);
            return await responseMessage.Content
                .ReadFromJsonAsync<MultiSearchResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Creates and index with an UID and a primary key.
        /// </summary>
        /// <param name="uid">Unique identifier of the index.</param>
        /// <param name="primaryKey">Primary key for documents.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the associated task.</returns>
        public async Task<TaskInfo> CreateIndexAsync(string uid, string primaryKey = default,
            CancellationToken cancellationToken = default)
        {
            var index = new Index(uid, primaryKey);
            var responseMessage = await _http.PostJsonCustomAsync("indexes", index,
                    Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Changes the primary key of the index.
        /// </summary>
        /// <param name="uid">Unique identifier of the index.</param>
        /// <param name="primarykeytoChange">Primary key set.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the associated task.</returns>
        public async Task<TaskInfo> UpdateIndexAsync(string uid, string primarykeytoChange,
            CancellationToken cancellationToken = default)
        {
            return await Index(uid).UpdateAsync(primarykeytoChange, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the index.
        /// It's not a recovery delete. You will also lose the documents within the index.
        /// </summary>
        /// <param name="uid">Unique identifier of the index.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the associated task.</returns>
        public async Task<TaskInfo> DeleteIndexAsync(string uid, CancellationToken cancellationToken = default)
        {
            return await Index(uid).DeleteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all the raw indexes for the instance as returned by the resposne of the Meilisearch server. Throws error if the index does not exist.
        /// </summary>
        /// <param name="query">Query parameters. Supports limit and offset.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>An IEnumerable of indexes in JsonElement format.</returns>
        public async Task<JsonDocument> GetAllRawIndexesAsync(IndexesQuery query = default,
            CancellationToken cancellationToken = default)
        {
            var uri = query.ToQueryString(uri: "indexes");
            var response = await _http.GetAsync(uri, cancellationToken).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonDocument.Parse(content);
        }

        /// <summary>
        /// Gets all the Indexes for the instance. Throws error if the index does not exist.
        /// </summary>
        /// <param name="query">Query parameters. Supports limit and offset.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Return Enumerable of Index.</returns>
        public async Task<ResourceResults<IEnumerable<Index>>> GetAllIndexesAsync(IndexesQuery query = default,
            CancellationToken cancellationToken = default)
        {
            var uri = query.ToQueryString(uri: "indexes");
            var response = await _http.GetAsync(uri, cancellationToken).ConfigureAwait(false);

            var content = await response.Content
                .ReadFromJsonAsync<ResourceResults<IEnumerable<Index>>>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            content.Results
                .Select(p => p.WithHttpClient(_http))
                .ToList();
            return content;
        }

        /// <summary>
        /// Gets an index with the unique ID.
        /// </summary>
        /// <param name="uid">Unique identifier of the index.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns Index.</returns>
        /// <exception cref="MeilisearchApiError">Throws if the index doesn't exist.</exception>
        public async Task<Index> GetIndexAsync(string uid, CancellationToken cancellationToken = default)
        {
            return await Index(uid).FetchInfoAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets an index in raw format.
        /// </summary>
        /// <param name="uid">Unique identifier of the index.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>A <see cref="JsonElement"/> which represents the raw index as a JSON object.</returns>
        public async Task<JsonElement> GetRawIndexAsync(string uid, CancellationToken cancellationToken = default)
        {
            var json = await (
                    await Meilisearch.Index.GetRawAsync(_http, uid, cancellationToken).ConfigureAwait(false))
                .Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonDocument.Parse(json).RootElement;
        }

        /// <summary>
        /// Gets the tasks.
        /// </summary>
        /// <param name="query">Query parameters supports by the method.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns a list of tasks.</returns>
        public async Task<Result<IEnumerable<TaskResource>>> GetTasksAsync(TasksQuery query = default,
            CancellationToken cancellationToken = default)
        {
            return await TaskEndpoint().GetTasksAsync(query, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get on task.
        /// </summary>
        /// <param name="taskUid">Unique identifier of the task.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Return the task.</returns>
        public async Task<TaskResource> GetTaskAsync(int taskUid, CancellationToken cancellationToken = default)
        {
            return await TaskEndpoint().GetTaskAsync(taskUid, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Waits until the asynchronous task was done.
        /// </summary>
        /// <param name="taskUid">Unique identifier of the asynchronous task.</param>
        /// <param name="timeoutMs">Timeout in millisecond.</param>
        /// <param name="intervalMs">Interval in millisecond between each check.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of finished task.</returns>
        public async Task<TaskResource> WaitForTaskAsync(
            int taskUid,
            double timeoutMs = 5000.0,
            int intervalMs = 50,
            CancellationToken cancellationToken = default)
        {
            return await TaskEndpoint().WaitForTaskAsync(taskUid, timeoutMs, intervalMs, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets stats of all indexes.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns stats of all indexes.</returns>
        public async Task<Stats> GetStatsAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<Stats>("stats", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets health state of the server.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns whether server is healthy or throw an error.</returns>
        public async Task<MeiliSearchHealth> HealthAsync(CancellationToken cancellationToken = default)
        {
            var response = await _http.GetAsync("health", cancellationToken).ConfigureAwait(false);

            return await response.Content.ReadFromJsonAsync<MeiliSearchHealth>(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Gets health state of the server.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns whether server is healthy or not.</returns>
        public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await HealthAsync(cancellationToken).ConfigureAwait(false);
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
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns dump creation status with uid and processing status.</returns>
        public async Task<TaskInfo> CreateDumpAsync(CancellationToken cancellationToken = default)
        {
            var response = await _http.PostAsync("dumps", default, cancellationToken).ConfigureAwait(false);

            return await response.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the API keys.
        /// </summary>
        /// <param name="query">Query parameters supports by the method.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns a list of the API keys.</returns>
        public async Task<ResourceResults<IEnumerable<Key>>> GetKeysAsync(KeysQuery query = default,
            CancellationToken cancellationToken = default)
        {
            var uri = query.ToQueryString(uri: "keys");
            return await _http
                .GetFromJsonAsync<ResourceResults<IEnumerable<Key>>>(uri, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets one API key.
        /// </summary>
        /// <param name="keyOrUid">Unique identifier of the API key or the Key.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the API key information.</returns>
        public async Task<Key> GetKeyAsync(string keyOrUid, CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<Key>($"keys/{keyOrUid}", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Creates an API key for the Meilisearch server.
        /// </summary>
        /// <param name="keyOptions">The options of the API key.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the created API key.</returns>
        public async Task<Key> CreateKeyAsync(Key keyOptions, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PostAsJsonAsync("keys", keyOptions, Constants.JsonSerializerOptionsWriteNulls,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            return await responseMessage.Content.ReadFromJsonAsync<Key>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Cancel tasks given a specific query.
        /// </summary>
        /// <param name="query">Query parameters supports by the method.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of finished task.</returns>
        public async Task<TaskInfo> CancelTasksAsync(CancelTasksQuery query,
            CancellationToken cancellationToken = default)
        {
            return await TaskEndpoint().CancelTasksAsync(query, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete tasks given a specific query.
        /// </summary>
        /// <param name="query">Query parameters supports by the method.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of finished task.</returns>
        public async Task<TaskInfo> DeleteTasksAsync(DeleteTasksQuery query,
            CancellationToken cancellationToken = default)
        {
            return await TaskEndpoint().DeleteTasksAsync(query, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates an API key for the Meilisearch server.
        /// </summary>
        /// <param name="keyOrUid">Unique identifier of the API key or the Key</param>
        /// <param name="description">A description to give meaning to the key.</param>
        /// <param name="name">A name to label the key internally.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the updated API key.</returns>
        public async Task<Key> UpdateKeyAsync(string keyOrUid, string description = null, string name = null,
            CancellationToken cancellationToken = default)
        {
            var key = new Key { Name = name, Description = description };

            var responseMessage =
                await _http.PatchAsJsonAsync($"keys/{keyOrUid}", key, Constants.JsonSerializerOptionsRemoveNulls,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            return await responseMessage.Content.ReadFromJsonAsync<Key>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes an API key from the Meilisearch server.
        /// </summary>
        /// <param name="keyOrUid">Unique identifier of the API key or the Key</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns true if the API key was deleted.</returns>
        public async Task<bool> DeleteKeyAsync(string keyOrUid, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.DeleteAsync($"keys/{keyOrUid}", cancellationToken: cancellationToken).ConfigureAwait(false);
            return responseMessage.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Swaps indexes unique identifiers.
        /// </summary>
        /// <param name="indexes">List of IndexSwap objects.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of finished task.</returns>
        public async Task<TaskInfo> SwapIndexesAsync(List<IndexSwap> indexes,
            CancellationToken cancellationToken = default)
        {
            var response = await _http.PostAsJsonAsync("swap-indexes", indexes,
                    Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await response.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Generate a tenant token string to be used during search.
        /// </summary>
        /// <param name="apiKeyUid">Unique identifier of the API key.</param>
        /// <param name="searchRules">Object with the rules applied in a search call.</param>
        /// <param name="apiKey">API Key which signs the generated token.</param>
        /// <param name="expiresAt">Date to express how long the generated token will last. If null the token will last forever.</param>
        /// <exception cref="MeilisearchTenantTokenApiKeyInvalid">When there is no <paramref name="apiKey"/> defined in the client or as argument.</exception>
        /// <exception cref="MeilisearchTenantTokenExpired">When the sent <paramref name="expiresAt"/> param is in the past</exception>
        /// <returns>Returns a generated tenant token.</returns>
        public string GenerateTenantToken(string apiKeyUid, TenantTokenRules searchRules, string apiKey = null,
            DateTime? expiresAt = null)
        {
            return TenantToken.GenerateToken(apiKeyUid, searchRules, apiKey ?? ApiKey, expiresAt);
        }

        /// <summary>
        /// Create a local reference to a task, without doing an HTTP call.
        /// </summary>
        /// <returns>Returns a Task instance.</returns>
        private TaskEndpoint TaskEndpoint()
        {
            if (_taskEndpoint == null)
            {
                _taskEndpoint = new TaskEndpoint();
                _taskEndpoint.WithHttpClient(_http);
            }

            return _taskEndpoint;
        }
    }
}

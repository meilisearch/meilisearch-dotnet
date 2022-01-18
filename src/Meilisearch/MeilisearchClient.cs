namespace Meilisearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Meilisearch.Extensions;

    /// <summary>
    /// Typed client for MeiliSearch.
    /// </summary>
    public class MeilisearchClient
    {
        /// <summary>
        /// JsonSerializer options used when serializing objects.
        /// </summary>
        public static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly HttpClient http;
        private TaskEndpoint _taskEndpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchClient"/> class.
        /// Default client for MeiliSearch API.
        /// </summary>
        /// <param name="url">URL corresponding to MeiliSearch server.</param>
        /// <param name="apiKey">API Key to connect to the MeiliSearch server.</param>
        public MeilisearchClient(string url, string apiKey = default)
        {
            this.http = new HttpClient(new MeilisearchMessageHandler(new HttpClientHandler())) { BaseAddress = new Uri(url) };
            this.http.AddApiKeyToHeader(apiKey);
            this._taskEndpoint = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchClient"/> class.
        /// Custom client for MeiliSearch API. Use it with proper Http Client Factory.
        /// </summary>
        /// <param name="client">Injects the reusable HttpClient.</param>
        /// <param name="apiKey">API Key to connect to the MeiliSearch server. Best practice is to use HttpClient default header rather than this parameter.</param>
        public MeilisearchClient(HttpClient client, string apiKey = default)
        {
            this.http = client;
            this.http.AddApiKeyToHeader(apiKey);
        }

        /// <summary>
        /// Gets the current MeiliSearch version. For more details on response.
        /// https://docs.meilisearch.com/reference/api/version.html#get-version-of-meilisearch.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the MeiliSearch version with commit and build version.</returns>
        public async Task<MeiliSearchVersion> GetVersionAsync(CancellationToken cancellationToken = default)
        {
            var response = await this.http.GetAsync("/version", cancellationToken).ConfigureAwait(false);

            return await response.Content.ReadFromJsonAsync<MeiliSearchVersion>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a local reference to an index identified by UID, without doing an HTTP call.
        /// Calling this method doesn't create an index in the MeiliSearch instance, but grants access to all the other methods in the Index class.
        /// </summary>
        /// <param name="uid">Unique Id.</param>
        /// <returns>Returns an Index instance.</returns>
        public Index Index(string uid)
        {
            Index index = new Index(uid);
            index.WithHttpClient(this.http);
            return index;
        }

        /// <summary>
        /// Creates and index with an UID and a primary key.
        /// BEWARE : Throws error if the index already exist. Use GetIndex before using Create.
        /// </summary>
        /// <param name="uid">Unique Id.</param>
        /// <param name="primaryKey">Primary key for documents.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns Index.</returns>
        public async Task<TaskInfo> CreateIndexAsync(string uid, string primaryKey = default, CancellationToken cancellationToken = default)
        {
            Index index = new Index(uid, primaryKey);
            var responseMessage = await this.http.PostJsonCustomAsync("/indexes", index, JsonSerializerOptions, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Changes the primary key of the index.
        /// </summary>
        /// <param name="uid">Unique Id.</param>
        /// <param name="primarykeytoChange">Primary key set.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns Index.</returns>
        public async Task<TaskInfo> UpdateIndexAsync(string uid, string primarykeytoChange, CancellationToken cancellationToken = default)
        {
            return await this.Index(uid).UpdateAsync(primarykeytoChange, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the index.
        /// It's not a recovery delete. You will also lose the documents within the index.
        /// </summary>
        /// <param name="uid">unique dump identifier.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task.</returns>
        public async Task<TaskInfo> DeleteIndexAsync(string uid, CancellationToken cancellationToken = default)
        {
            return await this.Index(uid).DeleteAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all the raw indexes for the instance as returned by the resposne of the Meilisearch server. Throws error if the index does not exist.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>An IEnumerable of indexes in JsonElement format.</returns>
        public async Task<IEnumerable<JsonElement>> GetAllRawIndexesAsync(CancellationToken cancellationToken = default)
        {
            var response = await this.http.GetAsync("/indexes", cancellationToken).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JsonDocument.Parse(content);
            List<JsonElement> indexes = new List<JsonElement>();

            foreach (var element in json.RootElement.EnumerateArray())
            {
                indexes.Add(element);
            }

            return indexes;
        }

        /// <summary>
        /// Gets all the Indexes for the instance. Throws error if the index does not exist.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Return Enumerable of Index.</returns>
        public async Task<IEnumerable<Index>> GetAllIndexesAsync(CancellationToken cancellationToken = default)
        {
            var response = await this.http.GetAsync("/indexes", cancellationToken).ConfigureAwait(false);

            var content = await response.Content.ReadFromJsonAsync<IEnumerable<Index>>(cancellationToken: cancellationToken).ConfigureAwait(false);
            return content
                .Select(p => p.WithHttpClient(this.http));
        }

        /// <summary>
        /// Gets and index with the unique ID.
        /// </summary>
        /// <param name="uid">UID of the index.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns Index or Null if the index does not exist.</returns>
        public async Task<Index> GetIndexAsync(string uid, CancellationToken cancellationToken = default)
        {
            return await this.Index(uid).FetchInfoAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets an index in raw format.
        /// </summary>
        /// <param name="uid">UID of the index to get.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>A <see cref="JsonElement"/> which represents the raw index as a JSON object.</returns>
        public async Task<JsonElement> GetRawIndexAsync(string uid, CancellationToken cancellationToken = default)
        {
            var json = await (
                await Meilisearch.Index.GetRawAsync(this.http, uid, cancellationToken).ConfigureAwait(false))
                .Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonDocument.Parse(json).RootElement;
        }

        /// <summary>
        /// Gets the tasks.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns a list of tasks.</returns>
        public async Task<Result<IEnumerable<TaskInfo>>> GetTasksAsync(CancellationToken cancellationToken = default)
        {
            return await this.TaskEndpoint().GetTasksAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get on task.
        /// </summary>
        /// <param name="taskUid">Uid of the task.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Return the task.</returns>
        public async Task<TaskInfo> GetTaskAsync(int taskUid, CancellationToken cancellationToken = default)
        {
            return await this.TaskEndpoint().GetTaskAsync(taskUid, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Waits until the asynchronous task was done.
        /// </summary>
        /// <param name="taskUid">Unique identifier of the asynchronous task.</param>
        /// <param name="timeoutMs">Timeout in millisecond.</param>
        /// <param name="intervalMs">Interval in millisecond between each check.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of finished task.</returns>
        public async Task<TaskInfo> WaitForTaskAsync(
            int taskUid,
            double timeoutMs = 5000.0,
            int intervalMs = 50,
            CancellationToken cancellationToken = default)
        {
            return await this.TaskEndpoint().WaitForTaskAsync(taskUid, timeoutMs, intervalMs, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets stats of all indexes.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns stats of all indexes.</returns>
        public async Task<Stats> GetStats(CancellationToken cancellationToken = default)
        {
            return await this.http.GetFromJsonAsync<Stats>("/stats", cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets health state of the server.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns whether server is healthy or throw an error.</returns>
        public async Task<MeiliSearchHealth> HealthAsync(CancellationToken cancellationToken = default)
        {
            var response = await this.http.GetAsync("/health", cancellationToken).ConfigureAwait(false);

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
                await this.HealthAsync(cancellationToken).ConfigureAwait(false);
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
        public async Task<DumpStatus> CreateDumpAsync(CancellationToken cancellationToken = default)
        {
            var response = await this.http.PostAsync("/dumps", default, cancellationToken).ConfigureAwait(false);

            return await response.Content.ReadFromJsonAsync<DumpStatus>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a dump creation status.
        /// </summary>
        /// <param name="uid">unique dump identifier.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns dump creation status with uid and processing status.</returns>
        public async Task<DumpStatus> GetDumpStatusAsync(string uid, CancellationToken cancellationToken = default)
        {
            var response = await this.http.GetAsync($"/dumps/{uid}/status", cancellationToken).ConfigureAwait(false);

            return await response.Content.ReadFromJsonAsync<DumpStatus>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a local reference to a task, without doing an HTTP call.
        /// </summary>
        /// <returns>Returns a Task instance.</returns>
        private TaskEndpoint TaskEndpoint()
        {
            if (this._taskEndpoint == null)
            {
                this._taskEndpoint = new TaskEndpoint();
                this._taskEndpoint.WithHttpClient(this.http);
            }

            return this._taskEndpoint;
        }
    }
}

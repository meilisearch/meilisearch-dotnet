using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Meilisearch
{

    /// <summary>
    /// Meilisearch index to search and manage documents.
    /// </summary>
    public partial class Index
    {
        private HttpClient _http;
        private TaskEndpoint _taskEndpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="Index"/> class.
        /// Initializes with the UID (mandatory) and the primary key.
        /// </summary>
        /// <param name="uid">Unique index identifier.</param>
        /// <param name="primaryKey">Documents primary key.</param>
        /// <param name="createdAt">The creation date of the index.</param>
        /// <param name="updatedAt">The latest update of the index.</param>
        public Index(string uid, string primaryKey = default, DateTimeOffset? createdAt = default, DateTimeOffset? updatedAt = default)
        {
            Uid = uid;
            PrimaryKey = primaryKey;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            _taskEndpoint = null;
        }

        /// <summary>
        /// Gets unique identifier of the index.
        /// </summary>
        public string Uid { get; internal set; }

        /// <summary>
        /// Gets primary key of the documents.
        /// </summary>
        public string PrimaryKey { get; internal set; }

        /// <summary>
        /// Gets the latest update date of the index.
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; internal set; }

        /// <summary>
        /// Gets the creation date of the index.
        /// </summary>
        public DateTimeOffset? CreatedAt { get; internal set; }

        /// <summary>
        /// Gets raw index call response.
        /// </summary>
        /// <param name="http">HTTP client to make the call.</param>
        /// <param name="uid">Uid of the index to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Call response.</returns>
        public static async Task<HttpResponseMessage> GetRawAsync(HttpClient http, string uid, CancellationToken cancellationToken = default)
        {
            return await http.GetAsync($"indexes/{uid}", cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Fetch the info of the index.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>An instance of the index fetch.</returns>
        public async Task<Index> FetchInfoAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetRawAsync(_http, Uid, cancellationToken).ConfigureAwait(false);
            var content = await response.Content.ReadFromJsonAsync<Index>(cancellationToken: cancellationToken).ConfigureAwait(false);
            PrimaryKey = content.PrimaryKey;
            CreatedAt = content.CreatedAt;
            UpdatedAt = content.UpdatedAt;
            return this;
        }

        /// <summary>
        /// Fetch the primary key of the index.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Primary key of the fetched index.</returns>
        public async Task<string> FetchPrimaryKey(CancellationToken cancellationToken = default)
        {
            return (await FetchInfoAsync(cancellationToken).ConfigureAwait(false)).PrimaryKey;
        }

        /// <summary>
        /// Changes the primary key of the index.
        /// </summary>
        /// <param name="primarykeytoChange">Primary key set.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Index with the updated Primary Key.</returns>
        public async Task<TaskInfo> UpdateAsync(string primarykeytoChange, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PutAsJsonAsync($"indexes/{Uid}", new { primaryKey = primarykeytoChange }, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the index.
        /// It's not a recovery delete. You will also lose the documents within the index.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info.</returns>
        public async Task<TaskInfo> DeleteAsync(CancellationToken cancellationToken = default)
        {
            var responseMessage = await _http.DeleteAsync($"indexes/{Uid}", cancellationToken).ConfigureAwait(false);

            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get stats.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Return index stats.</returns>
        public async Task<IndexStats> GetStatsAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<IndexStats>($"indexes/{Uid}/stats", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Initializes the Index with HTTP client. Only for internal usage.
        /// </summary>
        /// <param name="http">HttpRequest instance used.</param>
        /// <returns>The same object with the initialization.</returns>
        // internal Index WithHttpClient(HttpClient client)
        internal Index WithHttpClient(HttpClient http)
        {
            _http = http;
            return this;
        }

        /// <summary>
        /// Create a local reference to a task, without doing an HTTP call.
        /// </summary>
        /// <returns>Returns a TaskEndpoint instance.</returns>
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

namespace Meilisearch
{
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

    /// <summary>
    /// MeiliSearch index to search and manage documents.
    /// </summary>
    public class Index
    {
        private HttpClient http;

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
            this.Uid = uid;
            this.PrimaryKey = primaryKey;
            this.CreatedAt = createdAt;
            this.UpdatedAt = updatedAt;
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
            var response = await GetRawAsync(this.http, this.Uid, cancellationToken).ConfigureAwait(false);
            var content = await response.Content.ReadFromJsonAsync<Index>(cancellationToken: cancellationToken).ConfigureAwait(false);
            this.PrimaryKey = content.PrimaryKey;
            this.CreatedAt = content.CreatedAt;
            this.UpdatedAt = content.UpdatedAt;
            return this;
        }

        /// <summary>
        /// Fetch the primary key of the index.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Primary key of the fetched index.</returns>
        public async Task<string> FetchPrimaryKey(CancellationToken cancellationToken = default)
        {
            return (await this.FetchInfoAsync(cancellationToken).ConfigureAwait(false)).PrimaryKey;
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
                await this.http.PutAsJsonAsync($"indexes/{this.Uid}", new { primaryKey = primarykeytoChange }, cancellationToken: cancellationToken)
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
            var responseMessage = await this.http.DeleteAsync($"/indexes/{this.Uid}", cancellationToken).ConfigureAwait(false);

            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Add documents.
        /// </summary>
        /// <param name="documents">Documents to add.</param>
        /// <param name="primaryKey">Primary key for the documents.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">Type of the document. Even though documents are schemaless in MeiliSearch, making it typed helps in compile time.</typeparam>
        /// <returns>Returns the task info.</returns>
        public async Task<TaskInfo> AddDocumentsAsync<T>(IEnumerable<T> documents, string primaryKey = default, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage responseMessage;
            string uri = $"/indexes/{this.Uid}/documents";
            if (primaryKey != default)
            {
                uri = $"{uri}?{new { primaryKey = primaryKey }.ToQueryString()}";
            }

            responseMessage = await this.http.PostJsonCustomAsync(uri, documents, cancellationToken: cancellationToken).ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Adds documents in batches with size specified with <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="documents">Documents to add.</param>
        /// <param name="batchSize">Size of documents batches while adding them.</param>
        /// <param name="primaryKey">Primary key for the documents.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">Type of the document. Even though documents are schemaless in MeiliSearch, making it typed helps in compile time.</typeparam>
        /// <returns>Returns the task list.</returns>
        public async Task<IEnumerable<TaskInfo>> AddDocumentsInBatchesAsync<T>(IEnumerable<T> documents, int batchSize = 1000, string primaryKey = default, CancellationToken cancellationToken = default)
        {
            async Task AddActionAsync(List<T> items, List<TaskInfo> tasks, CancellationToken localCancellationToken)
            {
                tasks.Add(await this.AddDocumentsAsync(items, primaryKey, localCancellationToken).ConfigureAwait(false));
            }

            var result = await BatchOperationAsync(documents, batchSize, AddActionAsync).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Update documents.
        /// </summary>
        /// <param name="documents">Documents to update.</param>
        /// <param name="primaryKey">Primary key for the documents.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">Type of document. Even though documents are schemaless in MeiliSearch, making it typed helps in compile time.</typeparam>
        /// <returns>Returns the task list.</returns>
        public async Task<TaskInfo> UpdateDocumentsAsync<T>(IEnumerable<T> documents, string primaryKey = default, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage responseMessage;
            string uri = $"/indexes/{this.Uid}/documents";
            if (primaryKey != default)
            {
                uri = $"{uri}?{new { primaryKey = primaryKey }.ToQueryString()}";
            }

            var filteredDocuments = documents.RemoveNullValues();
            responseMessage = await this.http.PutJsonCustomAsync(uri, filteredDocuments, cancellationToken: cancellationToken).ConfigureAwait(false);

            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates documents in batches with size specified with <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="documents">Documents to update.</param>
        /// <param name="batchSize">Size of documents batches while updating them.</param>
        /// <param name="primaryKey">Primary key for the documents.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">Type of the document. Even though documents are schemaless in MeiliSearch, making it typed helps in compile time.</typeparam>
        /// <returns>Returns the task list.</returns>
        public async Task<IEnumerable<TaskInfo>> UpdateDocumentsInBatchesAsync<T>(IEnumerable<T> documents, int batchSize = 1000, string primaryKey = default, CancellationToken cancellationToken = default)
        {
            async Task UpdateActionAsync(List<T> items, List<TaskInfo> tasks, CancellationToken localCancellationToken)
            {
                tasks.Add(await this.UpdateDocumentsAsync(items, primaryKey, localCancellationToken).ConfigureAwait(false));
            }

            var result = await BatchOperationAsync(documents, batchSize, UpdateActionAsync, cancellationToken).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Get document by its ID.
        /// </summary>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">Type of the document.</typeparam>
        /// <returns>Returns the document, with the according type if the object is available.</returns>
        public async Task<T> GetDocumentAsync<T>(string documentId, CancellationToken cancellationToken = default)
        {
            return await this.http.GetFromJsonAsync<T>($"/indexes/{this.Uid}/documents/{documentId}", cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get document by its ID.
        /// </summary>
        /// <param name="documentId">Document Id for query.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">Type to return for document.</typeparam>
        /// <returns>Type if the object is availble.</returns>
        public async Task<T> GetDocumentAsync<T>(int documentId, CancellationToken cancellationToken = default)
        {
            return await this.GetDocumentAsync<T>(documentId.ToString(), cancellationToken);
        }

        /// <summary>
        /// Get documents with the allowed Query Parameters.
        /// </summary>
        /// <param name="query">Query parameters. Supports limit, offset and attributes to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">Type of the document.</typeparam>
        /// <returns>Returns the list of documents.</returns>
        public async Task<IEnumerable<T>> GetDocumentsAsync<T>(DocumentQuery query = default, CancellationToken cancellationToken = default)
        {
            string uri = $"/indexes/{this.Uid}/documents";
            if (query != null)
            {
                uri = $"{uri}?{query.ToQueryString()}";
            }

            return await this.http.GetFromJsonAsync<IEnumerable<T>>(uri, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete one document.
        /// </summary>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info.</returns>
        public async Task<TaskInfo> DeleteOneDocumentAsync(string documentId, CancellationToken cancellationToken = default)
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/documents/{documentId}", cancellationToken).ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete one document by its ID.
        /// </summary>
        /// <param name="documentId">document ID.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info.</returns>
        public async Task<TaskInfo> DeleteOneDocumentAsync(int documentId, CancellationToken cancellationToken = default)
        {
            return await this.DeleteOneDocumentAsync(documentId.ToString(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete documents in batch.
        /// </summary>
        /// <param name="documentIds">List of documents identifier.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info.</returns>
        public async Task<TaskInfo> DeleteDocumentsAsync(IEnumerable<string> documentIds, CancellationToken cancellationToken = default)
        {
            var httpresponse =
                await this.http.PostAsJsonAsync($"/indexes/{this.Uid}/documents/delete-batch", documentIds, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete documents in batch.
        /// </summary>
        /// <param name="documentIds">List of document Id.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Return the task info.</returns>
        public async Task<TaskInfo> DeleteDocumentsAsync(IEnumerable<int> documentIds, CancellationToken cancellationToken = default)
        {
            var docIds = documentIds.Select(id => id.ToString());
            return await this.DeleteDocumentsAsync(docIds, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete all the documents in the index.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info.</returns>
        public async Task<TaskInfo> DeleteAllDocumentsAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/documents", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a local reference to a task, without doing an HTTP call.
        /// </summary>
        /// <returns>Returns a TaskEndpoint instance.</returns>
        private TaskEndpoint TaskEndpoint()
        {
            var task = new TaskEndpoint();
            task.WithHttpClient(this.http);
            return task;
        }

        /// <summary>
        /// Gets the tasks.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns a list of the operations status.</returns>
        public async Task<Result<IEnumerable<TaskInfo>>> GetTasksAsync(CancellationToken cancellationToken = default)
        {
            return await this.TaskEndpoint().GetIndexTasksAsync(this.Uid, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get on task.
        /// </summary>
        /// <param name="taskUid">Uid of the task.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the tasks.</returns>
        public async Task<TaskInfo> GetTaskAsync(int taskUid, CancellationToken cancellationToken = default)
        {
            return await this.TaskEndpoint().GetIndexTaskAsync(this.Uid, taskUid, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Search documents according to search parameters.
        /// </summary>
        /// <param name="query">Query Parameter with Search.</param>
        /// <param name="searchAttributes">Attributes to search.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">Type parameter to return.</typeparam>
        /// <returns>Returns Enumerable of items.</returns>
        public async Task<SearchResult<T>> SearchAsync<T>(string query, SearchQuery searchAttributes = default(SearchQuery), CancellationToken cancellationToken = default)
        {
            SearchQuery body;
            if (searchAttributes == null)
            {
                body = new SearchQuery { Q = query };
            }
            else
            {
                body = searchAttributes;
                body.Q = query;
            }

            var responseMessage = await this.http.PostAsJsonAsync<SearchQuery>($"/indexes/{this.Uid}/search", body, MeilisearchClient.JsonSerializerOptions, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<SearchResult<T>>(cancellationToken: cancellationToken).ConfigureAwait(false);
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
        /// Gets all the settings of an index.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns all the settings.</returns>
        public async Task<Settings> GetSettingsAsync(CancellationToken cancellationToken = default)
        {
            return await this.http.GetFromJsonAsync<Settings>($"/indexes/{this.Uid}/settings", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates all the settings of an index.
        /// The settings that are not passed in parameter are not overwritten.
        /// </summary>
        /// <param name="settings">Settings object.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateSettingsAsync(Settings settings, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage responseMessage =
                await this.http.PostAsJsonAsync<Settings>($"/indexes/{this.Uid}/settings", settings, MeilisearchClient.JsonSerializerOptions, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Resets all the settings to their default values.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task UID of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetSettingsAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings", cancellationToken).ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the displayed attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the displayed attributes setting.</returns>
        public async Task<IEnumerable<string>> GetDisplayedAttributesAsync(CancellationToken cancellationToken = default)
        {
            return await this.http.GetFromJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/displayed-attributes", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the displayed attributes setting.
        /// </summary>
        /// <param name="displayedAttributes">Collection of displayed attributes.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateDisplayedAttributesAsync(IEnumerable<string> displayedAttributes, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage responseMessage =
                await this.http.PostAsJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/displayed-attributes", displayedAttributes, MeilisearchClient.JsonSerializerOptions, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the displayed attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetDisplayedAttributesAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/displayed-attributes", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the distinct attribute setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the distinct attribute setting.</returns>
        public async Task<string> GetDistinctAttributeAsync(CancellationToken cancellationToken = default)
        {
            return await this.http.GetFromJsonAsync<string>($"/indexes/{this.Uid}/settings/distinct-attribute", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the distinct attribute setting.
        /// </summary>
        /// <param name="distinctAttribute">Name of distinct attribute.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateDistinctAttributeAsync(string distinctAttribute, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage responseMessage =
                await this.http.PostAsJsonAsync<string>($"/indexes/{this.Uid}/settings/distinct-attribute", distinctAttribute, MeilisearchClient.JsonSerializerOptions, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the distinct attribute setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetDistinctAttributeAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/distinct-attribute", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the filterable attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the filterable attributes setting.</returns>
        public async Task<IEnumerable<string>> GetFilterableAttributesAsync(CancellationToken cancellationToken = default)
        {
            return await this.http.GetFromJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/filterable-attributes", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the filterable attributes setting.
        /// </summary>
        /// <param name="filterableAttributes">Collection of filterable attributes.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateFilterableAttributesAsync(IEnumerable<string> filterableAttributes, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage responseMessage =
                await this.http.PostAsJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/filterable-attributes", filterableAttributes, MeilisearchClient.JsonSerializerOptions, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the filterable attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetFilterableAttributesAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/filterable-attributes", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the ranking rules setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the ranking rules setting.</returns>
        public async Task<IEnumerable<string>> GetRankingRulesAsync(CancellationToken cancellationToken = default)
        {
            return await this.http.GetFromJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/ranking-rules", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the ranking rules setting.
        /// </summary>
        /// <param name="rankingRules">Collection of ranking rules.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateRankingRulesAsync(IEnumerable<string> rankingRules, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage responseMessage =
                await this.http.PostAsJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/ranking-rules", rankingRules, MeilisearchClient.JsonSerializerOptions, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the ranking rules setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetRankingRulesAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/ranking-rules", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the searchable attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the searchable attributes setting.</returns>
        public async Task<IEnumerable<string>> GetSearchableAttributesAsync(CancellationToken cancellationToken = default)
        {
            return await this.http.GetFromJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/searchable-attributes", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the searchable attributes setting.
        /// </summary>
        /// <param name="searchableAttributes">Collection of searchable attributes.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateSearchableAttributesAsync(IEnumerable<string> searchableAttributes, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage responseMessage =
                await this.http.PostAsJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/searchable-attributes", searchableAttributes, MeilisearchClient.JsonSerializerOptions, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the searchable attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetSearchableAttributesAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/searchable-attributes", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the sortable attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the sortable attributes setting.</returns>
        public async Task<IEnumerable<string>> GetSortableAttributesAsync(CancellationToken cancellationToken = default)
        {
            return await this.http.GetFromJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/sortable-attributes", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the sortable attributes setting.
        /// </summary>
        /// <param name="sortableAttributes">Collection of sortable attributes.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateSortableAttributesAsync(IEnumerable<string> sortableAttributes, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage responseMessage =
                await this.http.PostAsJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/sortable-attributes", sortableAttributes, MeilisearchClient.JsonSerializerOptions, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the sortable attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetSortableAttributesAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/sortable-attributes", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the stop words setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the stop words setting.</returns>
        public async Task<IEnumerable<string>> GetStopWordsAsync(CancellationToken cancellationToken = default)
        {
            return await this.http.GetFromJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/stop-words", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the stop words setting.
        /// </summary>
        /// <param name="stopWords">Collection of stop words.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateStopWordsAsync(IEnumerable<string> stopWords, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage responseMessage =
                await this.http.PostAsJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/stop-words", stopWords, MeilisearchClient.JsonSerializerOptions, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the stop words setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetStopWordsAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/stop-words", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the synonyms setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the synonyms setting.</returns>
        public async Task<Dictionary<string, IEnumerable<string>>> GetSynonymsAsync(CancellationToken cancellationToken = default)
        {
            return await this.http.GetFromJsonAsync<Dictionary<string, IEnumerable<string>>>($"/indexes/{this.Uid}/settings/synonyms", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the synonyms setting.
        /// </summary>
        /// <param name="synonyms">Collection of synonyms.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateSynonymsAsync(Dictionary<string, IEnumerable<string>> synonyms, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage responseMessage =
                await this.http.PostAsJsonAsync<Dictionary<string, IEnumerable<string>>>($"/indexes/{this.Uid}/settings/synonyms", synonyms, MeilisearchClient.JsonSerializerOptions, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the synonyms setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetSynonymsAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/synonyms", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get stats.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Return index stats.</returns>
        public async Task<IndexStats> GetStatsAsync(CancellationToken cancellationToken = default)
        {
            return await this.http.GetFromJsonAsync<IndexStats>($"/indexes/{this.Uid}/stats", cancellationToken: cancellationToken)
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
            this.http = http;
            return this;
        }

        private static async Task<List<TaskInfo>> BatchOperationAsync<T>(IEnumerable<T> items, int batchSize, Func<List<T>, List<TaskInfo>, CancellationToken, Task> action, CancellationToken cancellationToken = default)
        {
            var itemsList = new List<T>(items);
            var numberOfBatches = Math.Ceiling((double)itemsList.Count / batchSize);
            var result = new List<TaskInfo>();
            for (var i = 0; i < numberOfBatches; i++)
            {
                var actualSize = Math.Min(batchSize, itemsList.Count - (i * batchSize));
                var batch = itemsList.GetRange(i * batchSize, actualSize);
                await action.Invoke(batch, result, cancellationToken).ConfigureAwait(false);
            }

            return result;
        }
    }
}

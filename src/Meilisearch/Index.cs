namespace Meilisearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.WebUtilities;

    /// <summary>
    /// MeiliSearch index to search and manage documents.
    /// </summary>
    public class Index
    {
        private HttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="Index"/> class.
        /// Initializes with the UID (mandatory) and the primary key.
        /// </summary>
        /// <param name="uid">Unique index identifier.</param>
        /// <param name="primaryKey">Documents primary key.</param>
        public Index(string uid, string primaryKey = default)
        {
            this.Uid = uid;
            this.PrimaryKey = primaryKey;
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
        /// Fetch the info of the index.
        /// </summary>
        /// <returns>An instance of the index fetch.</returns>
        public async Task<Index> FetchInfo()
        {
            var response = await this.client.GetAsync($"indexes/{this.Uid}");
            var content = await response.Content.ReadFromJsonAsync<Index>();
            this.PrimaryKey = content.PrimaryKey;
            return this;
        }

        /// <summary>
        /// Fetch the primary key of the index.
        /// </summary>
        /// <returns>Primary key of the fetched index.</returns>
        public async Task<string> FetchPrimaryKey()
        {
            return (await this.FetchInfo()).PrimaryKey;
        }

        /// <summary>
        /// Changes the primary key of the index.
        /// </summary>
        /// <param name="primarykeytoChange">Primary key set.</param>
        /// <returns>Index with the updated Primary Key.</returns>
        public async Task<Index> UpdateIndex(string primarykeytoChange)
        {
            var message = await this.client.PutAsJsonAsync($"indexes/{this.Uid}", new { primaryKey = primarykeytoChange });
            var responsecontent = await message.Content.ReadFromJsonAsync<Index>();
            this.PrimaryKey = responsecontent.PrimaryKey;
            return this;
        }

        /// <summary>
        /// Deletes the index.
        /// It's not a recovery delete. You will also lose the documents within the index.
        /// </summary>
        /// <returns>Returns the updateID of this async operation.</returns>
        public async Task<bool> Delete()
        {
            var responseMessage = await this.client.DeleteAsync($"/indexes/{this.Uid}");
            return responseMessage.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Add documents.
        /// </summary>
        /// <param name="documents">Documents to add.</param>
        /// <param name="primaryKey">Primary key for the documents.</param>
        /// <typeparam name="T">Type of the document. Even though documents are schemaless in MeiliSearch, making it typed helps in compile time.</typeparam>
        /// <returns>Returns the updateID of this async operation.</returns>
        public async Task<UpdateStatus> AddDocuments<T>(IEnumerable<T> documents, string primaryKey = default)
        {
            HttpResponseMessage responseMessage;
            string uri = $"/indexes/{this.Uid}/documents";
            if (primaryKey != default)
            {
                uri = QueryHelpers.AddQueryString(uri, new { primaryKey = primaryKey }.AsDictionary());
            }

            responseMessage = await this.client.PostAsJsonAsync(uri, documents);

            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Update documents.
        /// </summary>
        /// <param name="documents">Documents to update.</param>
        /// <param name="primaryKey">Primary key for the documents.</param>
        /// <typeparam name="T">Type of document. Even though documents are schemaless in MeiliSearch, making it typed helps in compile time.</typeparam>
        /// <returns>Returns the updateID of this async operation.</returns>
        public async Task<UpdateStatus> UpdateDocuments<T>(IEnumerable<T> documents, string primaryKey = default)
        {
            HttpResponseMessage responseMessage;
            string uri = $"/indexes/{this.Uid}/documents";
            if (primaryKey != default)
            {
                uri = QueryHelpers.AddQueryString(uri, new { primaryKey = primaryKey }.AsDictionary());
            }

            responseMessage = await this.client.PutAsJsonAsync(uri, documents);

            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Get document by its ID.
        /// </summary>
        /// <param name="documentId">Document identifier.</param>
        /// <typeparam name="T">Type of the document.</typeparam>
        /// <returns>Returns the document, with the according type if the object is available.</returns>
        public async Task<T> GetDocument<T>(string documentId)
        {
            return await this.client.GetFromJsonAsync<T>($"/indexes/{this.Uid}/documents/{documentId}");
        }

        /// <summary>
        /// Get document by its ID.
        /// </summary>
        /// <param name="documentId">Document Id for query.</param>
        /// <typeparam name="T">Type to return for document.</typeparam>
        /// <returns>Type if the object is availble.</returns>
        public async Task<T> GetDocument<T>(int documentId)
        {
            return await this.GetDocument<T>(documentId.ToString());
        }

        /// <summary>
        /// Get documents with the allowed Query Parameters.
        /// </summary>
        /// <param name="query">Query parameters. Supports limit, offset and attributes to retrieve.</param>
        /// <typeparam name="T">Type of the document.</typeparam>
        /// <returns>Returns the list of documents.</returns>
        public async Task<IEnumerable<T>> GetDocuments<T>(DocumentQuery query = default)
        {
            string uri = $"/indexes/{this.Uid}/documents";
            if (query != null)
            {
                uri = QueryHelpers.AddQueryString(uri, query.AsDictionary());
            }

            return await this.client.GetFromJsonAsync<IEnumerable<T>>(uri);
        }

        /// <summary>
        /// Delete one document.
        /// </summary>
        /// <param name="documentId">Document identifier.</param>
        /// <returns>Returns the updateID of this async operation.</returns>
        public async Task<UpdateStatus> DeleteOneDocument(string documentId)
        {
            var httpresponse = await this.client.DeleteAsync($"/indexes/{this.Uid}/documents/{documentId}");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Delete one document by its ID.
        /// </summary>
        /// <param name="documentId">document ID.</param>
        /// <returns>Update Status with ID to look for document.</returns>
        public async Task<UpdateStatus> DeleteOneDocument(int documentId)
        {
            return await this.DeleteOneDocument(documentId.ToString());
        }

        /// <summary>
        /// Delete documents in batch.
        /// </summary>
        /// <param name="documentIds">List of documents identifier.</param>
        /// <returns>Returns the updateID of this async operation.</returns>
        public async Task<UpdateStatus> DeleteDocuments(IEnumerable<string> documentIds)
        {
            var httpresponse = await this.client.PostAsJsonAsync($"/indexes/{this.Uid}/documents/delete-batch", documentIds);
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Delete documents in batch.
        /// </summary>
        /// <param name="documentIds">List of document Id.</param>
        /// <returns>Update status with ID to look for progress of update.</returns>
        public async Task<UpdateStatus> DeleteDocuments(IEnumerable<int> documentIds)
        {
            var docIds = documentIds.Select(id => id.ToString());
            return await this.DeleteDocuments(docIds);
        }

        /// <summary>
        /// Delete all the documents in the index.
        /// </summary>
        /// <returns>Returns the updateID of this async operation.</returns>
        public async Task<UpdateStatus> DeleteAllDocuments()
        {
            var httpresponse = await this.client.DeleteAsync($"/indexes/{this.Uid}/documents");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Gets the update status of all the asynchronous operations.
        /// </summary>
        /// <returns>Returns a list of the operations status.</returns>
        public async Task<IEnumerable<UpdateStatus>> GetAllUpdateStatus()
        {
            return await this.client.GetFromJsonAsync<IEnumerable<UpdateStatus>>($"/indexes/{this.Uid}/updates");
        }

        /// <summary>
        /// Get Update Status by Status Id.
        /// </summary>
        /// <param name="updateId">UpdateId for the operation.</param>
        /// <returns>Return the current status of the operation.</returns>
        public async Task<UpdateStatus> GetUpdateStatus(int updateId)
        {
            return await this.client.GetFromJsonAsync<UpdateStatus>($"/indexes/{this.Uid}/updates/{updateId}");
        }

        /// <summary>
        /// Search documents according to search parameters.
        /// </summary>
        /// <param name="query">Query Parameter with Search.</param>
        /// <param name="searchAttributes">Attributes to search.</param>
        /// <typeparam name="T">Type parameter to return.</typeparam>
        /// <returns>Returns Enumerable of items.</returns>
        public async Task<SearchResult<T>> Search<T>(string query, SearchQuery searchAttributes = default(SearchQuery))
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

            var responseMessage = await this.client.PostAsJsonAsync<SearchQuery>($"/indexes/{this.Uid}/search", body);
            return await responseMessage.Content.ReadFromJsonAsync<SearchResult<T>>();
        }

        /// <summary>
        /// Waits until the asynchronous task was done.
        /// </summary>
        /// <param name="updateId">Unique identifier of the asynchronous task.</param>
        /// <param name="timeoutMs">Timeout in millisecond.</param>
        /// <param name="intervalMs">Interval in millisecond between each check.</param>
        /// <returns>Returns the status of asynchronous task.</returns>
        public async Task<UpdateStatus> WaitForPendingUpdate(
            int updateId,
            double timeoutMs = 5000.0,
            int intervalMs = 50)
        {
            DateTime endingTime = DateTime.Now.AddMilliseconds(timeoutMs);

            while (DateTime.Now < endingTime)
            {
                var response = await this.GetUpdateStatus(updateId);

                if (response.Status != "enqueued")
                {
                    return response;
                }

                await Task.Delay(intervalMs);
            }

            throw new MeilisearchTimeoutError("The task " + updateId.ToString() + " timed out.");
        }

        /// <summary>
        /// Gets all the settings of an index.
        /// </summary>
        /// <returns>Returns all the settings.</returns>
        public async Task<Settings> GetSettings()
        {
            return await this.client.GetFromJsonAsync<Settings>($"/indexes/{this.Uid}/settings");
        }

        /// <summary>
        /// Updates all the settings of an index.
        /// The settings that are not passed in parameter are not overwritten.
        /// </summary>
        /// <param name="settings">Settings object.</param>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> UpdateSettings(Settings settings)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { IgnoreNullValues = true };
            HttpResponseMessage responseMessage = await this.client.PostAsJsonAsync<Settings>($"/indexes/{this.Uid}/settings", settings, options);
            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Resets all the settings to their default values.
        /// </summary>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> ResetSettings()
        {
            var httpresponse = await this.client.DeleteAsync($"/indexes/{this.Uid}/settings");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Get stats.
        /// </summary>
        /// <returns>Return index stats.</returns>
        public async Task<IndexStats> GetStats()
        {
            return await this.client.GetFromJsonAsync<IndexStats>($"/indexes/{this.Uid}/stats");
        }

        /// <summary>
        /// Initializes the Index with HTTP client. Only for internal usage.
        /// </summary>
        /// <param name="client">HTTP client from the base client.</param>
        /// <returns>The same object with the initialization.</returns>
        internal Index WithHttpClient(HttpClient client)
        {
            this.client = client;
            return this;
        }
    }
}

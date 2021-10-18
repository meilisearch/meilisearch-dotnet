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
    using Meilisearch.Extensions;
    using Microsoft.AspNetCore.WebUtilities;

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
            var response = await this.http.GetAsync($"indexes/{this.Uid}");
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
        public async Task<Index> Update(string primarykeytoChange)
        {
            var message = await this.http.PutAsJsonAsync($"indexes/{this.Uid}", new { primaryKey = primarykeytoChange });
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
            var responseMessage = await this.http.DeleteAsync($"/indexes/{this.Uid}");
            return responseMessage.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Deletes the index if it exists.
        /// It's not a recovery delete. You will also lose the documents within the index.
        /// </summary>
        /// <returns>Returns the status of the delete operation.
        /// True if the index existed and was deleted. False if it did not exist. </returns>
        public async Task<bool> DeleteIfExists()
        {
            try
            {
                var responseMessage = await this.http.DeleteAsync($"/indexes/{this.Uid}");
                if (responseMessage.StatusCode != HttpStatusCode.NoContent)
                {
                    throw new HttpRequestException($"Client failed to delete index ${this.Uid}");
                }

                return true;
            }
            catch (MeilisearchApiError error)
            {
                if (error.ErrorCode == "index_not_found")
                {
                    return false;
                }

                throw;
            }
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

            responseMessage = await this.http.PostJsonCustomAsync(uri, documents);
            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Adds documents in batches with size specified with <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="documents">Documents to add.</param>
        /// <param name="batchSize">Size of documents batches while adding them.</param>
        /// <param name="primaryKey">Primary key for the documents.</param>
        /// <typeparam name="T">Type of the document. Even though documents are schemaless in MeiliSearch, making it typed helps in compile time.</typeparam>
        /// <returns>Returns the updateID of this async operation.</returns>
        public async Task<IEnumerable<UpdateStatus>> AddDocumentsInBatches<T>(IEnumerable<T> documents, int batchSize = 1000, string primaryKey = default)
        {
            async Task AddAction(List<T> items, List<UpdateStatus> updates)
            {
                updates.Add(await this.AddDocuments(items, primaryKey));
            }

            var result = await BatchOperation(documents, batchSize, AddAction);
            return result;
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

            var filteredDocuments = documents.RemoveNullValues();
            responseMessage = await this.http.PutJsonCustomAsync(uri, filteredDocuments);

            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Updates documents in batches with size specified with <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="documents">Documents to update.</param>
        /// <param name="batchSize">Size of documents batches while updating them.</param>
        /// <param name="primaryKey">Primary key for the documents.</param>
        /// <typeparam name="T">Type of the document. Even though documents are schemaless in MeiliSearch, making it typed helps in compile time.</typeparam>
        /// <returns>Returns the updateID of this async operation.</returns>
        public async Task<IEnumerable<UpdateStatus>> UpdateDocumentsInBatches<T>(IEnumerable<T> documents, int batchSize = 1000, string primaryKey = default)
        {
            async Task UpdateAction(List<T> items, List<UpdateStatus> updates)
            {
                updates.Add(await this.UpdateDocuments(items, primaryKey));
            }

            var result = await BatchOperation(documents, batchSize, UpdateAction);
            return result;
        }

        /// <summary>
        /// Get document by its ID.
        /// </summary>
        /// <param name="documentId">Document identifier.</param>
        /// <typeparam name="T">Type of the document.</typeparam>
        /// <returns>Returns the document, with the according type if the object is available.</returns>
        public async Task<T> GetDocument<T>(string documentId)
        {
            return await this.http.GetFromJsonAsync<T>($"/indexes/{this.Uid}/documents/{documentId}");
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

            return await this.http.GetFromJsonAsync<IEnumerable<T>>(uri);
        }

        /// <summary>
        /// Delete one document.
        /// </summary>
        /// <param name="documentId">Document identifier.</param>
        /// <returns>Returns the updateID of this async operation.</returns>
        public async Task<UpdateStatus> DeleteOneDocument(string documentId)
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/documents/{documentId}");
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
            var httpresponse = await this.http.PostAsJsonAsync($"/indexes/{this.Uid}/documents/delete-batch", documentIds);
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
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/documents");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Gets the update status of all the asynchronous operations.
        /// </summary>
        /// <returns>Returns a list of the operations status.</returns>
        public async Task<IEnumerable<UpdateStatus>> GetAllUpdateStatus()
        {
            return await this.http.GetFromJsonAsync<IEnumerable<UpdateStatus>>($"/indexes/{this.Uid}/updates");
        }

        /// <summary>
        /// Get Update Status by Status Id.
        /// </summary>
        /// <param name="updateId">UpdateId for the operation.</param>
        /// <returns>Return the current status of the operation.</returns>
        public async Task<UpdateStatus> GetUpdateStatus(int updateId)
        {
            return await this.http.GetFromJsonAsync<UpdateStatus>($"/indexes/{this.Uid}/updates/{updateId}");
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

            JsonSerializerOptions options = new JsonSerializerOptions { IgnoreNullValues = true };

            var responseMessage = await this.http.PostAsJsonAsync<SearchQuery>($"/indexes/{this.Uid}/search", body, options);
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

                if (response.Status != "enqueued" && response.Status != "processing")
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
            return await this.http.GetFromJsonAsync<Settings>($"/indexes/{this.Uid}/settings");
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
            HttpResponseMessage responseMessage = await this.http.PostAsJsonAsync<Settings>($"/indexes/{this.Uid}/settings", settings, options);
            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Resets all the settings to their default values.
        /// </summary>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> ResetSettings()
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Gets the displayed attributes setting.
        /// </summary>
        /// <returns>Returns the displayed attributes setting.</returns>
        public async Task<IEnumerable<string>> GetDisplayedAttributes()
        {
            return await this.http.GetFromJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/displayed-attributes");
        }

        /// <summary>
        /// Updates the displayed attributes setting.
        /// </summary>
        /// <param name="displayedAttributes">Collection of displayed attributes.</param>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> UpdateDisplayedAttributes(IEnumerable<string> displayedAttributes)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { IgnoreNullValues = true };
            HttpResponseMessage responseMessage = await this.http.PostAsJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/displayed-attributes", displayedAttributes, options);
            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Resets the displayed attributes setting.
        /// </summary>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> ResetDisplayedAttributes()
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/displayed-attributes");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Gets the distinct attribute setting.
        /// </summary>
        /// <returns>Returns the distinct attribute setting.</returns>
        public async Task<string> GetDistinctAttribute()
        {
            return await this.http.GetFromJsonAsync<string>($"/indexes/{this.Uid}/settings/distinct-attribute");
        }

        /// <summary>
        /// Updates the distinct attribute setting.
        /// </summary>
        /// <param name="distinctAttribute">Name of distinct attribute.</param>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> UpdateDistinctAttribute(string distinctAttribute)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { IgnoreNullValues = true };
            HttpResponseMessage responseMessage = await this.http.PostAsJsonAsync<string>($"/indexes/{this.Uid}/settings/distinct-attribute", distinctAttribute, options);
            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Resets the distinct attribute setting.
        /// </summary>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> ResetDistinctAttribute()
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/distinct-attribute");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Gets the filterable attributes setting.
        /// </summary>
        /// <returns>Returns the filterable attributes setting.</returns>
        public async Task<IEnumerable<string>> GetFilterableAttributes()
        {
            return await this.http.GetFromJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/filterable-attributes");
        }

        /// <summary>
        /// Updates the filterable attributes setting.
        /// </summary>
        /// <param name="filterableAttributes">Collection of filterable attributes.</param>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> UpdateFilterableAttributes(IEnumerable<string> filterableAttributes)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { IgnoreNullValues = true };
            HttpResponseMessage responseMessage = await this.http.PostAsJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/filterable-attributes", filterableAttributes, options);
            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Resets the filterable attributes setting.
        /// </summary>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> ResetFilterableAttributes()
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/filterable-attributes");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Gets the ranking rules setting.
        /// </summary>
        /// <returns>Returns the ranking rules setting.</returns>
        public async Task<IEnumerable<string>> GetRankingRules()
        {
            return await this.http.GetFromJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/ranking-rules");
        }

        /// <summary>
        /// Updates the ranking rules setting.
        /// </summary>
        /// <param name="rankingRules">Collection of ranking rules.</param>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> UpdateRankingRules(IEnumerable<string> rankingRules)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { IgnoreNullValues = true };
            HttpResponseMessage responseMessage = await this.http.PostAsJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/ranking-rules", rankingRules, options);
            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Resets the ranking rules setting.
        /// </summary>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> ResetRankingRules()
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/ranking-rules");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Gets the searchable attributes setting.
        /// </summary>
        /// <returns>Returns the searchable attributes setting.</returns>
        public async Task<IEnumerable<string>> GetSearchableAttributes()
        {
            return await this.http.GetFromJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/searchable-attributes");
        }

        /// <summary>
        /// Updates the searchable attributes setting.
        /// </summary>
        /// <param name="searchableAttributes">Collection of searchable attributes.</param>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> UpdateSearchableAttributes(IEnumerable<string> searchableAttributes)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { IgnoreNullValues = true };
            HttpResponseMessage responseMessage = await this.http.PostAsJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/searchable-attributes", searchableAttributes, options);
            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Resets the searchable attributes setting.
        /// </summary>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> ResetSearchableAttributes()
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/searchable-attributes");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Gets the sortable attributes setting.
        /// </summary>
        /// <returns>Returns the sortable attributes setting.</returns>
        public async Task<IEnumerable<string>> GetSortableAttributes()
        {
            return await this.http.GetFromJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/sortable-attributes");
        }

        /// <summary>
        /// Updates the sortable attributes setting.
        /// </summary>
        /// <param name="sortableAttributes">Collection of sortable attributes.</param>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> UpdateSortableAttributes(IEnumerable<string> sortableAttributes)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { IgnoreNullValues = true };
            HttpResponseMessage responseMessage = await this.http.PostAsJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/sortable-attributes", sortableAttributes, options);
            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Resets the sortable attributes setting.
        /// </summary>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> ResetSortableAttributes()
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/sortable-attributes");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Gets the stop words setting.
        /// </summary>
        /// <returns>Returns the stop words setting.</returns>
        public async Task<IEnumerable<string>> GetStopWords()
        {
            return await this.http.GetFromJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/stop-words");
        }

        /// <summary>
        /// Updates the stop words setting.
        /// </summary>
        /// <param name="stopWords">Collection of stop words.</param>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> UpdateStopWords(IEnumerable<string> stopWords)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { IgnoreNullValues = true };
            HttpResponseMessage responseMessage = await this.http.PostAsJsonAsync<IEnumerable<string>>($"/indexes/{this.Uid}/settings/stop-words", stopWords, options);
            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Resets the stop words setting.
        /// </summary>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> ResetStopWords()
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/stop-words");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Gets the synonyms setting.
        /// </summary>
        /// <returns>Returns the synonyms setting.</returns>
        public async Task<Dictionary<string, IEnumerable<string>>> GetSynonyms()
        {
            return await this.http.GetFromJsonAsync<Dictionary<string, IEnumerable<string>>>($"/indexes/{this.Uid}/settings/synonyms");
        }

        /// <summary>
        /// Updates the synonyms setting.
        /// </summary>
        /// <param name="synonyms">Collection of synonyms.</param>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> UpdateSynonyms(Dictionary<string, IEnumerable<string>> synonyms)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { IgnoreNullValues = true };
            HttpResponseMessage responseMessage = await this.http.PostAsJsonAsync<Dictionary<string, IEnumerable<string>>>($"/indexes/{this.Uid}/settings/synonyms", synonyms, options);
            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Resets the synonyms setting.
        /// </summary>
        /// <returns>Returns the updateID of the asynchronous task.</returns>
        public async Task<UpdateStatus> ResetSynonyms()
        {
            var httpresponse = await this.http.DeleteAsync($"/indexes/{this.Uid}/settings/synonyms");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Get stats.
        /// </summary>
        /// <returns>Return index stats.</returns>
        public async Task<IndexStats> GetStats()
        {
            return await this.http.GetFromJsonAsync<IndexStats>($"/indexes/{this.Uid}/stats");
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

        private static async Task<List<UpdateStatus>> BatchOperation<T>(IEnumerable<T> items, int batchSize, Func<List<T>, List<UpdateStatus>, Task> action)
        {
            var itemsList = new List<T>(items);
            var numberOfBatches = Math.Ceiling((double)itemsList.Count / batchSize);
            var result = new List<UpdateStatus>();
            for (var i = 0; i < numberOfBatches; i++)
            {
                var batch = itemsList.GetRange(i * batchSize, batchSize);
                await action.Invoke(batch, result);
            }

            return result;
        }
    }
}

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace Meilisearch
{
    /// <summary>
    /// MeiliSearch Index for Search and managing document.
    /// </summary>
    public class Index
    {
        private HttpClient _client;

        /// <summary>
        /// Initializes with the default unique identifier and Primary Key.
        /// </summary>
        /// <param name="uid">Unique Identifier</param>
        /// <param name="primaryKey"></param>
        public Index(string uid,string primaryKey=default)
        {
            this.Uid = uid;
            this.PrimaryKey = primaryKey;
        }

        /// <summary>
        /// Unique Identifier for the Index.
        /// </summary>
       public string Uid { get; internal set; }

        /// <summary>
        /// Primary key of the document.
        /// </summary>
         public string PrimaryKey { get; internal set; }

        /// <summary>
        /// Initialize the Index with HTTP client. Only for internal use
        /// </summary>
        /// <param name="client">HTTP client from the base client</param>
        /// <returns>The same object with the initialization.</returns>
        internal Index WithHttpClient(HttpClient client)
        {
            this._client = client;
            return this;
        }

        /// <summary>
        /// Changes the Primary Key for a given index.
        /// </summary>
        /// <param name="primarykeytoChange"></param>
        /// <returns>Index with the updated Primary Key.</returns>
        public async Task<Index> ChangePrimaryKey(string primarykeytoChange)
        {
            var message = await this._client.PutAsJsonAsync($"indexes/{Uid}", new {primaryKey = primarykeytoChange});
            var responsecontent = await message.Content.ReadFromJsonAsync<Index>();
            this.PrimaryKey = responsecontent.PrimaryKey;
            return this;
        }

        /// <summary>
        /// Deletes the Index with unique identifier.
        /// Its a no recovery delete. You will also lose the documents within the index.
        /// </summary>
        /// <returns>Success or failure of the operation.</returns>
        public async Task<bool> Delete()
        {
           var responseMessage = await this._client.DeleteAsync($"/indexes/{Uid}");
           return responseMessage.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Add documents.
        /// </summary>
        /// <param name="documents">Documents to add</param>
        /// <typeparam name="T">Type of document. Even though document is schemaless in MeiliSearch making it typed helps in compile time.</typeparam>
        /// <returns>This action is Async in MeiliSearch so status is returned back.</returns>
        public async Task<UpdateStatus> AddDocuments<T>(IEnumerable<T> documents)
        {
            var responseMessage = await this._client.PostAsJsonAsync($"/indexes/{Uid}/documents", documents);
            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Add or Update Document .
        /// </summary>
        /// <param name="documents">Documents to update</param>
        /// <typeparam name="T">Type of document. Even though document is schemaless in MeiliSearch making it typed helps in compile time.</typeparam>
        /// <returns>This action is Async in MeiliSearch so status is returned back.</returns>
        public async Task<UpdateStatus> UpdateDocuments<T>(IEnumerable<T> documents)
        {
            var responseMessage = await this._client.PutAsJsonAsync($"/indexes/{Uid}/documents", documents);
            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Get document by its ID
        /// </summary>
        /// <param name="documentId">Document Id for query</param>
        /// <typeparam name="T">Type to return for document</typeparam>
        /// <returns>Type if the object is availble.</returns>
        public async Task<T> GetDocument<T>(string documentId)
        {
           return await this._client.GetFromJsonAsync<T>($"/indexes/{Uid}/documents/{documentId}");
        }

        /// <summary>
        /// Get documents with the allowed Query Parameters.
        /// </summary>
        /// <param name="query">Query Parameter. Supports Limit,offset and attributes to get</param>
        /// <typeparam name="T">Type of Object.</typeparam>
        /// <returns>List of document for the query.</returns>
        public async Task<IEnumerable<T>> GetDocuments<T>(DocumentQuery query=default)
        {
            string uri = $"/indexes/{Uid}/documents";
            if (query != null)
            {
                uri = QueryHelpers.AddQueryString(uri, query.AsDictionary());
            }
            return await this._client.GetFromJsonAsync<IEnumerable<T>>(uri);
        }

        /// <summary>
        /// Delete one document by its ID
        /// </summary>
        /// <param name="documentId">document ID</param>
        /// <returns>Update Status with ID to look for document.</returns>
        public async Task<UpdateStatus> DeleteOneDocument(string documentId)
        {
            var httpresponse = await this._client.DeleteAsync($"/indexes/{Uid}/documents/{documentId}");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Delete documents in batch.
        /// </summary>
        /// <param name="documentIds">List of document Id</param>
        /// <returns>Update status with ID to look for progress of update.</returns>
        public async Task<UpdateStatus> DeleteDocuments(IEnumerable<string> documentIds)
        {
            var httpresponse = await this._client.PostAsJsonAsync($"/indexes/{Uid}/documents/delete-batch", documentIds);
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Delete all the documents in the index
        /// </summary>
        /// <returns>Update status with ID to look for progress of update.</returns>
        public async Task<UpdateStatus> DeleteAllDocuments()
        {
            var httpresponse = await this._client.DeleteAsync($"/indexes/{Uid}/documents");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Gets the update status of all the asynchronous operation.
        /// </summary>
        /// <returns>Update status with the Operation status.</returns>
        public async Task<IEnumerable<UpdateStatus>> GetAllUpdateStatus()
        {
            return await this._client.GetFromJsonAsync<IEnumerable<UpdateStatus>>($"/indexes/{Uid}/updates");
        }

        /// <summary>
        /// Get Update Status by Status Id
        /// </summary>
        /// <param name="updateId">UpdateId for the Operation</param>
        /// <returns>Current status of the operation.</returns>
        public async Task<UpdateStatus> GetUpdateStatus(int updateId)
        {
            return await this._client.GetFromJsonAsync<UpdateStatus>($"/indexes/{Uid}/updates/{updateId}");
        }

        /// <summary>
        /// Search documents with a default Search Query
        /// </summary>
        /// <param name="query">Query Parameter with Search</param>
        /// <param name="searchattributes">Attributes to search.</param>
        /// <typeparam name="T">Type parameter to return</typeparam>
        /// <returns>Enumerable of items</returns>
        public async Task<SearchResult<T>> Search<T>(string query,SearchQuery searchattributes = default(SearchQuery))
        {
            string uri = $"/indexes/{Uid}/search?q={query}";
            if (searchattributes != null)
            {
                uri = QueryHelpers.AddQueryString(uri, searchattributes.AsDictionary());
            }

            var searchResults = await this._client.GetFromJsonAsync<SearchResult<T>>(uri);
            return searchResults;
        }
    }
}

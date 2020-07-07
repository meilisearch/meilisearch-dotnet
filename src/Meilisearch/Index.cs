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
    /// MeiliSearch index to search and manage documents.
    /// </summary>
    public class Index
    {
        private HttpClient _client;

        /// <summary>
        /// Initializes with the UID (mandatory) and the primary key.
        /// </summary>
        /// <param name="uid">Unique Identifier</param>
        /// <param name="primaryKey"></param>
        public Index(string uid,string primaryKey=default)
        {
            this.Uid = uid;
            this.PrimaryKey = primaryKey;
        }

        /// <summary>
        /// Unique identifier of the index.
        /// </summary>
       public string Uid { get; internal set; }

        /// <summary>
        /// Primary key of the documents.
        /// </summary>
         public string PrimaryKey { get; internal set; }

        /// <summary>
        /// Initializes the Index with HTTP client. Only for internal usage.
        /// </summary>
        /// <param name="client">HTTP client from the base client.</param>
        /// <returns>The same object with the initialization.</returns>
        internal Index WithHttpClient(HttpClient client)
        {
            this._client = client;
            return this;
        }

        /// <summary>
        /// Changes the primary key of the index.
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
        /// Deletes the index.
        /// It's not a recovery delete. You will also lose the documents within the index.
        /// </summary>
        /// <returns>Returns the updateID of this async operation.</returns>
        public async Task<bool> Delete()
        {
           var responseMessage = await this._client.DeleteAsync($"/indexes/{Uid}");
           return responseMessage.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Add documents.
        /// </summary>
        /// <param name="documents">Documents to add.</param>
        /// <typeparam name="T">Type of the document. Even though documents are schemaless in MeiliSearch, making it typed helps in compile time.</typeparam>
        /// <returns>Returns the updateID of this async operation.</returns>
        public async Task<UpdateStatus> AddDocuments<T>(IEnumerable<T> documents)
        {
            var responseMessage = await this._client.PostAsJsonAsync($"/indexes/{Uid}/documents", documents);
            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Update documents.
        /// </summary>
        /// <param name="documents">Documents to update.</param>
        /// <typeparam name="T">Type of document. Even though documents are schemaless in MeiliSearch, making it typed helps in compile time.</typeparam>
        /// <returns>Returns the updateID of this async operation.</returns>
        public async Task<UpdateStatus> UpdateDocuments<T>(IEnumerable<T> documents)
        {
            var responseMessage = await this._client.PutAsJsonAsync($"/indexes/{Uid}/documents", documents);
            return await responseMessage.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Get document by its ID
        /// </summary>
        /// <param name="documentId">Document identifier.</param>
        /// <typeparam name="T">Type of the document.</typeparam>
        /// <returns>Returns the document, with the according type if the object is available.</returns>
        public async Task<T> GetDocument<T>(string documentId)
        {
           return await this._client.GetFromJsonAsync<T>($"/indexes/{Uid}/documents/{documentId}");
        }

        /// <summary>
        /// Get documents with the allowed query parameters.
        /// </summary>
        /// <param name="query">Query parameters. Supports limit, offset and attributes to retrieve.</param>
        /// <typeparam name="T">Type of the document.</typeparam>
        /// <returns>Returns the list of documents.</returns>
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
        /// Delete one document.
        /// </summary>
        /// <param name="documentId">Document identifier.</param>
        /// <returns>Returns the updateID of this async operation.</returns>
        public async Task<UpdateStatus> DeleteOneDocument(string documentId)
        {
            var httpresponse = await this._client.DeleteAsync($"/indexes/{Uid}/documents/{documentId}");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Delete documents in batch.
        /// </summary>
        /// <param name="documentIds">List of documents identifier.</param>
        /// <returns>Returns the updateID of this async operation.</returns>
        public async Task<UpdateStatus> DeleteDocuments(IEnumerable<string> documentIds)
        {
            var httpresponse = await this._client.PostAsJsonAsync($"/indexes/{Uid}/documents/delete-batch", documentIds);
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Delete all the documents in the index.
        /// </summary>
        /// <returns>Returns the updateID of this async operation.</returns>
        public async Task<UpdateStatus> DeleteAllDocuments()
        {
            var httpresponse = await this._client.DeleteAsync($"/indexes/{Uid}/documents");
            return await httpresponse.Content.ReadFromJsonAsync<UpdateStatus>();
        }

        /// <summary>
        /// Gets the update status of all the asynchronous operations.
        /// </summary>
        /// <returns>Returns a list of the operations status.</returns>
        public async Task<IEnumerable<UpdateStatus>> GetAllUpdateStatus()
        {
            return await this._client.GetFromJsonAsync<IEnumerable<UpdateStatus>>($"/indexes/{Uid}/updates");
        }

        /// <summary>
        /// Get Update Status by Status Id
        /// </summary>
        /// <param name="updateId">UpdateId for the operation.</param>
        /// <returns>Return the current status of the operation.</returns>
        public async Task<UpdateStatus> GetUpdateStatus(int updateId)
        {
            return await this._client.GetFromJsonAsync<UpdateStatus>($"/indexes/{Uid}/updates/{updateId}");
        }

        /// <summary>
        /// Search documents according to search parameters.
        /// </summary>
        /// <param name="query">Query Parameter with Search</param>
        /// <param name="searchattributes">Attributes to search.</param>
        /// <typeparam name="T">Type parameter to return</typeparam>
        /// <returns>Returns Enumerable of items</returns>
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

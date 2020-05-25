using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace Meilisearch
{
    /// <summary>
    /// Meilisearch Index for Search and managing document. 
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
        [JsonProperty(PropertyName = "uid")] public string Uid { get; internal set; }

        /// <summary>
        /// Primary key of the document.
        /// </summary>
        [JsonProperty(PropertyName = "primaryKey")] public string PrimaryKey { get; internal set; }

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
            var content = JsonConvert.SerializeObject(new {primaryKey = primarykeytoChange});
            var message = await this._client.PutAsync($"indexes/{Uid}", new StringContent(content, Encoding.UTF8));
            var responsecontent = await message.Content.ReadAsStringAsync();
            var index = JsonConvert.DeserializeObject<Index>(responsecontent);
            this.PrimaryKey = index.PrimaryKey;
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
        /// Add or Update Document .
        /// </summary>
        /// <param name="documents">Documents to update</param>
        /// <typeparam name="T">Type of document. Even though document is schemaless in meilisearch making it typed helps in compile time.</typeparam>
        /// <returns>Document create or update is Async in Meilisearch so status is returned back.</returns>
        public async Task<UpdateStatus> AddorUpdateDocuments<T>(IEnumerable<T> documents)
        {
            var content = JsonConvert.SerializeObject(documents);
            var responseMessage = await this._client.PostAsync($"/indexes/{Uid}/documents", new StringContent(content));
            var responsecontent = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UpdateStatus>(responsecontent);
        }
        
        /// <summary>
        /// Get document by its ID
        /// </summary>
        /// <param name="documentId">Document Id for query</param>
        /// <typeparam name="T">Type to return for document</typeparam>
        /// <returns>Type if the object is availble.</returns>
        public async Task<T> GetDocument<T>(string documentId)
        {
           var responseMessage = await this._client.GetAsync($"/indexes/{Uid}/documents/{documentId}");
           var responsecontent = await responseMessage.Content.ReadAsStringAsync();
           return JsonConvert.DeserializeObject<T>(responsecontent);
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
            var responseMessage = await this._client.GetAsync(uri);
            var responseContent = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<T>>(responseContent);
        }
        
        public async Task<UpdateStatus> DeleteOneDocument<T>(string documentId)
        {
            var httpresponse = await this._client.DeleteAsync($"/indexes/{Uid}/documents/{documentId}");
            var responsecontent = await httpresponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UpdateStatus>(responsecontent); 
        }
        
        public async Task<UpdateStatus> DeleteDocuments(IEnumerable<string> documentIds)
        {
            var content = JsonConvert.SerializeObject(documentIds);
            var httpresponse = await this._client.PostAsync($"/indexes/{Uid}/documents/delete-batch", new StringContent(content));
            var responsecontent = await httpresponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UpdateStatus>(responsecontent); 
        }
        
        public async Task<UpdateStatus> DeleteAllDocuments()
        {
            var httpresponse = await this._client.DeleteAsync($"/indexes/{Uid}/documents");
            var responsecontent = await httpresponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UpdateStatus>(responsecontent); 
        }


        /*
        public async Task<UpdateStatus> UpdateDocuments(IEnumerable<object> documents)
        {
            Need to support partial update of document.  Would be nice if Patch can be supported.
        }
  
        public async Task<IEnumerable<T>> Search<T>(string search)
        {
            
        }
        */
    }
}
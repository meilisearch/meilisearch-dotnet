using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

using Meilisearch.Extensions;
using Meilisearch.HttpContents;

namespace Meilisearch
{
    public partial class Index
    {
        /// <summary>
        /// Add documents.
        /// </summary>
        /// <param name="documents">Documents to add.</param>
        /// <param name="primaryKey">Primary key for the documents.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">Type of the document. Even though documents are schemaless in Meilisearch, making it typed helps in compile time.</typeparam>
        /// <returns>Returns the task info.</returns>
        public async Task<TaskInfo> AddDocumentsJsonAsync<T>(IEnumerable<T> documents, string primaryKey = default, CancellationToken cancellationToken = default)
        {
            var uri = $"indexes/{Uid}/documents";

            if (primaryKey != default)
            {
                uri = $"{uri}?{new { primaryKey = primaryKey }.ToQueryString()}";
            }

            var content = new JsonHttpContent(documents);
            var responseMessage = await _http.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Adds documents in batches with size specified with <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="documents">Documents to add.</param>
        /// <param name="batchSize">Size of documents batches while adding them.</param>
        /// <param name="primaryKey">Primary key for the documents.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">Type of the document. Even though documents are schemaless in Meilisearch, making it typed helps in compile time.</typeparam>
        /// <returns>Returns the task list.</returns>
        public async Task<IEnumerable<TaskInfo>> AddDocumentsJsonInBatchesAsync<T>(IEnumerable<T> documents, int batchSize = 1000, string primaryKey = default, CancellationToken cancellationToken = default)
        {
            var tasks = new List<TaskInfo>();
            foreach (var chunk in documents.GetChunks(batchSize))
            {
                tasks.Add(await AddDocumentsJsonAsync(chunk, primaryKey, cancellationToken).ConfigureAwait(false));
            }

            return tasks;
        }

        /// <summary>
        /// Update documents.
        /// </summary>
        /// <param name="documents">Documents to update.</param>
        /// <param name="primaryKey">Primary key for the documents.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">Type of document. Even though documents are schemaless in Meilisearch, making it typed helps in compile time.</typeparam>
        /// <returns>Returns the task list.</returns>
        public async Task<TaskInfo> UpdateDocumentsJsonAsync<T>(IEnumerable<T> documents, string primaryKey = default, CancellationToken cancellationToken = default)
        {
            var uri = $"indexes/{Uid}/documents";

            if (primaryKey != default)
            {
                uri = $"{uri}?{new { primaryKey = primaryKey }.ToQueryString()}";
            }

            var content = new JsonHttpContent(documents);
            var responseMessage = await _http.PutAsync(uri, content, cancellationToken).ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates documents in batches with size specified with <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="documents">Documents to update.</param>
        /// <param name="batchSize">Size of documents batches while updating them.</param>
        /// <param name="primaryKey">Primary key for the documents.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">Type of the document. Even though documents are schemaless in Meilisearch, making it typed helps in compile time.</typeparam>
        /// <returns>Returns the task list.</returns>
        public async Task<IEnumerable<TaskInfo>> UpdateDocumentsJsonInBatchesAsync<T>(IEnumerable<T> documents, int batchSize = 1000, string primaryKey = default, CancellationToken cancellationToken = default)
        {
            var tasks = new List<TaskInfo>();
            foreach (var chunk in documents.GetChunks(batchSize))
            {
                tasks.Add(await UpdateDocumentsJsonAsync(chunk, primaryKey, cancellationToken).ConfigureAwait(false));
            }

            return tasks;
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
            return await _http.GetFromJsonAsync<T>($"indexes/{Uid}/documents/{documentId}", cancellationToken: cancellationToken).ConfigureAwait(false);
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
            return await GetDocumentAsync<T>(documentId.ToString(), cancellationToken);
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
            var uri = $"indexes/{Uid}/documents";
            if (query != null)
            {
                uri = $"{uri}?{query.ToQueryString()}";
            }

            return await _http.GetFromJsonAsync<IEnumerable<T>>(uri, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete one document.
        /// </summary>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info.</returns>
        public async Task<TaskInfo> DeleteOneDocumentAsync(string documentId, CancellationToken cancellationToken = default)
        {
            var httpresponse = await _http.DeleteAsync($"indexes/{Uid}/documents/{documentId}", cancellationToken).ConfigureAwait(false);
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
            return await DeleteOneDocumentAsync(documentId.ToString(), cancellationToken).ConfigureAwait(false);
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
                await _http.PostAsJsonAsync($"indexes/{Uid}/documents/delete-batch", documentIds, cancellationToken: cancellationToken)
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
            return await DeleteDocumentsAsync(docIds, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete all the documents in the index.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info.</returns>
        public async Task<TaskInfo> DeleteAllDocumentsAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await _http.DeleteAsync($"indexes/{Uid}/documents", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
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

            var responseMessage = await _http.PostAsJsonAsync($"indexes/{Uid}/search", body, Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<SearchResult<T>>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}

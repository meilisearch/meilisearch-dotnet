using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Meilisearch
{
    public partial class Index
    {
        /// <summary>
        /// Search for similar documents.
        /// </summary>
        /// <param name="query">The query to search for similar documents.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">The type of the documents to return.</typeparam>
        /// <returns>Returns the similar documents.</returns>
        public async Task<SimilarDocumentsResult<T>> SearchSimilarDocumentsAsync<T>(
            SimilarDocumentsQuery query,
            CancellationToken cancellationToken = default)
        {
            var responseMessage = await _http
                .PostAsJsonAsync(
                    $"indexes/{Uid}/similar",
                    query,
                    Constants.JsonSerializerOptionsRemoveNulls,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await responseMessage.Content
                .ReadFromJsonAsync<SimilarDocumentsResult<T>>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}

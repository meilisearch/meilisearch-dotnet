using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Meilisearch
{
    public partial class Index
    {

        /// <summary>
        /// Gets the search cutoff in milliseconds.
        /// </summary>
        /// <returns>Returns the search cutoff in milliseconds.</returns>
        public async Task<int?> GetSearchCutoffMsAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<int?>($"indexes/{Uid}/settings/search-cutoff-ms", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        /// <summary>
        /// Sets the search cutoff in milliseconds.
        /// </summary>
        /// <param name="searchCutoffMs">The search cutoff in milliseconds.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateSearchCutoffMsAsync(int searchCutoffMs, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PutAsJsonAsync($"indexes/{Uid}/settings/search-cutoff-ms", searchCutoffMs, Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the search cutoff in milliseconds. (default: 1500)
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetSearchCutoffMsAsync(CancellationToken cancellationToken = default)
        {
            var responseMessage = await _http.DeleteAsync($"indexes/{Uid}/settings/search-cutoff-ms", cancellationToken)
                .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}

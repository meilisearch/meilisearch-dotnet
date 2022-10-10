using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

using Meilisearch.Extensions;
namespace Meilisearch
{
    public partial class Index
    {
        /// <summary>
        /// Gets the pagination setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the pagination setting.</returns>
        public async Task<Pagination> GetPaginationAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<Pagination>($"indexes/{Uid}/settings/pagination", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the pagination setting.
        /// </summary>
        /// <param name="pagination">Pagination instance</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdatePaginationAsync(Pagination pagination, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PatchAsJsonAsync($"indexes/{Uid}/settings/pagination", pagination, Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the pagination setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetPaginationAsync(CancellationToken cancellationToken = default)
        {
            var response = await _http.DeleteAsync($"indexes/{Uid}/settings/pagination", cancellationToken)
                .ConfigureAwait(false);

            return await response.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}

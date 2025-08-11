using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

using Meilisearch.Extensions;
namespace Meilisearch
{
    public partial class Index
    {
        /// <summary>
        /// Gets the embedders setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the embedders setting.</returns>
        public async Task<Dictionary<string, Embedder>> GetEmbeddersAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<Dictionary<string, Embedder>>($"indexes/{Uid}/settings/embedders", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the embedders setting.
        /// </summary>
        /// <param name="embedders">Collection of embedders</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateEmbeddersAsync(Dictionary<string, Embedder> embedders, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PatchAsJsonAsync($"indexes/{Uid}/settings/embedders", embedders, Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the embedders setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetEmbeddersAsync(CancellationToken cancellationToken = default)
        {
            var response = await _http.DeleteAsync($"indexes/{Uid}/settings/embedders", cancellationToken)
                .ConfigureAwait(false);

            return await response.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}

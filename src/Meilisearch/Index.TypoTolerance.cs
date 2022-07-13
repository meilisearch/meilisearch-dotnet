using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

using Meilisearch.Extensions;
namespace Meilisearch
{
    public partial class Index
    {
        /// <summary>
        /// Gets the typo tolerance setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the typo tolerance setting.</returns>
        public async Task<TypoTolerance> GetTypoToleranceAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<TypoTolerance>($"indexes/{Uid}/settings/typo-tolerance", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the typo tolerance setting.
        /// </summary>
        /// <param name="typoTolerance">TypoTolerance instance</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateTypoToleranceAsync(TypoTolerance typoTolerance, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PatchAsJsonAsync($"indexes/{Uid}/settings/typo-tolerance", typoTolerance, Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the typo tolerance setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetTypoToleranceAsync(CancellationToken cancellationToken = default)
        {
            var response = await _http.DeleteAsync($"indexes/{Uid}/settings/typo-tolerance", cancellationToken)
                .ConfigureAwait(false);

            return await response.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}

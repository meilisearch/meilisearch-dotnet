using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Meilisearch
{
    public partial class Index
    {
        /// <summary>
        /// Gets all the non separator tokens settings.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns all the configured non separator tokens.</returns>
        public async Task<List<string>> GetNonSeparatorTokensAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<List<string>>($"indexes/{Uid}/settings/non-separator-tokens", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates all the non separator tokens settings.
        /// </summary>
        /// <param name="nonSeparatorTokens">Collection of separator tokens.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateNonSeparatorTokensAsync(IEnumerable<string> nonSeparatorTokens, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PutAsJsonAsync($"indexes/{Uid}/settings/non-separator-tokens", nonSeparatorTokens, Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Resets all the non separator tokens settings.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetNonSeparatorTokensAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await _http.DeleteAsync($"indexes/{Uid}/settings/non-separator-tokens", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}

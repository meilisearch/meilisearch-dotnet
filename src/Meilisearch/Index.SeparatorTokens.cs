using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Meilisearch
{
    public partial class Index
    {
        /// <summary>
        /// Gets all the separator tokens settings.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns all the configured separator tokens.</returns>
        public async Task<IEnumerable<string>> GetSeparatorTokensAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<IEnumerable<string>>($"indexes/{Uid}/settings/separator-tokens", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates all the separator tokens settings.
        /// </summary>
        /// <param name="separatorTokens">Collection of separator tokens.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateSeparatorTokensAsync(IEnumerable<string> separatorTokens, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PutAsJsonAsync($"indexes/{Uid}/settings/separator-tokens", separatorTokens, Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Resets all the separator tokens settings.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetSeparatorTokensAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await _http.DeleteAsync($"indexes/{Uid}/settings/separator-tokens", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}

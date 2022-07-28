using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Meilisearch
{
    public partial class Index
    {
        /// <summary>
        /// Gets the synonyms setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the synonyms setting.</returns>
        public async Task<Dictionary<string, IEnumerable<string>>> GetSynonymsAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<Dictionary<string, IEnumerable<string>>>($"indexes/{Uid}/settings/synonyms", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the synonyms setting.
        /// </summary>
        /// <param name="synonyms">Collection of synonyms.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateSynonymsAsync(Dictionary<string, IEnumerable<string>> synonyms, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PutAsJsonAsync($"indexes/{Uid}/settings/synonyms", synonyms, Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the synonyms setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetSynonymsAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await _http.DeleteAsync($"indexes/{Uid}/settings/synonyms", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}

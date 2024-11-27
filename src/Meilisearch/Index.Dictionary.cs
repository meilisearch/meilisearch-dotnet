using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Meilisearch
{
    public partial class Index
    {
        /// <summary>
        /// Gets the dictionary of an index.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the dictionary.</returns>
        public async Task<string[]> GetDictionaryAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<string[]>($"indexes/{Uid}/settings/dictionary", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the dictionary of an index.
        /// </summary>
        /// <param name="dictionary">Dictionary object.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateDictionaryAsync(string[] dictionary, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PutAsJsonAsync($"indexes/{Uid}/settings/dictionary", dictionary, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the dictionary to their default values.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetDictionaryAsync(CancellationToken cancellationToken = default)
        {
            var httpResponse = await _http.DeleteAsync($"indexes/{Uid}/settings/dictionary", cancellationToken).ConfigureAwait(false);
            return await httpResponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}

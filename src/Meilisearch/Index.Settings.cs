using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Meilisearch
{
    public partial class Index
    {
        /// <summary>
        /// Gets all the settings of an index.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns all the settings.</returns>
        public async Task<Settings> GetSettingsAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<Settings>($"indexes/{Uid}/settings", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates all the settings of an index.
        /// The settings that are not passed in parameter are not overwritten.
        /// </summary>
        /// <param name="settings">Settings object.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateSettingsAsync(Settings settings, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PostAsJsonAsync($"indexes/{Uid}/settings", settings, Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Resets all the settings to their default values.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetSettingsAsync(CancellationToken cancellationToken = default)
        {
            var httpResponse = await _http.DeleteAsync($"indexes/{Uid}/settings", cancellationToken).ConfigureAwait(false);
            return await httpResponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}

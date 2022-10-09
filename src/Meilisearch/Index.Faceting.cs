using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

using Meilisearch.Extensions;
namespace Meilisearch
{
    public partial class Index
    {
        /// <summary>
        /// Gets the faceting setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the faceting setting.</returns>
        public async Task<Faceting> GetFacetingAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<Faceting>($"indexes/{Uid}/settings/faceting", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the faceting setting.
        /// </summary>
        /// <param name="faceting">Faceting instance</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateFacetingAsync(Faceting faceting, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PatchAsJsonAsync($"indexes/{Uid}/settings/faceting", faceting, Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the faceting setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetFacetingAsync(CancellationToken cancellationToken = default)
        {
            var response = await _http.DeleteAsync($"indexes/{Uid}/settings/faceting", cancellationToken)
                .ConfigureAwait(false);

            return await response.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}

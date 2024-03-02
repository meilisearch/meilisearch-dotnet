using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

using Meilisearch.Extensions;

namespace Meilisearch
{
    public partial class Index
    {
        /// <summary>
        /// Gets the proximity precision setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the proximity precision setting.</returns>
        public async Task<string> GetProximityPrecisionAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<string>($"indexes/{Uid}/settings/proximity-precision", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the proximity precision setting.
        /// </summary>
        /// <param name="proximityPrecision">The new proximity precision setting.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateProximityPrecisionAsync(string proximityPrecision, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PutAsJsonAsync($"indexes/{Uid}/settings/proximity-precision", proximityPrecision, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Resets proximity precision setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetProximityPrecisionAsync(CancellationToken cancellationToken = default)
        {
            var response = await _http.DeleteAsync($"indexes/{Uid}/settings/proximity-precision", cancellationToken)
                .ConfigureAwait(false);

            return await response.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}

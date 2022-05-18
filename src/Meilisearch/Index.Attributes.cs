using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Meilisearch
{
    public partial class Index
    {
        /// <summary>
        /// Gets the displayed attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the displayed attributes setting.</returns>
        public async Task<IEnumerable<string>> GetDisplayedAttributesAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<IEnumerable<string>>($"indexes/{Uid}/settings/displayed-attributes", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the displayed attributes setting.
        /// </summary>
        /// <param name="displayedAttributes">Collection of displayed attributes.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateDisplayedAttributesAsync(IEnumerable<string> displayedAttributes, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PostAsJsonAsync($"indexes/{Uid}/settings/displayed-attributes", displayedAttributes,
                        Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the displayed attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetDisplayedAttributesAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await _http.DeleteAsync($"indexes/{Uid}/settings/displayed-attributes", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the distinct attribute setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the distinct attribute setting.</returns>
        public async Task<string> GetDistinctAttributeAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<string>($"indexes/{Uid}/settings/distinct-attribute", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the distinct attribute setting.
        /// </summary>
        /// <param name="distinctAttribute">Name of distinct attribute.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateDistinctAttributeAsync(string distinctAttribute, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PostAsJsonAsync($"indexes/{Uid}/settings/distinct-attribute", distinctAttribute, Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the distinct attribute setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task Uid of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetDistinctAttributeAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await _http.DeleteAsync($"indexes/{Uid}/settings/distinct-attribute", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the filterable attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the filterable attributes setting.</returns>
        public async Task<IEnumerable<string>> GetFilterableAttributesAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<IEnumerable<string>>($"indexes/{Uid}/settings/filterable-attributes", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the filterable attributes setting.
        /// </summary>
        /// <param name="filterableAttributes">Collection of filterable attributes.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateFilterableAttributesAsync(IEnumerable<string> filterableAttributes, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PostAsJsonAsync($"indexes/{Uid}/settings/filterable-attributes", filterableAttributes, Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the filterable attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetFilterableAttributesAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await _http.DeleteAsync($"indexes/{Uid}/settings/filterable-attributes", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the searchable attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the searchable attributes setting.</returns>
        public async Task<IEnumerable<string>> GetSearchableAttributesAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<IEnumerable<string>>($"indexes/{Uid}/settings/searchable-attributes", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the searchable attributes setting.
        /// </summary>
        /// <param name="searchableAttributes">Collection of searchable attributes.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateSearchableAttributesAsync(IEnumerable<string> searchableAttributes, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PostAsJsonAsync($"indexes/{Uid}/settings/searchable-attributes", searchableAttributes, Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the searchable attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetSearchableAttributesAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await _http.DeleteAsync($"indexes/{Uid}/settings/searchable-attributes", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the sortable attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the sortable attributes setting.</returns>
        public async Task<IEnumerable<string>> GetSortableAttributesAsync(CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<IEnumerable<string>>($"indexes/{Uid}/settings/sortable-attributes", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the sortable attributes setting.
        /// </summary>
        /// <param name="sortableAttributes">Collection of sortable attributes.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> UpdateSortableAttributesAsync(IEnumerable<string> sortableAttributes, CancellationToken cancellationToken = default)
        {
            var responseMessage =
                await _http.PostAsJsonAsync($"indexes/{Uid}/settings/sortable-attributes", sortableAttributes, Constants.JsonSerializerOptionsRemoveNulls, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            return await responseMessage.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Resets the sortable attributes setting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of the asynchronous task.</returns>
        public async Task<TaskInfo> ResetSortableAttributesAsync(CancellationToken cancellationToken = default)
        {
            var httpresponse = await _http.DeleteAsync($"indexes/{Uid}/settings/sortable-attributes", cancellationToken)
                .ConfigureAwait(false);
            return await httpresponse.Content.ReadFromJsonAsync<TaskInfo>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}

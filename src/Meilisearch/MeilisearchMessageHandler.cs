using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Meilisearch
{
    /// <summary>
    /// Typed http request for Meilisearch.
    /// </summary>
    public class MeilisearchMessageHandler : DelegatingHandler
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchMessageHandler"/> class.
        /// Default message handler for Meilisearch API.
        /// </summary>
        public MeilisearchMessageHandler()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchMessageHandler"/> class.
        /// Default message handler for Meilisearch API.
        /// </summary>
        /// <param name="innerHandler">InnerHandler.</param>
        public MeilisearchMessageHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        /// <summary>
        /// Override SendAsync to handle errors.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Return HttpResponseMessage.</returns>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.Content.Headers.ContentLength != 0)
                    {
                        var content = await response.Content.ReadFromJsonAsync<MeilisearchApiErrorContent>(cancellationToken: cancellationToken).ConfigureAwait(false);
                        throw new MeilisearchApiError(content);
                    }

                    throw new MeilisearchApiError(response.StatusCode, response.ReasonPhrase);
                }

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new MeilisearchCommunicationError("CommunicationError", ex);
            }
        }
    }
}

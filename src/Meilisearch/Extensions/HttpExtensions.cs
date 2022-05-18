using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Meilisearch.Extensions
{
    /// <summary>
    /// Class to communicate with the Meilisearch server without charset-utf-8 as Content-Type.
    /// </summary>
    public static class HttpExtensions
    {
        /// <summary>
        /// Sends JSON payload using POST without "charset-utf-8" as Content-Type and using JSON serializer options.
        /// </summary>
        /// <param name="client">HttpClient.</param>
        /// <param name="uri">Endpoint.</param>
        /// <param name="body">Body sent.</param>
        /// <param name="options">Json options for serialization.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">Type of the body to send.</typeparam>
        /// <returns>Returns the HTTP response from the Meilisearch server.</returns>
        public static async Task<HttpResponseMessage> PostJsonCustomAsync<T>(this HttpClient client, string uri, T body, JsonSerializerOptions options, CancellationToken cancellationToken = default)
        {
            var payload = PrepareJsonPayload(body, options);

            return await client.PostAsync(uri, payload, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Add the API Key to the Authorization header.
        /// </summary>
        /// <param name="client">HttpClient.</param>
        /// <param name="apiKey">API Key.</param>
        public static void AddApiKeyToHeader(this HttpClient client, string apiKey)
        {
            if (!string.IsNullOrEmpty(apiKey))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }
        }

        public static void AddDefaultUserAgent(this HttpClient client)
        {
            var version = new Version();

            client.DefaultRequestHeaders.Add("User-Agent", version.GetQualifiedVersion());
        }

        private static StringContent PrepareJsonPayload<T>(T body, JsonSerializerOptions options = null)
        {
            options = options ?? Constants.JsonSerializerOptionsWriteNulls;
            var payload = new StringContent(JsonSerializer.Serialize(body, options), Encoding.UTF8, "application/json");
            payload.Headers.ContentType.CharSet = string.Empty;

            return payload;
        }
    }
}

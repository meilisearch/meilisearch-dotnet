using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Meilisearch.Extensions
{
    /// <summary>
    /// Class to communicate with the Meilisearch server without charset-utf-8 as Content-Type.
    /// </summary>
    internal static class HttpExtensions
    {
        /// <summary>
        /// Sends JSON payload using POST without "charset-utf-8" as Content-Type.
        /// </summary>
        /// <param name="client">HttpClient.</param>
        /// <param name="uri">Endpoint.</param>
        /// <param name="body">Body sent.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">Type of the body to send.</typeparam>
        /// <returns>Returns the HTTP response from the Meilisearch server.</returns>
        internal static async Task<HttpResponseMessage> PostJsonCustomAsync<T>(this HttpClient client, string uri, T body, CancellationToken cancellationToken = default)
        {
            var payload = PrepareJsonPayload(body);

            return await client.PostAsync(uri, payload, cancellationToken).ConfigureAwait(false);
        }

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
        internal static async Task<HttpResponseMessage> PostJsonCustomAsync<T>(this HttpClient client, string uri, T body, JsonSerializerOptions options, CancellationToken cancellationToken = default)
        {
            var payload = PrepareJsonPayload(body, options);

            return await client.PostAsync(uri, payload, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends JSON payload using PUT without "charset-utf-8" as Content-Type.
        /// </summary>
        /// <param name="client">HttpClient.</param>
        /// <param name="uri">Endpoint.</param>
        /// <param name="body">Body sent.</param>
        /// <param name="options">Json options for serialization.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <typeparam name="T">Type of the body to send.</typeparam>
        /// <returns>Returns the HTTP response from the Meilisearch server.</returns>
        internal static async Task<HttpResponseMessage> PutJsonCustomAsync<T>(this HttpClient client, string uri, T body, JsonSerializerOptions options, CancellationToken cancellationToken = default)
        {
            var payload = PrepareJsonPayload(body, options);

            return await client.PutAsync(uri, payload, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Add the API Key to the Authorization header.
        /// </summary>
        /// <param name="client">HttpClient.</param>
        /// <param name="apiKey">API Key.</param>
        internal static void AddApiKeyToHeader(this HttpClient client, string apiKey)
        {
            if (!string.IsNullOrEmpty(apiKey))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }
        }

        internal static void AddDefaultUserAgent(this HttpClient client)
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

        private static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var uri = new Uri(requestUri, UriKind.RelativeOrAbsolute);
            return client.PatchAsync(uri, content, cancellationToken);
        }

        private static Task<HttpResponseMessage> PatchAsync(this HttpClient client, Uri requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            // HttpClient.PatchAsync is not available in .NET standard and NET462
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri) { Content = content };
            return client.SendAsync(request, cancellationToken);
        }

        internal static Task<HttpResponseMessage> PatchAsJsonAsync<TValue>(this HttpClient client, string requestUri, TValue value, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        {
            var content = JsonContent.Create(value, mediaType: null, options);
            return client.PatchAsync(requestUri, content, cancellationToken);
        }
    }
}

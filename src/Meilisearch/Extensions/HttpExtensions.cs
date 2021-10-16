namespace Meilisearch.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// Class to communicate with the MeiliSearch server without charset-utf-8 as Content-Type.
    /// </summary>
    public static class HttpExtensions
    {
        /// <summary>
        /// Sends JSON payload using POST without "charset-utf-8" as Content-Type.
        /// </summary>
        /// <param name="client">HttpClient.</param>
        /// <param name="uri">Endpoint.</param>
        /// <param name="body">Body sent.</param>
        /// <typeparam name="T">Type of the body to send.</typeparam>
        /// <returns>Returns the HTTP response from the MeiliSearch server.</returns>
        public static async Task<HttpResponseMessage> PostJsonWithoutCharsetAsync<T>(this HttpClient client, string uri, T body)
        {
            var payload = PrepareJsonPayload<T>(body);

            return await client.PostAsync(uri, payload);
        }

        /// <summary>
        /// Sends JSON payload using POST without "charset-utf-8" as Content-Type and using JSON serializer options.
        /// </summary>
        /// <param name="client">HttpClient.</param>
        /// <param name="uri">Endpoint.</param>
        /// <param name="body">Body sent.</param>
        /// <param name="options">Json options for serialization.</param>
        /// <typeparam name="T">Type of the body to send.</typeparam>
        /// <returns>Returns the HTTP response from the MeiliSearch server.</returns>
        public static async Task<HttpResponseMessage> PostJsonWithoutCharsetAsync<T>(this HttpClient client, string uri, T body, JsonSerializerOptions options)
        {
            var payload = PrepareJsonPayload<T>(body, options);

            return await client.PostAsync(uri, payload);
        }

        /// <summary>
        /// Sends JSON payload using PUT without "charset-utf-8" as Content-Type.
        /// </summary>
        /// <param name="client">HttpClient.</param>
        /// <param name="uri">Endpoint.</param>
        /// <param name="body">Body sent.</param>
        /// <typeparam name="T">Type of the body to send.</typeparam>
        /// <returns>Returns the HTTP response from the MeiliSearch server.</returns>
        public static async Task<HttpResponseMessage> PutJsonWithoutCharsetAsync<T>(this HttpClient client, string uri, T body)
        {
            var payload = PrepareJsonPayload<T>(body);

            return await client.PutAsync(uri, payload);
        }

        /// <summary>
        /// Add the API Key to the X-Meili-API-Key header.
        /// </summary>
        /// <param name="client">HttpClient.</param>
        /// <param name="apiKey">API Key.</param>
        public static void AddApiKeyToHeader(this HttpClient client, string apiKey)
        {
            if (!string.IsNullOrEmpty(apiKey))
            {
                client.DefaultRequestHeaders.Add("X-Meili-API-Key", apiKey);
            }
        }

        private static StringContent PrepareJsonPayload<T>(T body, JsonSerializerOptions options = default)
        {
            if (options == null)
            {
                options = new JsonSerializerOptions();
            }

            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            var payload = new StringContent(JsonSerializer.Serialize(body, options), Encoding.UTF8, "application/json");
            payload.Headers.ContentType.CharSet = string.Empty;

            return payload;
        }
    }
}

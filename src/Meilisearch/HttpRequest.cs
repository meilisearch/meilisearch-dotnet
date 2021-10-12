namespace Meilisearch
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// Class to communicate with the MeiliSearch server.
    /// </summary>
    public class HttpRequest
    {
        private HttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequest"/> class.
        /// Default HTTP request handler for Meilisearch API.
        /// </summary>
        /// <param name="url">URL corresponding to MeiliSearch server.</param>
        /// <param name="apiKey">API Key to connect to the MeiliSearch server.</param>
        public HttpRequest(string url, string apiKey = default)
        {
            this.client = new HttpClient(new MeilisearchMessageHandler(new HttpClientHandler())) { BaseAddress = new Uri(url) };
            this.AddApiKeyToHeader(apiKey);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequest"/> class using a customed client.
        /// </summary>
        /// <param name="client">Injects the reusable HttpClient.</param>
        /// <param name="apiKey">API Key to connect to the MeiliSearch server. Best practice is to use HttpClient default header rather than this parameter.</param>
        public HttpRequest(HttpClient client, string apiKey = default)
        {
            this.client = client;
            this.AddApiKeyToHeader(apiKey);
        }

        /// <summary>
        /// Sends JSON payload using POST without "charset=utf-8" as Content-Type.
        /// </summary>
        /// <param name="uri">Endpoint.</param>
        /// <param name="body">Body sent.</param>
        /// <typeparam name="T">Type of the body to send.</typeparam>
        /// <returns>Returns the HTTP response from the MeiliSearch server.</returns>
        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string uri, T body)
        {
            var payload = this.PrepareJsonPayload<T>(body);

            return await this.client.PostAsync(uri, payload);
        }

        /// <summary>
        /// Sends JSON payload using POST without "charset=utf-8" as Content-Type and using JSON serializer options.
        /// </summary>
        /// <param name="uri">Endpoint.</param>
        /// <param name="body">Body sent.</param>
        /// <param name="options">Json options for serialization.</param>
        /// <typeparam name="T">Type of the body to send.</typeparam>
        /// <returns>Returns the HTTP response from the MeiliSearch server.</returns>
        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string uri, T body, JsonSerializerOptions options)
        {
            var payload = this.PrepareJsonPayload<T>(body, options);

            return await this.client.PostAsync(uri, payload);
        }

        /// <summary>
        /// Sends JSON payload using PUT without "charset=utf-8" as Content-Type.
        /// </summary>
        /// <param name="uri">Endpoint.</param>
        /// <param name="body">Body sent.</param>
        /// <typeparam name="T">Type of the body to send.</typeparam>
        /// <returns>Returns the HTTP response from the MeiliSearch server.</returns>
        public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string uri, T body)
        {
            var payload = this.PrepareJsonPayload<T>(body);

            return await this.client.PutAsync(uri, payload);
        }

        /// <summary>
        /// GetAsync of HttpClient.
        /// </summary>
        /// <param name="uri">Endpoint.</param>
        /// <returns>Returns the HTTP response from the MeiliSearch server.</returns>
        public async Task<HttpResponseMessage> GetAsync(string uri) {
            return await this.client.GetAsync(uri);
        }

        /// <summary>
        /// GetFromJsonAsync of HttpClient.
        /// </summary>
        /// <param name="uri">Endpoint.</param>
        /// <typeparam name="T">Type of the body to send.</typeparam>
        /// <returns>Returns the HTTP response from the MeiliSearch server.</returns>
        public async Task<T> GetFromJsonAsync<T>(string uri)
        {
            return await this.client.GetFromJsonAsync<T>(uri);
        }

        /// <summary>
        /// PostAsync of HttpClient.
        /// </summary>
        /// <param name="uri">Endpoint.</param>
        /// <returns>Returns the HTTP response from the MeiliSearch server.</returns>
        public async Task<HttpResponseMessage> PostAsync(string uri)
        {
            return await this.client.PostAsync(uri, default, default);
        }

        /// <summary>
        /// DeleteAsync of HttpClient.
        /// </summary>
        /// <param name="uri">Endpoint.</param>
        /// <returns>Returns the HTTP response from the MeiliSearch server.</returns>
        public async Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            return await this.client.DeleteAsync(uri);
        }

        private void AddApiKeyToHeader(string apiKey)
        {
            if (!string.IsNullOrEmpty(apiKey))
            {
                this.client.DefaultRequestHeaders.Add("X-Meili-API-Key", apiKey);
            }
        }

        private StringContent PrepareJsonPayload<T>(T body, JsonSerializerOptions options = default)
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

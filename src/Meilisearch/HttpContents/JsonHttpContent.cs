namespace Meilisearch.HttpContents
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// Send and receive HTTP content as JSON.
    /// </summary>
    public class JsonHttpContent : HttpContent
    {
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly object values;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonHttpContent"/> class.
        /// </summary>
        /// <param name="values">Values to be serialized.</param>
        /// <param name="jsonSerializerOptions">Override default JsonSerializerOptions.</param>
        /// <exception cref="ArgumentNullException">Thrown if values is null.</exception>
        public JsonHttpContent(object values, JsonSerializerOptions jsonSerializerOptions = null)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            this.values = values;

            this.jsonSerializerOptions = jsonSerializerOptions ?? MeilisearchClient.JsonSerializerOptions;
            this.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        /// <inheritdoc/>
        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            await JsonSerializer.SerializeAsync(stream, this.values, this.jsonSerializerOptions).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}

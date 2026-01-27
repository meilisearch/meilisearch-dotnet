using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

using Meilisearch.Compression;

namespace Meilisearch
{
    /// <summary>
    /// Typed http request for Meilisearch.
    /// </summary>
    public class MeilisearchMessageHandler : DelegatingHandler
    {
        private readonly CompressionOptions _compressionOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchMessageHandler"/> class.
        /// Default message handler for Meilisearch API.
        /// </summary>
        public MeilisearchMessageHandler()
        {
            _compressionOptions = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchMessageHandler"/> class.
        /// Default message handler for Meilisearch API.
        /// </summary>
        /// <param name="innerHandler">InnerHandler.</param>
        public MeilisearchMessageHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _compressionOptions = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchMessageHandler"/> class
        /// with compression options.
        /// </summary>
        /// <param name="innerHandler">InnerHandler.</param>
        /// <param name="compressionOptions">Compression configuration options.</param>
        public MeilisearchMessageHandler(HttpMessageHandler innerHandler, CompressionOptions compressionOptions)
            : base(innerHandler)
        {
            _compressionOptions = compressionOptions;
        }

        /// <summary>
        /// Override SendAsync to handle errors and compression.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Return HttpResponseMessage.</returns>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                // Apply compression if enabled and request has content
                if (_compressionOptions != null &&
                    _compressionOptions.Algorithm != CompressionAlgorithm.None &&
                    request.Content != null)
                {
                    // Validate algorithm support
                    if (!CompressionHelper.IsAlgorithmSupported(_compressionOptions.Algorithm))
                    {
                        var message = $"Compression algorithm '{_compressionOptions.Algorithm}' is not supported in the current runtime.";

                        if (_compressionOptions.Algorithm == CompressionAlgorithm.Deflate)
                        {
                            message += " Deflate requires .NET 6.0+ for ZLibStream support. Please use Gzip instead.";
                        }
                        else if (_compressionOptions.Algorithm == CompressionAlgorithm.Brotli)
                        {
                            message += " Brotli requires .NET Standard 2.1+ or .NET Core 2.1+.";
                        }

                        throw new NotSupportedException(message);
                    }

                    request.Content = await CompressionHelper.CompressAsync(request.Content, _compressionOptions)
                        .ConfigureAwait(false);
                }

                // Note: Accept-Encoding headers for response decompression are automatically
                // added by HttpClientHandler.AutomaticDecompression when enabled

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

using System;

namespace Meilisearch
{
    /// <summary>
    /// Compression algorithms supported by the SDK.
    /// </summary>
    public enum CompressionAlgorithm
    {
        /// <summary>No compression.</summary>
        None = 0,

        /// <summary>Gzip compression (RFC 1952).</summary>
        Gzip = 1,

        /// <summary>Deflate compression (RFC 1951).</summary>
        Deflate = 2,

        /// <summary>Brotli compression (.NET Standard 2.1+ / .NET Core 2.1+).</summary>
        Brotli = 3
    }

    /// <summary>
    /// Configuration options for HTTP request compression.
    /// </summary>
    public class CompressionOptions
    {
        /// <summary>
        /// Gets or sets the compression algorithm to use.
        /// Default is None (no compression).
        /// </summary>
        public CompressionAlgorithm Algorithm { get; set; } = CompressionAlgorithm.None;

        /// <summary>
        /// Gets or sets the minimum payload size in bytes before compression is applied.
        /// Default is 1400 bytes (1.4 KB).
        /// Set to 0 to compress all payloads.
        /// </summary>
        public int MinimumSizeBytes { get; set; } = 1400;

        /// <summary>
        /// Gets or sets whether to request compressed responses from the server
        /// by setting the Accept-Encoding header.
        /// Default is false.
        /// </summary>
        public bool EnableResponseDecompression { get; set; } = false;

        /// <summary>
        /// Creates default compression options with no compression enabled.
        /// </summary>
        public static CompressionOptions None => new CompressionOptions();

        /// <summary>
        /// Creates compression options with Gzip compression enabled.
        /// </summary>
        /// <param name="minimumSizeBytes">Minimum payload size to compress. Default is 1400 bytes.</param>
        /// <returns>Compression options configured for Gzip.</returns>
        public static CompressionOptions Gzip(int minimumSizeBytes = 1400) =>
            new CompressionOptions
            {
                Algorithm = CompressionAlgorithm.Gzip,
                MinimumSizeBytes = minimumSizeBytes
            };

        /// <summary>
        /// Creates compression options with Deflate compression enabled.
        /// </summary>
        /// <param name="minimumSizeBytes">Minimum payload size to compress. Default is 1400 bytes.</param>
        /// <returns>Compression options configured for Deflate.</returns>
        public static CompressionOptions Deflate(int minimumSizeBytes = 1400) =>
            new CompressionOptions
            {
                Algorithm = CompressionAlgorithm.Deflate,
                MinimumSizeBytes = minimumSizeBytes
            };
    }
}

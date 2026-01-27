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

        /// <summary>Deflate compression with zlib wrapper (RFC 1950). Requires .NET 6.0+.</summary>
        Deflate = 2,

        /// <summary>Brotli compression (.NET Standard 2.1+ / .NET Core 2.1+).</summary>
        Brotli = 3
    }

    /// <summary>
    /// Configuration options for HTTP request compression.
    /// </summary>
    public class CompressionOptions
    {
        private int _minimumSizeBytes = 1400;

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
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when value is negative.</exception>
        public int MinimumSizeBytes
        {
            get => _minimumSizeBytes;
            set
            {
                if (value < 0)
                {
                    throw new System.ArgumentOutOfRangeException(nameof(value), value, "MinimumSizeBytes cannot be negative.");
                }
                _minimumSizeBytes = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to enable automatic decompression of compressed responses from the server.
        /// When enabled, the client will request compressed responses and automatically decompress them.
        /// Default is false.
        /// </summary>
        /// <remarks>
        /// This option only takes effect when using the default <see cref="MeilisearchClient"/> constructor
        /// that creates its own HttpClient internally. If you provide a custom HttpClient, you must configure
        /// <see cref="System.Net.HttpClientHandler.AutomaticDecompression"/> on your HttpClientHandler yourself.
        /// </remarks>
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
        public static CompressionOptions Gzip(int minimumSizeBytes = 1400)
        {
            if (minimumSizeBytes < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(minimumSizeBytes), minimumSizeBytes, "MinimumSizeBytes cannot be negative.");
            }
            return new CompressionOptions
            {
                Algorithm = CompressionAlgorithm.Gzip,
                MinimumSizeBytes = minimumSizeBytes
            };
        }

        /// <summary>
        /// Creates compression options with Deflate compression enabled.
        /// </summary>
        /// <param name="minimumSizeBytes">Minimum payload size to compress. Default is 1400 bytes.</param>
        /// <returns>Compression options configured for Deflate.</returns>
        public static CompressionOptions Deflate(int minimumSizeBytes = 1400)
        {
            if (minimumSizeBytes < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(minimumSizeBytes), minimumSizeBytes, "MinimumSizeBytes cannot be negative.");
            }
            return new CompressionOptions
            {
                Algorithm = CompressionAlgorithm.Deflate,
                MinimumSizeBytes = minimumSizeBytes
            };
        }

        /// <summary>
        /// Creates compression options with Brotli compression enabled.
        /// </summary>
        /// <param name="minimumSizeBytes">Minimum payload size to compress. Default is 1400 bytes.</param>
        /// <returns>Compression options configured for Brotli.</returns>
        public static CompressionOptions Brotli(int minimumSizeBytes = 1400)
        {
            if (minimumSizeBytes < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(minimumSizeBytes), minimumSizeBytes, "MinimumSizeBytes cannot be negative.");
            }
            return new CompressionOptions
            {
                Algorithm = CompressionAlgorithm.Brotli,
                MinimumSizeBytes = minimumSizeBytes
            };
        }
    }
}

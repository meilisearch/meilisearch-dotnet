using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace Meilisearch.Compression
{
    /// <summary>
    /// Helper class for compressing HTTP request content.
    /// </summary>
    internal static class CompressionHelper
    {
        /// <summary>
        /// Wraps existing HttpContent with compression if applicable.
        /// </summary>
        /// <param name="content">Original HTTP content.</param>
        /// <param name="options">Compression options.</param>
        /// <returns>Compressed content or original if compression not applicable.</returns>
        internal static async Task<HttpContent> CompressAsync(HttpContent content, CompressionOptions options)
        {
            if (!ShouldCompress(content, options))
            {
                return content;
            }

            var originalBytes = await content.ReadAsByteArrayAsync().ConfigureAwait(false);

            if (!MeetsSizeThreshold(originalBytes, options))
            {
                // Content stream was already consumed; reconstruct it with original headers
                var reconstructedContent = new ByteArrayContent(originalBytes);
                foreach (var header in content.Headers)
                {
                    reconstructedContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
                return reconstructedContent;
            }

            var compressedBytes = CompressData(originalBytes, options.Algorithm);
            return CreateCompressedContent(compressedBytes, content, options.Algorithm);
        }

        /// <summary>
        /// Determines whether content should be compressed.
        /// </summary>
        private static bool ShouldCompress(HttpContent content, CompressionOptions options)
        {
            return content != null && options?.Algorithm != CompressionAlgorithm.None;
        }

        /// <summary>
        /// Checks if the data size meets the minimum threshold for compression.
        /// </summary>
        private static bool MeetsSizeThreshold(byte[] data, CompressionOptions options)
        {
            return data.Length >= options.MinimumSizeBytes;
        }

        /// <summary>
        /// Compresses data using the specified algorithm.
        /// </summary>
        private static byte[] CompressData(byte[] data, CompressionAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case CompressionAlgorithm.Gzip:
                    return CompressWithGzip(data);

                case CompressionAlgorithm.Deflate:
                    return CompressWithDeflate(data);

                case CompressionAlgorithm.Brotli:
                    return CompressWithBrotli(data);

                default:
                    throw new ArgumentException($"Unsupported compression algorithm: {algorithm}", nameof(algorithm));
            }
        }

        /// <summary>
        /// Creates new HttpContent with compressed data and appropriate headers.
        /// </summary>
        private static HttpContent CreateCompressedContent(byte[] compressedBytes, HttpContent originalContent, CompressionAlgorithm algorithm)
        {
            var compressedContent = new ByteArrayContent(compressedBytes);

            // Copy headers from original content, excluding Content-Encoding and Content-Length
            // as these will be set explicitly for the compressed content
            foreach (var header in originalContent.Headers)
            {
                if (header.Key != "Content-Encoding" && header.Key != "Content-Length")
                {
                    compressedContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Set Content-Encoding header
            var contentEncoding = GetContentEncoding(algorithm);
            compressedContent.Headers.ContentEncoding.Add(contentEncoding);

            // Set Content-Length
            compressedContent.Headers.ContentLength = compressedBytes.Length;

            return compressedContent;
        }

        /// <summary>
        /// Compresses data using Gzip algorithm.
        /// </summary>
        private static byte[] CompressWithGzip(byte[] data)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionLevel.Fastest, leaveOpen: true))
                {
                    gzipStream.Write(data, 0, data.Length);
                    gzipStream.Flush();
                }
                return outputStream.ToArray();
            }
        }

        /// <summary>
        /// Compresses data using Deflate algorithm with zlib wrapper.
        /// Meilisearch expects zlib format (RFC 1950), not raw deflate (RFC 1951).
        /// ZLibStream is only available in .NET 6.0+
        /// </summary>
        private static byte[] CompressWithDeflate(byte[] data)
        {
#if NET6_0_OR_GREATER
            using (var outputStream = new MemoryStream())
            {
                using (var zlibStream = new ZLibStream(outputStream, CompressionLevel.Fastest, leaveOpen: true))
                {
                    zlibStream.Write(data, 0, data.Length);
                    zlibStream.Flush();
                }
                return outputStream.ToArray();
            }
#else
            throw new NotSupportedException(
                "Deflate compression requires .NET 6.0+ for ZLibStream support. " +
                "Current target framework is .NET Standard 2.0. " +
                "DeflateStream produces raw deflate (RFC 1951), but Meilisearch expects zlib format (RFC 1950). " +
                "Please use Gzip compression instead, or target .NET 6.0+ in your project.");
#endif
        }

        /// <summary>
        /// Compresses data using Brotli algorithm.
        /// Only available in .NET Standard 2.1+ / .NET Core 2.1+
        /// </summary>
        private static byte[] CompressWithBrotli(byte[] data)
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            using (var outputStream = new MemoryStream())
            {
                using (var brotliStream = new BrotliStream(outputStream, CompressionLevel.Fastest, leaveOpen: true))
                {
                    brotliStream.Write(data, 0, data.Length);
                    brotliStream.Flush();
                }
                return outputStream.ToArray();
            }
#else
            throw new NotSupportedException(
                "Brotli compression requires .NET Standard 2.1+ or .NET Core 2.1+. " +
                "Current target framework is .NET Standard 2.0. " +
                "Please use Gzip or Deflate compression instead.");
#endif
        }

        /// <summary>
        /// Gets the Content-Encoding header value for a given algorithm.
        /// </summary>
        internal static string GetContentEncoding(CompressionAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case CompressionAlgorithm.Gzip:
                    return "gzip";
                case CompressionAlgorithm.Deflate:
                    return "deflate";
                case CompressionAlgorithm.Brotli:
                    return "br";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Checks if the compression algorithm is supported in the current runtime.
        /// </summary>
        internal static bool IsAlgorithmSupported(CompressionAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case CompressionAlgorithm.Gzip:
                    return true;

                case CompressionAlgorithm.Deflate:
#if NET6_0_OR_GREATER
                    return true;
#else
                    return false;
#endif

                case CompressionAlgorithm.Brotli:
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                    return true;
#else
                    return false;
#endif

                default:
                    return false;
            }
        }
    }
}

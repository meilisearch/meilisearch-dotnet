using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Meilisearch.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// This method makes sure that the uri has a trailing slash.
        /// If not, it will append one silently.
        /// </summary>
        /// <param name="uri">uri to Meilisearch server.</param>
        /// <returns>A well formatted Uri</returns>
        /// <exception cref="ArgumentNullException">Thrown when uri is not or whitespace.</exception>
        public static Uri ToSafeUri(this string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                throw new ArgumentNullException(nameof(uri));
            }

            var trimmed = uri.Trim();
            return trimmed.EndsWith("/") ? new Uri(trimmed) : new Uri($"{trimmed}/");
        }

        /// <summary>
        /// Returns chunks from a CSV string.
        /// </summary>
        /// <param name="csvString">The CSV string to split.</param>
        /// <param name="chunkSize">Size of the chunks.</param>
        /// <returns>List of CSV string.</returns>
        /// <exception cref="ArgumentNullException">Thrown if csvString is null.</exception>
        /// <exception cref="ArgumentException">Throw if chunkSize is lower than 1.</exception>
        internal static IEnumerable<string> GetCsvChunks(this string csvString, int chunkSize)
        {
            if (string.IsNullOrWhiteSpace(csvString))
            {
                throw new ArgumentNullException(nameof(csvString));
            }

            if (chunkSize < 1)
            {
                throw new ArgumentException("chunkSize value must be greater than 0", nameof(chunkSize));
            }

            using (var sr = new StringReader(csvString))
            {
                // We extract the CSV header on first line
                var csvHeader = sr.ReadLine();

                var sb = new StringBuilder();
                // We add the CSV header first on our chunck
                sb.AppendLine(csvHeader);
                var line = "";
                var lineNumber = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                    ++lineNumber;

                    if (lineNumber % chunkSize == 0)
                    {
                        // We return our chunk, we clear our string builder and add back on first line the CSV header
                        yield return sb.ToString();
                        sb.Clear();
                        sb.AppendLine(csvHeader);
                    }
                }

                // After the last line we check if we have something to send
                if (lineNumber % chunkSize != 0)
                {
                    yield return sb.ToString();
                }
            }
        }

        /// <summary>
        /// Returns chunks from a NDJSON string.
        /// </summary>
        /// <param name="ndjsonString">The NDJSON string to split.</param>
        /// <param name="chunkSize">Size of the chunks.</param>
        /// <returns>List of NDJSON string.</returns>
        /// <exception cref="ArgumentNullException">Thrown if ndjsonString is null.</exception>
        /// <exception cref="ArgumentException">Throw if chunkSize is lower than 1.</exception>
        internal static IEnumerable<string> GetNdjsonChunks(this string ndjsonString, int chunkSize)
        {
            if (string.IsNullOrWhiteSpace(ndjsonString))
            {
                throw new ArgumentNullException(nameof(ndjsonString));
            }

            if (chunkSize < 1)
            {
                throw new ArgumentException("chunkSize value must be greater than 0", nameof(chunkSize));
            }

            using (var sr = new StringReader(ndjsonString))
            {
                var sb = new StringBuilder();
                var line = "";
                var lineNumber = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                    ++lineNumber;

                    if (lineNumber % chunkSize == 0)
                    {
                        // We return our chunk, we clear our string builder
                        yield return sb.ToString();
                        sb.Clear();
                    }
                }

                // After the last line we check if we have something to send
                if (lineNumber % chunkSize != 0)
                {
                    yield return sb.ToString();
                }
            }
        }
    }
}

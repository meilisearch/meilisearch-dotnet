using System;

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
    }
}

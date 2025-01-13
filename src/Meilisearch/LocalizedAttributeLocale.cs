using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Localized attribute locale.
    /// </summary>
    public class LocalizedAttributeLocale
    {
        /// <summary>
        /// Gets or sets the locales.
        /// </summary>
        [JsonPropertyName("locale")]
        public IEnumerable<string> Locale { get; set; }

        /// <summary>
        /// Gets or sets the attribute patterns.
        /// </summary>
        [JsonPropertyName("attributePatterns")]
        public IEnumerable<string> AttributePatterns { get; set; }
    }
}

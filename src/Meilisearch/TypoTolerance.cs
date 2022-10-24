using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Typo Tolerance configuration.
    /// </summary>
    public class TypoTolerance
    {
        /// <summary>
        /// Whether the typo tolerance feature is enabled
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool? Enabled { get; set; }

        /// <summary>
        /// Disable the typo tolerance feature on the specified document attributes.
        /// </summary>
        [JsonPropertyName("disableOnAttributes")]
        public IEnumerable<string> DisableOnAttributes { get; set; }

        /// <summary>
        /// Disable the typo tolerance feature for a given set of terms in a search query.
        /// </summary>
        [JsonPropertyName("disableOnWords")]
        public IEnumerable<string> DisableOnWords { get; set; }

        /// <summary>
        /// Customize the minimum word size to tolerate typos.
        /// </summary>
        [JsonPropertyName("minWordSizeForTypos")]
        public TypoSize MinWordSizeForTypos { get; set; }

        public class TypoSize
        {
            /// <summary>
            /// Customize the minimum word size to tolerate 1 typo.
            /// </summary>
            [JsonPropertyName("oneTypo")]
            public int? OneTypo { get; set; }

            /// <summary>
            /// Customize the minimum word size to tolerate 2 typos.
            /// </summary>
            [JsonPropertyName("twoTypos")]
            public int? TwoTypos { get; set; }
        }
    }
}

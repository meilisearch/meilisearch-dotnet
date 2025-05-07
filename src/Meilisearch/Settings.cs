using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Setttings of an index.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Gets or sets the ranking rules.
        /// </summary>
        [JsonPropertyName("rankingRules")]
        public IEnumerable<string> RankingRules { get; set; }

        /// <summary>
        /// Gets or sets the distinct attribute.
        /// </summary>
        [JsonPropertyName("distinctAttribute")]
        public string DistinctAttribute { get; set; }

        /// <summary>
        /// Gets or sets the searchable attributes.
        /// </summary>
        [JsonPropertyName("searchableAttributes")]
        public IEnumerable<string> SearchableAttributes { get; set; }

        /// <summary>
        /// Gets or sets the displayed attributes.
        /// </summary>
        [JsonPropertyName("displayedAttributes")]
        public IEnumerable<string> DisplayedAttributes { get; set; }

        /// <summary>
        /// Gets or sets the stop-words list.
        /// </summary>
        [JsonPropertyName("stopWords")]
        public IEnumerable<string> StopWords { get; set; }

        /// <summary>
        /// Gets or sets the synonyms list.
        /// </summary>
        [JsonPropertyName("synonyms")]
        public Dictionary<string, IEnumerable<string>> Synonyms { get; set; }

        /// <summary>
        /// Gets or sets the non separator tokens list.
        /// </summary>
        [JsonPropertyName("nonSeparatorTokens")]
        public List<string> NonSeparatorTokens { get; set; }

        /// <summary>
        /// Gets or sets the separator tokens list.
        /// </summary>
        [JsonPropertyName("separatorTokens")]
        public List<string> SeparatorTokens { get; set; }

        /// <summary>
        /// Gets or sets the filterable attributes.
        /// </summary>
        [JsonPropertyName("filterableAttributes")]
        public IEnumerable<string> FilterableAttributes { get; set; }

        /// <summary>
        /// Gets or sets the localized attributes.
        /// </summary>
        [JsonPropertyName("localizedAttributes")]
        public IEnumerable<LocalizedAttributeLocale> LocalizedAttributes { get; set; }

        /// <summary>
        /// Gets or sets the sortable attributes.
        /// </summary>
        [JsonPropertyName("sortableAttributes")]
        public IEnumerable<string> SortableAttributes { get; set; }

        /// <summary>
        /// Gets or sets the typo tolerance attributes.
        /// </summary>
        [JsonPropertyName("typoTolerance")]
        public TypoTolerance TypoTolerance { get; set; }

        /// <summary>
        /// Gets or sets the faceting attributes.
        /// </summary>
        [JsonPropertyName("faceting")]
        public Faceting Faceting { get; set; }

        /// <summary>
        /// Gets or sets the pagination attributes.
        /// </summary>
        [JsonPropertyName("pagination")]
        public Pagination Pagination { get; set; }

        /// <summary>
        /// Gets or sets the dictionary object.
        /// </summary>
        [JsonPropertyName("dictionary")]
        public IEnumerable<string> Dictionary { get; set; }

        /// <summary>
        /// Gets or sets the proximity precision attribute.
        /// </summary>
        [JsonPropertyName("proximityPrecision")]
        public string ProximityPrecision { get; set; }

        /// <summary>
        /// Gets or sets the searchCutoffMs attribute.
        /// </summary>
        [JsonPropertyName("searchCutoffMs")]
        public int? SearchCutoffMs { get; set; }
    }
}

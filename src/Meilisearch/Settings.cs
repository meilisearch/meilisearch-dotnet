using System.Collections.Generic;

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
        public IEnumerable<string> RankingRules { get; set; }

        /// <summary>
        /// Gets or sets the distinct attribute.
        /// </summary>
        public string DistinctAttribute { get; set; }

        /// <summary>
        /// Gets or sets the searchable attributes.
        /// </summary>
        public IEnumerable<string> SearchableAttributes { get; set; }

        /// <summary>
        /// Gets or sets the displayed attributes.
        /// </summary>
        public IEnumerable<string> DisplayedAttributes { get; set; }

        /// <summary>
        /// Gets or sets the stop-words list.
        /// </summary>
        public IEnumerable<string> StopWords { get; set; }

        /// <summary>
        /// Gets or sets the synonyms list.
        /// </summary>
        public Dictionary<string, IEnumerable<string>> Synonyms { get; set; }

        /// <summary>
        /// Gets or sets the attributes for faceting.
        /// </summary>
        public IEnumerable<string> AttributesForFaceting { get; set; }
    }
}

using System.Collections.Generic;
using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    /// <summary>
    /// Represents a single filterable attribute entry.
    ///
    /// Since Meilisearch v1.14, <c>filterableAttributes</c> accepts either a plain
    /// attribute name or a granular pattern object that opts in/out of specific features.
    /// Use the implicit conversion from <see cref="string"/> for the legacy form, or set
    /// <see cref="AttributePatterns"/> and <see cref="Features"/> for the granular form.
    /// </summary>
    [JsonConverter(typeof(FilterableAttributeConverter))]
    public class FilterableAttribute
    {
        /// <summary>
        /// Plain attribute name. When set, the entry is serialized as a JSON string
        /// and <see cref="AttributePatterns"/> / <see cref="Features"/> are ignored.
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// Attribute name patterns (granular syntax).
        /// </summary>
        [JsonPropertyName("attributePatterns")]
        public IEnumerable<string> AttributePatterns { get; set; }

        /// <summary>
        /// Per-feature opt-in flags (granular syntax).
        /// </summary>
        [JsonPropertyName("features")]
        public FilterableAttributeFeatures Features { get; set; }

        /// <summary>
        /// Implicitly wraps a plain attribute name in a <see cref="FilterableAttribute"/>.
        /// Enables passing a <see cref="string"/> wherever a <see cref="FilterableAttribute"/> is expected.
        /// A <c>null</c> input produces a <c>null</c> result so it serializes as JSON <c>null</c>
        /// rather than an empty object.
        /// </summary>
        /// <param name="attribute">The attribute name, or <c>null</c>.</param>
        public static implicit operator FilterableAttribute(string attribute)
        {
            return attribute == null ? null : new FilterableAttribute { Attribute = attribute };
        }
    }

    /// <summary>
    /// Feature opt-in flags for a granular <see cref="FilterableAttribute"/>.
    /// </summary>
    public class FilterableAttributeFeatures
    {
        /// <summary>
        /// Gets or sets whether facet search is enabled for the matched attributes.
        /// </summary>
        [JsonPropertyName("facetSearch")]
        public bool FacetSearch { get; set; }

        /// <summary>
        /// Gets or sets the filter-related feature opt-ins for the matched attributes.
        /// </summary>
        [JsonPropertyName("filter")]
        public FilterableAttributeFilterFeatures Filter { get; set; }
    }

    /// <summary>
    /// Filter-related feature opt-ins inside <see cref="FilterableAttributeFeatures"/>.
    /// </summary>
    public class FilterableAttributeFilterFeatures
    {
        /// <summary>
        /// Gets or sets whether equality filtering (e.g. <c>genre = comedy</c>) is enabled.
        /// </summary>
        [JsonPropertyName("equality")]
        public bool Equality { get; set; }

        /// <summary>
        /// Gets or sets whether comparison filtering (e.g. <c>price &gt; 10</c>) is enabled.
        /// </summary>
        [JsonPropertyName("comparison")]
        public bool Comparison { get; set; }
    }
}

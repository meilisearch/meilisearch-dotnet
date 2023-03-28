using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// The default implmentation of <see cref="IFormatContainer{TOriginal,TFormatted}"/>
    /// </summary>
    /// <typeparam name="TOriginal"></typeparam>
    /// <typeparam name="TFormatted"></typeparam>
    public class DefaultFormattable<TOriginal, TFormatted> : IFormatContainer<TOriginal, TFormatted>
    {
        /// <summary>
        /// Creates a formatted document
        /// </summary>
        /// <param name="original"></param>
        /// <param name="formatted"></param>
        public DefaultFormattable(TOriginal original, TFormatted formatted)
        {
            Original = original;
            Formatted = formatted;
        }

        /// <summary>
        /// The original document
        /// </summary>
        public TOriginal Original { get; }

        /// <summary>
        /// The formatted document
        /// </summary>
        [JsonPropertyName("_formatted")]
        public TFormatted Formatted { get; }
    }
}

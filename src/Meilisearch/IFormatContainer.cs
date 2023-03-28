using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Used to receive document formatting information
    /// </summary>
    /// <typeparam name="TOriginal">Original data type</typeparam>
    /// <typeparam name="TFormatted">Formatted data type</typeparam>
    [JsonConverter(typeof(IFormatContainerJsonConverterFactory))]
    public interface IFormatContainer<TOriginal, TFormatted>
    {
        /// <summary>
        /// The original result
        /// </summary>
        TOriginal Original { get; }

        /// <summary>
        /// The formatted result
        /// </summary>
        TFormatted Formatted { get; }
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    /// <summary>
    /// This class adds some defaults to work with Meilisearch client.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// JsonSerializer options used when serializing objects that needs to remove null values.
        /// </summary>
        internal static readonly JsonSerializerOptions FederatedSearchJsonSerializerOptionsRemoveNulls =
            new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new MultiSearchFederationOptionsConverter() }
            };

        /// <summary>
        /// JsonSerializer options used when serializing objects that needs to remove null values.
        /// </summary>
        internal static readonly JsonSerializerOptions JsonSerializerOptionsRemoveNulls = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        /// <summary>
        /// JsonSerializer options used when serializing objects that keeps null values.
        /// </summary>
        internal static readonly JsonSerializerOptions JsonSerializerOptionsWriteNulls = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        internal static string VersionErrorHintMessage(string message, string method)
        {
            return
                $"{message}\nHint: It might not be working because maybe you're not up to date with the Meilisearch version that ${method} call requires.";
        }
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// This class adds some defaults to work with Meilisearch client.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// JsonSerializer options used when serializing objects that needs to remove null values.
        /// </summary>
        public static readonly JsonSerializerOptions JsonSerializerOptionsRemoveNulls = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        /// <summary>
        /// JsonSerializer options used when serializing objects that keeps null values.
        /// </summary>
        public static readonly JsonSerializerOptions JsonSerializerOptionsWriteNulls = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }
}
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper for index stats.
    /// </summary>
    public class Stats
    {
        public Stats(int databaseSize, DateTime? lastUpdate, IReadOnlyDictionary<string, IndexStats> indexes)
        {
            DatabaseSize = databaseSize;
            LastUpdate = lastUpdate;
            Indexes = indexes;
        }

        /// <summary>
        /// Gets or sets database size.
        /// </summary>
        [JsonPropertyName("databaseSize")]
        public int DatabaseSize { get; }

        /// <summary>
        /// Gets or sets last update timestamp.
        /// </summary>
        [JsonPropertyName("lastUpdate")]
        public DateTime? LastUpdate { get; }

        /// <summary>
        /// Gets or sets index stats.
        /// </summary>
        [JsonPropertyName("indexes")]
        public IReadOnlyDictionary<string, IndexStats> Indexes { get; }
    }
}

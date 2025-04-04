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
        public Stats(long databaseSize, DateTime? lastUpdate, IReadOnlyDictionary<string, IndexStats> indexes, long usedDatabaseSize)
        {
            DatabaseSize = databaseSize;
            LastUpdate = lastUpdate;
            Indexes = indexes;
            UsedDatabaseSize = usedDatabaseSize;
        }

        /// <summary>
        /// Gets database size.
        /// </summary>
        [JsonPropertyName("databaseSize")]
        public long DatabaseSize { get; }

        /// <summary>
        /// Gets the total space used by the data stored in Meilisearch.
        /// </summary>
        [JsonPropertyName("usedDatabaseSize")]
        public long UsedDatabaseSize { get; }

        /// <summary>
        /// Gets last update timestamp.
        /// </summary>
        [JsonPropertyName("lastUpdate")]
        public DateTime? LastUpdate { get; }

        /// <summary>
        /// Gets index stats.
        /// </summary>
        [JsonPropertyName("indexes")]
        public IReadOnlyDictionary<string, IndexStats> Indexes { get; }
    }
}

using System.Text.Json;

using FluentAssertions;

using Xunit;

namespace Meilisearch.Tests
{
    public class IndexStatsTests
    {
        [Fact]
        public void DeserializesIndexSizesWhenReported()
        {
            var json = @"{
                ""numberOfDocuments"": 2,
                ""isIndexing"": false,
                ""fieldDistribution"": { ""objectID"": 2 },
                ""rawDocumentDbSize"": 4096,
                ""avgDocumentSize"": 2040,
                ""numberOfEmbeddedDocuments"": 0,
                ""numberOfEmbeddings"": 0,
                ""indexSize"": 98304,
                ""usedIndexSize"": 81920
            }";

            var stats = JsonSerializer.Deserialize<IndexStats>(json);

            stats.IndexSize.Should().Be(98304);
            stats.UsedIndexSize.Should().Be(81920);
        }

        [Fact]
        public void IndexSizesAreNullWhenNotReported()
        {
            // Meilisearch instances older than v1.53.0 omit both fields.
            var json = @"{
                ""numberOfDocuments"": 2,
                ""isIndexing"": false,
                ""fieldDistribution"": { ""objectID"": 2 },
                ""rawDocumentDbSize"": 4096,
                ""avgDocumentSize"": 2040,
                ""numberOfEmbeddedDocuments"": 0,
                ""numberOfEmbeddings"": 0
            }";

            var stats = JsonSerializer.Deserialize<IndexStats>(json);

            stats.IndexSize.Should().BeNull();
            stats.UsedIndexSize.Should().BeNull();
            stats.NumberOfDocuments.Should().Be(2);
            stats.RawDocumentDbSize.Should().Be(4096);
        }

        [Fact]
        public void DeserializesIndexSizesFromAllIndexesStats()
        {
            var json = @"{
                ""databaseSize"": 1146880,
                ""usedDatabaseSize"": 925696,
                ""lastUpdate"": ""2025-11-15T10:03:15.000000000Z"",
                ""indexes"": {
                    ""movies"": {
                        ""numberOfDocuments"": 2,
                        ""isIndexing"": false,
                        ""fieldDistribution"": { ""objectID"": 2 },
                        ""rawDocumentDbSize"": 4096,
                        ""avgDocumentSize"": 2040,
                        ""numberOfEmbeddedDocuments"": 0,
                        ""numberOfEmbeddings"": 0,
                        ""indexSize"": 98304,
                        ""usedIndexSize"": 81920
                    }
                }
            }";

            var stats = JsonSerializer.Deserialize<Stats>(json);

            stats.Indexes["movies"].IndexSize.Should().Be(98304);
            stats.Indexes["movies"].UsedIndexSize.Should().Be(81920);
        }
    }
}

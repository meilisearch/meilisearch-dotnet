namespace Meilisearch.Tests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    [Collection("Sequential")]
    public class IndexTests
    {
        private MeilisearchClient defaultClient;

        public IndexTests(IndexFixture fixture)
        {
            fixture.DeleteAllIndexes().Wait(); // Test context cleaned for each [Fact]
            this.defaultClient = fixture.DefaultClient;
        }

        [Fact]
        public async Task UpdatePrimaryKey()
        {
            var index = await this.defaultClient.GetOrCreateIndex("UpdatePrimaryKeyTest");
            var primarykey = "MovieId" + new Random().Next();
            var modifiedIndex = await index.ChangePrimaryKey(primarykey);
            modifiedIndex.PrimaryKey.Should().Be(primarykey);
        }

        [Fact]
        public async Task GetStats()
        {
            var index = await this.defaultClient.GetOrCreateIndex("GetStatsTests");
            var stats = await index.GetStats();
            stats.Should().NotBeNull();
        }
    }
}

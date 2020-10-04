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
        private Random random;

        public IndexTests()
        {
            this.defaultClient = new MeilisearchClient("http://localhost:7700", "masterKey");
            this.random = new Random();
        }

        [Fact]
        public async Task UpdatePrimaryKey()
        {
            var index = await this.defaultClient.CreateIndex("Indextest" + this.random.Next());
            var primarykey = "MovieId" + new Random().Next();
            var modifiedIndex = await index.ChangePrimaryKey(primarykey);
            modifiedIndex.PrimaryKey.Should().Be(primarykey);
        }

        [Fact]
        public async Task GetStats()
        {
            var index = await this.defaultClient.GetOrCreateIndex("Statstest" + this.random.Next());
            var stats = await index.GetStats();
            stats.Should().NotBeNull();
        }
    }
}

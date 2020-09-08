namespace Meilisearch.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    [Collection("Sequential")]

    public class StatusTests
    {
        public StatusTests()
        {
        }

        [Fact]
        public async Task GetAllUpdateStatus()
        {
            var client = new MeilisearchClient("http://localhost:7700", "masterKey");
            var indexName = "MoviesStatus" + new Random().Next();
            var index = await client.CreateIndex(indexName);
            await index.AddDocuments(new[] { new Movie { Id = "1" } });
            var status = await index.GetAllUpdateStatus();
            status.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task GetOneUpdateStatus()
        {
            var client = new MeilisearchClient("http://localhost:7700", "masterKey");
            var indexName = "MoviesStatus" + new Random().Next();
            var index = await client.CreateIndex(indexName);
            var status = await index.AddDocuments(new[] { new Movie { Id = "2" } });
            UpdateStatus individualStatus = await index.GetUpdateStatus(status.UpdateId);
            individualStatus.Should().NotBeNull();
        }
    }
}

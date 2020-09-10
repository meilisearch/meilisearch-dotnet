namespace Meilisearch.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using HttpClientFactoryLite;
    using Xunit;

    [Collection("Sequential")]
    public class MeilisearchClientTests
    {
        private MeilisearchClient defaultClient;

        public MeilisearchClientTests()
        {
            this.defaultClient = new MeilisearchClient("http://localhost:7700", "masterKey");
        }

        [Fact]
        public async Task GetVersionWithCustomClient()
        {
            var httpClient = ClientFactory.Instance.CreateClient<MeilisearchClient>();
            var client = new MeilisearchClient(httpClient);
            var meilisearchversion = await client.GetVersion();
            meilisearchversion.Version.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetVersionWithDefaultClient()
        {
            var meilisearchversion = await this.defaultClient.GetVersion();
            meilisearchversion.Version.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task BasicUsageOfCustomClient()
        {
            var httpClient = ClientFactory.Instance.CreateClient<MeilisearchClient>();
            MeilisearchClient ms = new MeilisearchClient(httpClient);
            var indexUid = "uid" + new Random().Next();
            Meilisearch.Index index = await ms.CreateIndex(indexUid);
            var updateStatus = await index.AddDocuments(new[] { new Movie { Id = "1", Name = "Batman" } });
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task BasicIndexCreation()
        {
            var indexUid = "uid" + new Random().Next();
            var index = await this.defaultClient.CreateIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
        }

        [Fact]
        public async Task IndexCreationWithPrimaryKey()
        {
            var indexUid = "uid2" + new Random().Next();
            var index = await this.defaultClient.CreateIndex(indexUid, "movieId");
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be("movieId");
        }

        [Fact]
        public async Task IndexAlreadyExistsError()
        {
            var indexUid = "uid3" + new Random().Next();
            var index = await this.defaultClient.CreateIndex(indexUid, "movieId");
            await Assert.ThrowsAsync<Exception>(() => this.defaultClient.CreateIndex(indexUid, "movieId"));
        }

        [Fact]
        public async Task IndexUIDWrongFormattedError()
        {
            await Assert.ThrowsAsync<Exception>(() => this.defaultClient.CreateIndex("wrong UID"));
        }

        [Fact]
        public async Task GetAllExistingIndexes()
        {
            var indexUid = "uid4" + new Random().Next();
            await this.defaultClient.CreateIndex(indexUid, "movieId");
            var indexes = await this.defaultClient.GetAllIndexes();
            indexes.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task GetOneExistingIndexes()
        {
            var indexUid = "uid5" + new Random().Next();
            await this.defaultClient.CreateIndex(indexUid, "movieId");
            var index = await this.defaultClient.GetIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be("movieId");
        }

        [Fact]
        public async Task GetAnNonExistingIndex()
        {
            var indexes = await this.defaultClient.GetIndex("someRandomIndex");
            indexes.Should().BeNull();
        }
    }
}

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
        private string defaultPrimaryKey;

        public MeilisearchClientTests(IndexFixture fixture)
        {
            fixture.DeleteAllIndexes().Wait(); // Test context cleaned for each [Fact]
            this.defaultClient = fixture.DefaultClient;
            this.defaultPrimaryKey = "movieId";
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
            var indexUid = "BasicUsageOfCustomClientTest";
            Meilisearch.Index index = await ms.CreateIndex(indexUid);
            var updateStatus = await index.AddDocuments(new[] { new Movie { Id = "1", Name = "Batman" } });
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task BasicIndexCreation()
        {
            var indexUid = "BasicIndexCreationTest";
            var index = await this.defaultClient.CreateIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
        }

        [Fact]
        public async Task IndexCreationWithPrimaryKey()
        {
            var indexUid = "IndexCreationWithPrimaryKeyTest";
            var index = await this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be(this.defaultPrimaryKey);
        }

        [Fact]
        public async Task IndexAlreadyExistsError()
        {
            var indexUid = "IndexAlreadyExistsErrorTest";
            var index = await this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey);
            await Assert.ThrowsAsync<Exception>(() => this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey));
        }

        [Fact]
        public async Task IndexNameWrongFormattedError()
        {
            await Assert.ThrowsAsync<Exception>(() => this.defaultClient.CreateIndex("wrong UID"));
        }

        [Fact]
        public async Task GetAllExistingIndexes()
        {
            var indexUid = "GetAllExistingIndexesTest";
            await this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey);
            var indexes = await this.defaultClient.GetAllIndexes();
            indexes.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task GetOneExistingIndexes()
        {
            var indexUid = "GetOneExistingIndexesTest";
            await this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey);
            var index = await this.defaultClient.GetIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be(this.defaultPrimaryKey);
        }

        [Fact]
        public async Task GetAnNonExistingIndex()
        {
            var indexes = await this.defaultClient.GetIndex("someRandomIndex");
            indexes.Should().BeNull();
        }

        [Fact]
        public async Task GetOrCreateIndexIfIndexDoesNotExist()
        {
            var indexUid = "GetOrCreateIndexIfIndexDoesNotExistTest";
            var index = await this.defaultClient.GetOrCreateIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
        }

        [Fact]
        public async Task GetOrCreateIndexIfIndexAlreadyExists()
        {
            var indexUid = "GetOrCreateIndexIfIndexAlreadyExistsTest";
            await this.defaultClient.GetOrCreateIndex(indexUid);
            var index = await this.defaultClient.GetOrCreateIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
        }

        [Fact]
        public async Task GetOrCreateIndexWithPrimaryKey()
        {
            var indexUid = "GetOrCreateIndexWithPrimaryKeyTest";
            await this.defaultClient.GetOrCreateIndex(indexUid, this.defaultPrimaryKey);
            var index = await this.defaultClient.GetOrCreateIndex(indexUid, this.defaultPrimaryKey);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be(this.defaultPrimaryKey);
        }

        [Fact]
        public async Task GetStats()
        {
            var stats = await this.defaultClient.GetStats();
            stats.Should().NotBeNull();
        }

        [Fact]
        public async Task Health()
        {
            var health = await this.defaultClient.Health();
            health.Should().BeTrue();
        }
    }
}

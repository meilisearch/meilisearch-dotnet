namespace Meilisearch.Tests
{
    using System;
    using System.Linq;
    using System.Net.Http;
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
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey));
            Assert.Equal("index_already_exists", ex.ErrorCode);
        }

        [Fact]
        public async Task ErrorHandlerOfCustomClient()
        {
            var httpClient = ClientFactory.Instance.CreateClient<MeilisearchClient>();
            MeilisearchClient ms = new MeilisearchClient(httpClient);
            var indexUid = "IndexAlreadyExistsErrorTest";
            var index = await ms.CreateIndex(indexUid, this.defaultPrimaryKey);
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => ms.CreateIndex(indexUid, this.defaultPrimaryKey));
            Assert.Equal("index_already_exists", ex.ErrorCode);
        }

        [Fact]
        public async Task IndexNameWrongFormattedError()
        {
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.defaultClient.CreateIndex("wrong UID"));
            Assert.Equal("invalid_index_uid", ex.ErrorCode);
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
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.defaultClient.GetIndex("someRandomIndex"));
            Assert.Equal("index_not_found", ex.ErrorCode);
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
        public async Task CreateDumps()
        {
            var dumpResponse = await this.defaultClient.CreateDump();

            dumpResponse.Status.Should().Be("in_progress");
            Assert.Matches("\\d+-\\d+", dumpResponse.Uid);
        }

        [Fact]
        public async Task GetDumpStatusById()
        {
            var dump = await this.defaultClient.CreateDump();
            Assert.NotNull(dump);

            var dumpStatus = await this.defaultClient.GetDumpStatus(dump.Uid);

            dumpStatus.Status.Should().Be("done");
            Assert.Equal(dump.Uid, dumpStatus.Uid);
        }

        [Fact]
        public async Task Health()
        {
            var health = await this.defaultClient.Health();
            health.Status.Should().Be("available");
        }

        [Fact]
        public async Task HealthWithBadUrl()
        {
            var client = new MeilisearchClient("http://wrongurl:1234", "masterKey");
            MeilisearchCommunicationError ex = await Assert.ThrowsAsync<MeilisearchCommunicationError>(() => client.Health());
            Assert.Equal("CommunicationError", ex.Message);
        }

        [Fact]
        public async Task IsHealthy()
        {
            var health = await this.defaultClient.IsHealthy();
            health.Should().BeTrue();
        }

        [Fact]
        public async Task IsHealthyWithBadUrl()
        {
            var client = new MeilisearchClient("http://wrongurl:1234", "masterKey");
            var health = await client.IsHealthy();
            health.Should().BeFalse();
        }

        [Fact]
        public async Task ExceptionWithBadPath()
        {
            var client = new HttpClient(new MeilisearchMessageHandler(new HttpClientHandler())) { BaseAddress = new Uri("http://localhost:7700/") };
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => client.GetAsync("/wrong-path"));
            Assert.Equal("MeilisearchApiError, Message: Not Found, ErrorCode: 404", ex.Message);
        }
    }
}

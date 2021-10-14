namespace Meilisearch.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using HttpClientFactoryLite;
    using Xunit;

    [Collection("Sequential")]
    public class MeilisearchClientTests : IAsyncLifetime
    {
        private MeilisearchClient defaultClient;
        private string defaultPrimaryKey;

        private IndexFixture fixture;

        public MeilisearchClientTests(IndexFixture fixture)
        {
            this.fixture = fixture;
            this.defaultClient = fixture.DefaultClient;
            this.defaultPrimaryKey = "movieId";
        }

        public async Task InitializeAsync() => await this.fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]

        public Task DisposeAsync() => Task.CompletedTask;

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
            await index.WaitForPendingUpdate(updateStatus.UpdateId);
            index.FetchPrimaryKey().Should().Equals("id"); // Check the JSON has been well serialized and the primary key is not equal to "Id"
        }

        [Fact]
        public async Task ErrorHandlerOfCustomClient()
        {
            var httpClient = ClientFactory.Instance.CreateClient<MeilisearchClient>();
            MeilisearchClient ms = new MeilisearchClient(httpClient);
            var indexUid = "ErrorHandlerOfCustomClientTest";
            var index = await ms.CreateIndex(indexUid, this.defaultPrimaryKey);
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => ms.CreateIndex(indexUid, this.defaultPrimaryKey));
            Assert.Equal("index_already_exists", ex.ErrorCode);
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

        [Fact]
        public async Task DeleteIndex()
        {
            var httpClient = ClientFactory.Instance.CreateClient<MeilisearchClient>();
            MeilisearchClient ms = new MeilisearchClient(httpClient);
            var indexUid = "DeleteIndexTest";
            var index = await ms.CreateIndex(indexUid, this.defaultPrimaryKey);
            var deletedResult = await ms.DeleteIndex(indexUid);
            deletedResult.Should().BeTrue();
        }

        [Fact]
        public async Task GivenValidIndex_DeleteIndexIfExists_ShouldReturnTrue()
        {
            var httpClient = ClientFactory.Instance.CreateClient<MeilisearchClient>();
            MeilisearchClient ms = new MeilisearchClient(httpClient);
            var indexUid = "DeleteIndexIfExistsTest";
            var index = await ms.CreateIndex(indexUid, this.defaultPrimaryKey);
            var deletedResult = await ms.DeleteIndexIfExists(indexUid);
            deletedResult.Should().BeTrue();
        }

        [Fact]
        public async Task GivenInvalidIndex_DeleteIndexIfExists_ShouldReturnFalse()
        {
            var httpClient = ClientFactory.Instance.CreateClient<MeilisearchClient>();
            MeilisearchClient ms = new MeilisearchClient(httpClient);
            var invalidIndexUid = "NonExistingIndexTest";
            var deletedResult = await ms.DeleteIndexIfExists(invalidIndexUid);
            deletedResult.Should().BeFalse();
        }
    }
}

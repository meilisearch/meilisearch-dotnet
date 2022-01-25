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
            var meilisearchversion = await client.GetVersionAsync();
            meilisearchversion.Version.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetVersionWithDefaultClient()
        {
            var meilisearchversion = await this.defaultClient.GetVersionAsync();
            meilisearchversion.Version.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task BasicUsageOfCustomClient()
        {
            var indexUid = "BasicUsageOfCustomClientTest";

            var httpClient = ClientFactory.Instance.CreateClient<MeilisearchClient>();
            MeilisearchClient ms = new MeilisearchClient(httpClient);

            var task = await ms.CreateIndexAsync(indexUid);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            var index = this.defaultClient.Index(indexUid);
            task = await index.AddDocumentsAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.Uid);
            index.FetchPrimaryKey().Should().Equals("id"); // Check the JSON has been well serialized and the primary key is not equal to "Id"
        }

        [Fact]
        public async Task ErrorHandlerOfCustomClient()
        {
            var httpClient = ClientFactory.Instance.CreateClient<MeilisearchClient>();
            MeilisearchClient ms = new MeilisearchClient(httpClient);
            var indexUid = "wrong UID";
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => ms.CreateIndexAsync(indexUid, this.defaultPrimaryKey));
            Assert.Equal("invalid_index_uid", ex.Code);
        }

        [Fact]
        public async Task GetStats()
        {
            var stats = await this.defaultClient.GetStats();
            stats.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAndGetDumps()
        {
            var dumpResponse = await this.defaultClient.CreateDumpAsync();
            Assert.NotNull(dumpResponse);

            dumpResponse.Status.Should().Be("in_progress");
            Assert.Matches("\\d+-\\d+", dumpResponse.Uid);

            var dumpStatus = await this.defaultClient.GetDumpStatusAsync(dumpResponse.Uid);
            dumpStatus.Status.Should().BeOneOf("done", "in_progress");
            Assert.Equal(dumpResponse.Uid, dumpStatus.Uid);
        }

        [Fact]
        public async Task Health()
        {
            var health = await this.defaultClient.HealthAsync();
            health.Status.Should().Be("available");
        }

        [Fact]
        public async Task HealthWithBadUrl()
        {
            var client = new MeilisearchClient("http://wrongurl:1234", "masterKey");
            MeilisearchCommunicationError ex = await Assert.ThrowsAsync<MeilisearchCommunicationError>(() => client.HealthAsync());
            Assert.Equal("CommunicationError", ex.Message);
        }

        [Fact]
        public async Task IsHealthy()
        {
            var health = await this.defaultClient.IsHealthyAsync();
            health.Should().BeTrue();
        }

        [Fact]
        public async Task IsHealthyWithBadUrl()
        {
            var client = new MeilisearchClient("http://wrongurl:1234", "masterKey");
            var health = await client.IsHealthyAsync();
            health.Should().BeFalse();
        }

        [Fact]
        public async Task ExceptionWithBadPath()
        {
            var client = new HttpClient(new MeilisearchMessageHandler(new HttpClientHandler())) { BaseAddress = new Uri("http://localhost:7700/") };
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => client.GetAsync("/wrong-path"));
            Assert.Equal("MeilisearchApiError, Message: Not Found, Code: 404", ex.Message);
        }

        [Fact]
        public async Task DeleteIndex()
        {
            var httpClient = ClientFactory.Instance.CreateClient<MeilisearchClient>();
            MeilisearchClient ms = new MeilisearchClient(httpClient);
            var indexUid = "DeleteIndexTest";
            await ms.CreateIndexAsync(indexUid, this.defaultPrimaryKey);
            var task = await ms.DeleteIndexAsync(indexUid);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            var finishedTask = await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);
            Assert.Equal("succeeded", finishedTask.Status);
        }
    }
}

using System;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;

using Meilisearch.Extensions;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class MeilisearchClientTests<TFixture> : IAsyncLifetime where TFixture : IndexFixture
    {
        private readonly MeilisearchClient _defaultClient;
        private readonly string _defaultPrimaryKey;

        private readonly TFixture _fixture;

        public MeilisearchClientTests(TFixture fixture)
        {
            _fixture = fixture;
            _defaultClient = fixture.DefaultClient;
            _defaultPrimaryKey = "movieId";
        }

        public async Task InitializeAsync() => await _fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetVersionWithCustomClient()
        {
            var meilisearchversion = await _fixture.ClientWithCustomHttpClient.GetVersionAsync();
            meilisearchversion.Version.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetVersionWithDefaultClient()
        {
            var meilisearchversion = await _defaultClient.GetVersionAsync();
            meilisearchversion.Version.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task BasicUsageOfCustomClient()
        {
            var indexUid = "BasicUsageOfCustomClientTest";

            var task = await _fixture.ClientWithCustomHttpClient.CreateIndexAsync(indexUid);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await _defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            var index = _defaultClient.Index(indexUid);
            task = await index.AddDocumentsAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.Uid);

            // Check the JSON has been well serialized and the primary key is equal to "id"
            Assert.Equal("id", await index.FetchPrimaryKey());
        }

        [Fact]
        public async Task ErrorHandlerOfCustomClient()
        {
            var indexUid = "wrong UID";
            var ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => _fixture.ClientWithCustomHttpClient.CreateIndexAsync(indexUid, _defaultPrimaryKey));
            Assert.Equal("invalid_index_uid", ex.Code);
        }

        [Fact]
        public async Task GetStats()
        {
            var stats = await _defaultClient.GetStats();
            stats.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAndGetDumps()
        {
            var dumpResponse = await _defaultClient.CreateDumpAsync();
            Assert.NotNull(dumpResponse);

            dumpResponse.Status.Should().Be("in_progress");
            Assert.Matches("\\d+-\\d+", dumpResponse.Uid);

            var dumpStatus = await _defaultClient.GetDumpStatusAsync(dumpResponse.Uid);
            dumpStatus.Status.Should().BeOneOf("done", "in_progress");
            Assert.Equal(dumpResponse.Uid, dumpStatus.Uid);
        }

        [Fact]
        public async Task Health()
        {
            var health = await _defaultClient.HealthAsync();
            health.Status.Should().Be("available");
        }

        [Fact]
        public async Task HealthWithBadUrl()
        {
            var client = new MeilisearchClient(_fixture.MeilisearchAddress.Replace("localhost", "badhost"), "masterKey");
            var ex = await Assert.ThrowsAsync<MeilisearchCommunicationError>(() => client.HealthAsync());
            Assert.Equal("CommunicationError", ex.Message);
        }

        [Fact]
        public async Task IsHealthy()
        {
            var health = await _defaultClient.IsHealthyAsync();
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
            var client = new HttpClient(new MeilisearchMessageHandler(new HttpClientHandler())) { BaseAddress = _fixture.MeilisearchAddress.ToSafeUri() };
            var ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => client.GetAsync("wrong-path"));
            Assert.Equal("MeilisearchApiError, Message: Not Found, Code: 404", ex.Message);
        }

        [Fact]
        public async Task DeleteIndex()
        {
            var indexUid = "DeleteIndexTest";
            await _fixture.ClientWithCustomHttpClient.CreateIndexAsync(indexUid, _defaultPrimaryKey);
            var task = await _fixture.ClientWithCustomHttpClient.DeleteIndexAsync(indexUid);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            var finishedTask = await _defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);
            Assert.Equal("succeeded", finishedTask.Status);
        }
    }
}

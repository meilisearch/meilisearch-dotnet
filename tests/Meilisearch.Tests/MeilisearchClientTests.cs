using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using FluentAssertions;

using Meilisearch.Converters;
using Meilisearch.Extensions;
using Meilisearch.QueryParameters;

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
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await _defaultClient.Index(indexUid).WaitForTaskAsync(task.TaskUid);

            var index = _defaultClient.Index(indexUid);
            task = await index.AddDocumentsAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

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
            var stats = await _defaultClient.GetStatsAsync();
            stats.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateAndGetDumps()
        {
            var dumpResponse = await _defaultClient.CreateDumpAsync();
            Assert.NotNull(dumpResponse);

            dumpResponse.Status.Should().Be(TaskInfoStatus.Enqueued);

            var dumpTask = await _defaultClient.GetTaskAsync(dumpResponse.TaskUid);
            dumpTask.Status.Should().BeOneOf(TaskInfoStatus.Succeeded, TaskInfoStatus.Processing, TaskInfoStatus.Enqueued);
            Assert.Equal(dumpResponse.TaskUid, dumpTask.Uid);
        }

        [Fact]
        public async Task CreateAndGetSnapshots()
        {
            var snapshotResponse = await _defaultClient.CreateSnapshotAsync();
            Assert.NotNull(snapshotResponse);

            snapshotResponse.Status.Should().Be(TaskInfoStatus.Enqueued);

            var snapshotTask = await _defaultClient.GetTaskAsync(snapshotResponse.TaskUid);
            snapshotTask.Status.Should().BeOneOf(TaskInfoStatus.Succeeded, TaskInfoStatus.Processing, TaskInfoStatus.Enqueued);
            Assert.Equal(snapshotResponse.TaskUid, snapshotTask.Uid);
        }

        [Fact]
        public async Task CancelTasks()
        {
            var date = DateTime.Now;
            var formattedDate = Uri.EscapeDataString(((DateTime)date).ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz"));
            var response = await _defaultClient.CancelTasksAsync(new CancelTasksQuery
            {
                Uids = new List<int> { 1, 4 },
                AfterStartedAt = date
            });
            var task = await _defaultClient.WaitForTaskAsync(response.TaskUid);

            response.TaskUid.Should().Be(task.Uid);
            response.Type.Should().Be(TaskInfoType.TaskCancelation);
            task.Status.Should().Be(TaskInfoStatus.Succeeded);
            Assert.Equal($"?uids=1,4&afterStartedAt={formattedDate}", task.Details["originalFilter"].ToString());
        }

        [Fact]
        public async Task DeleteTasks()
        {
            var date = DateTime.Now;
            var formattedDate = Uri.EscapeDataString(((DateTime)date).ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz"));
            var response = await _defaultClient.DeleteTasksAsync(new DeleteTasksQuery
            {
                Uids = new List<int> { 1, 4 },
                AfterStartedAt = date
            });
            var task = await _defaultClient.WaitForTaskAsync(response.TaskUid);

            response.TaskUid.Should().Be(task.Uid);
            response.Type.Should().Be(TaskInfoType.TaskDeletion);
            task.Status.Should().Be(TaskInfoStatus.Succeeded);
            Assert.Equal($"?uids=1,4&afterStartedAt={formattedDate}", task.Details["originalFilter"].ToString());
        }

        [Fact]
        public async Task SwapIndexes()
        {
            var swaps = new List<IndexSwap> { new IndexSwap("indexA", "indexB") };
            var response = await _defaultClient.SwapIndexesAsync(swaps);
            var task = await _defaultClient.WaitForTaskAsync(response.TaskUid);

            response.TaskUid.Should().Be(task.Uid);
            response.Type.Should().Be(TaskInfoType.IndexSwap);
            task.Status.Should().Be(TaskInfoStatus.Failed);
            Assert.Equal(task.Details["swaps"].ToString(), JsonSerializer.Serialize(swaps).ToString());
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
            var client = new MeilisearchClient("http://badhost:7700", "masterKey");
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
            var client = new HttpClient(new MeilisearchMessageHandler(new HttpClientHandler())) { BaseAddress = _fixture.MeilisearchAddress().ToSafeUri() };
            var ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => client.GetAsync("wrong-path"));
            Assert.Equal("MeilisearchApiError, Message: Not Found, Code: 404", ex.Message);
        }

        [Fact]
        public async Task DeleteIndex()
        {
            var indexUid = "DeleteIndexTest";
            await _fixture.ClientWithCustomHttpClient.CreateIndexAsync(indexUid, _defaultPrimaryKey);
            var task = await _fixture.ClientWithCustomHttpClient.DeleteIndexAsync(indexUid);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            var finishedTask = await _defaultClient.Index(indexUid).WaitForTaskAsync(task.TaskUid);
            Assert.Equal(TaskInfoStatus.Succeeded, finishedTask.Status);
        }

        [Fact]
        public void Deserialize_UnknownTaskType_ReturnsUnknown()
        {
            var json = "\"NonExistentTaskType\"";
            var options = new JsonSerializerOptions
            {
                Converters = { new TaskInfoTypeConverter() }
            };

            var result = JsonSerializer.Deserialize<TaskInfoType>(json, options);

            Assert.Equal(TaskInfoType.Unknown, result);
        }

        [Fact]
        public void Deserialize_KnownTaskType_ReturnsEnumValue()
        {
            var json = "\"IndexCreation\"";
            var options = new JsonSerializerOptions
            {
                Converters = { new TaskInfoTypeConverter() }
            };

            var result = JsonSerializer.Deserialize<TaskInfoType>(json, options);

            Assert.Equal(TaskInfoType.IndexCreation, result);
        }

        [Fact]
        public async Task GetExperimentalFeatures()
        {
            await ResetExperimentalFeatures();

            var features = await _defaultClient.GetExperimentalFeaturesAsync();

            Assert.NotNull(features);
            Assert.NotEmpty(features);
            Assert.All(features, x => Assert.False(x.Value));
        }

        [Fact]
        public async Task UpdateExperimentalFeatures()
        {
            await ResetExperimentalFeatures();

            var currentFeatures = await _defaultClient.GetExperimentalFeaturesAsync();
            Assert.Contains(currentFeatures, x => x.Key == "vectorStore" && x.Value == false);

            var result = await _defaultClient.UpdateExperimentalFeatureAsync("vectorStore", true);

            Assert.Equal("vectorStore", result.Key);
            Assert.True(result.Value);

            var updatedFeatures = await _defaultClient.GetExperimentalFeaturesAsync();
            Assert.Contains(updatedFeatures, x => x.Key == "vectorStore" && x.Value == true);
        }

        private async Task ResetExperimentalFeatures()
        {
            foreach (var feature in await _defaultClient.GetExperimentalFeaturesAsync())
                await _defaultClient.UpdateExperimentalFeatureAsync(feature.Key, false);
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Meilisearch.QueryParameters;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class IndexTests<TFixture> : IAsyncLifetime where TFixture : IndexFixture
    {
        private readonly MeilisearchClient _client;
        private readonly string _defaultPrimaryKey;
        private readonly TFixture _fixture;

        public IndexTests(TFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.DefaultClient;
            _defaultPrimaryKey = "movieId";
        }

        public async Task InitializeAsync() => await _fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task BasicIndexCreation()
        {
            var indexUid = "BasicIndexCreationTest";
            await _fixture.SetUpEmptyIndex(indexUid);

            var index = await _client.GetIndexAsync(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
        }

        [Fact]
        public async Task IndexCreationWithPrimaryKey()
        {
            var indexUid = "IndexCreationWithPrimaryKeyTest";
            await _fixture.SetUpEmptyIndex(indexUid, _defaultPrimaryKey);

            var index = await _client.GetIndexAsync(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be(_defaultPrimaryKey);
        }

        [Fact]
        public async Task BasicUsageOfIndexMethod()
        {
            var indexUid = "BasicUsageOfIndexMethodTest";
            var index = _client.Index(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
            var ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => _client.GetIndexAsync(indexUid));
            Assert.Equal("index_not_found", ex.Code);
        }

        [Fact]
        public async Task IndexMethodUsageOnExistingIndex()
        {
            var indexUid = "IndexMethodUsageOnExistingIndexTest";
            var index = await _fixture.SetUpEmptyIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();

            var document = await index.AddDocumentsAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
            document.TaskUid.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task IndexFetchInfoPrimaryKey()
        {
            var indexUid = "IndexFetchInfoPrimaryKeyTest";
            var index = await _fixture.SetUpEmptyIndex(indexUid, _defaultPrimaryKey);

            await index.FetchInfoAsync();
            Assert.Equal(index.Uid, indexUid);
            index.PrimaryKey.Should().Be(_defaultPrimaryKey);

            var primaryKey = await index.FetchPrimaryKey();
            Assert.Equal(index.PrimaryKey, primaryKey);
        }

        [Fact]
        public async Task IndexAlreadyExistsError()
        {
            var indexUid = "IndexAlreadyExistsErrorTest";

            await _client.CreateIndexAsync(indexUid, _defaultPrimaryKey);
            var task = await _client.CreateIndexAsync(indexUid, _defaultPrimaryKey);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            var finishedTask = await _client.Index(indexUid).WaitForTaskAsync(task.TaskUid);

            Assert.Equal(task.TaskUid, finishedTask.Uid);
            Assert.Equal(indexUid, finishedTask.IndexUid);
            Assert.Equal(TaskInfoStatus.Failed, finishedTask.Status);
            var error = finishedTask.Error;
            error.Should().NotBeNull();
            Assert.Equal("index_already_exists", error["code"]);
        }

        [Fact]
        public async Task UpdateIndex()
        {
            var indexUid = "UpdateIndexTest";
            var primarykey = "MovieId" + new Random().Next();

            await _fixture.SetUpEmptyIndex(indexUid);

            var task = await _client.UpdateIndexAsync(indexUid, primarykey);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await _client.Index(indexUid).WaitForTaskAsync(task.TaskUid);

            var index = await _client.GetIndexAsync(indexUid);
            index.PrimaryKey.Should().Be(primarykey);
        }

        [Fact]
        public async Task IndexNameWrongFormattedError()
        {
            var indexUid = "Wrong UID";

            var ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => _client.CreateIndexAsync(indexUid));
            Assert.Equal("invalid_index_uid", ex.Code);
        }

        [Fact]
        public async Task GetAllRawIndexes()
        {
            var indexUid = "GetAllRawIndexesTest";
            await _fixture.SetUpEmptyIndex(indexUid, _defaultPrimaryKey);

            var indexes = await _client.GetAllRawIndexesAsync();
            var results = indexes.RootElement.GetProperty("results");
            var index = results[0];
            Assert.Equal(index.GetProperty("uid").GetString(), indexUid);
            Assert.Equal(index.GetProperty("primaryKey").GetString(), _defaultPrimaryKey);
        }

        [Fact]
        public async Task GetMultipleRawExistingIndexesWithLimit()
        {
            var indexUid1 = "GetMultipleRawExistingIndexesWithLimit1";
            var indexUid2 = "GetMultipleRawExistingIndexesWithLimit2";
            await _fixture.SetUpEmptyIndex(indexUid1, _defaultPrimaryKey);
            await _fixture.SetUpEmptyIndex(indexUid2, _defaultPrimaryKey);

            var indexes = await _client.GetAllRawIndexesAsync(new IndexesQuery() { Limit = 1 });
            var results = indexes.RootElement.GetProperty("results");
            var limit = indexes.RootElement.GetProperty("limit").GetInt32();
            results.GetArrayLength().Should().BeGreaterThanOrEqualTo(1);
            Assert.Equal(1, limit);
        }

        [Fact]
        public async Task GetMultipleRawExistingIndexesWithOffset()
        {
            var indexUid1 = "GetMultipleRawExistingIndexesWithOffset1";
            var indexUid2 = "GetMultipleRawExistingIndexesWithOffset2";
            await _fixture.SetUpEmptyIndex(indexUid1, _defaultPrimaryKey);
            await _fixture.SetUpEmptyIndex(indexUid2, _defaultPrimaryKey);

            var indexes = await _client.GetAllRawIndexesAsync(new IndexesQuery() { Offset = 1 });
            var results = indexes.RootElement.GetProperty("results");
            var offset = indexes.RootElement.GetProperty("offset").GetInt32();
            results.GetArrayLength().Should().BeGreaterThanOrEqualTo(1);
            Assert.Equal(1, offset);
        }

        [Fact]
        public async Task GetMultipleExistingIndexes()
        {
            var indexUid1 = "GetMultipleExistingIndexesTest1";
            var indexUid2 = "GetMultipleExistingIndexesTest2";
            await _fixture.SetUpEmptyIndex(indexUid1, _defaultPrimaryKey);
            await _fixture.SetUpEmptyIndex(indexUid2, _defaultPrimaryKey);

            var indexes = await _client.GetAllIndexesAsync();
            indexes.Results.Count().Should().BeGreaterOrEqualTo(2);
        }

        [Fact]
        public async Task GetMultipleExistingIndexesWithLimit()
        {
            var indexUid1 = "GetMultipleExistingIndexesWithLimit1";
            var indexUid2 = "GetMultipleExistingIndexesWithLimit2";
            await _fixture.SetUpEmptyIndex(indexUid1, _defaultPrimaryKey);
            await _fixture.SetUpEmptyIndex(indexUid2, _defaultPrimaryKey);

            var indexes = await _client.GetAllIndexesAsync(new IndexesQuery() { Limit = 1 });
            indexes.Results.Count().Should().BeGreaterOrEqualTo(1);
            indexes.Limit.Should().BeGreaterOrEqualTo(1);
            Assert.Equal(1, indexes.Limit);
        }

        [Fact]
        public async Task GetMultipleExistingIndexesWithOffset()
        {
            var indexUid1 = "GetMultipleExistingIndexesWithOffset1";
            var indexUid2 = "GetMultipleExistingIndexesWithOffset2";
            await _fixture.SetUpEmptyIndex(indexUid1, _defaultPrimaryKey);
            await _fixture.SetUpEmptyIndex(indexUid2, _defaultPrimaryKey);

            var indexes = await _client.GetAllIndexesAsync(new IndexesQuery() { Offset = 1 });
            indexes.Results.Count().Should().BeGreaterOrEqualTo(1);
            Assert.Equal(1, indexes.Offset);
        }

        [Fact]
        public async Task GetOneExistingIndex()
        {
            var indexUid = "GetOneExistingIndexTest";
            await _fixture.SetUpEmptyIndex(indexUid, _defaultPrimaryKey);

            var index = await _client.GetIndexAsync(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be(_defaultPrimaryKey);
            index.CreatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
            index.UpdatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public async Task GetAnNonExistingIndex()
        {
            var indexUid = "GetAnNonExistingIndexTest";

            var ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => _client.GetIndexAsync(indexUid));
            Assert.Equal("index_not_found", ex.Code);
        }

        [Fact]
        public async Task FetchPrimaryKey()
        {
            var indexUid = "FetchPrimaryKeyTest";
            var index = await _fixture.SetUpEmptyIndex(indexUid, _defaultPrimaryKey);

            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
            await index.FetchPrimaryKey();
            Assert.Equal(_defaultPrimaryKey, index.PrimaryKey);
        }

        [Fact]
        public async Task UpdateIndexMethod()
        {
            var indexUid = "UpdateIndexMethodTest";
            var primarykey = "MovieId" + new Random().Next();
            await _fixture.SetUpEmptyIndex(indexUid);

            var task = await _client.Index(indexUid).UpdateAsync(primarykey);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await _client.Index(indexUid).WaitForTaskAsync(task.TaskUid);

            var index = await _client.GetIndexAsync(indexUid);
            index.PrimaryKey.Should().Be(primarykey);
            index.CreatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
            index.UpdatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public async Task GetStats()
        {
            var indexUid = "GetStatsTests";
            await _fixture.SetUpEmptyIndex(indexUid);

            var stats = await _client.Index(indexUid).GetStatsAsync();
            stats.Should().NotBeNull();
        }

        [Fact]
        public async Task GetRawIndex()
        {
            var indexUid = "GetRawIndex";
            await _fixture.SetUpEmptyIndex(indexUid);

            var rawIndex = await _client.GetRawIndexAsync(indexUid);
            rawIndex.GetProperty("uid").GetString().Should().Be(indexUid);
        }
    }
}

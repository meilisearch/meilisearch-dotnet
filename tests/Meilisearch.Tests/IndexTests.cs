using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

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
            document.Uid.Should().BeGreaterOrEqualTo(0);
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
            task.Uid.Should().BeGreaterOrEqualTo(0);
            var finishedTask = await _client.Index(indexUid).WaitForTaskAsync(task.Uid);

            Assert.Equal(task.Uid, finishedTask.Uid);
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
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await _client.Index(indexUid).WaitForTaskAsync(task.Uid);

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
            indexes.Count().Should().BeGreaterOrEqualTo(1);
            var index = indexes.First();
            Assert.Equal(index.GetProperty("uid").GetString(), indexUid);
            Assert.Equal(index.GetProperty("name").GetString(), indexUid);
            Assert.Equal(index.GetProperty("primaryKey").GetString(), _defaultPrimaryKey);
        }

        [Fact]
        public async Task GetAllExistingIndexes()
        {
            var indexUid = "GetAllExistingIndexesTest";
            var index = await _fixture.SetUpEmptyIndex(indexUid, _defaultPrimaryKey);

            var indexes = await _client.GetAllIndexesAsync();
            indexes.Count().Should().BeGreaterOrEqualTo(1);
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
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await _client.Index(indexUid).WaitForTaskAsync(task.Uid);

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

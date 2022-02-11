using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Xunit;

namespace Meilisearch.Tests
{
    [Collection("Sequential")]
    public class IndexTests : IAsyncLifetime
    {
        private readonly MeilisearchClient client;
        private readonly string defaultPrimaryKey;
        private readonly IndexFixture fixture;

        public IndexTests(IndexFixture fixture)
        {
            this.fixture = fixture;
            this.client = fixture.DefaultClient;
            this.defaultPrimaryKey = "movieId";
        }

        public async Task InitializeAsync() => await this.fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task BasicIndexCreation()
        {
            var indexUid = "BasicIndexCreationTest";
            await this.fixture.SetUpEmptyIndex(indexUid);

            var index = await this.client.GetIndexAsync(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
        }

        [Fact]
        public async Task IndexCreationWithPrimaryKey()
        {
            var indexUid = "IndexCreationWithPrimaryKeyTest";
            await this.fixture.SetUpEmptyIndex(indexUid, this.defaultPrimaryKey);

            var index = await this.client.GetIndexAsync(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be(this.defaultPrimaryKey);
        }

        [Fact]
        public async Task BasicUsageOfIndexMethod()
        {
            var indexUid = "BasicUsageOfIndexMethodTest";
            var index = this.client.Index(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.client.GetIndexAsync(indexUid));
            Assert.Equal("index_not_found", ex.Code);
        }

        [Fact]
        public async Task IndexMethodUsageOnExistingIndex()
        {
            var indexUid = "IndexMethodUsageOnExistingIndexTest";
            var index = await this.fixture.SetUpEmptyIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();

            var document = await index.AddDocumentsAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
            document.Uid.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task IndexFetchInfoPrimaryKey()
        {
            var indexUid = "IndexFetchInfoPrimaryKeyTest";
            var index = await this.fixture.SetUpEmptyIndex(indexUid, this.defaultPrimaryKey);

            await index.FetchInfoAsync();
            Assert.Equal(index.Uid, indexUid);
            index.PrimaryKey.Should().Be(this.defaultPrimaryKey);

            var primaryKey = await index.FetchPrimaryKey();
            Assert.Equal(index.PrimaryKey, primaryKey);
        }

        [Fact]
        public async Task IndexAlreadyExistsError()
        {
            var indexUid = "IndexAlreadyExistsErrorTest";

            await this.client.CreateIndexAsync(indexUid, this.defaultPrimaryKey);
            var task = await this.client.CreateIndexAsync(indexUid, this.defaultPrimaryKey);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            var finishedTask = await this.client.Index(indexUid).WaitForTaskAsync(task.Uid);

            Assert.Equal(task.Uid, finishedTask.Uid);
            Assert.Equal(indexUid, finishedTask.IndexUid);
            Assert.Equal("failed", finishedTask.Status);
            var error = finishedTask.Error;
            error.Should().NotBeNull();
            Assert.Equal("index_already_exists", error["code"]);
        }

        [Fact]
        public async Task UpdateIndex()
        {
            var indexUid = "UpdateIndexTest";
            var primarykey = "MovieId" + new Random().Next();

            await this.fixture.SetUpEmptyIndex(indexUid);

            var task = await this.client.UpdateIndexAsync(indexUid, primarykey);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.client.Index(indexUid).WaitForTaskAsync(task.Uid);

            var index = await this.client.GetIndexAsync(indexUid);
            index.PrimaryKey.Should().Be(primarykey);
        }

        [Fact]
        public async Task IndexNameWrongFormattedError()
        {
            var indexUid = "Wrong UID";

            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.client.CreateIndexAsync(indexUid));
            Assert.Equal("invalid_index_uid", ex.Code);
        }

        [Fact]
        public async Task GetAllRawIndexes()
        {
            var indexUid = "GetAllRawIndexesTest";
            await this.fixture.SetUpEmptyIndex(indexUid, this.defaultPrimaryKey);

            var indexes = await this.client.GetAllRawIndexesAsync();
            indexes.Count().Should().BeGreaterOrEqualTo(1);
            var index = indexes.First();
            Assert.Equal(index.GetProperty("uid").GetString(), indexUid);
            Assert.Equal(index.GetProperty("name").GetString(), indexUid);
            Assert.Equal(index.GetProperty("primaryKey").GetString(), this.defaultPrimaryKey);
        }

        [Fact]
        public async Task GetAllExistingIndexes()
        {
            var indexUid = "GetAllExistingIndexesTest";
            var index = await this.fixture.SetUpEmptyIndex(indexUid, this.defaultPrimaryKey);

            var indexes = await this.client.GetAllIndexesAsync();
            indexes.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task GetOneExistingIndex()
        {
            var indexUid = "GetOneExistingIndexTest";
            await this.fixture.SetUpEmptyIndex(indexUid, this.defaultPrimaryKey);

            var index = await this.client.GetIndexAsync(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be(this.defaultPrimaryKey);
            index.CreatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
            index.UpdatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public async Task GetAnNonExistingIndex()
        {
            var indexUid = "GetAnNonExistingIndexTest";

            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.client.GetIndexAsync(indexUid));
            Assert.Equal("index_not_found", ex.Code);
        }

        [Fact]
        public async Task FetchPrimaryKey()
        {
            var indexUid = "FetchPrimaryKeyTest";
            var index = await this.fixture.SetUpEmptyIndex(indexUid, this.defaultPrimaryKey);

            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
            await index.FetchPrimaryKey();
            Assert.Equal(this.defaultPrimaryKey, index.PrimaryKey);
        }

        [Fact]
        public async Task UpdateIndexMethod()
        {
            var indexUid = "UpdateIndexMethodTest";
            var primarykey = "MovieId" + new Random().Next();
            await this.fixture.SetUpEmptyIndex(indexUid);

            var task = await this.client.Index(indexUid).UpdateAsync(primarykey);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.client.Index(indexUid).WaitForTaskAsync(task.Uid);

            var index = await this.client.GetIndexAsync(indexUid);
            index.PrimaryKey.Should().Be(primarykey);
            index.CreatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
            index.UpdatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public async Task GetStats()
        {
            var indexUid = "GetStatsTests";
            await this.fixture.SetUpEmptyIndex(indexUid);

            var stats = await this.client.Index(indexUid).GetStatsAsync();
            stats.Should().NotBeNull();
        }

        [Fact]
        public async Task GetRawIndex()
        {
            var indexUid = "GetRawIndex";
            await this.fixture.SetUpEmptyIndex(indexUid);

            var rawIndex = await this.client.GetRawIndexAsync(indexUid);
            rawIndex.GetProperty("uid").GetString().Should().Be(indexUid);
        }
    }
}

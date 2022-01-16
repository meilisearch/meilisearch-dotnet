namespace Meilisearch.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using HttpClientFactoryLite;
    using Xunit;

    [Collection("Sequential")]
    public class IndexTests : IAsyncLifetime
    {
        private readonly MeilisearchClient defaultClient;
        private readonly string defaultPrimaryKey;
        private readonly IndexFixture fixture;

        public IndexTests(IndexFixture fixture)
        {
            this.fixture = fixture;
            this.defaultClient = fixture.DefaultClient;
            this.defaultPrimaryKey = "movieId";
        }

        public async Task InitializeAsync() => await this.fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task BasicIndexCreation()
        {
            var indexUid = "BasicIndexCreationTest";

            var task = await this.defaultClient.CreateIndexAsync(indexUid);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.defaultClient.WaitForTaskAsync(task.Uid);
            // await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            var index = await this.defaultClient.GetIndexAsync(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
        }

        [Fact]
        public async Task IndexCreationWithPrimaryKey()
        {
            var indexUid = "IndexCreationWithPrimaryKeyTest";

            var task = await this.defaultClient.CreateIndexAsync(indexUid, this.defaultPrimaryKey);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            var index = await this.defaultClient.GetIndexAsync(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be(this.defaultPrimaryKey);
        }

        [Fact]
        public async Task BasicUsageOfIndexMethod()
        {
            var indexUid = "BasicUsageOfIndexMethodTest";
            var index = this.defaultClient.Index(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.defaultClient.GetIndexAsync(indexUid));
            Assert.Equal("index_not_found", ex.Code);
        }

        [Fact]
        public async Task IndexMethodUsageOnExistingIndex()
        {
            Meilisearch.Index index;
            var indexUid = "IndexMethodUsageOnExistingIndexTest";

            var task = await this.defaultClient.CreateIndexAsync(indexUid);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            index = this.defaultClient.Index(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();

            var document = await index.AddDocumentsAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
            document.Uid.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task IndexFetchInfoPrimaryKey()
        {
            var indexUid = "IndexFetchInfoPrimaryKeyTest";

            var task = await this.defaultClient.CreateIndexAsync(indexUid, this.defaultPrimaryKey);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            var index = this.defaultClient.Index(indexUid);
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

            await this.defaultClient.CreateIndexAsync(indexUid, this.defaultPrimaryKey);
            var task = await this.defaultClient.CreateIndexAsync(indexUid, this.defaultPrimaryKey);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            var finishedTask = await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            Assert.Equal(task.Uid, finishedTask.Uid);
            Assert.Equal(indexUid, finishedTask.IndexUid);
            Assert.Equal("failed", finishedTask.Status);
            var error = finishedTask.Error;
            error.Should().NotBeNull();
            Assert.Equal(error["code"], "index_already_exists");
        }

        [Fact]
        public async Task UpdateIndex()
        {
            var indexUid = "UpdateIndexTest";
            var primarykey = "MovieId" + new Random().Next();

            var task = await this.defaultClient.CreateIndexAsync(indexUid);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            task = await this.defaultClient.UpdateIndexAsync(indexUid, primarykey);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            var index = await this.defaultClient.GetIndexAsync(indexUid);
            index.PrimaryKey.Should().Be(primarykey);

        }

        [Fact]
        public async Task IndexNameWrongFormattedError()
        {
            var indexUid = "Wrong UID";

            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.defaultClient.CreateIndexAsync(indexUid));
            Assert.Equal("invalid_index_uid", ex.Code);
        }

        [Fact]
        public async Task GetAllRawIndexes()
        {
            var indexUid = "GetAllRawIndexesTest";
            var task = await this.defaultClient.CreateIndexAsync(indexUid, this.defaultPrimaryKey);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            var indexes = await this.defaultClient.GetAllRawIndexesAsync();
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

            var task = await this.defaultClient.CreateIndexAsync(indexUid, this.defaultPrimaryKey);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            var indexes = await this.defaultClient.GetAllIndexesAsync();
            indexes.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task GetOneExistingIndex()
        {
            var indexUid = "GetOneExistingIndexTest";

            var task = await this.defaultClient.CreateIndexAsync(indexUid, this.defaultPrimaryKey);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            var index = await this.defaultClient.GetIndexAsync(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be(this.defaultPrimaryKey);
            index.CreatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
            index.UpdatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public async Task GetAnNonExistingIndex()
        {
            var indexUid = "GetAnNonExistingIndexTest";

            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.defaultClient.GetIndexAsync(indexUid));
            Assert.Equal("index_not_found", ex.Code);
        }

        [Fact]
        public async Task FetchPrimaryKey()
        {
            var indexUid = "FetchPrimaryKeyTest";

            var task = await this.defaultClient.CreateIndexAsync(indexUid, this.defaultPrimaryKey);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            var index = this.defaultClient.Index(indexUid);
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

            var task = await this.defaultClient.CreateIndexAsync(indexUid);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            task = await this.defaultClient.Index(indexUid).UpdateAsync(primarykey);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            var index = await this.defaultClient.GetIndexAsync(indexUid);
            index.PrimaryKey.Should().Be(primarykey);
            index.CreatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
            index.UpdatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public async Task GetStats()
        {
            var indexUid = "GetStatsTests";

            var task = await this.defaultClient.CreateIndexAsync(indexUid);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            var stats = await this.defaultClient.Index(indexUid).GetStatsAsync();
            stats.Should().NotBeNull();
        }

        [Fact]
        public async Task GetRawIndex()
        {
            var indexUid = "GetRawIndex";

            var task = await this.defaultClient.CreateIndexAsync(indexUid);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await this.defaultClient.Index(indexUid).WaitForTaskAsync(task.Uid);

            var rawIndex = await this.defaultClient.GetRawIndexAsync(indexUid);
            rawIndex.GetProperty("uid").GetString().Should().Be(indexUid);
        }
    }
}

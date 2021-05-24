namespace Meilisearch.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    [Collection("Sequential")]
    public class IndexTests
    {
        private MeilisearchClient defaultClient;
        private string defaultPrimaryKey;

        public IndexTests(IndexFixture fixture)
        {
            fixture.DeleteAllIndexes().Wait(); // Test context cleaned for each [Fact]
            this.defaultClient = fixture.DefaultClient;
            this.defaultPrimaryKey = "movieId";
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
        public async Task BasicUsageOfIndexMethod()
        {
            var indexUid = "BasicUsageOfIndexMethodTest";
            var index = this.defaultClient.Index(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.defaultClient.GetIndex(indexUid));
            Assert.Equal("index_not_found", ex.ErrorCode);
        }

        [Fact]
        public async Task IndexMethodUsageOnExistingIndex()
        {
            Meilisearch.Index index;
            var indexUid = "IndexMethodUsageOnExistingIndexTest";
            index = await this.defaultClient.CreateIndex(indexUid);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().BeNull();
            index = this.defaultClient.Index(indexUid);
            index.Uid.Should().Be(indexUid);

            var document = await index.AddDocuments(new[] { new Movie { Id = "1", Name = "Batman" } });

            document.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task IndexFetchExistingIndexPrimaryKey()
        {
            var indexUid = "IndexFetchExistingIndexPrimaryKeyTest";
            var createIndex = await this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey);
            var indexObject = this.defaultClient.Index(indexUid);
            Assert.Equal(createIndex.Uid, indexObject.Uid);
            createIndex.PrimaryKey.Should().Be(this.defaultPrimaryKey);
            indexObject.PrimaryKey.Should().BeNull();
            var primaryKey = await indexObject.FetchPrimaryKey();
            Assert.Equal(createIndex.PrimaryKey, primaryKey);
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
        public async Task IndexNameWrongFormattedError()
        {
            var indexUid = "Wrong UID";
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.defaultClient.CreateIndex(indexUid));
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
            var indexUid = "GetAnNonExistingIndexTest";
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.defaultClient.GetIndex(indexUid));
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
        public async Task FetchPrimaryKey()
        {
            var indexUid = "FetchPrimaryKeyTest";
            var index = await this.defaultClient.CreateIndex(indexUid, this.defaultPrimaryKey);
            index.Uid.Should().Be(indexUid);
            index.PrimaryKey.Should().Be(this.defaultPrimaryKey);
            await index.FetchPrimaryKey();
            Assert.Equal(this.defaultPrimaryKey, index.PrimaryKey);
        }

        [Fact]
        public async Task UpdatePrimaryKey()
        {
            var index = await this.defaultClient.GetOrCreateIndex("UpdatePrimaryKeyTest");
            var primarykey = "MovieId" + new Random().Next();
            var modifiedIndex = await index.ChangePrimaryKey(primarykey);
            modifiedIndex.PrimaryKey.Should().Be(primarykey);
        }

        [Fact]
        public async Task GetStats()
        {
            var index = await this.defaultClient.GetOrCreateIndex("GetStatsTests");
            var stats = await index.GetStats();
            stats.Should().NotBeNull();
        }
    }
}

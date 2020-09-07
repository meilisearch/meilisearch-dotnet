using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using HttpClientFactoryLite;

namespace Meilisearch.Tests
{
    [Collection("Sequential")]
    public class MeilisearchClientTests
    {

        [Fact]
        public async Task Should_be_Able_To_Get_Version_By_Custom_Client()
        {
            var _httpClient = ClientFactory.Instance.CreateClient<MeilisearchClient>();
            var client = new MeilisearchClient(_httpClient);
            var meilisearchversion = await client.GetVersion();
            meilisearchversion.Version.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Should_be_Able_To_Get_Version_By_Default_Client()
        {
            var client = new MeilisearchClient("http://localhost:7700", "masterKey");
            var meilisearchversion = await client.GetVersion();
            meilisearchversion.Version.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Basic_Usage_Of_Custom_Client()
        {
            var _httpClient = ClientFactory.Instance.CreateClient<MeilisearchClient>();
            MeilisearchClient ms = new MeilisearchClient(_httpClient);
            var indexName = "uid" + new Random().Next();
            Index index = await ms.CreateIndex(indexName);
            var updateStatus = await index.AddDocuments(new[] { new Movie { Id = "1", Name = "Batman" } });
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task Should_be_able_To_Create_Index()
        {
            var client = new MeilisearchClient("http://localhost:7700", "masterKey");
            var indexName = "uid" + new Random().Next();
            var index = await client.CreateIndex(indexName);
            index.Uid.Should().Be(indexName);
        }

        [Fact]
        public async Task Should_be_able_To_Create_Index_with_primaryKey()
        {
            var client = new MeilisearchClient("http://localhost:7700", "masterKey");
            var indexName = "uid2" + new Random().Next();
            var index = await client.CreateIndex(indexName, "movieId");
            index.Uid.Should().Be(indexName);
            index.PrimaryKey.Should().Be("movieId");
        }

        [Fact]
        public async Task Should_Throw_an_Exception_if_the_Index_Is_already_Taken()
        {
            var client = new MeilisearchClient("http://localhost:7700", "masterKey");
            var indexName = "uid3" + new Random().Next();
            var index = await client.CreateIndex(indexName, "movieId");
            await Assert.ThrowsAsync<Exception>(() => client.CreateIndex(indexName, "movieId"));
        }

        [Fact]
        public async Task Should_Fail_to_Create_If_the_Index_is_of_bad_Format()
        {
            var client = new MeilisearchClient("http://localhost:7700", "masterKey");
            await Assert.ThrowsAsync<Exception>(() => client.CreateIndex("wrong UID"));
        }

        [Fact]
        public async Task Should_return_All_The_Index_In_the_System()
        {
            var client = new MeilisearchClient("http://localhost:7700", "masterKey");
            var indexName = "uid4" + new Random().Next();
            var index = await client.CreateIndex(indexName, "movieId");
            var indexes = await client.GetAllIndexes();
            indexes.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task Should_return_the_index_requested()
        {
            var client = new MeilisearchClient("http://localhost:7700", "masterKey");
            var indexName = "uid5" + new Random().Next();
            var index = await client.CreateIndex(indexName, "movieId");
            var indexes = await client.GetIndex(indexName);
            index.Uid.Should().Be(indexName);
        }

        [Fact]
        public async Task Should_return_Null_If_the_Index_Does_not_Exist()
        {
            var client = new MeilisearchClient("http://localhost:7700", "masterKey");
            var indexes = await client.GetIndex("somerandomIndex");
            indexes.Should().BeNull();
        }
    }
}

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Meilisearch.Tests
{
    public class MeilisearchClientTests
    {
        private static HttpClient _httpClient = new HttpClient
        {
            // TODO : Should default URL in the next change.
            BaseAddress = new Uri("http://localhost:7700/")
        };

        [Fact]
        public async Task Should_be_Able_To_Get_Version()
        {
            var client = new MeilisearchClient(_httpClient);
            var meilisearchversion = await client.GetVersion();
            meilisearchversion.Version.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Should_be_able_To_Create_Index()
        {
            var client = new MeilisearchClient(_httpClient);
            var index = await client.CreateIndex("uid1");
            index.Uid.Should().Be("uid1");
        }

        [Fact]
        public async Task Should_be_able_To_Create_Index_with_primaryKey()
        {
            var client = new MeilisearchClient(_httpClient);
            var index = await client.CreateIndex("uid2", "movieId");
            index.Uid.Should().Be("uid2");
            index.PrimaryKey.Should().Be("movieId");
        }
        [Fact]
        public async Task Should_Throw_an_Exception_if_the_Index_Is_already_Taken()
        {
            var client = new MeilisearchClient(_httpClient);
            var index = await client.CreateIndex("uid3", "movieId");
            await Assert.ThrowsAsync<Exception>(() => client.CreateIndex("uid3", "movieId"));
        }

        [Fact]
        public async Task Should_Fail_to_Create_If_the_Index_is_of_bad_Format()
        {
            var client = new MeilisearchClient(_httpClient);
            await Assert.ThrowsAsync<Exception>(() => client.CreateIndex("wrong UID"));
        }

        [Fact]
        public async Task Should_return_All_The_Index_In_the_System()
        {
            var client = new MeilisearchClient(_httpClient);
            var index = await client.CreateIndex("uid4", "movieId");
            var indexes = await client.GetAllIndexes();
            indexes.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task Should_return_the_index_requested()
        {
            var client = new MeilisearchClient(_httpClient);
            var index = await client.CreateIndex("uid5", "movieId");
            var indexes = await client.GetIndex("uid5");
            index.Uid.Should().Be("uid5");
        }

        [Fact]
        public async Task Should_return_Null_If_the_Index_Does_not_Exist()
        {
            var client = new MeilisearchClient(_httpClient);
            var indexes = await client.GetIndex("somerandomIndex");
            indexes.Should().BeNull();
        }
    }
}
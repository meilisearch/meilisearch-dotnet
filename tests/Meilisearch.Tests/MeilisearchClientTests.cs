using System;
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
    }
}
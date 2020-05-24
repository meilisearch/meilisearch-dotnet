using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Meilisearch.Tests
{
    public class IndexTests
    {
        private static HttpClient _httpClient = new HttpClient
        {
            // TODO : Should default URL in the next change.
            BaseAddress = new Uri("http://localhost:7700/")
        };
        
        [Fact]
        public async Task Should_be_Able_To_Modify_Primary_Key()
        {
            var client = new MeilisearchClient(_httpClient);
            var index = await client.CreateIndex("Indextest");
            var modifiedIndex = await index.ChangePrimaryKey("MovieId");
            modifiedIndex.PrimaryKey.Should().Be("MovieId");
        }

        [Fact]
        public async Task Should_be_Able_To_Delete_The_Documents()
        {
            var client = new MeilisearchClient(_httpClient);
            var index = await client.CreateIndex("DeleteTests");
            var isDeleteSucessfull= await index.Delete();
            isDeleteSucessfull.Should().BeTrue();
        }
        
        
    }
}
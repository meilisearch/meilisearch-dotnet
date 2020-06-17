using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Http;
using Xunit;

namespace Meilisearch.Tests
{
    [Collection("Sequential")]
    public class IndexTests
    {
        [Fact]
        public async Task Should_be_Able_To_Modify_Primary_Key()
        {
            var httpclient = ClientFactory.Instance.CreateClient<MeilisearchClient>();
            var client = new MeilisearchClient(httpclient);
            var index = await client.CreateIndex("Indextest"+new Random().Next());
            var primarykey = "MovieId"+new Random().Next();
            var modifiedIndex = await index.ChangePrimaryKey(primarykey);
            modifiedIndex.PrimaryKey.Should().Be(primarykey);
        }

        // [Fact]
        // public async Task Should_be_Able_To_Delete_The_Documents()
        // {
        //     var client = new MeilisearchClient(_httpClient);
        //     var index = await client.CreateIndex("DeleteTests");
        //     var isDeleteSucessfull= await index.Delete();
        //     isDeleteSucessfull.Should().BeTrue();
        // }
        
        
    }
}
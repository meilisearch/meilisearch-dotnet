using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Meilisearch.Tests
{
    [Collection("Sequential")]
    public class IndexTests
    {
        [Fact]
        public async Task Should_be_Able_To_Modify_Primary_Key()
        {
            var client = new MeilisearchClient("http://localhost:7700", "masterKey");
            var index = await client.CreateIndex("Indextest" + new Random().Next());
            var primarykey = "MovieId" + new Random().Next();
            var modifiedIndex = await index.ChangePrimaryKey(primarykey);
            modifiedIndex.PrimaryKey.Should().Be(primarykey);
        }
    }
}

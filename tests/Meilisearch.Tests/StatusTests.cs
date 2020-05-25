using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Meilisearch.Tests
{
    
    public class StatusTests 
    {
        public StatusTests()
        {
            
        }
        
        [Fact]
        public async Task Should_be_Able_to_get_All_The_Update_Status()
        {
            var client = new MeilisearchClient(DocumentFixture._httpClient);
            var index = await  client.CreateIndex("MoviesStatus");
            await index.AddorUpdateDocuments(new[] {new Movie {Id = "1"}});
            var status = await index.GetAllUpdateStatus();
            status.Count().Should().BeGreaterOrEqualTo(1);
        }
 
        [Fact]
        public async Task Should_be_Able_to_Get_Status_By_Id()
        {
            var client = new MeilisearchClient(DocumentFixture._httpClient);
            var index = await  client.CreateIndex("MoviesStatus");
            var status = await index.AddorUpdateDocuments(new[] {new Movie {Id = "2"}});
            UpdateStatus individualStatus = await index.GetUpdateStatus(status.UpdateId);
            individualStatus.Should().NotBeNull();
        } 
    } 
}
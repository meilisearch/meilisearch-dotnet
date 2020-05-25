using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Meilisearch.Tests
{
    /*
    public class StatusTests : IClassFixture<DocumentFixture>
    {
        private readonly Index index;

        public StatusTests(DocumentFixture fixture)
        {
            index = fixture.documentIndex;
        }
        
        [Fact]
        public async Task Should_be_Able_to_get_All_The_Update_Status()
        {
            var status = await index.GetAllUpdateStatus();
            status.Count().Should().BeGreaterOrEqualTo(1);
        }
 
        [Fact]
        public async Task Should_be_Able_to_Get_Status_By_Id()
        {
            var status = await index.GetAllUpdateStatus();
            UpdateStatus individualStatus = await index.GetUpdateStatus(status.First().UpdateId);
            individualStatus.Should().NotBeNull();
        } 
    } */
}
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Meilisearch.Tests
{
    [Collection("Sequential")]
    public class SearchTests : IClassFixture<DocumentFixture>
    {
        private readonly Index index;
        
        public SearchTests(DocumentFixture fixture)
        {
            this.index = fixture.documentIndex;
        }
        
        [Fact]
        public async Task Should_Be_to_Search_A_Index_Document()
        {
           var movies = await this.index.Search<Movie>("ironman");
           movies.Hits.Should().NotBeEmpty();
        }
    }
}
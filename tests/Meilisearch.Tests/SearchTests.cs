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

        [Fact]
        public async Task Should_Be_able_to_Send_Different_Parameters()
        {
            var movies = await this.index.Search<Movie>("ironman", new SearchQuery {Limit = 100});
            movies.Hits.Should().NotBeEmpty();
        }
    }
}
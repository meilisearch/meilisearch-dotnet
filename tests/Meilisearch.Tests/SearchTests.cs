using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Linq;

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
        public async Task BasicSearch()
        {
            var movies = await this.index.Search<Movie>("man");
            movies.Hits.Should().NotBeEmpty();
            Assert.Equal("Iron Man", movies.Hits.First().Name);
            Assert.Equal("Spider-Man", movies.Hits.ElementAt(1).Name);
        }

        [Fact]
        public async Task BasicSearchWithNoQuery()
        {
            var movies = await this.index.Search<Movie>(null);
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Id.Should().NotBeNull();
            movies.Hits.First().Name.Should().NotBeNull();
        }

        [Fact]
        public async Task BasicSearchWithEmptyQuery()
        {
            var movies = await this.index.Search<Movie>("");
            movies.Hits.Should().BeEmpty();
        }

        [Fact]
        public async Task CustomSearchWithLimit()
        {
            var movies = await this.index.Search<Movie>(
                "man",
                new SearchQuery { Limit = 1 }
            );
            movies.Hits.Should().NotBeEmpty();
            Assert.Single(movies.Hits);
            Assert.Equal("14", movies.Hits.First().Id);
            Assert.Equal("Iron Man", movies.Hits.First().Name);
            Assert.Equal("Action", movies.Hits.First().Genre);
        }

        [Fact]
        public async Task CustomSearchWithAttributesToHighlight()
        {
            var movies = await this.index.Search<FormattedMovie>(
                "man",
                new SearchQuery { AttributesToHighlight = new string[] { "name" } }
            );
            movies.Hits.Should().NotBeEmpty();
            Assert.Equal("14", movies.Hits.First().Id);
            Assert.Equal("Iron Man", movies.Hits.First().Name);
            Assert.Equal("Action", movies.Hits.First().Genre);
            Assert.Equal("Iron <em>Man</em>", movies.Hits.First()._Formatted.Name);
            Assert.Equal("Spider-<em>Man</em>", movies.Hits.ElementAt(1)._Formatted.Name);
        }

        [Fact]
        public async Task CustomSearchWithNoQuery()
        {
            var movies = await this.index.Search<FormattedMovie>(
                null,
                new SearchQuery { AttributesToHighlight = new string[] { "name" } }
            );
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Id.Should().NotBeNull();
            movies.Hits.First().Name.Should().NotBeNull();
            movies.Hits.First()._Formatted.Id.Should().NotBeNull();
            movies.Hits.First()._Formatted.Name.Should().NotBeNull();
        }

        [Fact]
        public async Task CustomSearchWithEmptyQuery()
        {
            var movies = await this.index.Search<FormattedMovie>(
                "",
                new SearchQuery { AttributesToHighlight = new string[] { "name" } }
            );
            movies.Hits.Should().BeEmpty();
        }

        [Fact]
        public async Task CustomSearchWithMultipleOptions()
        {
            var movies = await this.index.Search<FormattedMovie>(
                "man",
                new SearchQuery {
                    AttributesToHighlight = new string[] { "name" },
                    AttributesToRetrieve = new string[] { "name", "id" },
                    Offset = 1
                }
            );
            movies.Hits.Should().NotBeEmpty();
            Assert.Single(movies.Hits);
            Assert.Equal("Spider-Man", movies.Hits.First().Name);
            Assert.Equal("15", movies.Hits.First().Id);
            movies.Hits.First().Genre.Should().BeNull();
            Assert.Equal("Spider-<em>Man</em>", movies.Hits.First()._Formatted.Name);
            movies.Hits.First()._Formatted.Id.Should().BeNull();
            movies.Hits.First()._Formatted.Genre.Should().BeNull();
        }
    }
}

using System;
namespace Meilisearch.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    [Collection("Sequential")]
    public class SearchTests : IClassFixture<DocumentFixture>
    {
        private readonly Index index;
        private readonly Index indexForFaceting;

        public SearchTests(DocumentFixture fixture)
        {
            this.index = fixture.BasicIndexWithDocuments;
            this.indexForFaceting = fixture.IndexForFaceting;
        }

        [Fact]
        public async Task BasicSearch()
        {
            var movies = await this.index.Search<Movie>("man");
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Name.Should().NotBeEmpty();
            movies.Hits.ElementAt(1).Name.Should().NotBeEmpty();
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
            var movies = await this.index.Search<Movie>(string.Empty);
            movies.Hits.Should().BeEmpty();
        }

        [Fact]
        public async Task CustomSearchWithLimit()
        {
            var movies = await this.index.Search<Movie>(
                "man",
                new SearchQuery { Limit = 1 });
            movies.Hits.Should().NotBeEmpty();
            Assert.Single(movies.Hits);
            movies.Hits.First().Id.Should().NotBeEmpty();
            movies.Hits.First().Name.Should().NotBeEmpty();
            movies.Hits.First().Genre.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CustomSearchWithAttributesToHighlight()
        {
            var movies = await this.index.Search<FormattedMovie>(
                "man",
                new SearchQuery { AttributesToHighlight = new string[] { "name" } });
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Id.Should().NotBeEmpty();
            movies.Hits.First().Name.Should().NotBeEmpty();
            movies.Hits.First().Genre.Should().NotBeEmpty();
            movies.Hits.First()._Formatted.Name.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CustomSearchWithNoQuery()
        {
            var movies = await this.index.Search<FormattedMovie>(
                null,
                new SearchQuery { AttributesToHighlight = new string[] { "name" } });
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
                string.Empty,
                new SearchQuery { AttributesToHighlight = new string[] { "name" } });
            movies.Hits.Should().BeEmpty();
        }

        [Fact]
        public async Task CustomSearchWithMultipleOptions()
        {
            var movies = await this.index.Search<FormattedMovie>(
                "man",
                new SearchQuery
                {
                    AttributesToHighlight = new string[] { "name" },
                    AttributesToRetrieve = new string[] { "name", "id" },
                    Offset = 1,
                });
            movies.Hits.Should().NotBeEmpty();
            Assert.Single(movies.Hits);
            movies.Hits.First().Name.Should().NotBeEmpty();
            movies.Hits.First().Id.Should().NotBeEmpty();
            movies.Hits.First().Genre.Should().BeNull();
            movies.Hits.First()._Formatted.Name.Should().NotBeEmpty();
            movies.Hits.First()._Formatted.Id.Should().BeNull();
            movies.Hits.First()._Formatted.Genre.Should().BeNull();
        }

        [Fact]
        public async Task CustomSearchWithFacetFilters()
        {
            var movies = await this.indexForFaceting.Search<Movie>(
                null,
                new SearchQuery
                {
                    FacetFilters = new [] { new string[] { "genre:SF" }},
                });
            movies.Hits.Should().NotBeEmpty();
            Assert.Equal(2, movies.Hits.Count());
            Assert.Equal("12", movies.Hits.First().Id);
            Assert.Equal("Star Wars", movies.Hits.First().Name);
            Assert.Equal("SF", movies.Hits.First().Genre);
            Assert.Equal("SF", movies.Hits.ElementAt(1).Genre);
        }
    }
}

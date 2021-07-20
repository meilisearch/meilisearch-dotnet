namespace Meilisearch.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    [Collection("Sequential")]
    public class SearchTests
    {
        private readonly Index basicIndex;
        private readonly Index indexForFaceting;
        private readonly Index indexWithIntId;

        public SearchTests(IndexFixture fixture)
        {
            fixture.DeleteAllIndexes().Wait(); // Context test cleaned for each [Fact]
            var client = fixture.DefaultClient;
            this.basicIndex = fixture.SetUpBasicIndex("BasicIndex-SearchTests").Result;
            this.indexForFaceting = fixture.SetUpIndexForFaceting("IndexForFaceting-SearchTests").Result;
            this.indexWithIntId = fixture.SetUpBasicIndexWithIntId("IndexWithIntId-SearchTests").Result;
        }

        [Fact]
        public async Task BasicSearch()
        {
            var movies = await this.basicIndex.Search<Movie>("man");
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Name.Should().NotBeEmpty();
            movies.Hits.ElementAt(1).Name.Should().NotBeEmpty();
        }

        [Fact]
        public async Task BasicSearchWithNoQuery()
        {
            var movies = await this.basicIndex.Search<Movie>(null);
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Id.Should().NotBeNull();
            movies.Hits.First().Name.Should().NotBeNull();
        }

        [Fact]
        public async Task BasicSearchWithEmptyQuery()
        {
            var movies = await this.basicIndex.Search<Movie>(string.Empty);
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Id.Should().NotBeNull();
            movies.Hits.First().Name.Should().NotBeNull();
        }

        [Fact]
        public async Task CustomSearchWithLimit()
        {
            var movies = await this.basicIndex.Search<Movie>(
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
            Settings newFilters = new Settings
            {
                FilterableAttributes = new string[] { "name" },
            };
            UpdateStatus update = await this.basicIndex.UpdateSettings(newFilters);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await this.basicIndex.WaitForPendingUpdate(update.UpdateId);

            var movies = await this.basicIndex.Search<FormattedMovie>(
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
            var movies = await this.basicIndex.Search<FormattedMovie>(
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
            var movies = await this.basicIndex.Search<FormattedMovie>(
                string.Empty,
                new SearchQuery { AttributesToHighlight = new string[] { "name" } });
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Id.Should().NotBeNull();
            movies.Hits.First().Name.Should().NotBeNull();
            movies.Hits.First()._Formatted.Id.Should().NotBeNull();
            movies.Hits.First()._Formatted.Name.Should().NotBeNull();
        }

        [Fact]
        public async Task CustomSearchWithMultipleOptions()
        {
            var movies = await this.basicIndex.Search<FormattedMovie>(
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
            movies.Hits.First()._Formatted.Id.Should().Equals(15);
            movies.Hits.First()._Formatted.Genre.Should().BeNull();
        }

        [Fact]
        public async Task CustomSearchWithFilter()
        {
            var movies = await this.indexForFaceting.Search<Movie>(
                null,
                new SearchQuery
                {
                    Filter = "genre = SF",
                });
            movies.Hits.Should().NotBeEmpty();
            movies.FacetsDistribution.Should().BeNull();
            Assert.Equal(2, movies.Hits.Count());
            Assert.Equal("12", movies.Hits.First().Id);
            Assert.Equal("Star Wars", movies.Hits.First().Name);
            Assert.Equal("SF", movies.Hits.First().Genre);
            Assert.Equal("SF", movies.Hits.ElementAt(1).Genre);
        }

        [Fact]
        public async Task CustomSearchWithFilterArray()
        {
            var movies = await this.indexForFaceting.Search<Movie>(
                null,
                new SearchQuery
                {
                    Filter = new string[] { "genre = SF" },
                });
            movies.Hits.Should().NotBeEmpty();
            movies.FacetsDistribution.Should().BeNull();
            Assert.Equal(2, movies.Hits.Count());
            Assert.Equal("12", movies.Hits.First().Id);
            Assert.Equal("Star Wars", movies.Hits.First().Name);
            Assert.Equal("SF", movies.Hits.First().Genre);
            Assert.Equal("SF", movies.Hits.ElementAt(1).Genre);
        }

        [Fact]
        public async Task CustomSearchWithFilterMultipleArray()
        {
            var movies = await this.indexForFaceting.Search<Movie>(
                null,
                new SearchQuery
                {
                    Filter = new string[][] { new string[] { "genre = SF", "genre = SF" }, new string[] { "genre = SF" } },
                });
            movies.Hits.Should().NotBeEmpty();
            movies.FacetsDistribution.Should().BeNull();
            Assert.Equal(2, movies.Hits.Count());
            Assert.Equal("12", movies.Hits.First().Id);
            Assert.Equal("Star Wars", movies.Hits.First().Name);
            Assert.Equal("SF", movies.Hits.First().Genre);
            Assert.Equal("SF", movies.Hits.ElementAt(1).Genre);
        }

        [Fact]
        public async Task CustomSearchWithNumberFilter()
        {
            Settings newFilters = new Settings
            {
                FilterableAttributes = new string[] { "id" },
            };
            UpdateStatus update = await this.indexWithIntId.UpdateSettings(newFilters);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await this.indexWithIntId.WaitForPendingUpdate(update.UpdateId);

            var movies = await this.indexWithIntId.Search<MovieWithIntId>(
                null,
                new SearchQuery
                {
                    Filter = "id = 12",
                });
            movies.Hits.Should().NotBeEmpty();
            movies.FacetsDistribution.Should().BeNull();
            Assert.Single(movies.Hits);
            Assert.Equal(12, movies.Hits.First().Id);
            Assert.Equal("Star Wars", movies.Hits.First().Name);
            Assert.Equal("SF", movies.Hits.First().Genre);
        }

        [Fact]
        public async Task CustomSearchWithMultipleFilter()
        {
            Settings newFilters = new Settings
            {
                FilterableAttributes = new string[] { "genre", "id" },
            };
            UpdateStatus update = await this.indexWithIntId.UpdateSettings(newFilters);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await this.indexWithIntId.WaitForPendingUpdate(update.UpdateId);

            var movies = await this.indexWithIntId.Search<MovieWithIntId>(
                null,
                new SearchQuery
                {
                    Filter = "genre = SF AND id > 12",
                });
            movies.Hits.Should().NotBeEmpty();
            movies.FacetsDistribution.Should().BeNull();
            Assert.Single(movies.Hits);
            Assert.Equal(13, movies.Hits.First().Id);
            Assert.Equal("Harry Potter", movies.Hits.First().Name);
            Assert.Equal("SF", movies.Hits.First().Genre);
        }

        [Fact]
        public async Task CustomSearchWithPhraseSearch()
        {
            var movies = await this.indexForFaceting.Search<Movie>("coco \"harry\"");
            movies.Hits.Should().NotBeEmpty();
            movies.FacetsDistribution.Should().BeNull();
            Assert.Single(movies.Hits);
            Assert.Equal("13", movies.Hits.First().Id);
            Assert.Equal("Harry Potter", movies.Hits.First().Name);
            Assert.Equal("SF", movies.Hits.First().Genre);
        }

        [Fact]
        public async Task CustomSearchWithFacetsDistribution()
        {
            Settings newFilters = new Settings
            {
                FilterableAttributes = new string[] { "genre" },
            };
            UpdateStatus update = await this.basicIndex.UpdateSettings(newFilters);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await this.basicIndex.WaitForPendingUpdate(update.UpdateId);

            var movies = await this.indexForFaceting.Search<Movie>(
                null,
                new SearchQuery
                {
                    FacetsDistribution = new string[] { "genre" },
                });
            movies.Hits.Should().NotBeEmpty();
            movies.FacetsDistribution.Should().NotBeEmpty();
            movies.FacetsDistribution["genre"].Should().NotBeEmpty();
            Assert.Equal(3, movies.FacetsDistribution["genre"]["action"]);
            Assert.Equal(2, movies.FacetsDistribution["genre"]["sf"]);
            Assert.Equal(1, movies.FacetsDistribution["genre"]["french movie"]);
        }
    }
}

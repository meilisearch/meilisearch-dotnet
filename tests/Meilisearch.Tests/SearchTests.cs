using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Xunit;

namespace Meilisearch.Tests
{
    [Collection("Sequential")]
    public class SearchTests : IAsyncLifetime
    {
        private Index _basicIndex;
        private Index _indexForFaceting;
        private Index _indexWithIntId;

        private readonly IndexFixture _fixture;

        public SearchTests(IndexFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]
            _basicIndex = await _fixture.SetUpBasicIndex("BasicIndex-SearchTests");
            _indexForFaceting = await _fixture.SetUpIndexForFaceting("IndexForFaceting-SearchTests");
            _indexWithIntId = await _fixture.SetUpBasicIndexWithIntId("IndexWithIntId-SearchTests");
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task BasicSearch()
        {
            var movies = await _basicIndex.SearchAsync<Movie>("man");
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Name.Should().NotBeEmpty();
            movies.Hits.ElementAt(1).Name.Should().NotBeEmpty();
        }

        [Fact]
        public async Task BasicSearchWithNoQuery()
        {
            var movies = await _basicIndex.SearchAsync<Movie>(null);
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Id.Should().NotBeNull();
            movies.Hits.First().Name.Should().NotBeNull();
        }

        [Fact]
        public async Task BasicSearchWithEmptyQuery()
        {
            var movies = await _basicIndex.SearchAsync<Movie>(string.Empty);
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Id.Should().NotBeNull();
            movies.Hits.First().Name.Should().NotBeNull();
        }

        [Fact]
        public async Task CustomSearchWithLimit()
        {
            var movies = await _basicIndex.SearchAsync<Movie>(
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
            var newFilters = new Settings
            {
                FilterableAttributes = new string[] { "name" },
            };
            var task = await _basicIndex.UpdateSettingsAsync(newFilters);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await _basicIndex.WaitForTaskAsync(task.Uid);

            var movies = await _basicIndex.SearchAsync<FormattedMovie>(
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
            var movies = await _basicIndex.SearchAsync<FormattedMovie>(
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
            var movies = await _basicIndex.SearchAsync<FormattedMovie>(
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
            var movies = await _basicIndex.SearchAsync<FormattedMovie>(
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
            var movies = await _indexForFaceting.SearchAsync<Movie>(
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
        public async Task CustomSearchWithFilterWithSpaces()
        {
            var movies = await _indexForFaceting.SearchAsync<Movie>(
                null,
                new SearchQuery
                {
                    Filter = "genre = 'sci fi'",
                });
            movies.Hits.Should().NotBeEmpty();
            movies.FacetsDistribution.Should().BeNull();
            Assert.Single(movies.Hits);
            Assert.Equal("1344", movies.Hits.First().Id);
            Assert.Equal("The Hobbit", movies.Hits.First().Name);
        }

        [Fact]
        public async Task CustomSearchWithFilterArray()
        {
            var movies = await _indexForFaceting.SearchAsync<Movie>(
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
            var movies = await _indexForFaceting.SearchAsync<Movie>(
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
            var newFilters = new Settings
            {
                FilterableAttributes = new string[] { "id" },
            };
            var task = await _indexWithIntId.UpdateSettingsAsync(newFilters);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await _indexWithIntId.WaitForTaskAsync(task.Uid);

            var movies = await _indexWithIntId.SearchAsync<MovieWithIntId>(
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
            var newFilters = new Settings
            {
                FilterableAttributes = new string[] { "genre", "id" },
            };
            var task = await _indexWithIntId.UpdateSettingsAsync(newFilters);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await _indexWithIntId.WaitForTaskAsync(task.Uid);

            var movies = await _indexWithIntId.SearchAsync<MovieWithIntId>(
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
            var movies = await _indexForFaceting.SearchAsync<Movie>("coco \"harry\"");
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
            var movies = await _indexForFaceting.SearchAsync<Movie>(
                null,
                new SearchQuery
                {
                    FacetsDistribution = new string[] { "genre" },
                });
            movies.Hits.Should().NotBeEmpty();
            movies.FacetsDistribution.Should().NotBeEmpty();
            movies.FacetsDistribution["genre"].Should().NotBeEmpty();
            Assert.Equal(3, movies.FacetsDistribution["genre"]["Action"]);
            Assert.Equal(2, movies.FacetsDistribution["genre"]["SF"]);
            Assert.Equal(1, movies.FacetsDistribution["genre"]["French movie"]);
        }

        [Fact]
        public async Task CustomSearchWithSort()
        {
            var newSortable = new Settings
            {
                SortableAttributes = new string[] { "name" },
            };
            var task = await _basicIndex.UpdateSettingsAsync(newSortable);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await _basicIndex.WaitForTaskAsync(task.Uid);

            var movies = await _basicIndex.SearchAsync<Movie>(
                "man",
                new SearchQuery
                {
                    Sort = new string[] { "name:asc" },
                });
            movies.Hits.Should().NotBeEmpty();
            movies.FacetsDistribution.Should().BeNull();
            Assert.Equal(2, movies.Hits.Count());
            Assert.Equal("14", movies.Hits.First().Id);
        }
    }
}
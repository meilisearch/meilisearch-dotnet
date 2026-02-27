using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Meilisearch.Tests.Models;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class SearchTests<TFixture> : IAsyncLifetime where TFixture : IndexFixture
    {
        private Index _basicIndex;
        private Index _nestedIndex;
        private Index _indexForFaceting;
        private Index _indexForVectorSearch;
        private Index _indexWithIntId;
        private Index _productIndexForDistinct;
        private Index _indexForRankingScoreThreshold;

        private readonly TFixture _fixture;

        public SearchTests(TFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]
            _basicIndex = await _fixture.SetUpBasicIndex("BasicIndex-SearchTests");
            _indexForFaceting = await _fixture.SetUpIndexForFaceting("IndexForFaceting-SearchTests");
            _indexForVectorSearch = await _fixture.SetUpIndexForVectorSearch("IndexForVector-SearchTests");
            _indexWithIntId = await _fixture.SetUpBasicIndexWithIntId("IndexWithIntId-SearchTests");
            _nestedIndex = await _fixture.SetUpIndexForNestedSearch("IndexForNestedDocs-SearchTests");
            _productIndexForDistinct = await _fixture.SetUpIndexForDistinctProductsSearch("IndexForDistinctProducts-SearchTests");
            _indexForRankingScoreThreshold = await _fixture.SetUpIndexForRankingScoreThreshold("IndexForRankingThreshold-SearchTests");
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
        public async Task BasicSearchWithStruct()
        {
            var movies = await _basicIndex.SearchAsync<MovieStruct>("man");
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Name.Should().NotBeEmpty();
            movies.Hits.ElementAt(1).Name.Should().NotBeEmpty();
        }

        [Fact]
        public async Task PaginatedSearchWithStruct()
        {
            var movies = await _basicIndex.SearchAsync<MovieStruct>("man", new SearchQuery()
            {
                Page = 1,
                HitsPerPage = 2,
            });
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
        public async Task BasicSearchWithPerformanceDetails()
        {
            var movies = await _basicIndex.SearchAsync<Movie>("man", new SearchQuery()
            {
                ShowPerformanceDetails = true
            });

            movies.PerformanceDetails.Should().NotBeNullOrEmpty();
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Name.Should().NotBeEmpty();
            movies.Hits.ElementAt(1).Name.Should().NotBeEmpty();
        }

        [Fact]
        public async Task BasicSearchWithoutPerformanceDetails()
        {
            var movies = await _basicIndex.SearchAsync<Movie>("man");

            movies.PerformanceDetails.Should().BeNull();
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Name.Should().NotBeEmpty();
            movies.Hits.ElementAt(1).Name.Should().NotBeEmpty();
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
        public async Task CustomSearchWithPageAndPerformanceDetails()
        {
            var movies = (PaginatedSearchResult<Movie>)await _basicIndex.SearchAsync<Movie>(
                "man",
                new SearchQuery { Page = 1, HitsPerPage = 1, ShowPerformanceDetails = true });

            movies.PerformanceDetails.Should().NotBeNullOrEmpty();
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Name.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CustomSearchWithPageAndWithoutPerformanceDetails()
        {
            var movies = (PaginatedSearchResult<Movie>)await _basicIndex.SearchAsync<Movie>(
                "man",
                new SearchQuery { Page = 1, HitsPerPage = 1 });

            movies.PerformanceDetails.Should().BeNull();
            movies.Hits.Should().NotBeEmpty();
            movies.Hits.First().Name.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CustomSearchWithPage()
        {
            var movies = (PaginatedSearchResult<Movie>)await _basicIndex.SearchAsync<Movie>("man", new SearchQuery { Page = 1, HitsPerPage = 1 });

            Assert.Equal(1, movies.Page);
            Assert.Equal(1, movies.HitsPerPage);
            Assert.Equal(2, movies.TotalHits);
            Assert.Equal(2, movies.TotalPages);
            movies.Hits.First().Id.Should().NotBeEmpty();
            movies.Hits.First().Name.Should().NotBeEmpty();
            movies.Hits.First().Genre.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CustomSearchWithPageWithoutTypeCast()
        {
            var movies = await _basicIndex.SearchAsync<Movie>("man", new SearchQuery { Page = 1 });

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
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await _basicIndex.WaitForTaskAsync(task.TaskUid);

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
            var firstHit = movies.Hits.First();

            Assert.NotEmpty(movies.Hits);
            Assert.Single(movies.Hits);
            Assert.NotEmpty(firstHit.Name);
            Assert.NotEmpty(firstHit.Id);
            Assert.Null(firstHit.Genre);
            Assert.NotEmpty(firstHit._Formatted.Name);
            Assert.Equal("15", firstHit._Formatted.Id);
            Assert.Null(firstHit._Formatted.Genre);
        }

        [Fact]
        public async Task CustomSearchWithAttributesToSearchOn()
        {
            var movies = await _basicIndex.SearchAsync<FormattedMovie>(
                "Harry",
                new SearchQuery
                {
                    AttributesToSearchOn = new[] { "name" },
                });
            var firstHit = movies.Hits.First();

            Assert.NotEmpty(movies.Hits);
            Assert.Single(movies.Hits);
            Assert.NotEmpty(firstHit.Name);
            Assert.NotEmpty(firstHit.Id);
            Assert.NotEmpty(firstHit.Genre);
        }

        [Fact]
        public async Task CustomSearchWithAttributesToSearchOnNoResults()
        {
            var movies = await _basicIndex.SearchAsync<FormattedMovie>(
                "Harry",
                new SearchQuery
                {
                    AttributesToSearchOn = new[] { "genre" },
                });
            var firstHit = movies.Hits.FirstOrDefault();

            Assert.Null(firstHit);
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
            movies.FacetDistribution.Should().BeNull();
            Assert.Equal(2, movies.Hits.Count);
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
            movies.FacetDistribution.Should().BeNull();
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
            movies.FacetDistribution.Should().BeNull();
            Assert.Equal(2, movies.Hits.Count);
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
            movies.FacetDistribution.Should().BeNull();
            Assert.Equal(2, movies.Hits.Count);
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
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await _indexWithIntId.WaitForTaskAsync(task.TaskUid);

            var movies = await _indexWithIntId.SearchAsync<MovieWithIntId>(
                null,
                new SearchQuery
                {
                    Filter = "id = 12",
                });
            movies.Hits.Should().NotBeEmpty();
            movies.FacetDistribution.Should().BeNull();
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
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await _indexWithIntId.WaitForTaskAsync(task.TaskUid);

            var movies = await _indexWithIntId.SearchAsync<MovieWithIntId>(
                null,
                new SearchQuery
                {
                    Filter = "genre = SF AND id > 12",
                });
            movies.Hits.Should().NotBeEmpty();
            movies.FacetDistribution.Should().BeNull();
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
            movies.FacetDistribution.Should().BeNull();
            Assert.Single(movies.Hits);
            Assert.Equal("13", movies.Hits.First().Id);
            Assert.Equal("Harry Potter", movies.Hits.First().Name);
            Assert.Equal("SF", movies.Hits.First().Genre);
        }

        [Fact]
        public async Task CustomSearchWithFacetDistribution()
        {
            var movies = await _indexForFaceting.SearchAsync<Movie>(
                null,
                new SearchQuery
                {
                    Facets = new string[] { "genre" },
                });
            movies.Hits.Should().NotBeEmpty();
            movies.FacetDistribution.Should().NotBeEmpty();
            movies.FacetDistribution["genre"].Should().NotBeEmpty();
            Assert.Equal(3, movies.FacetDistribution["genre"]["Action"]);
            Assert.Equal(2, movies.FacetDistribution["genre"]["SF"]);
            Assert.Equal(1, movies.FacetDistribution["genre"]["French movie"]);
        }

        [Fact]
        public async Task CustomSearchWithFacetStats()
        {
            var newFilters = new Settings
            {
                FilterableAttributes = new string[] { "id" },
            };
            var task = await _indexWithIntId.UpdateSettingsAsync(newFilters);
            await _indexWithIntId.WaitForTaskAsync(task.TaskUid);
            var movies = await _indexWithIntId.SearchAsync<MovieWithIntId>(
                null,
                new SearchQuery
                {
                    Facets = new string[] { "id" },
                });
            movies.Hits.Should().NotBeEmpty();
            movies.FacetDistribution.Should().NotBeEmpty();
            movies.FacetDistribution["id"].Should().NotBeEmpty();
            Assert.Equal(10, movies.FacetStats["id"].Min);
            Assert.Equal(16, movies.FacetStats["id"].Max);
        }

        [Fact]
        public async Task CustomSearchWithSort()
        {
            var newSortable = new Settings
            {
                SortableAttributes = new string[] { "name" },
            };
            var task = await _basicIndex.UpdateSettingsAsync(newSortable);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await _basicIndex.WaitForTaskAsync(task.TaskUid);

            var movies = await _basicIndex.SearchAsync<Movie>(
                "man",
                new SearchQuery
                {
                    Sort = new string[] { "name:asc" },
                });
            movies.Hits.Should().NotBeEmpty();
            movies.FacetDistribution.Should().BeNull();
            Assert.Equal(2, movies.Hits.Count);
            Assert.Equal("14", movies.Hits.First().Id);
        }

        [Fact]
        public async Task CustomSearchWithCroppingParameters()
        {
            var movies = await _basicIndex.SearchAsync<FormattedMovie>(
                "man",
                new SearchQuery { CropLength = 1, AttributesToCrop = new string[] { "*" } }
            );

            Assert.NotEmpty(movies.Hits);
            Assert.Equal("…Man", movies.Hits.First()._Formatted.Name);
        }

        [Fact]
        public async Task CustomSearchWithCropMarker()
        {
            var movies = await _basicIndex.SearchAsync<FormattedMovie>(
                "man",
                new SearchQuery { CropLength = 1, AttributesToCrop = new string[] { "*" }, CropMarker = "[…] " }
            );

            Assert.NotEmpty(movies.Hits);
            Assert.Equal("[…] Man", movies.Hits.First()._Formatted.Name);
        }

        [Fact]
        public async Task CustomSearchWithCustomHighlightTags()
        {
            var movies = await _basicIndex.SearchAsync<FormattedMovie>(
                "man",
                new SearchQuery
                {
                    AttributesToHighlight = new string[] { "*" },
                    HighlightPreTag = "<mark>",
                    HighlightPostTag = "</mark>"
                }
            );

            Assert.NotEmpty(movies.Hits);
            Assert.Equal("Iron <mark>Man</mark>", movies.Hits.First()._Formatted.Name);
        }

        [Fact]
        public async Task CustomSearchWithinNestedDocuments()
        {
            var movies = await _nestedIndex.SearchAsync<MovieWithInfo>("wizard");

            Assert.NotEmpty(movies.Hits);
            Assert.Equal("Harry Potter", movies.Hits.First().Name);
            Assert.Equal("13", movies.Hits.First().Id);
            Assert.Equal("a movie about a wizard boy", movies.Hits.First().Info.Comment);
        }

        [Fact]
        public async Task CustomSearchWithinNestedDocumentsWithSearchableAttributesSettings()
        {
            var task = await _nestedIndex.UpdateSearchableAttributesAsync(new string[] { "name", "info.comment" });
            await _nestedIndex.WaitForTaskAsync(task.TaskUid);

            var movies = await _nestedIndex.SearchAsync<MovieWithInfo>("rich");

            Assert.NotEmpty(movies.Hits);
            Assert.Equal("Iron Man", movies.Hits.First().Name);
            Assert.Equal("14", movies.Hits.First().Id);
            Assert.Equal("a movie about a rich man", movies.Hits.First().Info.Comment);
        }

        [Fact]
        public async Task CustomSearchWithinNestedDocumentsWithSearchableAndSortableAttributesSettings()
        {
            var searchTask = await _nestedIndex.UpdateSearchableAttributesAsync(new string[] { "name", "info.comment" });
            await _nestedIndex.WaitForTaskAsync(searchTask.TaskUid);
            var sortTask = await _nestedIndex.UpdateSortableAttributesAsync(new string[] { "info.reviewNb" });
            await _nestedIndex.WaitForTaskAsync(sortTask.TaskUid);

            var query = new SearchQuery { Sort = new string[] { "info.reviewNb:desc" } };
            var movies = await _nestedIndex.SearchAsync<MovieWithInfo>("", query);

            Assert.NotEmpty(movies.Hits);
            Assert.Equal("Interstellar", movies.Hits.First().Name);
            Assert.Equal("11", movies.Hits.First().Id);
            Assert.Equal(1000, movies.Hits.First().Info.ReviewNb);
        }

        [Fact]
        public async Task CustomSearchWithMatchingStrategyALL()
        {
            var searchQuery = new SearchQuery() { MatchingStrategy = "all" };
            var movies = await _nestedIndex.SearchAsync<MovieWithInfo>("movie about rich", searchQuery);

            movies.Hits.Should().ContainSingle();
        }

        [Fact]
        public async Task CustomSearchWithMatchingStrategyLast()
        {
            var searchQuery = new SearchQuery() { MatchingStrategy = "last" };
            var movies = await _nestedIndex.SearchAsync<MovieWithInfo>("movie about rich", searchQuery);

            Assert.True(movies.Hits.Count > 1);
        }

        [Fact]
        public async Task CustomSearchWithShowRankingScore()
        {
            var searchQuery = new SearchQuery()
            {
                ShowRankingScore = true
            };
            var movies = await _basicIndex.SearchAsync<MovieWithRankingScore>("iron man", searchQuery);
            Assert.NotNull(movies.Hits.First()._RankingScore);
        }

        [Fact]
        public async Task CustomSearchWithShowRankingScoreDetails()
        {
            var searchQuery = new SearchQuery()
            {
                ShowRankingScoreDetails = true
            };
            var movies = await _basicIndex.SearchAsync<MovieWithRankingScoreDetails>("iron man", searchQuery);
            Assert.NotEmpty(movies.Hits.First()._RankingScoreDetails);
        }

        [Fact]
        public async Task CustomSearchProductsWithoutDistinct()
        {
            var searchQuery = new SearchQuery()
            {

            };
            var products = await _productIndexForDistinct.SearchAsync<Product>("", searchQuery);
            products.Hits.Count.Should().Be(14);
        }

        [Fact]
        public async Task CustomSearchProductsWithDistinct()
        {
            var searchQuery = new SearchQuery()
            {
                Distinct = "product_id"
            };
            var products = await _productIndexForDistinct.SearchAsync<Product>("", searchQuery);
            products.Hits.Count.Should().Be(6);
        }

        [Fact]
        public async Task CustomSearchWithRankingScoreThreshold()
        {
            var searchQuery = new SearchQuery { };
            var movies = await _indexForRankingScoreThreshold.SearchAsync<MovieWithInfo>("a wizard movie", searchQuery);
            movies.Hits.Count.Should().Be(4);

            searchQuery.RankingScoreThreshold = 0.5M;
            movies = await _indexForRankingScoreThreshold.SearchAsync<MovieWithInfo>("a wizard movie", searchQuery);
            movies.Hits.Count.Should().Be(1);
            movies.Hits.First().Id.Should().Be("13");
            movies.Hits.First().Name.Should().Be("Harry Potter");
        }

        [Fact]
        public async Task CustomSearchWithVector()
        {
            var searchQuery = new SearchQuery
            {
                Hybrid = new HybridSearch
                {
                    Embedder = "manual",
                    SemanticRatio = 1.0f
                },
                Vector = new[] { 0.1, 0.6, 0.8 },
            };

            var movies = await _indexForVectorSearch.SearchAsync<VectorMovie>(string.Empty, searchQuery);

            Assert.Equal("522681", movies.Hits.First().Id);
            Assert.Equal("Escape Room", movies.Hits.First().Title);
        }

        [Fact]
        public async Task CustomSearchWithSimilarDocuments()
        {
            var query = new SimilarDocumentsQuery("143")
            {
                Embedder = "manual"
            };

            var movies = await _indexForVectorSearch.SearchSimilarDocumentsAsync<VectorMovie>(query);

            Assert.Collection(movies.Hits,
                m => Assert.Equal("Escape Room", m.Title),
                m => Assert.Equal("Captain Marvel", m.Title),
                m => Assert.Equal("How to Train Your Dragon: The Hidden World", m.Title),
                m => Assert.Equal("Shazam!", m.Title)
            );
        }

        [Fact]
        public async Task CustomSearchWithSimilarDocumentsAndPerformanceDetails()
        {
            var query = new SimilarDocumentsQuery("143")
            {
                Embedder = "manual",
                ShowPerformanceDetails = true
            };

            var movies = await _indexForVectorSearch.SearchSimilarDocumentsAsync<VectorMovie>(query);

            movies.PerformanceDetails.Should().NotBeNullOrEmpty();

            Assert.Collection(movies.Hits,
                m => Assert.Equal("Escape Room", m.Title),
                m => Assert.Equal("Captain Marvel", m.Title),
                m => Assert.Equal("How to Train Your Dragon: The Hidden World", m.Title),
                m => Assert.Equal("Shazam!", m.Title)
            );
        }

        [Fact]
        public async Task CustomSearchWithSimilarDocumentsAndWithoutPerformanceDetails()
        {
            var query = new SimilarDocumentsQuery("143")
            {
                Embedder = "manual",
                ShowPerformanceDetails = false
            };

            var movies = await _indexForVectorSearch.SearchSimilarDocumentsAsync<VectorMovie>(query);

            movies.PerformanceDetails.Should().BeNull();

            Assert.Collection(movies.Hits,
                m => Assert.Equal("Escape Room", m.Title),
                m => Assert.Equal("Captain Marvel", m.Title),
                m => Assert.Equal("How to Train Your Dragon: The Hidden World", m.Title),
                m => Assert.Equal("Shazam!", m.Title)
            );
        }
    }
}

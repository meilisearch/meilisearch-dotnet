using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using FluentAssertions;

using Meilisearch.Tests.Fixtures;
using Meilisearch.Tests.Models;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class MultiIndexSearchTests<TFixture> : IAsyncLifetime
        where TFixture : IndexFixture
    {
        private Index _index1;
        private Index _index2;

        private readonly TFixture _fixture;

        public MultiIndexSearchTests(TFixture fixture)
        {
            _fixture = fixture;
        }

        public Task DisposeAsync() => Task.CompletedTask;

        public async Task InitializeAsync()
        {
            await _fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]
            _index1 = await _fixture.SetUpBasicIndex("BasicIndex-MultiSearch-Index1");
            _index2 = await _fixture.SetUpBasicIndex("BasicIndex-MultiSearch-Index2");
            var t1 = _index1.UpdateFilterableAttributesAsync(new[] { "genre" });
            var t2 = _index2.UpdateFilterableAttributesAsync(new[] { "genre" });
            await Task.WhenAll(
                (await Task.WhenAll(t1, t2)).Select(x => _fixture.DefaultClient.WaitForTaskAsync(x.TaskUid)));
        }

        [Fact]
        public async Task BasicSearch()
        {
            var result = await _fixture.DefaultClient.MultiSearchAsync(new MultiSearchQuery()
            {
                Queries = new System.Collections.Generic.List<SearchQuery>()
                {
                    new SearchQuery() { IndexUid = _index1.Uid, Q = "", Filter = "genre = 'SF'" },
                    new SearchQuery() { IndexUid = _index2.Uid, Q = "", Filter = "genre = 'Action'" }
                }
            });


            Movie GetMovie(IEnumerable<Movie> movies, string id)
            {
                return movies.FirstOrDefault(x => x.Id == id);
            }

            var original1 = await _index1.GetDocumentsAsync<Movie>();
            var originalHits1 = original1.Results;
            result.Results.Should().HaveCount(2);
            var res1 = result.Results[0];
            res1.IndexUid.Should().Be(_index1.Uid);
            var res1Hits = res1.Hits.Select(x => x.Deserialize<Movie>(Constants.JsonSerializerOptionsWriteNulls));
            res1Hits.Should().HaveCount(2);
            res1Hits.All(x =>
            {
                var og = GetMovie(originalHits1, x.Id);
                return og.Name == x.Name && og.Genre == x.Genre;
            }).Should().BeTrue();

            var original2 = await _index2.GetDocumentsAsync<Movie>();
            var originalHits2 = original2.Results.ToList();
            var res2 = result.Results[1];
            var res2Hits = res2.Hits.Select(x => x.Deserialize<Movie>(Constants.JsonSerializerOptionsWriteNulls));
            res2Hits.Should().HaveCount(2);
            res2.IndexUid.Should().Be(_index2.Uid);
            res1Hits.All(x =>
            {
                var og = GetMovie(originalHits2, x.Id);
                return og.Name == x.Name && og.Genre == x.Genre;
            }).Should().BeTrue();
        }


        [Fact]
        public async Task FederatedSearchWithNoFederationOptions()
        {
            var result = await _fixture.DefaultClient.FederatedMultiSearchAsync<Movie>(
                new FederatedMultiSearchQuery()
                {
                    Queries = new List<FederatedSearchQuery>()
                    {
                        new FederatedSearchQuery() { IndexUid = _index1.Uid, Q = "", Filter = "genre = 'SF'" },
                        new FederatedSearchQuery()
                        {
                            IndexUid = _index2.Uid, Q = "", Filter = "genre = 'Action'"
                        }
                    },
                });

            result.Hits.Should().HaveCount(4);
        }

        [Fact]
        public async Task FederatedSearchWithEmptyOptions()
        {
            var result = await _fixture.DefaultClient.FederatedMultiSearchAsync<Movie>(
                new FederatedMultiSearchQuery()
                {
                    Queries = new List<FederatedSearchQuery>()
                    {
                        new FederatedSearchQuery() { IndexUid = _index1.Uid, Q = "", Filter = "genre = 'SF'" },
                        new FederatedSearchQuery()
                        {
                            IndexUid = _index2.Uid, Q = "", Filter = "genre = 'Action'"
                        }
                    },
                    FederationOptions = new MultiSearchFederationOptions() { }
                });

            result.Hits.Should().HaveCount(4);
        }

        [Fact]
        public async Task FederatedSearchWithLimitAndOffset()
        {
            var federatedquer = new FederatedMultiSearchQuery()
            {
                Queries = new List<FederatedSearchQuery>()
                {
                    new FederatedSearchQuery() { IndexUid = _index1.Uid, Q = "", Filter = "genre = 'SF'" },
                    new FederatedSearchQuery() { IndexUid = _index2.Uid, Q = "", Filter = "genre = 'Action'" }
                },
                FederationOptions = new MultiSearchFederationOptions() { Limit = 2, Offset = 0 }
            };
            var result = await _fixture.DefaultClient.FederatedMultiSearchAsync<Movie>(federatedquer
            );

            var testJson = JsonSerializer.Serialize(federatedquer);

            result.Hits.Should().HaveCount(2);
        }

        [Fact]
        public async Task FederatedSearchWithRankingScoreThreshold()
        {
            // "Iron Spider" matches "Iron Man" loosely, "Star Wars Harry" matches "Star Wars" well.
            var query = new FederatedMultiSearchQuery
            {
                Queries = new List<FederatedSearchQuery>()
                {
                    new FederatedSearchQuery { IndexUid = _index1.Uid, Q = "Iron Spider" },
                    new FederatedSearchQuery { IndexUid = _index2.Uid, Q = "Star Wars Harry" }
                }
            };

            var unfiltered = await _fixture.DefaultClient.FederatedMultiSearchAsync<Movie>(query);
            unfiltered.Hits.Should().HaveCount(2);

            // The threshold is per query: only the loose match is dropped.
            query.Queries[0].RankingScoreThreshold = 0.5M;

            var filtered = await _fixture.DefaultClient.FederatedMultiSearchAsync<Movie>(query);
            filtered.Hits.Should().ContainSingle().Which.Name.Should().Be("Star Wars");
        }

        [Fact]
        public async Task FederatedSearchWithDistinct()
        {
            var query = new FederatedMultiSearchQuery
            {
                Queries = new List<FederatedSearchQuery>
                {
                    new FederatedSearchQuery { IndexUid = _index1.Uid, Q = "", Filter = "genre = 'SF'" },
                    new FederatedSearchQuery { IndexUid = _index2.Uid, Q = "", Filter = "genre = 'SF'" }
                }
            };

            var unfiltered = await _fixture.DefaultClient.FederatedMultiSearchAsync<Movie>(query);
            unfiltered.Hits.Should().HaveCount(4);

            foreach (var federatedQuery in query.Queries)
            {
                federatedQuery.Distinct = "genre";
            }

            // One hit per distinct genre value, per query.
            var distinct = await _fixture.DefaultClient.FederatedMultiSearchAsync<Movie>(query);
            distinct.Hits.Should().HaveCount(2);
        }

        [Fact]
        public async Task FederatedSearchWithPerQueryFederationOptions()
        {
            // Both queries match the same two movies; the weights decide which index wins the merge.
            var query = new FederatedMultiSearchQuery
            {
                Queries = new List<FederatedSearchQuery>
                {
                    new FederatedSearchQuery
                    {
                        IndexUid = _index1.Uid,
                        Q = "Star Wars",
                        FederationOptions = new FederatedSearchQueryOptions { Weight = 0.1M }
                    },
                    new FederatedSearchQuery
                    {
                        IndexUid = _index2.Uid,
                        Q = "Star Wars",
                        FederationOptions = new FederatedSearchQueryOptions { Weight = 1.0M }
                    }
                },
                FederationOptions = new MultiSearchFederationOptions { Limit = 1 }
            };

            var result = await _fixture.DefaultClient.FederatedMultiSearchAsync<FederatedMovie>(query);

            // Identical matches in both indexes, so the heavier query's hit is the one kept.
            var hit = result.Hits.Should().ContainSingle().Subject;
            hit.Name.Should().Be("Star Wars");
            hit.Federation.IndexUid.Should().Be(_index2.Uid);
            hit.Federation.WeightedRankingScore.Should().BeApproximately(1.0, 1e-6);
        }

        [Fact]
        public async Task FederatedSearchWithHybridAndVector()
        {
            var vectorIndex1 = await _fixture.SetUpIndexForVectorSearch("VectorIndex-MultiSearch-Index1");
            var vectorIndex2 = await _fixture.SetUpIndexForVectorSearch("VectorIndex-MultiSearch-Index2");

            FederatedSearchQuery VectorQuery(string indexUid) => new FederatedSearchQuery
            {
                IndexUid = indexUid,
                Q = string.Empty,
                Hybrid = new HybridSearch { Embedder = "manual", SemanticRatio = 1.0f },
                Vector = new[] { 0.1, 0.6, 0.8 },
                RetrieveVectors = true
            };

            var query = new FederatedMultiSearchQuery
            {
                Queries = new List<FederatedSearchQuery>
                {
                    VectorQuery(vectorIndex1.Uid),
                    VectorQuery(vectorIndex2.Uid)
                },
                FederationOptions = new MultiSearchFederationOptions { Limit = 2 }
            };

            var result = await _fixture.DefaultClient.FederatedMultiSearchAsync<VectorMovieWithEmbeddings>(query);

            // The vector queried for is "Escape Room"'s own, so it ranks first in both indexes.
            result.Hits.Should().HaveCount(2);
            result.Hits.Should().OnlyContain(movie => movie.Title == "Escape Room");
            // Meilisearch stores embeddings as f32, so the values come back slightly rounded.
            result.Hits.First().Vectors["manual"].Embeddings.Should()
                .ContainSingle().Which.Should().BeEquivalentTo(
                    new[] { 0.1, 0.6, 0.8 },
                    options => options.Using<double>(
                        ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-6)).WhenTypeIs<double>());
        }
    }
}

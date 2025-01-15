using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using FluentAssertions;

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
            await Task.WhenAll((await Task.WhenAll(t1, t2)).Select(x => _fixture.DefaultClient.WaitForTaskAsync(x.TaskUid)));
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
    }
}

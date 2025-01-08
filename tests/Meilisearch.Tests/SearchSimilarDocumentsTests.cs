using System.Threading.Tasks;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class SearchSimilarDocumentsTests<TFixture> : IAsyncLifetime where TFixture : IndexFixture
    {
        private Index _basicIndex;

        private readonly TFixture _fixture;

        public SearchSimilarDocumentsTests(TFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.DeleteAllIndexes();
            _basicIndex = await _fixture.SetUpBasicIndex("BasicIndex-SearchTests");
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task SearchSimilarDocuments()
        {
            _basicIndex.UpdateSettingsAsync()

            var response = await _basicIndex.SearchSimilarDocuments<Movie>("13");
            Assert.Single(response.Hits);
        }
    }
}

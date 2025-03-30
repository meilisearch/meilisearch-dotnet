using System.Threading.Tasks;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class SearchSimilarDocumentsTests<TFixture> : IAsyncLifetime where TFixture : IndexFixture
    {
        private readonly MeilisearchClient _client;
        private Index _basicIndex;

        private readonly TFixture _fixture;

        public SearchSimilarDocumentsTests(TFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.DefaultClient;
        }

        public async Task InitializeAsync()
        {
            await _fixture.DeleteAllIndexes();
            _basicIndex = await _fixture.SetUpIndexForSimilarDocumentsSearch("BasicIndexWithVectorStore-SearchTests");
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task SearchSimilarDocuments()
        {
            await _client.UpdateExperimentalFeatureAsync("vectorStore", true);

            //TODO: add embedder

            var response = await _basicIndex.SearchSimilarDocuments<Movie>("13");
            Assert.Single(response.Hits);
        }
    }
}

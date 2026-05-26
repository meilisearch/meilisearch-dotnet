using System;
using System.Net.Http;
using System.Threading.Tasks;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class MeilisearchClientFixture : IAsyncLifetime
    {
        public MeilisearchClientFixture()
        {
            DefaultClient = new MeilisearchClient(MeilisearchAddress(), ApiKey);
            var httpClient = new HttpClient(new MeilisearchMessageHandler(new HttpClientHandler())) { BaseAddress = new Uri(MeilisearchAddress()) };
            ClientWithCustomHttpClient = new MeilisearchClient(httpClient, ApiKey);
        }

        private const string ApiKey = "masterKey";

        public virtual string MeilisearchAddress()
        {
            throw new InvalidOperationException("Please override the MeilisearchAddress property in inhereted class.");
        }

        public MeilisearchClient DefaultClient { get; private set; }
        public MeilisearchClient ClientWithCustomHttpClient { get; private set; }

        public virtual Task InitializeAsync() => Task.CompletedTask;

        public abstract Task DisposeAsync();
    }
}

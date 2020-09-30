namespace Meilisearch.Tests
{
    using System;

    public class DocumentFixture : IDisposable
    {
        public DocumentFixture()
        {
            var client = new MeilisearchClient("http://localhost:7700", "masterKey");
            this.SetUp(client, "Movies");
            this.SetUpForDocumentsDeletion(client, "MoviesToDelete");
            this.SetUpForFaceting(client, "MoviesForFaceting");
        }

        public Meilisearch.Index BasicIndexWithDocuments { get; private set; }

        public Meilisearch.Index IndexForDocumentsDeletion { get; private set; }

        public Meilisearch.Index IndexForFaceting { get; private set; }

        public async void SetUp(MeilisearchClient client, string indexUid)
        {
            this.BasicIndexWithDocuments = client.GetOrCreateIndex(indexUid).Result;
            var movies = new[]
            {
                new Movie { Id = "10", Name = "Gladiator" },
                new Movie { Id = "11", Name = "Interstellar" },
                new Movie { Id = "12", Name = "Star Wars", Genre = "SF" },
                new Movie { Id = "13", Name = "Harry Potter", Genre = "SF" },
                new Movie { Id = "14", Name = "Iron Man", Genre = "Action" },
                new Movie { Id = "15", Name = "Spider-Man", Genre = "Action" },
                new Movie { Id = "16", Name = "Amélie Poulain", Genre = "French movie" },
            };
            await this.BasicIndexWithDocuments.AddDocuments(movies);
        }

        public void Dispose()
        {
        }

        private async void SetUpForDocumentsDeletion(MeilisearchClient client, string indexUid)
        {
            this.IndexForDocumentsDeletion = client.GetOrCreateIndex(indexUid).Result;
            var movies = new[] { new Movie { Id = "10", Name = "SuperMan" } };
            await this.IndexForDocumentsDeletion.AddDocuments(movies);
        }
        public async void SetUpForFaceting(MeilisearchClient client, string indexUid)
        {
            this.IndexForFaceting = client.GetOrCreateIndex(indexUid).Result;
            var movies = new[]
            {
                new Movie { Id = "10", Name = "Gladiator" },
                new Movie { Id = "11", Name = "Interstellar" },
                new Movie { Id = "12", Name = "Star Wars", Genre = "SF" },
                new Movie { Id = "13", Name = "Harry Potter", Genre = "SF" },
                new Movie { Id = "14", Name = "Iron Man", Genre = "Action" },
                new Movie { Id = "15", Name = "Spider-Man", Genre = "Action" },
                new Movie { Id = "16", Name = "Amélie Poulain", Genre = "French movie" },
                new Movie { Id = "17", Name = "Mission Impossible", Genre = "Action" },
            };
            await this.IndexForFaceting.AddDocuments(movies);
            Settings settings = new Settings
            {
                AttributesForFaceting = new string[] { "genre" },
            };
            await this.IndexForFaceting.UpdateAllSettings(settings);
        }
    }
}

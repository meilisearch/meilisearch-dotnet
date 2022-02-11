using System;
using System.Threading.Tasks;

using Xunit;

namespace Meilisearch.Tests
{
    public class IndexFixture : IAsyncLifetime
    {
        public IndexFixture()
        {
            DefaultClient = new MeilisearchClient("http://localhost:7700", "masterKey");
        }

        public MeilisearchClient DefaultClient { get; private set; }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync() => await DeleteAllIndexes(); // Let a clean Meilisearch instance, for maintainers convenience only.

        public async Task<Meilisearch.Index> SetUpEmptyIndex(string indexUid, string primaryKey = default)
        {
            var task = await DefaultClient.CreateIndexAsync(indexUid, primaryKey);

            // Check the index has been created
            var finishedTask = await DefaultClient.WaitForTaskAsync(task.Uid);
            if (finishedTask.Status != "succeeded")
            {
                throw new Exception("The index was not created in SetUpEmptyIndex. Impossible to run the tests.");
            }

            return DefaultClient.Index(indexUid);
        }

        public async Task<Meilisearch.Index> SetUpBasicIndex(string indexUid)
        {
            var index = DefaultClient.Index(indexUid);
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
            var task = await index.AddDocumentsAsync(movies);

            // Check the documents have been added
            var finishedTask = await index.WaitForTaskAsync(task.Uid);
            if (finishedTask.Status != "succeeded")
            {
                throw new Exception("The documents were not added during SetUpBasicIndex. Impossible to run the tests.");
            }

            return index;
        }

        public async Task<Meilisearch.Index> SetUpBasicIndexWithIntId(string indexUid)
        {
            var index = DefaultClient.Index(indexUid);
            var movies = new[]
            {
                new MovieWithIntId { Id = 10, Name = "Gladiator" },
                new MovieWithIntId { Id = 11, Name = "Interstellar" },
                new MovieWithIntId { Id = 12, Name = "Star Wars", Genre = "SF" },
                new MovieWithIntId { Id = 13, Name = "Harry Potter", Genre = "SF" },
                new MovieWithIntId { Id = 14, Name = "Iron Man", Genre = "Action" },
                new MovieWithIntId { Id = 15, Name = "Spider-Man", Genre = "Action" },
                new MovieWithIntId { Id = 16, Name = "Amélie Poulain", Genre = "French movie" },
            };
            var task = await index.AddDocumentsAsync(movies);

            // Check the documents have been added
            var finishedTask = await index.WaitForTaskAsync(task.Uid);
            if (finishedTask.Status != "succeeded")
            {
                throw new Exception("The documents were not added during SetUpBasicIndexWithIntId. Impossible to run the tests.");
            }

            return index;
        }

        public async Task<Meilisearch.Index> SetUpIndexForFaceting(string indexUid)
        {
            var index = DefaultClient.Index(indexUid);

            // Add documents
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
                new Movie { Id = "1344", Name = "The Hobbit", Genre = "sci fi" },
            };
            var task = await index.AddDocumentsAsync(movies);

            // Check the documents have been added
            var finishedTask = await index.WaitForTaskAsync(task.Uid);
            if (finishedTask.Status != "succeeded")
            {
                throw new Exception("The documents were not added during SetUpIndexForFaceting. Impossible to run the tests.");
            }

            // task settings
            var settings = new Settings
            {
                FilterableAttributes = new string[] { "genre" },
            };
            task = await index.UpdateSettingsAsync(settings);

            // Check the settings have been added
            finishedTask = await index.WaitForTaskAsync(task.Uid);
            if (finishedTask.Status != "succeeded")
            {
                throw new Exception("The settings were not added during SetUpIndexForFaceting. Impossible to run the tests.");
            }

            return index;
        }

        public async Task DeleteAllIndexes()
        {
            var indexes = await DefaultClient.GetAllIndexesAsync();
            foreach (var index in indexes)
            {
                await index.DeleteAsync();
            }
        }

        [CollectionDefinition("Sequential")]
        public class IndexCollection : ICollectionFixture<IndexFixture>
        {
            // This class has no code, and is never created. Its purpose is simply
            // to be the place to apply [CollectionDefinition] and all the
            // ICollectionFixture<> interfaces.

            // It makes the collections be executed sequentially because
            // the fixture and the tests are under the same collection named "Sequential"

            // Without using the fixture collection, this fixture would be called at the beginning of each
            // test class. We could control the execution order of the test classes but we could not control
            // the creation order of the fixture, which means the DeleteAllIndexes method would be called when
            // it's not expected.

            // cf https://xunit.net/docs/shared-context#collection-fixture
        }
    }
}

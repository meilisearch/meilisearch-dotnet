using System;
using System.Net.Http;
using System.Threading.Tasks;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class IndexFixture : IAsyncLifetime
    {
        public IndexFixture()
        {
            DefaultClient = new MeilisearchClient(MeilisearchAddress, ApiKey);
            var httpClient = new HttpClient(new MeilisearchMessageHandler(new HttpClientHandler())) { BaseAddress = new Uri(MeilisearchAddress) };
            ClientWithCustomHttpClient = new MeilisearchClient(httpClient, ApiKey);
        }

        private const string ApiKey = "masterKey";

        public virtual string MeilisearchAddress =>
            throw new InvalidOperationException("Please override the MeilisearchAddress property in inhereted class.");

        public MeilisearchClient DefaultClient { get; private set; }
        public MeilisearchClient ClientWithCustomHttpClient { get; private set; }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync() => await DeleteAllIndexes(); // Let a clean Meilisearch instance, for maintainers convenience only.

        public async Task<Index> SetUpEmptyIndex(string indexUid, string primaryKey = default)
        {
            var task = await DefaultClient.CreateIndexAsync(indexUid, primaryKey);

            // Check the index has been created
            var finishedTask = await DefaultClient.WaitForTaskAsync(task.TaskUid);
            if (finishedTask.Status != TaskInfoStatus.Succeeded)
            {
                throw new Exception("The index was not created in SetUpEmptyIndex. Impossible to run the tests.");
            }

            return DefaultClient.Index(indexUid);
        }

        public async Task<Index> SetUpBasicIndex(string indexUid)
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
            var finishedTask = await index.WaitForTaskAsync(task.TaskUid);
            if (finishedTask.Status != TaskInfoStatus.Succeeded)
            {
                throw new Exception("The documents were not added during SetUpBasicIndex. Impossible to run the tests.");
            }

            return index;
        }

        public async Task<Index> SetUpBasicIndexWithIntId(string indexUid)
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
            var finishedTask = await index.WaitForTaskAsync(task.TaskUid);
            if (finishedTask.Status != TaskInfoStatus.Succeeded)
            {
                throw new Exception("The documents were not added during SetUpBasicIndexWithIntId. Impossible to run the tests.");
            }

            return index;
        }

        public async Task<Index> SetUpIndexForFaceting(string indexUid)
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
            var finishedTask = await index.WaitForTaskAsync(task.TaskUid);
            if (finishedTask.Status != TaskInfoStatus.Succeeded)
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
            finishedTask = await index.WaitForTaskAsync(task.TaskUid);
            if (finishedTask.Status != TaskInfoStatus.Succeeded)
            {
                throw new Exception("The settings were not added during SetUpIndexForFaceting. Impossible to run the tests.");
            }

            return index;
        }

        public async Task<Index> SetUpIndexForNestedSearch(string indexUid)
        {
            var index = DefaultClient.Index(indexUid);
            var movies = new[]
            {
                new MovieWithInfo
                {
                    Id = "10",
                    Name = "Gladiator",
                    Info = new MovieInfo { Comment = "a movie about old times", ReviewNb = 700 }
                },
                new MovieWithInfo
                {
                    Id = "11",
                    Name = "Interstellar",
                    Info = new MovieInfo { Comment = "the best movie", ReviewNb = 1000 }
                },
                new MovieWithInfo
                {
                    Id = "12",
                    Name = "Star Wars",
                    Info = new MovieInfo { Comment = "a lot of wars in the stars", ReviewNb = 900 }
                },
                new MovieWithInfo
                {
                    Id = "13",
                    Name = "Harry Potter",
                    Info = new MovieInfo { Comment = "a movie about a wizard boy", ReviewNb = 900 }
                },
                new MovieWithInfo
                {
                    Id = "14",
                    Name = "Iron Man",
                    Info = new MovieInfo { Comment = "a movie about a rich man", ReviewNb = 800 }
                },
                new MovieWithInfo
                {
                    Id = "15",
                    Name = "Spider-Man",
                    Info = new MovieInfo { Comment = "the spider bit the boy", ReviewNb = 900 }
                },
                new MovieWithInfo
                {
                    Id = "16",
                    Name = "Amélie Poulain",
                    Info = new MovieInfo { Comment = "talks about hapiness", ReviewNb = 800 }
                },
            };
            var task = await index.AddDocumentsAsync(movies);

            // Check the documents have been added
            var finishedTask = await index.WaitForTaskAsync(task.TaskUid);
            if (finishedTask.Status != TaskInfoStatus.Succeeded)
            {
                throw new Exception("The documents were not added during SetUpIndexForNestedSearch. Impossible to run the tests.");
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
    }
}

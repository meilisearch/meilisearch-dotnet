using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class IndexFixture : IAsyncLifetime
    {
        public IndexFixture()
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
            var movies = await JsonFileReader.ReadAsync<List<Movie>>(Datasets.MoviesWithStringIdJsonPath);
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
            var movies = await JsonFileReader.ReadAsync<List<MovieWithIntId>>(Datasets.MoviesWithIntIdJsonPath);
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
            var movies = await JsonFileReader.ReadAsync<List<Movie>>(Datasets.MoviesForFacetingJsonPath);
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
            var movies = await JsonFileReader.ReadAsync<List<MovieWithInfo>>(Datasets.MoviesWithInfoJsonPath);
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
            foreach (var index in indexes.Results)
            {
                await index.DeleteAsync();
            }
        }
    }
}

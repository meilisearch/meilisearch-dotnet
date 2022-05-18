using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class DocumentTests<TFixture> : IAsyncLifetime where TFixture : IndexFixture
    {
        private readonly MeilisearchClient _client;

        private readonly TFixture _fixture;

        public DocumentTests(TFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.DefaultClient;
        }

        public async Task InitializeAsync() => await _fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]

        public Task DisposeAsync() => Task.CompletedTask;

        [Theory]
        [InlineData("json")]
        public async Task BasicDocumentsAddition(string format)
        {
            var indexUID = $"BasicDocumentsAdditionTest_{format}";
            var index = _client.Index(indexUID);

            // Add the documents
            TaskInfo task;
            switch (format)
            {
                case "json":
                    task = await index.AddDocumentsJsonAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
                    break;
                default:
                    throw new IndexOutOfRangeException($"Unsupported format: {format}");
            }

            task.Uid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.Uid);

            // Check the documents have been added
            var docs = await index.GetDocumentsAsync<Movie>();
            Assert.Equal("1", docs.First().Id);
            Assert.Equal("Batman", docs.First().Name);
            docs.First().Genre.Should().BeNull();
        }

        [Theory]
        [InlineData("json")]
        public async Task BasicDocumentsAdditionInBatches(string format)
        {
            var indexUID = $"BasicDocumentsAdditionInBatchesTest_{format}";
            var index = _client.Index(indexUID);

            // Add the documents
            Movie[] movies =
            {
                new Movie { Id = "1", Name = "Batman" },
                new Movie { Id = "2", Name = "Reservoir Dogs" },
                new Movie { Id = "3", Name = "Taxi Driver" },
                new Movie { Id = "4", Name = "Interstellar" },
                new Movie { Id = "5", Name = "Titanic" },
            };

            IEnumerable<TaskInfo> tasks;
            switch (format)
            {
                case "json":
                    tasks = await index.AddDocumentsJsonInBatchesAsync(movies, 2);
                    break;
                default:
                    throw new IndexOutOfRangeException($"Unsupported format: {format}");
            }

            foreach (var u in tasks)
            {
                u.Uid.Should().BeGreaterOrEqualTo(0);
                await index.WaitForTaskAsync(u.Uid);
            }

            // Check the documents have been added (one movie from each batch)
            var docs = (await index.GetDocumentsAsync<Movie>()).ToList();
            Assert.Equal("1", docs.ElementAt(0).Id);
            Assert.Equal("Batman", docs.ElementAt(0).Name);
            Assert.Equal("3", docs.ElementAt(2).Id);
            Assert.Equal("Taxi Driver", docs.ElementAt(2).Name);
        }

        [Theory]
        [InlineData("json")]
        public async Task BasicDocumentsAdditionWithAlreadyCreatedIndex(string format)
        {
            var indexUid = $"BasicDocumentsAdditionWithAlreadyCreatedIndexTest_{format}";
            var task = await _client.CreateIndexAsync(indexUid);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await _client.Index(indexUid).WaitForTaskAsync(task.Uid);

            // Add the documents
            var index = _client.Index(indexUid);
            switch (format)
            {
                case "json":
                    task = await index.AddDocumentsJsonAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
                    break;
                default:
                    throw new System.IndexOutOfRangeException($"Unsupported format: {format}");
            }

            task.Uid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.Uid);

            // Check the documents have been added
            var docs = await index.GetDocumentsAsync<Movie>();
            Assert.Equal("1", docs.First().Id);
            Assert.Equal("Batman", docs.First().Name);
            docs.First().Genre.Should().BeNull();
        }

        [Theory]
        [InlineData("json")]
        public async Task BasicDocumentsAdditionWithTimeoutError(string format)
        {
            var indexUID = $"BasicDocumentsAdditionWithTimeoutError_{format}";
            var index = _client.Index(indexUID);

            // Add the documents
            TaskInfo task;
            switch (format)
            {
                case "json":
                    task = await index.AddDocumentsJsonAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
                    break;
                default:
                    throw new System.IndexOutOfRangeException($"Unsupported format: {format}");
            }
            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => index.WaitForTaskAsync(task.Uid, 0));
        }

        [Theory]
        [InlineData("json")]
        public async Task BasicDocumentsAdditionWithTimeoutErrorByInterval(string format)
        {
            var indexUID = $"BasicDocumentsAdditionWithTimeoutErrorByIntervalTest_{format}";
            var index = _client.Index(indexUID);

            // Add the documents
            TaskInfo task;
            switch (format)
            {
                case "json":
                    task = await index.AddDocumentsJsonAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
                    break;
                default:
                    throw new System.IndexOutOfRangeException($"Unsupported format: {format}");
            }
            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => index.WaitForTaskAsync(task.Uid, 0, 10));
        }

        [Theory]
        [InlineData("json")]
        public async Task DocumentsAdditionWithPrimaryKey(string format)
        {
            var indexUid = $"DocumentsAdditionWithPrimaryKeyTest_{format}";
            var index = _client.Index(indexUid);
            index.PrimaryKey.Should().BeNull();

            // Add the documents
            TaskInfo task;
            switch (format)
            {
                case "json":
                    task = await index.AddDocumentsJsonAsync(new[] { new { Key = "1", Name = "Ironman" } }, "key");
                    break;
                default:
                    throw new System.IndexOutOfRangeException($"Unsupported format: {format}");
            }
            await index.WaitForTaskAsync(task.Uid);
            task.Uid.Should().BeGreaterOrEqualTo(0);

            // Check the primary key has been set
            await index.FetchPrimaryKey();
            Assert.Equal("key", index.PrimaryKey);
        }

        [Theory]
        [InlineData("json")]
        public async Task BasicDocumentsUpdate(string format)
        {
            var indexUID = $"BasicDocumentsUpdateTest_{format}";
            var index = _client.Index(indexUID);

            // Add the documents
            TaskInfo task;
            switch (format)
            {
                case "json":
                    task = await index.AddDocumentsJsonAsync(new[]
                    {
                        new Movie { Id = "1", Name = "Batman", Genre = "Action" },
                        new Movie { Id = "2", Name = "Superman" },
                    });
                    break;
                default:
                    throw new System.IndexOutOfRangeException($"Unsupported format: {format}");
            }
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.Uid);

            // Update the documents
            switch (format)
            {
                case "json":
                    task = await index.UpdateDocumentsJsonAsync(new[] { new Movie { Id = "1", Name = "Ironman" } });
                    break;
                default:
                    throw new System.IndexOutOfRangeException($"Unsupported format: {format}");
            }

            task.Uid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.Uid);

            // Check the documents have been updated and added
            var docs = await index.GetDocumentsAsync<Movie>();
            Assert.Equal("1", docs.First().Id);
            Assert.Equal("Ironman", docs.First().Name);
            Assert.Null(docs.First().Genre);

            Assert.Equal("2", docs.ElementAt(1).Id);
            Assert.Equal("Superman", docs.ElementAt(1).Name);
            docs.ElementAt(1).Genre.Should().BeNull();
        }

        [Theory]
        [InlineData("json")]
        public async Task BasicDocumentsUpdateInBatches(string format)
        {
            var indexUID = $"BasicDocumentsUpdateInBatchesTest_{format}";
            var index = _client.Index(indexUID);

            // Add the documents
            Movie[] movies =
            {
                new Movie { Id = "1", Name = "Batman" },
                new Movie { Id = "2", Name = "Reservoir Dogs" },
                new Movie { Id = "3", Name = "Taxi Driver" },
                new Movie { Id = "4", Name = "Interstellar" },
                new Movie { Id = "5", Name = "Titanic" },
            };

            IEnumerable<TaskInfo> tasks;
            switch (format)
            {
                case "json":
                    tasks = await index.AddDocumentsJsonInBatchesAsync(movies, 2);
                    break;
                default:
                    throw new System.IndexOutOfRangeException($"Unsupported format: {format}");
            }

            foreach (var u in tasks)
            {
                u.Uid.Should().BeGreaterOrEqualTo(0);
                await index.WaitForTaskAsync(u.Uid);
            }

            movies = new Movie[]
            {
                new Movie { Id = "1", Name = "Batman", Genre = "Action" },
                new Movie { Id = "2", Name = "Reservoir Dogs", Genre = "Drama" },
                new Movie { Id = "3", Name = "Taxi Driver", Genre = "Drama" },
                new Movie { Id = "4", Name = "Interstellar", Genre = "Sci-Fi" },
                new Movie { Id = "5", Name = "Titanic", Genre = "Drama" },
            };
            switch (format)
            {
                case "json":
                    tasks = await index.UpdateDocumentsJsonInBatchesAsync(movies, 2);
                    break;
                default:
                    throw new System.IndexOutOfRangeException($"Unsupported format: {format}");
            }
            foreach (var u in tasks)
            {
                u.Uid.Should().BeGreaterOrEqualTo(0);
                await index.WaitForTaskAsync(u.Uid);
            }

            // Assert movies have genre after updating
            var docs = (await index.GetDocumentsAsync<Movie>()).ToList();
            foreach (var movie in docs)
            {
                movie.Genre.Should().NotBeNull();
                movie.Genre.Should().NotBeEmpty();
            }
        }

        [Theory]
        [InlineData("json")]
        public async Task DocumentsUpdateWithPrimaryKey(string format)
        {
            var indexUid = $"DocumentsUpdateWithPrimaryKeyTest_{format}";
            var index = _client.Index(indexUid);
            index.PrimaryKey.Should().BeNull();

            // Add the documents
            TaskInfo task;
            switch (format)
            {
                case "json":
                    task = await index.UpdateDocumentsJsonAsync(new[] { new { Key = "1", Name = "Ironman" } }, "key");
                    break;
                default:
                    throw new System.IndexOutOfRangeException($"Unsupported format: {format}");
            }
            task = await index.WaitForTaskAsync(task.Uid);
            task.Uid.Should().BeGreaterOrEqualTo(0);

            // Check the primary key has been set
            await index.FetchPrimaryKey();
            Assert.Equal("key", index.PrimaryKey);
        }

        [Fact]
        public async Task GetOneExistingDocumentWithStringId()
        {
            var index = await _fixture.SetUpBasicIndex("GetOneExistingDocumentWithStringIdTest");
            var documents = await index.GetDocumentAsync<Movie>("10");
            documents.Id.Should().Be("10");
        }

        [Fact]
        public async Task GetOneExistingDocumentWithIntegerId()
        {
            var index = await _fixture.SetUpBasicIndexWithIntId("GetOneExistingDocumentWithIntegerIdTest");
            var documents = await index.GetDocumentAsync<MovieWithIntId>(10);
            documents.Id.Should().Be(10);
        }

        [Fact]
        public async Task GetMultipleExistingDocuments()
        {
            var index = await _fixture.SetUpBasicIndex("GetMultipleExistingDocumentTest");
            var documents = await index.GetDocumentsAsync<Movie>();
            Assert.Equal(7, documents.Count());
            documents.First().Id.Should().Be("10");
            documents.Last().Id.Should().Be("16");
        }

        [Fact]
        public async Task GetMultipleExistingDocumentsWithLimit()
        {
            var index = await _fixture.SetUpBasicIndex("GetMultipleExistingDocumentWithLimitTest");
            var documents = await index.GetDocumentsAsync<Movie>(new DocumentQuery() { Limit = 2 });
            Assert.Equal(2, documents.Count());
            documents.First().Id.Should().Be("10");
            documents.Last().Id.Should().Be("11");
        }

        [Fact]
        public async Task DeleteOneExistingDocumentWithStringId()
        {
            var index = await _fixture.SetUpBasicIndex("DeleteOneExistingDocumentWithStringIdTest");

            // Delete the document
            var task = await index.DeleteOneDocumentAsync("11");
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.Uid);

            // Check the document has been deleted
            var docs = await index.GetDocumentsAsync<Movie>();
            Assert.Equal(6, docs.Count());
            var ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<Movie>("11"));
            Assert.Equal("document_not_found", ex.Code);
        }

        [Fact]
        public async Task DeleteOneExistingDocumentWithIntId()
        {
            var index = await _fixture.SetUpBasicIndexWithIntId("DeleteOneExistingDocumentWithIntIdTest");

            // Delete the document
            var task = await index.DeleteOneDocumentAsync(11);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.Uid);

            // Check the document has been deleted
            var docs = await index.GetDocumentsAsync<MovieWithIntId>();
            Assert.Equal(6, docs.Count());
            var ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<MovieWithIntId>(11));
            Assert.Equal("document_not_found", ex.Code);
        }

        [Fact]
        public async Task DeleteMultipleDocumentsWithStringId()
        {
            var index = await _fixture.SetUpBasicIndex("DeleteMultipleDocumentsWithStringIdTest");

            // Delete the documents
            var task = await index.DeleteDocumentsAsync(new[] { "12", "13", "14" });
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.Uid);

            // Check the documents have been deleted
            var docs = await index.GetDocumentsAsync<Movie>();
            Assert.Equal(4, docs.Count());
            MeilisearchApiError ex;
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<Movie>("12"));
            Assert.Equal("document_not_found", ex.Code);
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<Movie>("13"));
            Assert.Equal("document_not_found", ex.Code);
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<Movie>("14"));
            Assert.Equal("document_not_found", ex.Code);
        }

        [Fact]
        public async Task DeleteMultipleDocumentsWithIntegerId()
        {
            var index = await _fixture.SetUpBasicIndexWithIntId("DeleteMultipleDocumentsWithIntegerIdTest");

            // Delete the documents
            var task = await index.DeleteDocumentsAsync(new[] { 12, 13, 14 });
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.Uid);

            // Check the documents have been deleted
            var docs = await index.GetDocumentsAsync<MovieWithIntId>();
            Assert.Equal(4, docs.Count());
            MeilisearchApiError ex;
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<MovieWithIntId>("12"));
            Assert.Equal("document_not_found", ex.Code);
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<MovieWithIntId>("13"));
            Assert.Equal("document_not_found", ex.Code);
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<MovieWithIntId>("14"));
            Assert.Equal("document_not_found", ex.Code);
        }

        [Fact]
        public async Task DeleteAllExistingDocuments()
        {
            var index = await _fixture.SetUpBasicIndex("DeleteAllExistingDocumentsTest");

            // Delete all the documents
            var task = await index.DeleteAllDocumentsAsync();
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.Uid);

            // Check all the documents have been deleted
            var docs = await index.GetDocumentsAsync<Movie>();
            docs.Should().BeEmpty();
        }
    }
}

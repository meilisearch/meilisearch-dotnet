using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Xunit;

namespace Meilisearch.Tests
{
    [Collection("Sequential")]
    public class DocumentTests : IAsyncLifetime
    {
        private readonly MeilisearchClient _client;

        private readonly IndexFixture _fixture;

        public DocumentTests(IndexFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.DefaultClient;
        }

        public async Task InitializeAsync() => await _fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task BasicDocumentsAddition()
        {
            var indexUID = "BasicDocumentsAdditionTest";
            var index = _client.Index(indexUID);

            // Add the documents
            var task = await index.AddDocumentsAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.Uid);

            // Check the documents have been added
            var docs = await index.GetDocumentsAsync<Movie>();
            Assert.Equal("1", docs.First().Id);
            Assert.Equal("Batman", docs.First().Name);
            docs.First().Genre.Should().BeNull();
        }

        [Fact]
        public async Task BasicDocumentsAdditionInBatches()
        {
            var indexUID = "BasicDocumentsAdditionInBatchesTest";
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
            var tasks = await index.AddDocumentsInBatchesAsync(movies, 2);
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

        [Fact]
        public async Task BasicDocumentsAdditionWithAlreadyCreatedIndex()
        {
            var indexUid = "BasicDocumentsAdditionWithAlreadyCreatedIndexTest";
            var task = await _client.CreateIndexAsync(indexUid);
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await _client.Index(indexUid).WaitForTaskAsync(task.Uid);

            // Add the documents
            var index = _client.Index(indexUid);
            task = await index.AddDocumentsAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.Uid);

            // Check the documents have been added
            var docs = await index.GetDocumentsAsync<Movie>();
            Assert.Equal("1", docs.First().Id);
            Assert.Equal("Batman", docs.First().Name);
            docs.First().Genre.Should().BeNull();
        }

        [Fact]
        public async Task BasicDocumentsAdditionWithTimeoutError()
        {
            var indexUID = "BasicDocumentsAdditionWithTimeoutError";
            var index = _client.Index(indexUID);

            // Add the documents
            var task = await index.AddDocumentsAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => index.WaitForTaskAsync(task.Uid, 0));
        }

        [Fact]
        public async Task BasicDocumentsAdditionWithTimeoutErrorByInterval()
        {
            var indexUID = "BasicDocumentsAdditionWithTimeoutErrorByIntervalTest";
            var index = _client.Index(indexUID);

            // Add the documents
            var task = await index.AddDocumentsAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => index.WaitForTaskAsync(task.Uid, 0, 10));
        }

        [Fact]
        public async Task DocumentsAdditionWithPrimaryKey()
        {
            var indexUid = "DocumentsAdditionWithPrimaryKeyTest";
            var index = _client.Index(indexUid);
            index.PrimaryKey.Should().BeNull();

            // Add the documents
            var task = await index.AddDocumentsAsync(new[] { new { Key = "1", Name = "Ironman" } }, "key");
            await index.WaitForTaskAsync(task.Uid);
            task.Uid.Should().BeGreaterOrEqualTo(0);

            // Check the primary key has been set
            await index.FetchPrimaryKey();
            Assert.Equal("key", index.PrimaryKey);
        }

        [Fact]
        public async Task BasicDocumentsUpdate()
        {
            var indexUID = "BasicDocumentsUpdateTest";
            var index = _client.Index(indexUID);

            // Add the documents
            var task = await index.AddDocumentsAsync(new[]
            {
                new Movie { Id = "1", Name = "Batman", Genre = "Action" },
                new Movie { Id = "2", Name = "Superman" },
            });
            task.Uid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.Uid);

            // Update the documents
            task = await index.UpdateDocumentsAsync(new[] { new Movie { Id = "1", Name = "Ironman" } });
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

        [Fact]
        public async Task BasicDocumentsUpdateInBatches()
        {
            var indexUID = "BasicDocumentsUpdateInBatchesTest";
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
            var tasks = await index.AddDocumentsInBatchesAsync(movies, 2);
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
            tasks = await index.UpdateDocumentsInBatchesAsync(movies, 2);
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

        [Fact]
        public async Task DocumentsUpdateWithPrimaryKey()
        {
            var indexUid = "DocumentsUpdateWithPrimaryKeyTest";
            var index = _client.Index(indexUid);
            index.PrimaryKey.Should().BeNull();

            // Add the documents
            var task = await index.UpdateDocumentsAsync(new[] { new { Key = "1", Name = "Ironman" } }, "key");
            await index.WaitForTaskAsync(task.Uid);
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

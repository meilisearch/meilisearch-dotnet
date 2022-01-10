namespace Meilisearch.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    [Collection("Sequential")]
    public class DocumentTests : IAsyncLifetime
    {
        private readonly MeilisearchClient client;

        private IndexFixture fixture;

        public DocumentTests(IndexFixture fixture)
        {
            this.fixture = fixture;
            this.client = fixture.DefaultClient;
        }

        public async Task InitializeAsync() => await this.fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]

        public Task DisposeAsync() => Task.CompletedTask;

        [Theory]
        [InlineData("json")]
        public async Task BasicDocumentsAddition(string format)
        {
            var indexUID = "BasicDocumentsAdditionTest";
            Index index = this.client.Index(indexUID);

            // Add the documents
            UpdateStatus update = format switch
            {
                "json" => await index.AddDocumentsJsonAsync(new[] { new Movie { Id = "1", Name = "Batman" } }),
                _ => throw new System.IndexOutOfRangeException($"Unsupported format: {format}")
            };

            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdateAsync(update.UpdateId);

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
            var indexUID = "BasicDocumentsAdditionInBatchesTest";
            Index index = this.client.Index(indexUID);

            // Add the documents
            Movie[] movies =
            {
                new Movie { Id = "1", Name = "Batman" },
                new Movie { Id = "2", Name = "Reservoir Dogs" },
                new Movie { Id = "3", Name = "Taxi Driver" },
                new Movie { Id = "4", Name = "Interstellar" },
                new Movie { Id = "5", Name = "Titanic" },
            };

            IEnumerable<UpdateStatus> updates = format switch
            {
                "json" => await index.AddDocumentsJsonInBatchesAsync(movies, 2),
                _ => throw new System.IndexOutOfRangeException($"Unsupported format: {format}")
            };

            foreach (var u in updates)
            {
                u.UpdateId.Should().BeGreaterOrEqualTo(0);
                await index.WaitForPendingUpdateAsync(u.UpdateId);
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
        public async Task BasicDocumentsAdditionWithCreateIndex(string format)
        {
            var indexUID = "BasicDocumentsAdditionWithCreateIndexTest";
            Index index = await this.client.CreateIndexAsync(indexUID);

            // Add the documents
            UpdateStatus update = format switch
            {
                "json" => await index.AddDocumentsJsonAsync(new[] { new Movie { Id = "1", Name = "Batman" } }),
                _ => throw new System.IndexOutOfRangeException($"Unsupported format: {format}")
            };

            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdateAsync(update.UpdateId);

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
            var indexUID = "BasicDocumentsAdditionWithTimeoutError";
            Index index = await this.client.GetOrCreateIndexAsync(indexUID);

            // Add the documents
            UpdateStatus update = format switch
            {
                "json" => await index.AddDocumentsJsonAsync(new[] { new Movie { Id = "1", Name = "Batman" } }),
                _ => throw new System.IndexOutOfRangeException($"Unsupported format: {format}")
            };

            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => index.WaitForPendingUpdateAsync(update.UpdateId, 0));
        }

        [Theory]
        [InlineData("json")]
        public async Task BasicDocumentsAdditionWithTimeoutErrorByInterval(string format)
        {
            var indexUID = "BasicDocumentsAdditionWithTimeoutErrorByIntervalTest";
            Index index = await this.client.GetOrCreateIndexAsync(indexUID);

            // Add the documents
            UpdateStatus update = format switch
            {
                "json" => await index.AddDocumentsJsonAsync(new[] { new Movie { Id = "1", Name = "Batman" } }),
                _ => throw new System.IndexOutOfRangeException($"Unsupported format: {format}")
            };

            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => index.WaitForPendingUpdateAsync(update.UpdateId, 0, 10));
        }

        [Theory]
        [InlineData("json")]
        public async Task DocumentsAdditionWithPrimaryKey(string format)
        {
            var indexUid = "DocumentsAdditionWithPrimaryKeyTest";
            var index = this.client.Index(indexUid);
            index.PrimaryKey.Should().BeNull();

            // Add the documents
            UpdateStatus update = format switch
            {
                "json" => await index.AddDocumentsJsonAsync(new[] { new { Key = "1", Name = "Ironman" } }, "key"),
                _ => throw new System.IndexOutOfRangeException($"Unsupported format: {format}")
            };

            await index.WaitForPendingUpdateAsync(update.UpdateId);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);

            // Check the primary key has been set
            await index.FetchPrimaryKey();
            Assert.Equal("key", index.PrimaryKey);
        }

        [Theory]
        [InlineData("json")]
        public async Task BasicDocumentsUpdate(string format)
        {
            var indexUID = "BasicDocumentsUpdateTest";
            Index index = this.client.Index(indexUID);

            // Add the documents
            UpdateStatus update = format switch
            {
                "json" => await index.AddDocumentsJsonAsync(new[]
                {
                    new Movie { Id = "1", Name = "Batman", Genre = "Action" },
                    new Movie { Id = "2", Name = "Superman" },
                }),
                _ => throw new System.IndexOutOfRangeException($"Unsupported format: {format}")
            };

            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdateAsync(update.UpdateId);

            // Update the documents
            update = format switch
            {
                "json" => await index.UpdateDocumentsJsonAsync(new[] { new Movie { Id = "1", Name = "Ironman" } }),
                _ => throw new System.IndexOutOfRangeException($"Unsupported format: {format}")
            };
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdateAsync(update.UpdateId);

            // Check the documents have been updated and added
            var docs = await index.GetDocumentsAsync<Movie>();
            Assert.Equal("1", docs.First().Id);
            Assert.Equal("Ironman", docs.First().Name);

            Assert.Equal("Action", docs.First().Genre);
            Assert.Equal("2", docs.ElementAt(1).Id);
            Assert.Equal("Superman", docs.ElementAt(1).Name);
            docs.ElementAt(1).Genre.Should().BeNull();
        }

        [Theory]
        [InlineData("json")]
        public async Task BasicDocumentsUpdateInBatches(string format)
        {
            var indexUID = "BasicDocumentsUpdateInBatchesTest";
            Index index = this.client.Index(indexUID);

            // Add the documents
            Movie[] movies =
            {
                new Movie { Id = "1", Name = "Batman" },
                new Movie { Id = "2", Name = "Reservoir Dogs" },
                new Movie { Id = "3", Name = "Taxi Driver" },
                new Movie { Id = "4", Name = "Interstellar" },
                new Movie { Id = "5", Name = "Titanic" },
            };
            IEnumerable<UpdateStatus> updates = format switch
            {
                "json" => await index.AddDocumentsJsonInBatchesAsync(movies, 2),
                _ => throw new System.IndexOutOfRangeException($"Unsupported format: {format}")
            };

            foreach (var u in updates)
            {
                u.UpdateId.Should().BeGreaterOrEqualTo(0);
                await index.WaitForPendingUpdateAsync(u.UpdateId);
            }

            movies = new Movie[]
            {
                new Movie { Id = "1", Name = "Batman", Genre = "Action" },
                new Movie { Id = "2", Name = "Reservoir Dogs", Genre = "Drama" },
                new Movie { Id = "3", Name = "Taxi Driver", Genre = "Drama" },
                new Movie { Id = "4", Name = "Interstellar", Genre = "Sci-Fi" },
                new Movie { Id = "5", Name = "Titanic", Genre = "Drama" },
            };
            updates = format switch
            {
                "json" => await index.UpdateDocumentsJsonInBatchesAsync(movies, 2),
                _ => throw new System.IndexOutOfRangeException($"Unsupported format: {format}")
            };

            foreach (var u in updates)
            {
                u.UpdateId.Should().BeGreaterOrEqualTo(0);
                await index.WaitForPendingUpdateAsync(u.UpdateId);
            }

            // Assert movies have genre after update
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
            var indexUid = "DocumentsUpdateWithPrimaryKeyTest";
            var index = this.client.Index(indexUid);
            index.PrimaryKey.Should().BeNull();

            // Add the documents
            var update = format switch
            {
                "json" => await index.UpdateDocumentsJsonAsync(new[] { new { Key = "1", Name = "Ironman" } }, "key"),
                _ => throw new System.IndexOutOfRangeException($"Unsupported format: {format}")
            };

            await index.WaitForPendingUpdateAsync(update.UpdateId);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);

            // Check the primary key has been set
            await index.FetchPrimaryKey();
            Assert.Equal("key", index.PrimaryKey);
        }

        [Fact]
        public async Task GetOneExistingDocumentWithStringId()
        {
            Index index = await this.fixture.SetUpBasicIndex("GetOneExistingDocumentWithStringIdTest");
            var documents = await index.GetDocumentAsync<Movie>("10");
            documents.Id.Should().Be("10");
        }

        [Fact]
        public async Task GetOneExistingDocumentWithIntegerId()
        {
            Index index = await this.fixture.SetUpBasicIndexWithIntId("GetOneExistingDocumentWithIntegerIdTest");
            var documents = await index.GetDocumentAsync<MovieWithIntId>(10);
            documents.Id.Should().Be(10);
        }

        [Fact]
        public async Task GetMultipleExistingDocuments()
        {
            Index index = await this.fixture.SetUpBasicIndex("GetMultipleExistingDocumentTest");
            var documents = await index.GetDocumentsAsync<Movie>();
            Assert.Equal(7, documents.Count());
            documents.First().Id.Should().Be("10");
            documents.Last().Id.Should().Be("16");
        }

        [Fact]
        public async Task GetMultipleExistingDocumentsWithLimit()
        {
            Index index = await this.fixture.SetUpBasicIndex("GetMultipleExistingDocumentWithLimitTest");
            var documents = await index.GetDocumentsAsync<Movie>(new DocumentQuery() { Limit = 2 });
            Assert.Equal(2, documents.Count());
            documents.First().Id.Should().Be("10");
            documents.Last().Id.Should().Be("11");
        }

        [Fact]
        public async Task DeleteOneExistingDocumentWithStringId()
        {
            Index index = await this.fixture.SetUpBasicIndex("DeleteOneExistingDocumentWithStringIdTest");

            // Delete the document
            UpdateStatus update = await index.DeleteOneDocumentAsync("11");
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdateAsync(update.UpdateId);

            // Check the document has been deleted
            var docs = await index.GetDocumentsAsync<Movie>();
            Assert.Equal(6, docs.Count());
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<Movie>("11"));
            Assert.Equal("document_not_found", ex.Code);
        }

        [Fact]
        public async Task DeleteOneExistingDocumentWithIntId()
        {
            Index index = await this.fixture.SetUpBasicIndexWithIntId("DeleteOneExistingDocumentWithIntIdTest");

            // Delete the document
            UpdateStatus update = await index.DeleteOneDocumentAsync(11);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdateAsync(update.UpdateId);

            // Check the document has been deleted
            var docs = await index.GetDocumentsAsync<MovieWithIntId>();
            Assert.Equal(6, docs.Count());
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<MovieWithIntId>(11));
            Assert.Equal("document_not_found", ex.Code);
        }

        [Fact]
        public async Task DeleteMultipleDocumentsWithStringId()
        {
            Index index = await this.fixture.SetUpBasicIndex("DeleteMultipleDocumentsWithStringIdTest");

            // Delete the documents
            UpdateStatus update = await index.DeleteDocumentsAsync(new[] { "12", "13", "14" });
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdateAsync(update.UpdateId);

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
            Index index = await this.fixture.SetUpBasicIndexWithIntId("DeleteMultipleDocumentsWithIntegerIdTest");

            // Delete the documents
            UpdateStatus update = await index.DeleteDocumentsAsync(new[] { 12, 13, 14 });
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdateAsync(update.UpdateId);

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
            Index index = await this.fixture.SetUpBasicIndex("DeleteAllExistingDocumentsTest");

            // Delete all the documents
            UpdateStatus update = await index.DeleteAllDocumentsAsync();
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdateAsync(update.UpdateId);

            // Check all the documents have been deleted
            var docs = await index.GetDocumentsAsync<Movie>();
            docs.Should().BeEmpty();
        }
    }
}

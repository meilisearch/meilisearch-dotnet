namespace Meilisearch.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    [Collection("Sequential")]
    public class DocumentTests
    {
        private readonly MeilisearchClient client;
        private readonly IndexFixture fixture;

        public DocumentTests(IndexFixture fixture)
        {
            fixture.DeleteAllIndexes().Wait(); // Context test cleaned for each [Fact]
            this.fixture = fixture;
            this.client = fixture.DefaultClient;
        }

        [Fact]
        public async Task BasicDocumentsAddition()
        {
            var indexUID = "BasicDocumentsAdditionTest";
            Index index = this.client.Index(indexUID);

            // Add the documents
            UpdateStatus update = await index.AddDocuments(new[] { new Movie { Id = "1", Name = "Batman" } });
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdate(update.UpdateId);

            // Check the documents have been added
            var docs = await index.GetDocuments<Movie>();
            Assert.Equal("1", docs.First().Id);
            Assert.Equal("Batman", docs.First().Name);
            docs.First().Genre.Should().BeNull();
        }

        [Fact]
        public async Task BasicDocumentsAdditionWithCreateIndex()
        {
            var indexUID = "BasicDocumentsAdditionTest";
            Index index = await this.client.CreateIndex(indexUID);

            // Add the documents
            UpdateStatus update = await index.AddDocuments(new[] { new Movie { Id = "1", Name = "Batman" } });
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdate(update.UpdateId);

            // Check the documents have been added
            var docs = await index.GetDocuments<Movie>();
            Assert.Equal("1", docs.First().Id);
            Assert.Equal("Batman", docs.First().Name);
            docs.First().Genre.Should().BeNull();
        }

        [Fact]
        public async Task BasicDocumentsAdditionWithTimeoutError()
        {
            var indexUID = "BasicDocumentsAdditionWithTimeoutError";
            Index index = await this.client.GetOrCreateIndex(indexUID);

            // Add the documents
            UpdateStatus update = await index.AddDocuments(new[] { new Movie { Id = "1", Name = "Batman" } });
            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => index.WaitForPendingUpdate(update.UpdateId, 0));
        }

        [Fact]
        public async Task BasicDocumentsAdditionWithTimeoutErrorByInterval()
        {
            var indexUID = "BasicDocumentsAdditionWithTimeoutError";
            Index index = await this.client.GetOrCreateIndex(indexUID);

            // Add the documents
            UpdateStatus update = await index.AddDocuments(new[] { new Movie { Id = "1", Name = "Batman" } });
            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => index.WaitForPendingUpdate(update.UpdateId, 0, 10));
        }

        [Fact]
        public async Task BasicDocumentsUpdate()
        {
            var indexUID = "BasicDocumentsUpdateTest";
            Index index = this.client.Index(indexUID);

            // Add the documents
            UpdateStatus update = await index.AddDocuments(new[]
            {
                new Movie { Id = "1", Name = "Batman", Genre = "Action" },
                new Movie { Id = "2", Name = "Superman" },
            });
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdate(update.UpdateId);

            // Update the documents
            update = await index.UpdateDocuments(new[] { new Movie { Id = "1", Name = "Ironman" } });
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdate(update.UpdateId);

            // Check the documents have been updated and added
            var docs = await index.GetDocuments<Movie>();
            Assert.Equal("1", docs.First().Id);
            Assert.Equal("Ironman", docs.First().Name);

            // Assert.Equal("Action", docs.First().Genre); // Commented until the issue #67 is fixed (https://github.com/meilisearch/meilisearch-dotnet/issues/67)
            Assert.Equal("2", docs.ElementAt(1).Id);
            Assert.Equal("Superman", docs.ElementAt(1).Name);
            docs.ElementAt(1).Genre.Should().BeNull();
        }

        [Fact]
        public async Task GetOneExistingDocumentWithStringId()
        {
            Index index = await this.fixture.SetUpBasicIndex("GetOneExistingDocumentWithStringIdTest");
            var documents = await index.GetDocument<Movie>("10");
            documents.Id.Should().Be("10");
        }

        [Fact]
        public async Task GetOneExistingDocumentWithIntegerId()
        {
            Index index = await this.fixture.SetUpBasicIndexWithIntId("GetOneExistingDocumentWithIntegerIdTest");
            var documents = await index.GetDocument<MovieWithIntId>(10);
            documents.Id.Should().Be(10);
        }

        [Fact]
        public async Task GetMultipleExistingDocuments()
        {
            Index index = await this.fixture.SetUpBasicIndex("GetMultipleExistingDocumentTest");
            var documents = await index.GetDocuments<Movie>();
            Assert.Equal(7, documents.Count());
            documents.First().Id.Should().Be("10");
            documents.Last().Id.Should().Be("16");
        }

        [Fact]
        public async Task GetMultipleExistingDocumentsWithLimit()
        {
            Index index = await this.fixture.SetUpBasicIndex("GetMultipleExistingDocumentWithLimitTest");
            var documents = await index.GetDocuments<Movie>(new DocumentQuery() { Limit = 2 });
            Assert.Equal(2, documents.Count());
            documents.First().Id.Should().Be("10");
            documents.Last().Id.Should().Be("11");
        }

        [Fact]
        public async Task DeleteOneExistingDocumentWithStringId()
        {
            Index index = await this.fixture.SetUpBasicIndex("DeleteOneExistingDocumentWithStringIdTest");

            // Delete the document
            UpdateStatus update = await index.DeleteOneDocument("11");
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdate(update.UpdateId);

            // Check the document has been deleted
            var docs = await index.GetDocuments<Movie>();
            Assert.Equal(6, docs.Count());
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocument<Movie>("11"));
            Assert.Equal("document_not_found", ex.ErrorCode);
        }

        [Fact]
        public async Task DeleteOneExistingDocumentWithIntId()
        {
            Index index = await this.fixture.SetUpBasicIndexWithIntId("DeleteOneExistingDocumentWithIntIdTest");

            // Delete the document
            UpdateStatus update = await index.DeleteOneDocument(11);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdate(update.UpdateId);

            // Check the document has been deleted
            var docs = await index.GetDocuments<MovieWithIntId>();
            Assert.Equal(6, docs.Count());
            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocument<MovieWithIntId>(11));
            Assert.Equal("document_not_found", ex.ErrorCode);
        }

        [Fact]
        public async Task DeleteMultipleDocumentsWithStringId()
        {
            Index index = await this.fixture.SetUpBasicIndex("DeleteMultipleDocumentsWithStringIdTest");

            // Delete the documents
            UpdateStatus update = await index.DeleteDocuments(new[] { "12", "13", "14" });
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdate(update.UpdateId);

            // Check the documents have been deleted
            var docs = await index.GetDocuments<Movie>();
            Assert.Equal(4, docs.Count());
            MeilisearchApiError ex;
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocument<Movie>("12"));
            Assert.Equal("document_not_found", ex.ErrorCode);
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocument<Movie>("13"));
            Assert.Equal("document_not_found", ex.ErrorCode);
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocument<Movie>("14"));
            Assert.Equal("document_not_found", ex.ErrorCode);
        }

        [Fact]
        public async Task DeleteMultipleDocumentsWithIntegerId()
        {
            Index index = await this.fixture.SetUpBasicIndexWithIntId("DeleteMultipleDocumentsWithIntegerIdTest");

            // Delete the documents
            UpdateStatus update = await index.DeleteDocuments(new[] { 12, 13, 14 });
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdate(update.UpdateId);

            // Check the documents have been deleted
            var docs = await index.GetDocuments<MovieWithIntId>();
            Assert.Equal(4, docs.Count());
            MeilisearchApiError ex;
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocument<MovieWithIntId>("12"));
            Assert.Equal("document_not_found", ex.ErrorCode);
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocument<MovieWithIntId>("13"));
            Assert.Equal("document_not_found", ex.ErrorCode);
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocument<MovieWithIntId>("14"));
            Assert.Equal("document_not_found", ex.ErrorCode);
        }

        [Fact]
        public async Task DeleteAllExistingDocuments()
        {
            Index index = await this.fixture.SetUpBasicIndex("DeleteMultipleDocumentsWithIntegerIdTest");

            // Delete all the documents
            UpdateStatus update = await index.DeleteAllDocuments();
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdate(update.UpdateId);

            // Check all the documents have been deleted
            var docs = await index.GetDocuments<Movie>();
            docs.Should().BeEmpty();
        }
    }
}

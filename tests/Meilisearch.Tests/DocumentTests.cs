namespace Meilisearch.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    [Collection("Sequential")]
    public class DocumentTests : IClassFixture<DocumentFixture>
    {
        private readonly Index index;
        private readonly Index indextoDelete;

        public DocumentTests(DocumentFixture fixture)
        {
            this.index = fixture.DocumentsIndex;
            this.indextoDelete = fixture.DocumentDeleteIndex;
        }

        [Fact]
        public async Task BasicDocumentsAddition()
        {
            var updateStatus = await this.index.AddDocuments(new[] { new Movie { Id = "1", Name = "Batman" } });
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task BasicDocumentsUpdate()
        {
            var updateStatus = await this.index.UpdateDocuments(new[] { new Movie { Id = "1", Name = "Batman" } });
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task GetOneExistingDocumentWithStringId()
        {
            var documents = await this.index.GetDocument<Movie>("10");
            documents.Id.Should().Be("10");
        }

        [Fact]
        public async Task GetOneExistingDocumentWithIntegerId()
        {
            var documents = await this.index.GetDocument<Movie>(10);
            documents.Id.Should().Be("10");
        }

        [Fact]
        public async Task GetMultipleExistingDocumentWithLimit()
        {
            var documents = await this.index.GetDocuments<Movie>(new DocumentQuery { Limit = 1 });
            documents.Count().Should().Be(1);
        }

        [Fact]
        public async Task DeleteOneExistingDocumentWithStringId()
        {
            var updateStatus = await this.index.DeleteOneDocument("11");
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task DeleteOneExistingDocumentWithIntegerId()
        {
            var updateStatus = await this.index.DeleteOneDocument(11);
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task DeleteMultipleDocumentsWithStringId()
        {
            var updateStatus = await this.index.DeleteDocuments(new[] { "12", "13", "14" });
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task DeleteMultipleDocumentsWithIntegerId()
        {
            var updateStatus = await this.index.DeleteDocuments(new[] { 12, 13, 14 });
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task DeleteAllExistingDocuments()
        {
            var updateStatus = await this.indextoDelete.DeleteAllDocuments();
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
        }
    }

    public class Movie
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Genre { get; set; }
    }

    public class FormattedMovie
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Genre { get; set; }

        #pragma warning disable SA1300
        public Movie _Formatted { get; set; }
    }
}

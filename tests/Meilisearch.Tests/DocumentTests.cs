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
        public async Task Should_be_Able_to_Add_Document_for_Index()
        {
            var updateStatus = await this.index.AddDocuments(new[] { new Movie { Id = "1", Name = "Batman" } });
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task Should_be_Able_to_Update_Document_for_Index()
        {
            var updateStatus = await this.index.UpdateDocuments(new[] { new Movie { Id = "1", Name = "Batman" } });
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task Should_be_Able_to_Get_One_Document_With_Id()
        {
            var documents = await this.index.GetDocument<Movie>("10");
            documents.Id.Should().Be("10");
        }

        [Fact]
        public async Task Should_be_Able_to_Get_One_Document_With_Int_Id()
        {
            var documents = await this.index.GetDocument<Movie>(10);
            documents.Id.Should().Be("10");
        }

        [Fact]
        public async Task Should_Be_able_to_get_Many_documents_By_Limit()
        {
            var documents = await this.index.GetDocuments<Movie>(new DocumentQuery { Limit = 1 });
            documents.Count().Should().Be(1);
        }

        [Fact]
        public async Task Should_be_Able_to_Delete_one_document()
        {
            var updateStatus = await this.index.DeleteOneDocument("11");
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task Should_be_Able_to_Delete_one_document_with_int_id()
        {
            var updateStatus = await this.index.DeleteOneDocument(11);
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task Should_be_Able_to_Delete_documents_by_ids()
        {
            var updateStatus = await this.index.DeleteDocuments(new[] { "12", "13", "14" });
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task Should_be_Able_to_Delete_documents_by_int_ids()
        {
            var updateStatus = await this.index.DeleteDocuments(new[] { 12, 13, 14 });
            updateStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task Should_Be_Delete_All_Documents()
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

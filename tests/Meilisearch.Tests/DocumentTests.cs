using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Meilisearch.Tests
{
    public class DocumentTests : IClassFixture<DocumentFixture>
    {
        private readonly Index index;

        public DocumentTests(DocumentFixture fixture)
        {
            index = fixture.documentIndex;
        }
       
        [Fact]
        public async Task Should_be_Able_to_add_Document_for_Index()
        {
           var updateStatus = await index.AddorUpdateDocuments(new[]{new  Movie {Id = "1", Name = "Batman"}});
           updateStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task Should_be_Able_to_Get_One_Document_With_Id()
        {
            var documents = await index.GetDocument<Movie>("10");
            documents.Id.Should().Be("10");
        }
        
        [Fact]
        public async Task Should_Be_able_to_get_Many_documents_By_Limit()
        {
            var documents = await index.GetDocuments<Movie>(new DocumentQuery {Limit = 1});
            documents.Count().Should().Be(1);
        }
        
        /* Future test .
        public async Task Should_Be_Delete_All_Documents()
        {
            
        }

        public async Task Should_be_Able_to_Delete_documents_by_ids()
        {
            
        }

        public async Task Should_be_Able_to_Delete_one_document()
        {
            
        } */
    }

    public class Movie
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
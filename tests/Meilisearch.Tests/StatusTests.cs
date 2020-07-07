using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Http;
using Xunit;

namespace Meilisearch.Tests
{
    [Collection("Sequential")]

    public class StatusTests
    {

        public StatusTests()
        {

        }

        [Fact]
        public async Task Should_be_Able_to_get_All_The_Update_Status()
        {
            var client = new MeilisearchClient("http://localhost:7700", "masterKey");
            var indexName = "MoviesStatus" + new Random().Next();
            var index = await  client.CreateIndex(indexName);
            await index.AddDocuments(new[] {new Movie {Id = "1"}});
            var status = await index.GetAllUpdateStatus();
            status.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task Should_be_Able_to_Get_Status_By_Id()
        {
            var client = new MeilisearchClient("http://localhost:7700", "masterKey");
            var indexName = "MoviesStatus" + new Random().Next();
            var index = await  client.CreateIndex(indexName);
            var status = await index.AddDocuments(new[] {new Movie {Id = "2"}});
            UpdateStatus individualStatus = await index.GetUpdateStatus(status.UpdateId);
            individualStatus.Should().NotBeNull();
        }
    }
}

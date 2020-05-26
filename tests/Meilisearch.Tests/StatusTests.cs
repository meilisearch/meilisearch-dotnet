using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Meilisearch.Tests
{

    public class StatusTests 
    {
        private static HttpClient _httpClient = new HttpClient
        {
            // TODO : Should default URL in the next change.
            BaseAddress = new Uri("http://localhost:7700/")
        };
        public StatusTests()
        {
            
        }
        
        [Fact]
        public async Task Should_be_Able_to_get_All_The_Update_Status()
        {
            var client = new MeilisearchClient(_httpClient);
            var indexName = "MoviesStatus" + new Random().Next();
            var index = await  client.CreateIndex(indexName);
            await index.AddorUpdateDocuments(new[] {new Movie {Id = "1"}});
            var status = await index.GetAllUpdateStatus();
            status.Count().Should().BeGreaterOrEqualTo(1);
        }
 
        [Fact]
        public async Task Should_be_Able_to_Get_Status_By_Id()
        {
            var client = new MeilisearchClient(_httpClient);
            var indexName = "MoviesStatus" + new Random().Next();
            var index = await  client.CreateIndex(indexName);
            var status = await index.AddorUpdateDocuments(new[] {new Movie {Id = "2"}});
            UpdateStatus individualStatus = await index.GetUpdateStatus(status.UpdateId);
            individualStatus.Should().NotBeNull();
        } 
    } 
}
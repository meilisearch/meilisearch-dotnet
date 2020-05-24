using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Meilisearch.Tests
{
    public class MeilisearchClientTests
    {
        private static HttpClient _httpClient = new HttpClient
        {
            // TODO : Should default URL in the next change.
            BaseAddress = new Uri("http://localhost:7700/")
        };
        
        [Fact]
        public async Task Should_be_Able_To_Get_Version()
        {
            var client = new MeilisearchClient(_httpClient);
            var meilisearchversion= await client.GetVersion();
            meilisearchversion.Version.Should().NotBeNullOrEmpty();
        }
    }
}

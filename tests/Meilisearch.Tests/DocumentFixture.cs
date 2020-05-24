using System;
using System.Net.Http;

namespace Meilisearch.Tests
{
    public class DocumentFixture : IDisposable
    {
        private static HttpClient _httpClient = new HttpClient
        {
            // TODO : Should default URL in the next change.
            BaseAddress = new Uri("http://localhost:7700/")
        };
        
        public Index documentIndex { get; private set; }

        public DocumentFixture()
        {
           SetUp();
        }

        public void SetUp()
        {
            var client = new MeilisearchClient(_httpClient);
            var index = client.GetIndex("Movies").Result;
            if (index == null)
            {
                this.documentIndex = client.CreateIndex("Movies").Result;
            }
            else
            {
                this.documentIndex = index;
            }
            
        }
        
        public void Dispose()
        {
        }
    }
}
using System;
using System.Net.Http;
using System.Net.Http.Headers;

using HttpClientFactoryLite;

namespace Meilisearch.Tests
{
    public class ClientFactory
    {
        private static readonly Lazy<HttpClientFactory>
            Lazy =
                new Lazy<HttpClientFactory>(() =>
                {
                    var httpClientFactory = new HttpClientFactory();
                    httpClientFactory.Register<MeilisearchClient>(builder => builder
                        .ConfigureHttpClient(p =>
                            {
                                p.BaseAddress = new Uri("http://localhost:7700/");
                                p.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "masterKey");
                            })
                        .ConfigurePrimaryHttpMessageHandler(() => new MeilisearchMessageHandler(new HttpClientHandler())));
                    return httpClientFactory;
                });

        public static HttpClientFactory Instance
        {
            get
            {
                return Lazy.Value;
            }
        }
    }
}
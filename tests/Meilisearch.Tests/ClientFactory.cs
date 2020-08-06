using System;
using HttpClientFactoryLite;

namespace Meilisearch.Tests
{
    public class ClientFactory
    {
        private static readonly Lazy<HttpClientFactory>
            lazy =
                new Lazy<HttpClientFactory>
                    (() =>
                {

                    var httpClientFactory = new HttpClientFactory();
                    httpClientFactory.Register<MeilisearchClient>(
                        builder =>
                            builder.ConfigureHttpClient(p =>
                            {
                                p.BaseAddress = new Uri("http://localhost:7700/");
                                p.DefaultRequestHeaders.Add("X-Meili-API-Key", "masterKey");
                            }));
                    return httpClientFactory;
                });

        public static HttpClientFactory Instance { get { return lazy.Value; } }
    }
}

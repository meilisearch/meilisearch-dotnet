using System;
using Microsoft.Extensions.Http;

namespace Meilisearch.Tests
{
    public class ClientFactory
    {
        private static readonly Lazy<HttpClientFactory>
            lazy =
                new Lazy<HttpClientFactory>
                    (() =>
                {

                    var httpclientfactory = new HttpClientFactory();
                    httpclientfactory.Register<MeilisearchClient>(
                        builder => builder.ConfigureHttpClient(p => p.BaseAddress = new Uri("http://localhost:7700/")));
                    return httpclientfactory;
                });

        public static HttpClientFactory Instance { get { return lazy.Value; } }
    }
}

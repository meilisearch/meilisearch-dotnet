using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace Meilisearch.Tests
{
    public class InlineSearchRuleTests
    {
        [Fact]
        public async Task SearchSerializesInlineRuleWithoutUnspecifiedFields()
        {
            var handler = new CaptureHandler(@"{""hits"":[], ""offset"":0, ""limit"":20, ""estimatedTotalHits"":0}");
            using (var http = CreateHttpClient(handler))
            {
                var client = new MeilisearchClient(http);
                var query = new SearchQuery
                {
                    InlineRule = new InlineSearchRule
                    {
                        Uid = "batman-preview",
                        Actions = new DSRActions
                        {
                            Scale = new[] { new DSRScale { Filter = "hero = batman", Weight = 1.2 } }
                        }
                    }
                };

                var result = await client.Index("movies").SearchAsync<JsonElement>("batman", query);

                using (var request = JsonDocument.Parse(handler.RequestBody))
                {
                    var root = request.RootElement;
                    Assert.Equal("batman", root.GetProperty("q").GetString());
                    var inlineRule = root.GetProperty("inlineRule");
                    Assert.Equal("batman-preview", inlineRule.GetProperty("uid").GetString());
                    Assert.Equal(1.2, inlineRule.GetProperty("actions").GetProperty("scale")[0].GetProperty("weight").GetDouble());
                    Assert.False(inlineRule.TryGetProperty("active", out _));
                }

                Assert.Null(result.Metadata);
                Assert.Null(typeof(InlineSearchRule).GetProperty("Description"));
            }
        }

        [Fact]
        public async Task MultiSearchSerializesInlineRuleAndReadsConditionOutcomes()
        {
            var handler = new CaptureHandler(@"
            {
                ""results"": [
                    {
                        ""hits"": [], ""offset"": 0, ""limit"": 20, ""estimatedTotalHits"": 0,
                        ""metadata"": {
                            ""queryUid"": ""query-123"",
                            ""indexUid"": ""products"",
                            ""inlineRuleConditionOutcomes"": {
                                ""satisfiesActiveCondition"": true,
                                ""satisfiesQueryWordsCondition"": { ""missingWord"": ""pants"" },
                                ""satisfiesFilterCondition"": { ""missingConstraintOnField"": { ""field"": ""stock_intent"" } }
                            }
                        }
                    }
                ]
            }");
            using (var http = CreateHttpClient(handler))
            {
                var client = new MeilisearchClient(http);
                var query = new MultiSearchQuery
                {
                    Queries = new List<SearchQuery>
                    {
                        new SearchQuery
                        {
                            IndexUid = "products",
                            Q = "fashion",
                            InlineRule = new InlineSearchRule
                            {
                                Uid = "special-sales",
                                Conditions = new DynamicSearchRuleConditions
                                {
                                    Query = new QueryCondition { Words = "pants" }
                                }
                            }
                        }
                    }
                };

                var result = await client.MultiSearchAsync(query, includeMetadata: true);

                Assert.True(handler.IncludeMetadataHeader);
                using (var request = JsonDocument.Parse(handler.RequestBody))
                {
                    var inlineRule = request.RootElement.GetProperty("queries")[0].GetProperty("inlineRule");
                    Assert.Equal("special-sales", inlineRule.GetProperty("uid").GetString());
                    Assert.Equal("pants", inlineRule.GetProperty("conditions").GetProperty("query").GetProperty("words").GetString());
                }

                Assert.Single(result.Results);
                var metadata = result.Results[0].Metadata;
                Assert.NotNull(metadata);
                Assert.Equal("query-123", metadata.QueryUid);
                Assert.Equal("products", metadata.IndexUid);
                Assert.True(metadata.InlineRuleConditionOutcomes.HasValue);

                var outcomes = metadata.InlineRuleConditionOutcomes.Value;
                Assert.True(outcomes.GetProperty("satisfiesActiveCondition").GetBoolean());
                Assert.Equal("pants", outcomes.GetProperty("satisfiesQueryWordsCondition").GetProperty("missingWord").GetString());
                Assert.Equal("stock_intent", outcomes.GetProperty("satisfiesFilterCondition").GetProperty("missingConstraintOnField").GetProperty("field").GetString());
            }
        }

        [Fact]
        public async Task MultiSearchWithoutMetadataKeepsOriginalRequestBehavior()
        {
            var handler = new CaptureHandler(@"{""results"":[{""hits"":[],""offset"":0,""limit"":20,""estimatedTotalHits"":0}]}");
            using (var http = CreateHttpClient(handler))
            {
                var client = new MeilisearchClient(http);
                var result = await client.MultiSearchAsync(new MultiSearchQuery
                {
                    Queries = new List<SearchQuery> { new SearchQuery { IndexUid = "movies", Q = "test" } }
                });

                Assert.False(handler.IncludeMetadataHeader);
                Assert.Null(result.Results[0].Metadata);

                using (var request = JsonDocument.Parse(handler.RequestBody))
                {
                    Assert.False(request.RootElement.GetProperty("queries")[0].TryGetProperty("inlineRule", out _));
                }
            }
        }

        [Fact]
        public void PaginatedSearchPreservesMetadata()
        {
            const string json = @"{
                ""hits"": [], ""page"": 1, ""hitsPerPage"": 10, ""totalPages"": 1, ""totalHits"": 0,
                ""metadata"": { ""inlineRuleConditionOutcomes"": { ""satisfiesActiveCondition"": false } }
            }";

            var result = JsonSerializer.Deserialize<ISearchable<JsonDocument>>(json);

            Assert.IsType<PaginatedSearchResult<JsonDocument>>(result);
            Assert.False(result.Metadata.InlineRuleConditionOutcomes.Value.GetProperty("satisfiesActiveCondition").GetBoolean());
        }

        private static HttpClient CreateHttpClient(HttpMessageHandler handler)
        {
            return new HttpClient(handler) { BaseAddress = new Uri("http://localhost:7700/") };
        }

        private sealed class CaptureHandler : HttpMessageHandler
        {
            private readonly string _response;

            public CaptureHandler(string response)
            {
                _response = response;
            }

            public string RequestBody { get; private set; }

            public bool IncludeMetadataHeader { get; private set; }

            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
            {
                RequestBody = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                IncludeMetadataHeader = request.Headers.TryGetValues("Meili-Include-Metadata", out var values)
                    && values.Single() == "true";

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_response, Encoding.UTF8, "application/json")
                };
            }
        }
    }
}

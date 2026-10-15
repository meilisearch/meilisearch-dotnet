using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using FluentAssertions;

using Xunit;

namespace Meilisearch.Tests
{
    public class FederatedSearchOptionsSerializationTests
    {
        [Fact]
        public void FederatedSearchSerializesFacetsByIndexAndMergeFacets()
        {
            var query = new FederatedMultiSearchQuery
            {
                Queries = new List<FederatedSearchQuery>
                {
                    new FederatedSearchQuery { IndexUid = "movies", Q = "Batman" },
                    new FederatedSearchQuery { IndexUid = "comics", Q = "Batman" }
                },
                FederationOptions = new MultiSearchFederationOptions
                {
                    FacetsByIndex = new Dictionary<string, string[]>
                    {
                        ["movies"] = new[] { "title", "id" },
                        ["comics"] = new[] { "title" }
                    },
                    MergeFacets = new MultiSearchFederationMergeFacetsOptions
                    {
                        MaxValuesPerFacet = 10
                    }
                }
            };

            using var document = JsonDocument.Parse(JsonSerializer.Serialize(query, Constants.JsonSerializerOptionsRemoveNulls));
            var federation = document.RootElement.GetProperty("federation");
            var facetsByIndex = federation.GetProperty("facetsByIndex");

            facetsByIndex.GetProperty("movies").EnumerateArray()
                .Select(facet => facet.GetString()).Should().Equal("title", "id");
            facetsByIndex.GetProperty("comics").EnumerateArray()
                .Select(facet => facet.GetString()).Should().Equal("title");
            federation.GetProperty("mergeFacets").GetProperty("maxValuesPerFacet")
                .GetInt32().Should().Be(10);
        }

        [Fact]
        public void FederatedSearchOmitsUnspecifiedFacetingOptions()
        {
            var query = new FederatedMultiSearchQuery
            {
                Queries = new List<FederatedSearchQuery>(),
                FederationOptions = new MultiSearchFederationOptions { Limit = 5 }
            };

            using var document = JsonDocument.Parse(JsonSerializer.Serialize(query, Constants.JsonSerializerOptionsRemoveNulls));
            var federation = document.RootElement.GetProperty("federation");

            federation.GetProperty("limit").GetInt32().Should().Be(5);
            federation.TryGetProperty("facetsByIndex", out _).Should().BeFalse();
            federation.TryGetProperty("mergeFacets", out _).Should().BeFalse();
        }

        [Fact]
        public void FederatedSearchKeepsEmptyFederationObject()
        {
            var query = new FederatedMultiSearchQuery
            {
                Queries = new List<FederatedSearchQuery>()
            };

            using var document = JsonDocument.Parse(JsonSerializer.Serialize(query, Constants.JsonSerializerOptionsRemoveNulls));
            var federation = document.RootElement.GetProperty("federation");

            federation.ValueKind.Should().Be(JsonValueKind.Object);
            federation.EnumerateObject().Should().BeEmpty();
        }
    }
}

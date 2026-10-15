using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using FluentAssertions;

using Xunit;

namespace Meilisearch.Tests
{
    public class FederatedSearchOptionsSerializationTests
    {
        /// <summary>
        /// Serializes per-index facets and merged facet settings together.
        /// </summary>
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

        /// <summary>
        /// Serializes per-index facet requests without merged facet settings.
        /// </summary>
        [Fact]
        public void FederatedSearchSerializesFacetsByIndexAlone()
        {
            var query = new FederatedMultiSearchQuery
            {
                Queries = new List<FederatedSearchQuery>(),
                FederationOptions = new MultiSearchFederationOptions
                {
                    FacetsByIndex = new Dictionary<string, string[]>
                    {
                        ["movies"] = new[] { "genre" }
                    }
                }
            };

            using var document = JsonDocument.Parse(JsonSerializer.Serialize(query, Constants.JsonSerializerOptionsRemoveNulls));
            var federation = document.RootElement.GetProperty("federation");

            federation.GetProperty("facetsByIndex").GetProperty("movies")
                .EnumerateArray().Select(facet => facet.GetString()).Should().Equal("genre");
            federation.TryGetProperty("mergeFacets", out _).Should().BeFalse();
        }

        /// <summary>
        /// Serializes merged facet settings without per-index facet requests.
        /// </summary>
        [Fact]
        public void FederatedSearchSerializesMergeFacetsAlone()
        {
            var query = new FederatedMultiSearchQuery
            {
                Queries = new List<FederatedSearchQuery>(),
                FederationOptions = new MultiSearchFederationOptions
                {
                    MergeFacets = new MultiSearchFederationMergeFacetsOptions
                    {
                        MaxValuesPerFacet = 20
                    }
                }
            };

            using var document = JsonDocument.Parse(JsonSerializer.Serialize(query, Constants.JsonSerializerOptionsRemoveNulls));
            var federation = document.RootElement.GetProperty("federation");

            federation.GetProperty("mergeFacets").GetProperty("maxValuesPerFacet")
                .GetInt32().Should().Be(20);
            federation.TryGetProperty("facetsByIndex", out _).Should().BeFalse();
        }

        /// <summary>
        /// Omits optional faceting settings when they have not been configured.
        /// </summary>
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

        /// <summary>
        /// Keeps the required empty federation object when no options are specified.
        /// </summary>
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

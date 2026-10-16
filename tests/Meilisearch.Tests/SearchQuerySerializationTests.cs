using System.Collections.Generic;
using System.Text.Json;

using Xunit;

namespace Meilisearch.Tests
{
    public class SearchQuerySerializationTests
    {
        [Fact]
        public void PersonalizeIsSerializedAsUserContext()
        {
            var query = new SearchQuery
            {
                Q = "man",
                Personalize = new Personalize { UserContext = "The user only watches science fiction movies" }
            };

            var json = JsonSerializer.Serialize(query, Constants.JsonSerializerOptionsRemoveNulls);

            Assert.Contains(
                "\"personalize\":{\"userContext\":\"The user only watches science fiction movies\"}",
                json);
        }

        [Fact]
        public void PersonalizeIsOmittedWhenNotSet()
        {
            var json = JsonSerializer.Serialize(
                new SearchQuery { Q = "man" }, Constants.JsonSerializerOptionsRemoveNulls);

            Assert.DoesNotContain("personalize", json);
        }

        [Fact]
        public void PersonalizeIsSerializedInsideMultiSearchQueries()
        {
            var query = new MultiSearchQuery
            {
                Queries = new List<SearchQuery>
                {
                    new SearchQuery
                    {
                        IndexUid = "movies",
                        Q = "man",
                        Personalize = new Personalize { UserContext = "sci-fi fan" }
                    }
                }
            };

            var json = JsonSerializer.Serialize(query, Constants.JsonSerializerOptionsRemoveNulls);

            Assert.Contains("\"personalize\":{\"userContext\":\"sci-fi fan\"}", json);
        }
    }
}

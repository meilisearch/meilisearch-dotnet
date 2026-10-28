using System.Text.Json;

using Xunit;

namespace Meilisearch.Tests
{
    public class SortFacetValuesConverterTests
    {
        [Theory]
        [InlineData(SortFacetValuesByType.Alpha, "\"alpha\"")]
        [InlineData(SortFacetValuesByType.Count, "\"count\"")]
        [InlineData(SortFacetValuesByType.SampledCount, "\"sampledCount\"")]
        public void WriteExpectedValue(SortFacetValuesByType value, string expectedJson)
        {
            Assert.Equal(expectedJson, JsonSerializer.Serialize(value));
        }

        [Theory]
        [InlineData("\"alpha\"", SortFacetValuesByType.Alpha)]
        [InlineData("\"count\"", SortFacetValuesByType.Count)]
        [InlineData("\"sampledCount\"", SortFacetValuesByType.SampledCount)]
        [InlineData("\"ALPHA\"", SortFacetValuesByType.Alpha)]
        [InlineData("\"COUNT\"", SortFacetValuesByType.Count)]
        [InlineData("\"SAMPLEDCOUNT\"", SortFacetValuesByType.SampledCount)]
        public void ReadValueIgnoringCase(string json, SortFacetValuesByType expectedValue)
        {
            Assert.Equal(expectedValue, JsonSerializer.Deserialize<SortFacetValuesByType>(json));
        }

        [Theory]
        [InlineData("\"unknown\"")]
        [InlineData("null")]
        [InlineData("42")]
        [InlineData("false")]
        public void ReadUnknownValueAsAlpha(string json)
        {
            Assert.Equal(SortFacetValuesByType.Alpha, JsonSerializer.Deserialize<SortFacetValuesByType>(json));
        }
    }
}

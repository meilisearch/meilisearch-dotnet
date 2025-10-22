using System.Collections.Generic;
using System.Text.Json;

using Xunit;

namespace Meilisearch.Tests
{
    public class IndexSwapTests
    {
        [Fact]
        public void PreventMoreThanTwoIndexesPerObject()
        {
            var swap = new IndexSwap("indexA", "indexB");

            Assert.Equal(new List<string> { "indexA", "indexB" }, swap.Indexes);
        }

        [Fact]
        public void CreateExpectedJSONFormat()
        {
            var swap = new IndexSwap("indexA", "indexB");

            var json = JsonSerializer.Serialize(swap);
            Assert.Contains("\"indexes\":[\"indexA\",\"indexB\"]", json);
            Assert.Contains("\"rename\":false", json);
        }

        [Fact]
        public void CreateExpectedJSONFormatWithRenameTrue()
        {
            var swap = new IndexSwap("indexA", "indexB", rename: true);

            var json = JsonSerializer.Serialize(swap);
            Assert.Contains("\"indexes\":[\"indexA\",\"indexB\"]", json);
            Assert.Contains("\"rename\":true", json);
        }
    }
}

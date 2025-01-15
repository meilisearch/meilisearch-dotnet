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

            Assert.Equal("{\"indexes\":[\"indexA\",\"indexB\"]}", JsonSerializer.Serialize(swap));
        }
    }
}

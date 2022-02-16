using Meilisearch.Extensions;

using Xunit;

namespace Meilisearch.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("http://localhost:7700", "http://localhost:7700/")]
        [InlineData("http://localhost:7700/", "http://localhost:7700/")]
        [InlineData("http://localhost:7700/api", "http://localhost:7700/api/")]
        [InlineData("http://localhost:7700/api/", "http://localhost:7700/api/")]
        public void CheckUrisEndWithSlash(string actual, string expected)
        {
            Assert.Equal(expected, actual.ToSafeUri().AbsoluteUri);
        }
    }
}

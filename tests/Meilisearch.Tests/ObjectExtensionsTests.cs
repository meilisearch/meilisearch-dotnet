using Microsoft.AspNetCore.WebUtilities;

using Xunit;

namespace Meilisearch.Tests
{
    public class ObjectExtensionsTests
    {
        [Theory]
        [InlineData("simple")]
        [InlineData("com pl <->& ex")]
        public void QueryStringsAreEqualsForPrimaryKey(string key)
        {
            var uri = "/indexes/myindex/documents";
            var o = new { primaryKey = key };

            var expected = QueryHelpers.AddQueryString(uri, o.AsDictionary());
            var actual = $"{uri}?{o.ToQueryString()}";
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null, "")]
        [InlineData(1, null, "")]
        [InlineData(null, 3, "")]
        [InlineData(null, null, "attr")]
        [InlineData(1, 2, "")]
        [InlineData(1, null, "attr")]
        [InlineData(null, 2, "attr")]
        [InlineData(1, 2, "attr")]
        public void QueryStringsAreEqualsForDocumentQuery(int? offset, int? limit, string attributesToRetrieve)
        {
            var uri = "/indexes/myindex/documents";
            var dq = new DocumentQuery { Offset = offset, Limit = limit, AttributesToRetrieve = attributesToRetrieve };

            var expected = QueryHelpers.AddQueryString(uri, dq.AsDictionary());
            var actual = $"{uri}?{dq.ToQueryString()}";
            Assert.Equal(expected, actual);
        }
    }
}
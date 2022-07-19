using System.Collections.Generic;

using Meilisearch.Extensions;

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
            var uri = "indexes/myindex/documents";
            var o = new { primaryKey = key };

            var expected = QueryHelpers.AddQueryString(uri, o.AsDictionary());
            var actual = $"{uri}?{o.ToQueryString()}";
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData(1, null, null)]
        [InlineData(null, 3, null)]
        [InlineData(null, null, "attr")]
        [InlineData(1, 2, null)]
        [InlineData(1, null, "attr")]
        [InlineData(null, 2, "attr")]
        [InlineData(1, 2, "attr")]
        public void QueryStringsAreEqualsForDocumentQuery(int? offset, int? limit, string fields)
        {
            var uri = "indexes/myindex/documents";
            var dq = new DocumentQuery { Offset = offset, Limit = limit, Fields = new List<string> { fields } };

            var expected = QueryHelpers.AddQueryString(uri, dq.AsDictionary());
            var actual = $"{uri}?{dq.ToQueryString()}";
            Assert.Equal(expected, actual);
        }
    }
}

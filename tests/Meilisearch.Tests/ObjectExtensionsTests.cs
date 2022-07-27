using System;
using System.Collections.Generic;

using Meilisearch.Extensions;
using Meilisearch.QueryParameters;

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
        [InlineData(null, null, new string[] { "attr" })]
        [InlineData(null, null, new string[] { "attr", "attr2", "attr3" })]
        [InlineData(1, 2, null)]
        [InlineData(1, null, new string[] { "attr" })]
        [InlineData(null, 2, new string[] { "attr" })]
        [InlineData(1, 2, new string[] { "attr" })]
        [InlineData(1, 2, new string[] { "attr", "attr2", "attr3" })]
        public void QueryStringsWithListAreEqualsForDocumentsQuery(int? offset, int? limit, string[] fields)
        {
            var uri = "indexes/myindex/documents";
            var dq = new DocumentsQuery { Offset = offset, Limit = limit, Fields = fields != null ? new List<string>(fields) : null };
            var actualQuery = $"{uri}?{dq.ToQueryString()}";

            Assert.NotEmpty(actualQuery);
            Assert.NotNull(actualQuery);
            if (limit != null)
            {
                Assert.Contains("limit", actualQuery);
                Assert.Contains(dq.Limit.ToString(), actualQuery);
            }
            if (offset != null)
            {
                Assert.Contains("offset", actualQuery);
                Assert.Contains(dq.Offset.ToString(), actualQuery);
            }
            if (fields != null)
            {
                Assert.Contains("fields", actualQuery);
                Assert.Contains(String.Join(",", dq.Fields), actualQuery);
            }
        }
    }
}

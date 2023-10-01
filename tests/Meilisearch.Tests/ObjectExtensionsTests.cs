using System;
using System.Collections.Generic;
using System.Linq;

using Meilisearch.Extensions;
using Meilisearch.QueryParameters;

using Microsoft.AspNetCore.WebUtilities;

using Xunit;

namespace Meilisearch.Tests
{
    public class FakeQuery
    {
        public DateTime? FakeDate { get; set; }
        public string FakeString { get; set; }
        public int? FakeInteger { get; set; }
        public List<string> FakeStringList { get; set; }
        public string Path { get; set; }
    }

    public class ObjectExtensionsTests
    {
        public static IEnumerable<object[]> FakeData()
        {
            var date = DateTime.Now;

            yield return new object[] {
                new FakeQuery { FakeString = "+1" },
                "fakeString=%2B1"
            };

            yield return new object[] {
                new FakeQuery { FakeString = "+1", FakeInteger = 22 },
                "fakeString=%2B1&fakeInteger=22"
            };

            yield return new object[] {
                new FakeQuery { FakeDate = date, FakeStringList = new List<string> { "hey", "ho" } },
                $"fakeDate={Uri.EscapeDataString(date.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz"))}&fakeStringList=hey,ho"
            };
        }

        [Theory]
        [MemberData(nameof(FakeData))]
        public void ToQueryStringConvertTypes(FakeQuery query, string expected)
        {
            Assert.Equal(query.ToQueryString(), expected);
        }

        [Theory]
        [InlineData("simple")]
        [InlineData("com pl <->& ex")]
        public void QueryStringsAreEqualsForPrimaryKey(string key)
        {
            var uri = "indexes/myindex/documents";
            var o = new { primaryKey = key };

            var expected = QueryHelpers.AddQueryString(uri, o.AsDictionary());
            var actual = o.ToQueryString(uri: uri);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("simple")]
        public void QueryStringOnlyQueryStringParameters(string key)
        {
            var uri = "";
            var o = new { primaryKey = key };

            var expected = string.Join(",", o.AsDictionary().Select(kv => kv.Key + "=" + kv.Value));
            var actual = o.ToQueryString();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("simple")]
        public void QueryStringPrependsUri(string key)
        {
            var uri = "indexes/myindex/documents";
            var o = new { primaryKey = key };

            var expected = QueryHelpers.AddQueryString(uri, o.AsDictionary());
            var actual = o.ToQueryString(uri: uri);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void QueryStringReturnsEmptyForNullObject()
        {
            object o = null;

            var expected = "";
            var actual = o.ToQueryString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void QueryStringReturnsUriForNullObject()
        {
            var uri = "indexes/myindex/documents";
            object o = null;

            var expected = uri;
            var actual = o.ToQueryString(uri: uri);
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
            var actualQuery = dq.ToQueryString(uri: uri);

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

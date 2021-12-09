namespace Meilisearch.Tests
{
    using Microsoft.AspNetCore.WebUtilities;
    using Xunit;

    public class ObjectExtensionsTests
    {
        [Theory]
        [InlineData("simple")]
        [InlineData("com pl <->& ex")]
        void QueryStringsAreEqualsForPrimaryKey(string key)
        {
            string uri = "/indexes/myindex/documents";
            var o = new { primaryKey = key };

            string expected = QueryHelpers.AddQueryString(uri, o.AsDictionary());
            string actual = $"{uri}?{o.ToQueryString()}";
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
        void QueryStringsAreEqualsForDocumentQuery(int? offset, int? limit, string attributesToRetrieve)
        {
            string uri = "/indexes/myindex/documents";
            var dq = new DocumentQuery { Offset = offset, Limit = limit, AttributesToRetrieve = attributesToRetrieve };

            string expected = QueryHelpers.AddQueryString(uri, dq.AsDictionary());
            string actual = $"{uri}?{dq.ToQueryString()}";
            Assert.Equal(expected, actual);
        }
    }
}

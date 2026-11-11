using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Xunit;

namespace Meilisearch.Tests
{
    public class CompressionHandlerTests
    {
        [Fact]
        public async Task Gzip_AtOrAboveThreshold_CompressesAndRoundTrips()
        {
            var payload = new string('a', 200);
            var captured = await SendCompressedAsync(CompressionOptions.Gzip(payload.Length), payload);

            captured.ContentEncoding.Should().Equal("gzip");
            captured.Body.Length.Should().BeLessThan(payload.Length);
            captured.ContentType.Should().StartWith("application/json");
            Decompress(captured.Body, CompressionMode.Decompress).Should().Equal(Encoding.UTF8.GetBytes(payload));
        }

        [Fact]
        public async Task Gzip_BelowThreshold_LeavesBodyAndContentTypeUntouched()
        {
            var payload = new string('a', 200);
            var captured = await SendCompressedAsync(CompressionOptions.Gzip(payload.Length + 1), payload);

            captured.ContentEncoding.Should().BeEmpty();
            captured.Body.Should().Equal(Encoding.UTF8.GetBytes(payload));
            captured.ContentType.Should().StartWith("application/json");
        }

        [Fact]
        public async Task Deflate_AtOrAboveThreshold_UsesZlibAndRoundTrips()
        {
            var payload = new string('b', 200);
            var captured = await SendCompressedAsync(CompressionOptions.Deflate(payload.Length), payload);

            captured.ContentEncoding.Should().Equal("deflate");
            captured.Body.Length.Should().BeLessThan(payload.Length);
            DecompressZlib(captured.Body).Should().Equal(Encoding.UTF8.GetBytes(payload));
        }

        [Fact]
        public async Task Brotli_AtOrAboveThreshold_CompressesAndRoundTrips()
        {
            var payload = new string('c', 200);
            var captured = await SendCompressedAsync(CompressionOptions.Brotli(payload.Length), payload);

            captured.ContentEncoding.Should().Equal("br");
            captured.Body.Length.Should().BeLessThan(payload.Length);
            DecompressBrotli(captured.Body).Should().Equal(Encoding.UTF8.GetBytes(payload));
        }

        [Fact]
        public async Task CustomHttpClientConstructor_DoesNotApplyCompressionOptions()
        {
            var payload = new string('d', 400);
            var recording = new RecordingHandler();
            using (var http = new HttpClient(recording) { BaseAddress = new Uri("http://localhost/") })
            {
                var client = new MeilisearchClient(http, "masterKey", CompressionOptions.Gzip(minimumSizeBytes: 0));
                var task = await client.Index("movies").AddDocumentsAsync(new[]
                {
                    new Movie { Id = "1", Name = payload }
                });

                task.TaskUid.Should().Be(1);
            }

            recording.Captured.ContentEncoding.Should().BeEmpty();
            Encoding.UTF8.GetString(recording.Captured.Body).Should().Contain(payload);
            recording.Captured.Body[0].Should().NotBe(0x1f);
        }

        private static async Task<CapturedRequest> SendCompressedAsync(CompressionOptions options, string payload)
        {
            var recording = new RecordingHandler();
            var handler = new MeilisearchMessageHandler(recording, options);
            using (var client = new HttpClient(handler))
            {
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost/indexes/movies/documents", content);
                response.EnsureSuccessStatusCode();
            }

            return recording.Captured;
        }

        private static byte[] Decompress(byte[] data, CompressionMode mode)
        {
            using (var input = new MemoryStream(data))
            using (var gzip = new GZipStream(input, mode))
            using (var output = new MemoryStream())
            {
                gzip.CopyTo(output);
                return output.ToArray();
            }
        }

        private static byte[] DecompressZlib(byte[] data)
        {
            using (var input = new MemoryStream(data))
            using (var zlib = new ZLibStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                zlib.CopyTo(output);
                return output.ToArray();
            }
        }

        private static byte[] DecompressBrotli(byte[] data)
        {
            using (var input = new MemoryStream(data))
            using (var brotli = new BrotliStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                brotli.CopyTo(output);
                return output.ToArray();
            }
        }

        private sealed class RecordingHandler : HttpMessageHandler
        {
            public CapturedRequest Captured { get; private set; }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var body = await request.Content.ReadAsByteArrayAsync();
                Captured = new CapturedRequest(
                    body,
                    request.Content.Headers.ContentEncoding.ToArray(),
                    request.Content.Headers.ContentType == null ? null : request.Content.Headers.ContentType.ToString());

                var taskJson = "{\"taskUid\":1,\"indexUid\":\"movies\",\"status\":\"enqueued\",\"type\":\"documentAdditionOrUpdate\",\"enqueuedAt\":\"2026-01-01T00:00:00Z\"}";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(taskJson, Encoding.UTF8, "application/json")
                };
            }
        }

        private sealed class CapturedRequest
        {
            public CapturedRequest(byte[] body, string[] contentEncoding, string contentType)
            {
                Body = body;
                ContentEncoding = contentEncoding;
                ContentType = contentType;
            }

            public byte[] Body { get; }

            public string[] ContentEncoding { get; }

            public string ContentType { get; }
        }
    }
}

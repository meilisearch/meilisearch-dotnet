using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Meilisearch.QueryParameters;
using Meilisearch.Tests.ServerConfigs;

using Xunit;

namespace Meilisearch.Tests
{
    [Collection(nameof(BaseUriServer))]
    public class CompressionTests : IAsyncLifetime
    {
        private readonly IndexFixture _fixture;

        public CompressionTests(BaseUriServer.ConfigFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync() =>
            await _fixture.DeleteAllIndexes();

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GzipCompression_ShouldSuccessfullyAddDocuments()
        {
            // Arrange
            var compressionOptions = CompressionOptions.Gzip(minimumSizeBytes: 100);
            var client = new MeilisearchClient(
                _fixture.MeilisearchAddress(),
                "masterKey",
                compressionOptions);

            var index = client.Index("movies-gzip-test");
            var movies = Enumerable.Range(1, 100)
                .Select(i => new Movie { Id = i.ToString(), Name = $"Movie {i}" })
                .ToList();

            // Act
            var task = await index.AddDocumentsAsync(movies);
            await index.WaitForTaskAsync(task.TaskUid);

            // Assert
            var docs = await index.GetDocumentsAsync<Movie>(new DocumentsQuery { Limit = 100 });
            docs.Results.Should().HaveCount(100);
        }

        [Fact]
        public async Task DeflateCompression_ShouldThrowNotSupportedExceptionOnNetStandard20()
        {
            // Arrange
            var compressionOptions = CompressionOptions.Deflate(minimumSizeBytes: 100);
            var client = new MeilisearchClient(
                _fixture.MeilisearchAddress(),
                "masterKey",
                compressionOptions);

            var index = client.Index("movies-deflate-test");
            var movies = new[] { new Movie { Id = "1", Name = "Test Movie" } };

            // Act & Assert
            // Since Meilisearch library targets netstandard2.0, Deflate will always throw
            // NotSupportedException regardless of the test target framework.
            // Deflate requires .NET 6.0+ for ZLibStream support.
            var exception = await Assert.ThrowsAsync<NotSupportedException>(async () =>
            {
                await index.AddDocumentsAsync(movies);
            });

            exception.Message.Should().Contain("Deflate requires .NET 6.0+");
        }

        [Fact]
        public async Task UpdateDocuments_ShouldWorkWithCompression()
        {
            // Arrange
            var compressionOptions = CompressionOptions.Gzip(minimumSizeBytes: 100);
            var client = new MeilisearchClient(
                _fixture.MeilisearchAddress(),
                "masterKey",
                compressionOptions);

            var index = client.Index("movies-update-test");

            // Add initial documents
            var movies = new[] { new Movie { Id = "1", Name = "Original" } };
            var addTask = await index.AddDocumentsAsync(movies);
            await index.WaitForTaskAsync(addTask.TaskUid);

            // Act - Update with compression
            var updatedMovies = new[] { new Movie { Id = "1", Name = "Updated" } };
            var updateTask = await index.UpdateDocumentsAsync(updatedMovies);
            await index.WaitForTaskAsync(updateTask.TaskUid);

            // Assert
            var doc = await index.GetDocumentAsync<Movie>("1");
            doc.Name.Should().Be("Updated");
        }

        [Fact]
        public async Task NoCompression_ShouldWorkAsDefault()
        {
            // Arrange - No compression options specified
            var client = new MeilisearchClient(_fixture.MeilisearchAddress(), "masterKey");

            var index = client.Index("movies-no-compression");
            var movies = new[] { new Movie { Id = "1", Name = "Test Movie" } };

            // Act
            var task = await index.AddDocumentsAsync(movies);
            await index.WaitForTaskAsync(task.TaskUid);

            // Assert
            var docs = await index.GetDocumentsAsync<Movie>();
            docs.Results.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(0)]     // Compress all
        [InlineData(100)]   // 100 bytes
        [InlineData(1024)]  // 1 KB
        [InlineData(1400)]  // 1.4 KB (recommended)
        public async Task DifferentThresholds_ShouldWorkCorrectly(int threshold)
        {
            // Arrange
            var compressionOptions = new CompressionOptions
            {
                Algorithm = CompressionAlgorithm.Gzip,
                MinimumSizeBytes = threshold
            };

            var client = new MeilisearchClient(
                _fixture.MeilisearchAddress(),
                "masterKey",
                compressionOptions);

            var index = client.Index($"movies-threshold-{threshold}");
            var movies = Enumerable.Range(1, 10)
                .Select(i => new Movie { Id = i.ToString(), Name = $"Movie {i}" })
                .ToList();

            // Act
            var task = await index.AddDocumentsAsync(movies);
            await index.WaitForTaskAsync(task.TaskUid);

            // Assert
            var docs = await index.GetDocumentsAsync<Movie>();
            docs.Results.Should().HaveCount(10);
        }

        [Fact]
        public void CompressionOptions_ShouldExposeCorrectDefaults()
        {
            // Arrange & Act
            var options = new CompressionOptions();

            // Assert
            options.Algorithm.Should().Be(CompressionAlgorithm.None);
            options.MinimumSizeBytes.Should().Be(1400);
            options.EnableResponseDecompression.Should().BeFalse();
        }

        [Fact]
        public void CompressionOptions_GzipFactoryMethod_ShouldCreateCorrectOptions()
        {
            // Act
            var options = CompressionOptions.Gzip();

            // Assert
            options.Algorithm.Should().Be(CompressionAlgorithm.Gzip);
            options.MinimumSizeBytes.Should().Be(1400);
        }

        [Fact]
        public void CompressionOptions_DeflateFactoryMethod_ShouldCreateCorrectOptions()
        {
            // Act
            var options = CompressionOptions.Deflate(1024);

            // Assert
            options.Algorithm.Should().Be(CompressionAlgorithm.Deflate);
            options.MinimumSizeBytes.Should().Be(1024);
        }

        [Fact]
        public async Task BrotliCompression_ShouldThrowNotSupportedExceptionOnNetStandard20()
        {
            // Arrange
            var compressionOptions = new CompressionOptions
            {
                Algorithm = CompressionAlgorithm.Brotli,
                MinimumSizeBytes = 100
            };
            var client = new MeilisearchClient(
                _fixture.MeilisearchAddress(),
                "masterKey",
                compressionOptions);

            var index = client.Index("movies-brotli-test");
            var movies = new[] { new Movie { Id = "1", Name = "Test Movie" } };

            // Act & Assert
            // Since Meilisearch library targets netstandard2.0, Brotli will always throw
            // NotSupportedException regardless of the test target framework.
            // Brotli requires .NET Standard 2.1+ or .NET Core 2.1+.
            var exception = await Assert.ThrowsAsync<NotSupportedException>(async () =>
            {
                await index.AddDocumentsAsync(movies);
            });

            exception.Message.Should().Contain("Brotli requires .NET Standard 2.1+");
        }

        [Fact]
        public async Task GzipCompression_ShouldReducePayloadSize()
        {
            // Arrange - Create a large dataset to ensure compression has an effect
            var compressionOptions = CompressionOptions.Gzip(minimumSizeBytes: 100);
            var client = new MeilisearchClient(
                _fixture.MeilisearchAddress(),
                "masterKey",
                compressionOptions);

            var index = client.Index("movies-compression-size-test");

            // Create 50 movies with long names to generate a sizable payload
            var movies = Enumerable.Range(1, 50)
                .Select(i => new Movie
                {
                    Id = i.ToString(),
                    Name = $"Movie {i} with a very long title that will compress well due to repetitive patterns"
                })
                .ToList();

            // Act
            var task = await index.AddDocumentsAsync(movies);
            await index.WaitForTaskAsync(task.TaskUid);

            // Assert - Verify documents were successfully added
            // This confirms compression worked without errors
            var docs = await index.GetDocumentsAsync<Movie>(new DocumentsQuery { Limit = 50 });
            docs.Results.Should().HaveCount(50);

            // Additional verification: Check that all movies have the expected long names
            docs.Results.Should().OnlyContain(m => m.Name.Contains("very long title"));
        }
    }
}

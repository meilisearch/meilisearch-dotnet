using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Meilisearch.QueryParameters;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class DocumentTests<TFixture> : IAsyncLifetime where TFixture : IndexFixture
    {
        private readonly MeilisearchClient _client;

        private readonly TFixture _fixture;

        public DocumentTests(TFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.DefaultClient;
        }

        public async Task InitializeAsync() =>
            await _fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task BasicDocumentsAddition()
        {
            var indexUID = "BasicDocumentsAdditionTest";
            var index = _client.Index(indexUID);

            // Add the documents
            var task = await index.AddDocumentsAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check the documents have been added
            var docs = await index.GetDocumentsAsync<Movie>();
            Assert.Equal("1", docs.Results.First().Id);
            Assert.Equal("Batman", docs.Results.First().Name);
            docs.Results.First().Genre.Should().BeNull();
        }

        [Fact]
        public async Task BasicDocumentAdditionFromJsonString()
        {
            var indexUID = nameof(BasicDocumentAdditionFromJsonString);
            var index = _client.Index(indexUID);

            var jsonDocuments = await File.ReadAllTextAsync(Datasets.SmallMoviesJsonPath);
            var task = await index.AddDocumentsJsonAsync(jsonDocuments);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check the documents have been added
            var docs = (await index.GetDocumentsAsync<DatasetSmallMovie>()).Results.ToList();
            Assert.NotEmpty(docs);
            var doc = docs.First();
            Assert.Equal("287947", doc.Id);
            Assert.Equal("Shazam!", doc.Title);
            Assert.Equal("action", doc.Genre);
        }

        [Fact]
        public async Task BasicDocumentAdditionFromCsvString()
        {
            var indexUID = nameof(BasicDocumentAdditionFromCsvString);
            var index = _client.Index(indexUID);

            var csvDocuments = await File.ReadAllTextAsync(Datasets.SongsCsvPath);
            var task = await index.AddDocumentsCsvAsync(csvDocuments);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check the documents have been added
            var docs = (await index.GetDocumentsAsync<DatasetSong>()).Results.ToList();
            Assert.NotEmpty(docs);
            var doc = docs.First();
            Assert.Equal("702481615", doc.Id);
            Assert.Equal("Armatage Shanks", doc.Title);
            Assert.Equal("Rock", doc.Genre);
        }

        [Fact]
        public async Task BasicDocumentAdditionFromCsvWithDelimiter()
        {
            var indexUID = nameof(BasicDocumentAdditionFromCsvWithDelimiter);
            var index = _client.Index(indexUID);

            var csvDocuments = await File.ReadAllTextAsync(Datasets.SongsCsvCustomDelimiterPath);
            var task = await index.AddDocumentsCsvAsync(csvDocuments, csvDelimiter: ';');
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check the documents have been added
            var docs = (await index.GetDocumentsAsync<DatasetSong>()).Results.ToList();
            Assert.NotEmpty(docs);
            var doc = docs.First();
            Assert.Equal("702481615", doc.Id);
            Assert.Equal("Armatage Shanks", doc.Title);
            Assert.Equal("Rock", doc.Genre);
        }

        [Fact]
        public async Task BasicDocumentAdditionFromNdjsonString()
        {
            var indexUID = nameof(BasicDocumentAdditionFromNdjsonString);
            var index = _client.Index(indexUID);

            var ndjsonDocuments = await File.ReadAllTextAsync(Datasets.SongsNdjsonPath);
            var task = await index.AddDocumentsNdjsonAsync(ndjsonDocuments);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check the documents have been added
            var docs = (await index.GetDocumentsAsync<DatasetSong>()).Results.ToList();
            Assert.NotEmpty(docs);
            var doc = docs.First();
            Assert.Equal("412559401", doc.Id);
            Assert.Equal("BEETHOVEN", doc.Title);
            Assert.Equal("Classical", doc.Genre);
        }

        [Fact]
        public async Task BasicDocumentsAdditionInBatches()
        {
            var indexUID = "BasicDocumentsAdditionInBatchesTest";
            var index = _client.Index(indexUID);

            // Add the documents
            Movie[] movies =
            {
                new Movie { Id = "1", Name = "Batman" }, new Movie { Id = "2", Name = "Reservoir Dogs" },
                new Movie { Id = "3", Name = "Taxi Driver" }, new Movie { Id = "4", Name = "Interstellar" },
                new Movie { Id = "5", Name = "Titanic" },
            };
            var tasks = await index.AddDocumentsInBatchesAsync(movies, 2);
            foreach (var u in tasks)
            {
                u.TaskUid.Should().BeGreaterOrEqualTo(0);
                await index.WaitForTaskAsync(u.TaskUid);
            }

            // Check the documents have been added (one movie from each batch)
            var docs = (await index.GetDocumentsAsync<Movie>()).Results.ToList();
            Assert.Equal("1", docs.ElementAt(0).Id);
            Assert.Equal("Batman", docs.ElementAt(0).Name);
            Assert.Equal("3", docs.ElementAt(2).Id);
            Assert.Equal("Taxi Driver", docs.ElementAt(2).Name);
        }

        [Fact]
        public async Task BasicDocumentAdditionFromCsvStringInBatches()
        {
            var indexUID = nameof(BasicDocumentAdditionFromCsvStringInBatches);
            var index = _client.Index(indexUID);

            var csvDocuments = await File.ReadAllTextAsync(Datasets.SongsCsvPath);
            var tasks = (await index.AddDocumentsCsvInBatchesAsync(csvDocuments, 250)).ToList();
            Assert.Equal(2, tasks.Count());
            foreach (var u in tasks)
            {
                u.TaskUid.Should().BeGreaterOrEqualTo(0);
                await index.WaitForTaskAsync(u.TaskUid);
            }

            // Check the documents have been added from first chunk
            var doc = await index.GetDocumentAsync<DatasetSong>("702481615");
            Assert.Equal("702481615", doc.Id);
            Assert.Equal("Armatage Shanks", doc.Title);
            Assert.Equal("Rock", doc.Genre);

            // Check the documents have been added from second chunk
            doc = await index.GetDocumentAsync<DatasetSong>("128391318");
            Assert.Equal("128391318", doc.Id);
            Assert.Equal("For What It's Worth", doc.Title);
            Assert.Equal("Rock", doc.Genre);
        }

        [Fact]
        public async Task BasicDocumentAdditionFromCsvWithDelimiterInBatches()
        {
            var indexUID = nameof(BasicDocumentAdditionFromCsvWithDelimiterInBatches);
            var index = _client.Index(indexUID);

            var csvDocuments = await File.ReadAllTextAsync(Datasets.SongsCsvCustomDelimiterPath);
            var tasks = (await index.AddDocumentsCsvInBatchesAsync(csvDocuments, 15, csvDelimiter: ';')).ToList();
            Assert.Equal(2, tasks.Count());
            foreach (var u in tasks)
            {
                u.TaskUid.Should().BeGreaterOrEqualTo(0);
                await index.WaitForTaskAsync(u.TaskUid);
            }

            // Check the documents have been added from first chunk
            var doc = await index.GetDocumentAsync<DatasetSong>("702481615");
            Assert.Equal("702481615", doc.Id);
            Assert.Equal("Armatage Shanks", doc.Title);
            Assert.Equal("Rock", doc.Genre);

            // Check the documents have been added from second chunk
            doc = await index.GetDocumentAsync<DatasetSong>("888221515");
            Assert.Equal("888221515", doc.Id);
            Assert.Equal("Old Folks", doc.Title);
            Assert.Equal("Jazz", doc.Genre);
        }

        [Fact]
        public async Task BasicDocumentAdditionFromNdjsonStringInBatches()
        {
            var indexUID = nameof(BasicDocumentAdditionFromNdjsonStringInBatches);
            var index = _client.Index(indexUID);

            var ndjsonDocuments = await File.ReadAllTextAsync(Datasets.SongsNdjsonPath);
            var tasks = (await index.AddDocumentsNdjsonInBatchesAsync(ndjsonDocuments, 150)).ToList();
            Assert.Equal(2, tasks.Count());
            foreach (var u in tasks)
            {
                u.TaskUid.Should().BeGreaterOrEqualTo(0);
                await index.WaitForTaskAsync(u.TaskUid);
            }

            // Check the documents have been added from first chunk
            var doc = await index.GetDocumentAsync<DatasetSong>("412559401");
            Assert.Equal("412559401", doc.Id);
            Assert.Equal("BEETHOVEN", doc.Title);
            Assert.Equal("Classical", doc.Genre);

            // Check the documents have been added from second chunk
            doc = await index.GetDocumentAsync<DatasetSong>("276177902");
            Assert.Equal("276177902", doc.Id);
            Assert.Equal("But The Shadow Marred The Master Plan", doc.Title);
            Assert.Equal("Jazz", doc.Genre);
        }

        [Fact]
        public async Task BasicDocumentsAdditionWithAlreadyCreatedIndex()
        {
            var indexUid = "BasicDocumentsAdditionWithAlreadyCreatedIndexTest";
            var task = await _client.CreateIndexAsync(indexUid);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await _client.Index(indexUid).WaitForTaskAsync(task.TaskUid);

            // Add the documents
            var index = _client.Index(indexUid);
            task = await index.AddDocumentsAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check the documents have been added
            var docs = await index.GetDocumentsAsync<Movie>();
            Assert.Equal("1", docs.Results.First().Id);
            Assert.Equal("Batman", docs.Results.First().Name);
            docs.Results.First().Genre.Should().BeNull();
        }

        [Fact]
        public async Task BasicDocumentsAdditionWithTimeoutError()
        {
            var indexUID = "BasicDocumentsAdditionWithTimeoutError";
            var index = _client.Index(indexUID);

            // Add the documents
            var task = await index.AddDocumentsAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => index.WaitForTaskAsync(task.TaskUid, 0));
        }

        [Fact]
        public async Task BasicDocumentsAdditionWithTimeoutErrorByInterval()
        {
            var indexUID = "BasicDocumentsAdditionWithTimeoutErrorByIntervalTest";
            var index = _client.Index(indexUID);

            // Add the documents
            var task = await index.AddDocumentsAsync(new[] { new Movie { Id = "1", Name = "Batman" } });
            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => index.WaitForTaskAsync(task.TaskUid, 0, 10));
        }

        [Fact]
        public async Task DocumentsAdditionWithPrimaryKey()
        {
            var indexUid = "DocumentsAdditionWithPrimaryKeyTest";
            var index = _client.Index(indexUid);
            index.PrimaryKey.Should().BeNull();

            // Add the documents
            var task = await index.AddDocumentsAsync(new[] { new { Key = "1", Name = "Ironman" } }, "key");
            await index.WaitForTaskAsync(task.TaskUid);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);

            // Check the primary key has been set
            await index.FetchPrimaryKey();
            Assert.Equal("key", index.PrimaryKey);
        }

        [Fact]
        public async Task BasicDocumentsUpdate()
        {
            var indexUID = "BasicDocumentsUpdateTest";
            var index = _client.Index(indexUID);

            // Add the documents
            var task = await index.AddDocumentsAsync(new[]
            {
                new Movie { Id = "1", Name = "Batman", Genre = "Action" },
                new Movie { Id = "2", Name = "Superman" },
            });
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Update the documents
            task = await index.UpdateDocumentsAsync(new[] { new Movie { Id = "1", Name = "Ironman" } });
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check the documents have been updated and added
            var docs = await index.GetDocumentsAsync<Movie>();
            var movieNames = docs.Results.Select(movie => movie.Name);

            // Ensure Genre didn't get removed
            docs.Results.First(x => x.Id == "1").Genre.Should().Be("Action");
            Assert.Contains("Ironman", movieNames);
            Assert.Contains("Superman", movieNames);
        }

        [Fact]
        public async Task BasicDocumentsUpdateFromJsonString()
        {
            var indexUID = nameof(BasicDocumentsUpdateFromJsonString);
            var index = _client.Index(indexUID);

            // Add the documents
            var task = await index.AddDocumentsAsync(new[]
            {
                new DatasetSmallMovie { Id = "287947", Title = "NOT A TITLE", Genre = "NO GENRE" },
            });
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Update the documents
            var jsonDocuments = await File.ReadAllTextAsync(Datasets.SmallMoviesJsonPath);
            task = await index.UpdateDocumentsJsonAsync(jsonDocuments);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check the documents have been updated and added
            var doc = await index.GetDocumentAsync<DatasetSmallMovie>("287947");
            Assert.Equal("287947", doc.Id);
            Assert.Equal("Shazam!", doc.Title);
            Assert.Equal("action", doc.Genre);
        }

        [Fact]
        public async Task BasicDocumentsUpdateFromCsvString()
        {
            var indexUID = nameof(BasicDocumentsUpdateFromCsvString);
            var index = _client.Index(indexUID);

            // Add the documents
            var task = await index.AddDocumentsAsync(new[]
            {
                new DatasetSong { Id = "702481615", Title = "NOT A TITLE", Genre = "NO GENRE" },
            });
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Update the documents
            var csvDocuments = await File.ReadAllTextAsync(Datasets.SongsCsvPath);
            task = await index.UpdateDocumentsCsvAsync(csvDocuments);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check the documents have been updated and added
            var doc = await index.GetDocumentAsync<DatasetSmallMovie>("702481615");
            Assert.Equal("702481615", doc.Id);
            Assert.Equal("Armatage Shanks", doc.Title);
            Assert.Equal("Rock", doc.Genre);
        }

        [Fact]
        public async Task BasicDocumentsUpdateFromNdjsonString()
        {
            var indexUID = nameof(BasicDocumentsUpdateFromNdjsonString);
            var index = _client.Index(indexUID);

            // Add the documents
            var task = await index.AddDocumentsAsync(new[]
            {
                new DatasetSong { Id = "412559401", Title = "NOT A TITLE", Genre = "NO GENRE" },
            });
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Update the documents
            var ndjsonDocuments = await File.ReadAllTextAsync(Datasets.SongsNdjsonPath);
            task = await index.UpdateDocumentsNdjsonAsync(ndjsonDocuments);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check the documents have been updated and added
            var doc = await index.GetDocumentAsync<DatasetSmallMovie>("412559401");
            Assert.Equal("412559401", doc.Id);
            Assert.Equal("BEETHOVEN", doc.Title);
            Assert.Equal("Classical", doc.Genre);
        }

        [Fact]
        public async Task BasicDocumentsUpdateInBatches()
        {
            var indexUID = "BasicDocumentsUpdateInBatchesTest";
            var index = _client.Index(indexUID);

            // Add the documents
            Movie[] movies =
            {
                new Movie { Id = "1", Name = "Batman" }, new Movie { Id = "2", Name = "Reservoir Dogs" },
                new Movie { Id = "3", Name = "Taxi Driver" }, new Movie { Id = "4", Name = "Interstellar" },
                new Movie { Id = "5", Name = "Titanic" },
            };
            var tasks = await index.AddDocumentsInBatchesAsync(movies, 2);
            foreach (var u in tasks)
            {
                u.TaskUid.Should().BeGreaterOrEqualTo(0);
                await index.WaitForTaskAsync(u.TaskUid);
            }

            movies = new Movie[]
            {
                new Movie { Id = "1", Name = "Batman", Genre = "Action" },
                new Movie { Id = "2", Name = "Reservoir Dogs", Genre = "Drama" },
                new Movie { Id = "3", Name = "Taxi Driver", Genre = "Drama" },
                new Movie { Id = "4", Name = "Interstellar", Genre = "Sci-Fi" },
                new Movie { Id = "5", Name = "Titanic", Genre = "Drama" },
            };
            tasks = await index.UpdateDocumentsInBatchesAsync(movies, 2);
            foreach (var u in tasks)
            {
                u.TaskUid.Should().BeGreaterOrEqualTo(0);
                await index.WaitForTaskAsync(u.TaskUid);
            }

            // Assert movies have genre after updating
            var docs = (await index.GetDocumentsAsync<Movie>()).Results.ToList();
            foreach (var movie in docs)
            {
                movie.Genre.Should().NotBeNull();
                movie.Genre.Should().NotBeEmpty();
            }
        }

        [Fact]
        public async Task BasicDocumentUpdateFromCsvStringInBatches()
        {
            var indexUID = nameof(BasicDocumentUpdateFromCsvStringInBatches);
            var index = _client.Index(indexUID);

            // Add the documents
            var task = await index.AddDocumentsAsync(new[]
            {
                new DatasetSong { Id = "702481615", Title = "NOT A TITLE", Genre = "NO GENRE" },
                new DatasetSong { Id = "128391318", Title = "NOT A TITLE", Genre = "NO GENRE" },
            });
            await index.WaitForTaskAsync(task.TaskUid);

            var csvDocuments = await File.ReadAllTextAsync(Datasets.SongsCsvPath);
            var tasks = (await index.UpdateDocumentsCsvInBatchesAsync(csvDocuments, 250)).ToList();
            Assert.Equal(2, tasks.Count());
            foreach (var u in tasks)
            {
                u.TaskUid.Should().BeGreaterOrEqualTo(0);
                await index.WaitForTaskAsync(u.TaskUid);
            }

            // Check the documents have been added from first chunk
            var doc = await index.GetDocumentAsync<DatasetSong>("702481615");
            Assert.Equal("702481615", doc.Id);
            Assert.Equal("Armatage Shanks", doc.Title);
            Assert.Equal("Rock", doc.Genre);

            // Check the documents have been added from second chunk
            doc = await index.GetDocumentAsync<DatasetSong>("128391318");
            Assert.Equal("128391318", doc.Id);
            Assert.Equal("For What It's Worth", doc.Title);
            Assert.Equal("Rock", doc.Genre);
        }

        [Fact]
        public async Task BasicDocumentUpdateFromNdjsonStringInBatches()
        {
            var indexUID = nameof(BasicDocumentUpdateFromNdjsonStringInBatches);
            var index = _client.Index(indexUID);

            // Add the documents
            var task = await index.AddDocumentsAsync(new[]
            {
                new DatasetSong { Id = "412559401", Title = "NOT A TITLE", Genre = "NO GENRE" },
                new DatasetSong { Id = "276177902", Title = "NOT A TITLE", Genre = "NO GENRE" },
            });
            await index.WaitForTaskAsync(task.TaskUid);

            var ndjsonDocuments = await File.ReadAllTextAsync(Datasets.SongsNdjsonPath);
            var tasks = (await index.UpdateDocumentsNdjsonInBatchesAsync(ndjsonDocuments, 150)).ToList();
            Assert.Equal(2, tasks.Count());
            foreach (var u in tasks)
            {
                u.TaskUid.Should().BeGreaterOrEqualTo(0);
                await index.WaitForTaskAsync(u.TaskUid);
            }

            // Check the documents have been added from first chunk
            var doc = await index.GetDocumentAsync<DatasetSong>("412559401");
            Assert.Equal("412559401", doc.Id);
            Assert.Equal("BEETHOVEN", doc.Title);
            Assert.Equal("Classical", doc.Genre);

            // Check the documents have been added from second chunk
            doc = await index.GetDocumentAsync<DatasetSong>("276177902");
            Assert.Equal("276177902", doc.Id);
            Assert.Equal("But The Shadow Marred The Master Plan", doc.Title);
            Assert.Equal("Jazz", doc.Genre);
        }

        [Fact]
        public async Task DocumentsUpdateWithPrimaryKey()
        {
            var indexUid = "DocumentsUpdateWithPrimaryKeyTest";
            var index = _client.Index(indexUid);
            index.PrimaryKey.Should().BeNull();

            // Add the documents
            var task = await index.UpdateDocumentsAsync(new[] { new { Key = "1", Name = "Ironman" } }, "key");
            await index.WaitForTaskAsync(task.TaskUid);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);

            // Check the primary key has been set
            await index.FetchPrimaryKey();
            Assert.Equal("key", index.PrimaryKey);
        }

        [Fact]
        public async Task GetOneExistingDocumentWithStringId()
        {
            var index = await _fixture.SetUpBasicIndex("GetOneExistingDocumentWithStringIdTest");
            var documents = await index.GetDocumentAsync<Movie>("10");
            documents.Id.Should().Be("10");
        }

        [Fact]
        public async Task GetOneExistingDocumentWithIntegerId()
        {
            var index = await _fixture.SetUpBasicIndexWithIntId("GetOneExistingDocumentWithIntegerIdTest");
            var documents = await index.GetDocumentAsync<MovieWithIntId>(10);
            documents.Id.Should().Be(10);
        }

        [Fact]
        public async Task GetOneExistingDocumentWithField()
        {
            var index = await _fixture.SetUpBasicIndex("GetOneExistingDocumentWithStringIdTest");
            var documents = await index.GetDocumentAsync<Movie>("10", new List<string> { "name" });
            documents.Id.Should().BeNull();
            documents.Name.Should().Be("Gladiator");
        }

        [Fact]
        public async Task GetOneExistingDocumentWithMultipleFields()
        {
            var index = await _fixture.SetUpBasicIndex("GetOneExistingDocumentWithStringIdTest");
            var documents = await index.GetDocumentAsync<Movie>("10", new List<string> { "name", "id" });
            documents.Id.Should().Be("10");
            documents.Name.Should().Be("Gladiator");
            documents.Genre.Should().BeNull();
        }

        [Fact]
        public async Task GetMultipleExistingDocuments()
        {
            var index = await _fixture.SetUpBasicIndex("GetMultipleExistingDocumentTest");
            var documents = await index.GetDocumentsAsync<Movie>();
            Assert.Equal(7, documents.Results.Count());
            documents.Results.First().Id.Should().Be("10");
            documents.Results.Last().Id.Should().Be("16");
        }

        [Fact]
        public async Task GetMultipleExistingDocumentsWithQuery()
        {
            var index = await _fixture.SetUpBasicIndex("GetMultipleExistingDocumentWithQueryTest");
            var taskUpdate = await index.UpdateFilterableAttributesAsync(new[] { "genre" });
            taskUpdate.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(taskUpdate.TaskUid);

            var documents = await index.GetDocumentsAsync<Movie>(new DocumentsQuery() { Filter = "genre = 'SF'" });
            Assert.Equal(2, documents.Results.Count());
            documents.Results.Should().ContainSingle(x => x.Id == "12");
            documents.Results.Should().ContainSingle(x => x.Id == "13");
        }

        [Fact]
        public async Task GetMultipleExistingDocumentsWithLimit()
        {
            var index = await _fixture.SetUpBasicIndex("GetMultipleExistingDocumentWithLimitTest");
            var documents = await index.GetDocumentsAsync<Movie>(new DocumentsQuery() { Limit = 2 });
            Assert.Equal(2, documents.Results.Count());
            documents.Results.First().Id.Should().Be("10");
            documents.Results.Last().Id.Should().Be("11");
        }

        [Fact]
        public async Task GetMultipleExistingDocumentsWithField()
        {
            var index = await _fixture.SetUpBasicIndex("GetMultipleExistingDocumentWithLimitTest");
            var documents =
                await index.GetDocumentsAsync<Movie>(new DocumentsQuery()
                {
                    Limit = 2,
                    Fields = new List<string> { "id" }
                });
            Assert.Equal(2, documents.Results.Count());
            documents.Results.First().Id.Should().Be("10");
            documents.Results.First().Name.Should().BeNull();
            documents.Results.Last().Id.Should().Be("11");
        }

        [Fact]
        public async Task GetMultipleExistingDocumentsWithMultipleFields()
        {
            var index = await _fixture.SetUpBasicIndex("GetMultipleExistingDocumentWithLimitTest");
            var documents =
                await index.GetDocumentsAsync<Movie>(new DocumentsQuery()
                {
                    Limit = 2,
                    Fields = new List<string> { "id", "name" }
                });
            Assert.Equal(2, documents.Results.Count());
            documents.Results.First().Id.Should().Be("10");
            documents.Results.First().Name.Should().Be("Gladiator");
            documents.Results.Last().Id.Should().Be("11");
        }

        [Fact]
        public async Task DeleteOneExistingDocumentWithStringId()
        {
            var index = await _fixture.SetUpBasicIndex("DeleteOneExistingDocumentWithStringIdTest");

            // Delete the document
            var task = await index.DeleteOneDocumentAsync("11");
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check the document has been deleted
            var docs = await index.GetDocumentsAsync<Movie>();
            Assert.Equal(6, docs.Results.Count());
            var ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<Movie>("11"));
            Assert.Equal("document_not_found", ex.Code);
        }

        [Fact]
        public async Task DeleteOneExistingDocumentWithIntId()
        {
            var index = await _fixture.SetUpBasicIndexWithIntId("DeleteOneExistingDocumentWithIntIdTest");

            // Delete the document
            var task = await index.DeleteOneDocumentAsync(11);
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check the document has been deleted
            var docs = await index.GetDocumentsAsync<MovieWithIntId>();
            Assert.Equal(6, docs.Results.Count());
            var ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<MovieWithIntId>(11));
            Assert.Equal("document_not_found", ex.Code);
        }

        [Fact]
        public async Task DeleteMultipleDocumentsWithStringId()
        {
            var index = await _fixture.SetUpBasicIndex("DeleteMultipleDocumentsWithStringIdTest");

            // Delete the documents
            var task = await index.DeleteDocumentsAsync(new[] { "12", "13", "14" });
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check the documents have been deleted
            var docs = await index.GetDocumentsAsync<Movie>();
            Assert.Equal(4, docs.Results.Count());
            MeilisearchApiError ex;
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<Movie>("12"));
            Assert.Equal("document_not_found", ex.Code);
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<Movie>("13"));
            Assert.Equal("document_not_found", ex.Code);
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<Movie>("14"));
            Assert.Equal("document_not_found", ex.Code);
        }

        [Fact]
        public async Task DeleteMultipleDocumentsWithIntegerId()
        {
            var index = await _fixture.SetUpBasicIndexWithIntId("DeleteMultipleDocumentsWithIntegerIdTest");

            // Delete the documents
            var task = await index.DeleteDocumentsAsync(new[] { 12, 13, 14 });
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check the documents have been deleted
            var docs = await index.GetDocumentsAsync<MovieWithIntId>();
            Assert.Equal(4, docs.Results.Count());
            MeilisearchApiError ex;
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<MovieWithIntId>("12"));
            Assert.Equal("document_not_found", ex.Code);
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<MovieWithIntId>("13"));
            Assert.Equal("document_not_found", ex.Code);
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<MovieWithIntId>("14"));
            Assert.Equal("document_not_found", ex.Code);
        }

        [Fact]
        public async Task DeleteMultipleDocumentsByFilter()
        {
            var index = await _fixture.SetUpBasicIndex("DeleteMultipleDocumentsByFilterTest");
            var taskUpdate = await index.UpdateFilterableAttributesAsync(new[] { "genre" });
            taskUpdate.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(taskUpdate.TaskUid);

            // Delete the documents
            var task = await index.DeleteDocumentsAsync(new DeleteDocumentsQuery() { Filter = "genre = SF" });
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check the documents have been deleted
            var docs = await index.GetDocumentsAsync<Movie>();
            Assert.Equal(5, docs.Results.Count());
            MeilisearchApiError ex;
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<Movie>("12"));
            Assert.Equal("document_not_found", ex.Code);
            ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => index.GetDocumentAsync<Movie>("13"));
            Assert.Equal("document_not_found", ex.Code);
        }

        [Fact]
        public async Task DeleteAllExistingDocuments()
        {
            var index = await _fixture.SetUpBasicIndex("DeleteAllExistingDocumentsTest");

            // Delete all the documents
            var task = await index.DeleteAllDocumentsAsync();
            task.TaskUid.Should().BeGreaterOrEqualTo(0);
            await index.WaitForTaskAsync(task.TaskUid);

            // Check all the documents have been deleted
            var docs = await index.GetDocumentsAsync<Movie>();
            docs.Results.Should().BeEmpty();
        }
    }
}

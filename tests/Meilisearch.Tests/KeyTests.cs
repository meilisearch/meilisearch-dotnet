using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class KeyTests<TFixture> : IAsyncLifetime where TFixture : IndexFixture
    {
        private readonly TFixture _fixture;
        private readonly MeilisearchClient _client;

        public KeyTests(TFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.DefaultClient;
        }

        public async Task InitializeAsync()
        {
            await _fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetAllKeys()
        {
            var keyResponse = await _client.GetKeysAsync();
            var keys = keyResponse.Results;

            keys.Count().Should().BeGreaterOrEqualTo(2);
        }

        [Fact]
        public async Task GetOneKeyUsingKeyUid()
        {
            var keyResponse = await _client.GetKeysAsync();
            var keys = keyResponse.Results;
            var firstKey = keys.First();

            var fetchedKey = await _client.GetKeyAsync(firstKey.KeyUid);

            Assert.Equal(fetchedKey.KeyUid, firstKey.KeyUid);
            Assert.Equal(fetchedKey.Description, firstKey.Description);
            Assert.Equal(fetchedKey.Indexes, firstKey.Indexes);
            Assert.Equal(fetchedKey.Actions, firstKey.Actions);
            Assert.Equal(fetchedKey.ExpiresAt, firstKey.ExpiresAt);
            Assert.Equal(fetchedKey.CreatedAt, firstKey.CreatedAt);
            Assert.Equal(fetchedKey.UpdatedAt, firstKey.UpdatedAt);
        }

        [Fact]
        public async Task CreateOneKey()
        {
            var keyOptions = new Key(DateTime.Parse("2042-04-02T00:42:42Z"))
            {
                Description = "Key to add document to all indexes.",
                Actions = new string[] { "documents.add" },
                Indexes = new string[] { "*" }
            };
            var createdKey = await _client.CreateKeyAsync(keyOptions);
            var createdKeyUid = createdKey.KeyUid;
            var fetchedKey = await _client.GetKeyAsync(createdKeyUid);

            Assert.Equal(fetchedKey.KeyUid, createdKey.KeyUid);
            Assert.Equal(fetchedKey.Description, createdKey.Description);
            Assert.Equal(fetchedKey.Indexes, createdKey.Indexes);
            Assert.Equal(fetchedKey.Actions, createdKey.Actions);
            Assert.Equal(fetchedKey.ExpiresAt, createdKey.ExpiresAt);
            Assert.Equal(fetchedKey.CreatedAt, createdKey.CreatedAt);
            Assert.Equal(fetchedKey.UpdatedAt, createdKey.UpdatedAt);
        }

        [Fact]
        public async Task CreateOneKeyWithNullExpiresAt()
        {
            var keyOptions = new Key(null)
            {
                Description = "Key to add document to all indexes.",
                Actions = new string[] { "documents.add" },
                Indexes = new string[] { "*" }
            };
            var createdKey = await _client.CreateKeyAsync(keyOptions);
            var createdKeyUid = createdKey.KeyUid;
            var fetchedKey = await _client.GetKeyAsync(createdKeyUid);

            Assert.Equal(fetchedKey.KeyUid, createdKey.KeyUid);
            Assert.Equal(fetchedKey.Description, createdKey.Description);
            Assert.Equal(fetchedKey.Indexes, createdKey.Indexes);
            Assert.Equal(fetchedKey.Actions, createdKey.Actions);
            Assert.Equal(fetchedKey.ExpiresAt, createdKey.ExpiresAt);
            Assert.Equal(fetchedKey.CreatedAt, createdKey.CreatedAt);
            Assert.Equal(fetchedKey.UpdatedAt, createdKey.UpdatedAt);
        }

        [Theory]
        [InlineData("NewDesc", null, null, null)]
        [InlineData(null, new[] { "documents.add", "search" }, null, null)]
        [InlineData(null, null, new[] { "TestIdx" }, null)]
        [InlineData(null, null, null, "2100-01-01")]
        [InlineData("NewDesc", new[] { "search" }, new[] { "*" }, "2100-01-01")]
        public async Task UpdateOneKey(string description, string[] actions, string[] indexes, string expiresAt)
        {
            var keyOptions = new Key(null)
            {
                Description = "Key to add document to all indexes.",
                Actions = new string[] { "documents.add" },
                Indexes = new string[] { "*" },
            };
            var createdKey = await _client.CreateKeyAsync(keyOptions);
            var createdKeyUid = createdKey.KeyUid;

            keyOptions.Description = description;
            keyOptions.Actions = actions;
            keyOptions.Indexes = indexes;
            if (expiresAt != null)
            {
                keyOptions.SetExpiresAt(DateTime.Parse(expiresAt));
            }
            else
            {
                keyOptions.SetExpiresAt(null);
            }

            var updatedKey = await _client.UpdateKeyAsync(createdKeyUid, keyOptions);
            var fetchedKey = await _client.GetKeyAsync(createdKeyUid);

            Assert.Equal(fetchedKey.KeyUid, updatedKey.KeyUid);
            Assert.Equal(fetchedKey.Description, updatedKey.Description);
            Assert.Equal(fetchedKey.Indexes, updatedKey.Indexes);
            Assert.Equal(fetchedKey.Actions, updatedKey.Actions);
            Assert.Equal(fetchedKey.ExpiresAt, updatedKey.ExpiresAt);
            Assert.Equal(fetchedKey.CreatedAt, updatedKey.CreatedAt);
            Assert.Equal(fetchedKey.UpdatedAt, updatedKey.UpdatedAt);
        }

        [Fact]
        public async Task DeleteOneKey()
        {
            var keyOptions = new Key(null)
            {
                Description = "Key to delete document to all indexes.",
                Actions = new string[] { "documents.delete" },
                Indexes = new string[] { "*" },
            };
            var createdKey = await _client.CreateKeyAsync(keyOptions);
            var createdKeyUid = createdKey.KeyUid;

            var success = await _client.DeleteKeyAsync(createdKeyUid);
            success.Should().BeTrue();

            var ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => _client.GetKeyAsync(createdKeyUid));
            Assert.Equal("api_key_not_found", ex.Code);
        }
    }
}

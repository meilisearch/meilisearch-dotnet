using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Meilisearch.QueryParameters;

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
        public async Task GetMultipleKeys()
        {
            var keyResponse = await _client.GetKeysAsync();
            var keys = keyResponse.Results;

            keys.Count().Should().BeGreaterOrEqualTo(2);
        }

        [Fact]
        public async Task GetMultipleKeysWithLimit()
        {
            var keys = await _client.GetKeysAsync(new KeysQuery { Limit = 1 });

            keys.Results.Count().Should().BeGreaterOrEqualTo(1);
            Assert.Equal(1, keys.Limit);
        }

        [Fact]
        public async Task GetMultipleKeysWithOffset()
        {
            var keys = await _client.GetKeysAsync(new KeysQuery { Offset = 1 });

            keys.Results.Count().Should().BeGreaterOrEqualTo(1);
            Assert.Equal(1, keys.Offset);
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
        public async Task GetOneKeyUsingUid()
        {
            var keyResponse = await _client.GetKeysAsync();
            var firstKey = keyResponse.Results.First();

            var fetchedKey = await _client.GetKeyAsync(firstKey.Uid);

            Assert.Equal(fetchedKey.KeyUid, firstKey.KeyUid);
            Assert.Equal(fetchedKey.Uid, firstKey.Uid);
            Assert.Equal(fetchedKey.Name, firstKey.Name);
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
            var keyOptions = new Key
            {
                Name = "AddDocumentToAllIndexes",
                Description = "Key to add document to all indexes.",
                Actions = new string[] { "documents.add" },
                Indexes = new string[] { "*" },
                ExpiresAt = DateTime.Parse("2042-04-02T00:42:42Z"),
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
            var keyOptions = new Key
            {
                Description = "Key to add document to all indexes.",
                Actions = new string[] { "documents.add" },
                Indexes = new string[] { "*" },
                ExpiresAt = null,
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
        public async Task CreateOneKeyWithUid()
        {
            try { await _client.DeleteKeyAsync("9cd7a335-5b9c-4312-be16-7f1fcf7fd250"); }
            catch { }

            var keyOptions = new Key
            {
                Uid = "9cd7a335-5b9c-4312-be16-7f1fcf7fd250",
                Description = "Key to add document to all indexes.",
                Actions = new string[] { "documents.add" },
                Indexes = new string[] { "*" },
                ExpiresAt = null,
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
        public async Task UpdateKey()
        {
            var keyOptions = new Key
            {
                Description = "Key to add document to all indexes.",
                Name = "key alias",
                Actions = new string[] { "documents.add" },
                Indexes = new string[] { "*" },
                ExpiresAt = null,
            };
            var createdKey = await _client.CreateKeyAsync(keyOptions);
            var fetchedKey = await _client.UpdateKeyAsync(createdKey.KeyUid, description: "my new Description");

            Assert.Equal(fetchedKey.KeyUid, createdKey.KeyUid);
            Assert.Equal("my new Description", fetchedKey.Description);
            Assert.Equal("key alias", createdKey.Name);

            var newerKey = await _client.UpdateKeyAsync(createdKey.KeyUid, name: "new name");

            Assert.Equal("my new Description", newerKey.Description);
            Assert.Equal("new name", newerKey.Name);
        }

        [Fact]
        public async Task DeleteOneKey()
        {
            var keyOptions = new Key
            {
                Description = "Key to delete document to all indexes.",
                Actions = new string[] { "documents.delete" },
                Indexes = new string[] { "*" },
                ExpiresAt = null,
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

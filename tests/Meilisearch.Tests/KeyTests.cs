namespace Meilisearch.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    [Collection("Sequential")]
    public class KeyTests : IAsyncLifetime
    {
        private IndexFixture fixture;
        private MeilisearchClient client;

        public KeyTests(IndexFixture fixture)
        {
            this.fixture = fixture;
            this.client = fixture.DefaultClient;
        }

        public async Task InitializeAsync()
        {
            await this.fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetAllKeys()
        {
            var keyResponse = await this.client.GetKeysAsync();
            var keys = keyResponse.Results;

            keys.Count().Should().BeGreaterOrEqualTo(2);
        }

        [Fact]
        public async Task GetOneKeyUsingKeyUid()
        {
            var keyResponse = await this.client.GetKeysAsync();
            var keys = keyResponse.Results;
            var firstKey = keys.First();

            var fetchedKey = await this.client.GetKeyAsync(firstKey.KeyUid);

            fetchedKey.KeyUid.Should().Equals(firstKey.KeyUid);
            fetchedKey.Description.Should().Equals(firstKey.Description);
            fetchedKey.Indexes.Should().Equals(firstKey.Indexes);
            fetchedKey.Actions.Should().Equals(firstKey.Actions);
            fetchedKey.ExpiresAt.Should().Equals(firstKey.ExpiresAt);
            fetchedKey.CreatedAt.Should().Equals(firstKey.CreatedAt);
            fetchedKey.UpdatedAt.Should().Equals(firstKey.UpdatedAt);
        }

        [Fact]
        public async Task CreateOneKey()
        {
            Key keyOptions = new Key
            {
                Description = "Key to add document to all indexes.",
                Actions = new string[] { "documents.add" },
                Indexes = new string[] { "*" },
                ExpiresAt = DateTime.Parse("2042-04-02T00:42:42Z"),
            };
            Key createdKey = await this.client.CreateKeyAsync(keyOptions);
            var createdKeyUid = createdKey.KeyUid;
            var fetchedKey = await this.client.GetKeyAsync(createdKeyUid);

            fetchedKey.KeyUid.Should().Equals(createdKey.KeyUid);
            fetchedKey.Description.Should().Equals(createdKey.Description);
            fetchedKey.Indexes.Should().Equals(createdKey.Indexes);
            fetchedKey.Actions.Should().Equals(createdKey.Actions);
            fetchedKey.ExpiresAt.Should().Equals(createdKey.ExpiresAt);
            fetchedKey.CreatedAt.Should().Equals(createdKey.CreatedAt);
            fetchedKey.UpdatedAt.Should().Equals(createdKey.UpdatedAt);
        }

        [Fact]
        public async Task CreateOneKeyWithNullExpiresAt()
        {
            Key keyOptions = new Key
            {
                Description = "Key to add document to all indexes.",
                Actions = new string[] { "documents.add" },
                Indexes = new string[] { "*" },
                ExpiresAt = null,
            };
            Key createdKey = await this.client.CreateKeyAsync(keyOptions);
            var createdKeyUid = createdKey.KeyUid;
            var fetchedKey = await this.client.GetKeyAsync(createdKeyUid);

            fetchedKey.KeyUid.Should().Equals(createdKey.KeyUid);
            fetchedKey.Description.Should().Equals(createdKey.Description);
            fetchedKey.Indexes.Should().Equals(createdKey.Indexes);
            fetchedKey.Actions.Should().Equals(createdKey.Actions);
            fetchedKey.ExpiresAt.Should().Equals(createdKey.ExpiresAt);
            fetchedKey.CreatedAt.Should().Equals(createdKey.CreatedAt);
            fetchedKey.UpdatedAt.Should().Equals(createdKey.UpdatedAt);
        }

        [Fact]
        public async Task DeleteOneKey()
        {
            Key keyOptions = new Key
            {
                Description = "Key to delete document to all indexes.",
                Actions = new string[] { "documents.delete" },
                Indexes = new string[] { "*" },
                ExpiresAt = null,
            };
            Key createdKey = await this.client.CreateKeyAsync(keyOptions);
            var createdKeyUid = createdKey.KeyUid;

            var success = await this.client.DeleteKeyAsync(createdKeyUid);
            success.Should().BeTrue();

            MeilisearchApiError ex = await Assert.ThrowsAsync<MeilisearchApiError>(() => this.client.GetKeyAsync(createdKeyUid));
            Assert.Equal("api_key_not_found", ex.Code);
        }
    }
}

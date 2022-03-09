using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class TenantTokenTests<TFixture> : IAsyncLifetime where TFixture : IndexFixture
    {
        private TenantTokenRules _searchRules = new TenantTokenRules(new string[] { "*" });

        private readonly TFixture _fixture;
        private JwtBuilder _builder;
        private Index _basicIndex;
        private readonly MeilisearchClient _client;
        private readonly string _indexName = "books";
        private string _key;

        public TenantTokenTests(TFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.DefaultClient;
            _key = Guid.NewGuid().ToString();
        }

        public async Task InitializeAsync()
        {
            await _fixture.DeleteAllIndexes();
            _basicIndex = await _fixture.SetUpBasicIndex(_indexName);
            _builder = JwtBuilder
                .Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .MustVerifySignature();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public void DoesNotGenerateASignedTokenWithoutAKey()
        {
            Assert.Throws<MeilisearchTenantTokenApiKeyInvalid>(
                () => TenantToken.GenerateToken(_searchRules, null, null)
            );
        }

        [Fact]
        public void SignsTokenWithGivenKey()
        {
            var token = TenantToken.GenerateToken(_searchRules, _key, null);

            Assert.Throws<SignatureVerificationException>(
                () => _builder.WithSecret("other-key").Decode(token)
            );

            _builder.WithSecret(_key).Decode(token);
        }

        [Fact]
        public void GeneratesTokenWithExpiresAt()
        {
            var expiration = DateTimeOffset.UtcNow.AddDays(1).DateTime;
            var token = TenantToken.GenerateToken(_searchRules, _key, expiration);

            _builder.WithSecret(_key).Decode(token);
        }

        [Fact]
        public void ThrowsExceptionWhenExpiresAtIsInThePast()
        {
            var expiresAt = new DateTime(1995, 12, 20);

            Assert.Throws<MeilisearchTenantTokenExpired>(
                () => TenantToken.GenerateToken(_searchRules, _key, expiresAt)
            );
        }

        [Fact]
        public void ContainsValidClaims()
        {
            var token = TenantToken.GenerateToken(_searchRules, _key, null);

            var claims = _builder.WithSecret(_key).Decode<IDictionary<string, object>>(token);

            Assert.Equal(claims["apiKeyPrefix"], _key.Substring(0, 8));
            Assert.Equal(claims["searchRules"], _searchRules.ToClaim());
        }

        [Fact]
        public void ClientDecodesSuccessfullyUsingApiKeyFromInstance()
        {
            var token = _client.GenerateTenantToken(_searchRules);

            _builder.WithSecret(_client.ApiKey).Decode(token);
        }

        [Fact]
        public void ClientDecodesSuccessfullyUsingApiKeyFromArgument()
        {
            var token = _client.GenerateTenantToken(_searchRules, apiKey: _key);

            _builder.WithSecret(_key).Decode(token);
        }

        [Fact]
        public void ClientThrowsIfNoKeyIsAvailable()
        {
            var customClient = new MeilisearchClient(_fixture.MeilisearchAddress);

            Assert.Throws<MeilisearchTenantTokenApiKeyInvalid>(
                () => customClient.GenerateTenantToken(_searchRules)
            );
        }

        [Theory]
        [MemberData(nameof(PossibleSearchRules))]
        public async void SearchesSuccessfullyWithTheNewToken(dynamic data)
        {
            var keyOptions = new Key
            {
                Description = "Key generate a tenant token",
                Actions = new string[] { "*" },
                Indexes = new string[] { "*" },
                ExpiresAt = null,
            };
            var createdKey = await _client.CreateKeyAsync(keyOptions);
            var admClient = new MeilisearchClient(_fixture.MeilisearchAddress, createdKey.KeyUid);
            var task = await admClient.Index(_indexName).UpdateFilterableAttributesAsync(new string[] { "tag", "book_id" });
            await admClient.Index(_indexName).WaitForTaskAsync(task.Uid);

            var token = admClient.GenerateTenantToken(new TenantTokenRules(data));
            var customClient = new MeilisearchClient(_fixture.MeilisearchAddress, token);

            await customClient.Index(_indexName).SearchAsync<Movie>(string.Empty);
        }

        public static IEnumerable<object[]> PossibleSearchRules()
        {
            // {'*': {}}
            yield return new object[] { new Dictionary<string, object> { { "*", new Dictionary<string, object> { } } } };
            // {'books': {}}
            yield return new object[] { new Dictionary<string, object> { { "books", new Dictionary<string, object> { } } } };
            // {'*': null}
            yield return new object[] { new Dictionary<string, object> { { "*", null } } };
            // {'books': null}
            yield return new object[] { new Dictionary<string, object> { { "books", null } } };
            // ['*']
            yield return new object[] { new string[] { "*" } };
            // ['books']
            yield return new object[] { new string[] { "books" } };
            // {'*': {"filter": 'tag = Tale'}}
            yield return new object[] { new Dictionary<string, object> { { "*", new Dictionary<string, object> { { "filter", "tag = Tale" } } } } };
            // {'books': {"filter": 'tag = Tale'}}
            yield return new object[] { new Dictionary<string, object> { { "books", new Dictionary<string, object> { { "filter", "tag = Tale" } } } } };
        }
    }
}

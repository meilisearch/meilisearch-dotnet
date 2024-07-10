using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Meilisearch.Tests
{
    public abstract class TenantTokenTests<TFixture> : IAsyncLifetime
        where TFixture : IndexFixture
    {
        private readonly TenantTokenRules _searchRules = new TenantTokenRules(new string[] { "*" });

        private readonly TFixture _fixture;
        private Index _basicIndex;
        private readonly MeilisearchClient _client;
        private readonly string _indexName = "books";
        private readonly string _uid;
        private readonly string _key;

        public TenantTokenTests(TFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.DefaultClient;
            _uid = Guid.NewGuid().ToString();
            _key = Guid.NewGuid().ToString();
        }

        public async Task InitializeAsync()
        {
            await _fixture.DeleteAllIndexes();
            _basicIndex = await _fixture.SetUpBasicIndex(_indexName);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public void DoesNotGenerateASignedTokenWithoutAKey()
        {
            Assert.Throws<MeilisearchTenantTokenApiKeyInvalid>(
                () => TenantToken.GenerateToken(_uid, _searchRules, null, null)
            );
        }

        [Fact]
        public void DoesNotGenerateASignedTokenWithoutAUid()
        {
            Assert.Throws<MeilisearchTenantTokenApiKeyUidInvalid>(
                () => TenantToken.GenerateToken(null, _searchRules, _key, null)
            );
        }

        [Fact]
        public void ThrowsExceptionWhenExpiresAtIsInThePast()
        {
            var expiresAt = new DateTime(1995, 12, 20);

            Assert.Throws<MeilisearchTenantTokenExpired>(
                () => TenantToken.GenerateToken(_uid, _searchRules, _key, expiresAt)
            );
        }

        [Fact]
        public void ClientThrowsIfNoKeyIsAvailable()
        {
            var customClient = new MeilisearchClient(_fixture.MeilisearchAddress());

            Assert.Throws<MeilisearchTenantTokenApiKeyInvalid>(
                () => customClient.GenerateTenantToken(_uid, _searchRules)
            );
        }

        
        [Fact]
        public void ClientThrowsIfKeyIsLessThan128Bits()
        {
            var customClient = new MeilisearchClient(_fixture.MeilisearchAddress(), "masterKey");            
            Assert.Throws<MeilisearchTenantTokenApiKeyInvalid>(
                () => customClient.GenerateTenantToken(_uid, _searchRules)
            );
        }



        [Theory]
        [MemberData(nameof(PossibleSearchRules))]
        public async void SearchesSuccessfullyWithTheNewToken(object data)
        {
            var keyOptions = new Key
            {
                Description = "Key generate a tenant token",
                Actions = new KeyAction[] { KeyAction.All },
                Indexes = new string[] { "*" },
                ExpiresAt = null,
            };
            var createdKey = await _client.CreateKeyAsync(keyOptions);
            var admClient = new MeilisearchClient(_fixture.MeilisearchAddress(), createdKey.KeyUid);
            var task = await admClient
                .Index(_indexName)
                .UpdateFilterableAttributesAsync(new string[] { "tag", "book_id" });
            await admClient.Index(_indexName).WaitForTaskAsync(task.TaskUid);

            TenantTokenRules tokenRules;
            if (data is string[] dataStringArray)
            {
                tokenRules = new TenantTokenRules(dataStringArray);
            }
            else if (data is IReadOnlyDictionary<string, object> dataDictionary)
            {
                tokenRules = new TenantTokenRules(dataDictionary);
            }
            else
            {
                throw new Exception("Invalid data type");
            }

            var token = admClient.GenerateTenantToken(createdKey.Uid, tokenRules);
            var customClient = new MeilisearchClient(_fixture.MeilisearchAddress(), token);

            await customClient.Index(_indexName).SearchAsync<Movie>(string.Empty);
        }

        [Fact]
        public async Task SearchFailsWhenTokenIsExpired()
        {
            var keyOptions = new Key
            {
                Description = "Key generate a tenant token",
                Actions = new KeyAction[] { KeyAction.All },
                Indexes = new string[] { "*" },
                ExpiresAt = null,
            };
            var createdKey = await _client.CreateKeyAsync(keyOptions);
            var admClient = new MeilisearchClient(_fixture.MeilisearchAddress(), createdKey.KeyUid);

            var token = admClient.GenerateTenantToken(
                createdKey.Uid,
                new TenantTokenRules(new[] { "*" }),
                expiresAt: DateTime.UtcNow.AddSeconds(1)
            );
            var customClient = new MeilisearchClient(_fixture.MeilisearchAddress(), token);
            Thread.Sleep(TimeSpan.FromSeconds(2));

            await Assert.ThrowsAsync<MeilisearchApiError>(
                async () => await customClient.Index(_indexName).SearchAsync<Movie>(string.Empty)
            );
        }

        [Fact]
        public async void SearchSucceedsWhenTokenIsNotExpired()
        {
            var keyOptions = new Key
            {
                Description = "Key generate a tenant token",
                Actions = new KeyAction[] { KeyAction.All },
                Indexes = new string[] { "*" },
                ExpiresAt = null,
            };
            var createdKey = await _client.CreateKeyAsync(keyOptions);
            var admClient = new MeilisearchClient(_fixture.MeilisearchAddress(), createdKey.KeyUid);

            var token = admClient.GenerateTenantToken(
                createdKey.Uid,
                new TenantTokenRules(new[] { "*" }),
                expiresAt: DateTime.UtcNow.AddMinutes(1)
            );
            var customClient = new MeilisearchClient(_fixture.MeilisearchAddress(), token);
            await customClient.Index(_indexName).SearchAsync<Movie>(string.Empty);
        }

        public static TheoryData<object> PossibleSearchRules()
        {
            IEnumerable<object> SubPossibleSearchRules()
            {
                // {'*': {}}
                yield return new Dictionary<string, object>
                {
                    {
                        "*",
                        new Dictionary<string, object> { }
                    }
                };
                // {'books': {}}
                yield return new Dictionary<string, object>
                {
                    {
                        "books",
                        new Dictionary<string, object> { }
                    }
                };
                // {'*': null}
                yield return new Dictionary<string, object> { { "*", null } };
                // {'books': null}
                yield return new Dictionary<string, object> { { "books", null } };
                // ['*']
                yield return new string[] { "*" };
                // ['books']
                yield return new string[] { "books" };
                // {'*': {"filter": 'tag = Tale'}}
                yield return new Dictionary<string, object>
                {
                    {
                        "*",
                        new Dictionary<string, object> { { "filter", "tag = Tale" } }
                    }
                };
                // {'books': {"filter": 'tag = Tale'}}
                yield return new Dictionary<string, object>
                {
                    {
                        "books",
                        new Dictionary<string, object> { { "filter", "tag = Tale" } }
                    }
                };
            }
            return new TheoryData<object>(SubPossibleSearchRules());
        }
    }
}

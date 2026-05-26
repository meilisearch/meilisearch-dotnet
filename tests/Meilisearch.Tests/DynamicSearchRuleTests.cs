using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Meilisearch.QueryParameters;

using Xunit;

namespace Meilisearch.Tests
{
    public class DynamicSearchRuleTests<TFixture> : IAsyncLifetime where TFixture : DynamicSearchRuleFixture
    {
        private readonly MeilisearchClient _client;

        private readonly TFixture _fixture;

        public DynamicSearchRuleTests(TFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.DefaultClient;
        }

        public Task InitializeAsync() => _fixture.InitializeAsync();

        public Task DisposeAsync() => _fixture.DisposeAsync();

        [Fact]
        public async Task CreateDynamicSearchRuleAsync()
        {
            const string dynamicSearchRuleUid = nameof(CreateDynamicSearchRuleAsync);
            var dynamicSearchRule = new PatchDynamicSearchRule
            {
                Description = "Black Friday 2025 rules",
                Priority = 10,
                Active = true,
                Conditions = new BaseCondition[]
                {
                    new QueryCondition { IsEmpty = true },
                    new TimeCondition
                    {
                        Start = new DateTimeOffset(2025, 11, 28, 0, 0, 0, TimeSpan.Zero),
                        End = new DateTimeOffset(2025, 11, 28, 23, 59, 59, TimeSpan.FromHours(3))
                    },
                },
                Actions = new[]
                {
                    new DSRAction
                    {
                        Selector = new DSRASelector { IndexUid = "products", Id = "123" },
                        Action = new PinAction { Position = 1 }
                    }
                }
            };

            var result = await _client.CreateOrUpdateDynamicSearchRuleAsync(dynamicSearchRuleUid, dynamicSearchRule);

            AssertDynamicSearchRule(dynamicSearchRule.ToDynamicSearchRule(dynamicSearchRuleUid), result);
        }

        [Fact]
        public async Task CreateDynamicSearchRuleWithoutActionsAsync()
        {
            const string dynamicSearchRuleUid = nameof(CreateDynamicSearchRuleWithoutActionsAsync);
            var dynamicSearchRule = new PatchDynamicSearchRule();

            var exception = await Assert.ThrowsAsync<MeilisearchApiError>(() => _client.CreateOrUpdateDynamicSearchRuleAsync(dynamicSearchRuleUid, dynamicSearchRule));
            Assert.Equal("invalid_dynamic_search_rule_actions", exception.Code);
        }

        [Fact]
        public async Task CreateDynamicSearchRuleWithEmptyActionsAsync()
        {
            const string dynamicSearchRuleUid = nameof(CreateDynamicSearchRuleWithEmptyActionsAsync);
            var dynamicSearchRule = new PatchDynamicSearchRule
            {
                Actions = Array.Empty<DSRAction>(),
            };

            var exception = await Assert.ThrowsAsync<MeilisearchApiError>(() => _client.CreateOrUpdateDynamicSearchRuleAsync(dynamicSearchRuleUid, dynamicSearchRule));
            Assert.Equal("invalid_dynamic_search_rule_actions", exception.Code);
        }

        [Fact]
        public async Task UpdateDynamicSearchRuleAsync()
        {
            const string dynamicSearchRuleUid = nameof(UpdateDynamicSearchRuleAsync);
            var (patchRule, _) = await _fixture.SetUpDynamicSearchRuleExampleAsync(dynamicSearchRuleUid);

            // modify dynamic-search-rule
            patchRule.Active = false;
            patchRule.Description = "Some updated description";

            var result = await _client.CreateOrUpdateDynamicSearchRuleAsync(dynamicSearchRuleUid, patchRule);
            AssertDynamicSearchRule(patchRule.ToDynamicSearchRule(dynamicSearchRuleUid), result);
        }

        [Theory]
        [MemberData(nameof(GetExistingDynamicSearchRuleCases))]
        public async Task GetExistingDynamicSearchRuleAsync(string dynamicSearchRuleJsonPath)
        {
            const string dynamicSearchRuleUid = nameof(GetExistingDynamicSearchRuleAsync);
            var (_, dynamicSearchRule) = await _fixture.SetUpDynamicSearchRuleAsync(dynamicSearchRuleUid, dynamicSearchRuleJsonPath);

            var result = await _client.GetDynamicSearchRuleAsync(dynamicSearchRuleUid);
            AssertDynamicSearchRule(dynamicSearchRule, result);
        }

        [Fact]
        public async Task GetNotExistingDynamicSearchRuleAsync()
        {
            var exception = await Assert.ThrowsAsync<MeilisearchApiError>(() =>
                _client.GetDynamicSearchRuleAsync(nameof(GetNotExistingDynamicSearchRuleAsync)));

            Assert.Equal("dynamic_search_rule_not_found", exception.Code);
        }

        [Fact]
        public async Task ListDynamicSearchRulesWithEmptyQuery()
        {
            const string dynamicSearchRuleUid = nameof(ListDynamicSearchRulesWithEmptyQuery);
            var (_, dynamicSearchRule) = await _fixture.SetUpDynamicSearchRuleExampleAsync(dynamicSearchRuleUid);

            var resourceResults = await _client.ListDynamicSearchRulesAsync();

            var results = resourceResults.Results.ToList();
            Assert.Equal(0, resourceResults.Offset);
            Assert.Equal(1, resourceResults.Total);
            Assert.Single(results);
            AssertDynamicSearchRule(dynamicSearchRule, results.First());
        }

        [Fact]
        public async Task ListDynamicSearchRulesWithOffset()
        {
            const string dynamicSearchRuleUid = nameof(ListDynamicSearchRulesWithOffset);
            await _fixture.SetUpDynamicSearchRuleExampleAsync(dynamicSearchRuleUid);

            var resourceResults = await _client.ListDynamicSearchRulesAsync(new DynamicSearchRulesQuery { Offset = 1 });

            Assert.Equal(1, resourceResults.Offset);
            Assert.Equal(1, resourceResults.Total);
            Assert.Empty(resourceResults.Results);
        }

        [Fact]
        public async Task ListDynamicSearchRulesWithLimit()
        {
            const string dynamicSearchRuleUid = nameof(ListDynamicSearchRulesWithLimit);
            var (_, dynamicSearchRule) = await _fixture.SetUpDynamicSearchRuleExampleAsync(dynamicSearchRuleUid);

            var resourceResults = await _client.ListDynamicSearchRulesAsync(new DynamicSearchRulesQuery { Limit = 1 });

            var results = resourceResults.Results.ToList();
            Assert.Equal(1, resourceResults.Limit);
            Assert.Equal(0, resourceResults.Offset);
            Assert.Equal(1, resourceResults.Total);
            Assert.Single(results);
            AssertDynamicSearchRule(dynamicSearchRule, results.First());
        }

        [Fact]
        public async Task DeleteExistingDynamicSearchRuleAsync()
        {
            const string dynamicSearchRuleUid = nameof(DeleteExistingDynamicSearchRuleAsync);
            await _fixture.SetUpDynamicSearchRuleExampleAsync(dynamicSearchRuleUid);

            var result = await _client.DeleteDynamicSearchRuleAsync(dynamicSearchRuleUid);
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteNotExistingDynamicSearchRuleAsync()
        {
            var exception = await Assert.ThrowsAsync<MeilisearchApiError>(() => _client.DeleteDynamicSearchRuleAsync(nameof(DeleteNotExistingDynamicSearchRuleAsync)));

            Assert.Equal("dynamic_search_rule_not_found", exception.Code);
        }

        private static void AssertDynamicSearchRule(DynamicSearchRule expected, DynamicSearchRule actual)
        {
            Assert.Equal(expected.Uid, actual.Uid);
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.Priority, actual.Priority);
            Assert.Equal(expected.Active, actual.Active);
            if (expected.Conditions != null)
            {
                Assert.NotNull(actual.Conditions);
                Assert.Equal(expected.Conditions.Count(), actual.Conditions.Count());
                Assert.Equivalent(expected.Conditions, actual.Conditions);
            } else Assert.Null(actual.Conditions);

            if (expected.Actions != null)
            {
                Assert.NotNull(actual.Actions);
                Assert.Equal(expected.Actions.Count(), actual.Actions.Count());

                foreach (var (expectedAction, actualAction) in expected.Actions.Zip(actual.Actions))
                {
                    Assert.Equivalent(expectedAction.Selector, actualAction.Selector);
                    Assert.Equivalent(expectedAction.Action, actualAction.Action);
                }
            } else Assert.Null(actual.Actions);
        }

        public static TheoryData<string> GetExistingDynamicSearchRuleCases() =>
            new TheoryData<string>(
                Datasets.DynamicSearchRuleJsonPath,
                Datasets.DynamicSearchRuleWithOptionalValuesJsonPath
            );
    }

    internal static class DynamicSearchRuleExtensions
    {
        public static DynamicSearchRule ToDynamicSearchRule(this PatchDynamicSearchRule patchRule, string uid)
        {
            var result = new DynamicSearchRule { Uid = uid };
            if (patchRule.Priority.HasValue)
                result.Priority = patchRule.Priority.Value;
            if (patchRule.Active.HasValue)
                result.Active = patchRule.Active.Value ?? true;
            if (patchRule.Description.HasValue)
                result.Description = patchRule.Description.Value;
            if (patchRule.Actions.HasValue)
                result.Actions = patchRule.Actions.Value;
            if (patchRule.Conditions.HasValue)
                result.Conditions = patchRule.Conditions.Value;
            return result;
        }
    }
}

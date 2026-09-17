using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using Meilisearch.QueryParameters;
using Meilisearch.Tests.Fixtures;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class DynamicSearchRuleTests<TFixture> : IAsyncLifetime where TFixture : DynamicSearchRuleFixture
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
            var patchRule = CreateDynamicSearchRulePatch();

            var taskInfo = await _client.CreateOrUpdateDynamicSearchRuleAsync(dynamicSearchRuleUid, patchRule);
            await WaitForTaskSucceededAsync(taskInfo);

            var result = await _client.GetDynamicSearchRuleAsync(dynamicSearchRuleUid);
            AssertPatchApplied(patchRule, dynamicSearchRuleUid, result);
            AssertDynamicSearchRule(patchRule.ToDynamicSearchRule(dynamicSearchRuleUid), result);
        }

        [Fact]
        public async Task CreateDynamicSearchRuleWithoutActionsAsync()
        {
            const string dynamicSearchRuleUid = nameof(CreateDynamicSearchRuleWithoutActionsAsync);
            var dynamicSearchRule = new PatchDynamicSearchRule();

            var taskInfo = await _client.CreateOrUpdateDynamicSearchRuleAsync(dynamicSearchRuleUid, dynamicSearchRule);
            await WaitForTaskSucceededAsync(taskInfo);

            var result = await _client.GetDynamicSearchRuleAsync(dynamicSearchRuleUid);
            Assert.Equal(dynamicSearchRuleUid, result.Uid);
        }

        [Fact]
        public async Task CreateDynamicSearchRuleWithEmptyActionsAsync()
        {
            const string dynamicSearchRuleUid = nameof(CreateDynamicSearchRuleWithEmptyActionsAsync);
            var dynamicSearchRule = new PatchDynamicSearchRule
            {
                Actions = Array.Empty<DSRAction>(),
            };

            var taskInfo = await _client.CreateOrUpdateDynamicSearchRuleAsync(dynamicSearchRuleUid, dynamicSearchRule);
            await WaitForTaskSucceededAsync(taskInfo);

            var result = await _client.GetDynamicSearchRuleAsync(dynamicSearchRuleUid);
            Assert.Equal(dynamicSearchRuleUid, result.Uid);
            Assert.True(result.Actions == null || !result.Actions.Any());
        }

        [Fact]
        public async Task UpdateDynamicSearchRuleAsync()
        {
            const string dynamicSearchRuleUid = nameof(UpdateDynamicSearchRuleAsync);
            var (_, originalRule) = await _fixture.SetUpDynamicSearchRuleExampleAsync(dynamicSearchRuleUid);

            // modify dynamic-search-rule
            var patchRule = new PatchDynamicSearchRule { Active = false };
            patchRule.Description = "Some updated description";

            var taskInfo = await _client.CreateOrUpdateDynamicSearchRuleAsync(dynamicSearchRuleUid, patchRule);
            await WaitForTaskSucceededAsync(taskInfo);

            var result = await _client.GetDynamicSearchRuleAsync(dynamicSearchRuleUid);
            Assert.Equal(dynamicSearchRuleUid, result.Uid);
            Assert.Equal(patchRule.Description.Value, result.Description);
            Assert.Equal(patchRule.Active.Value ?? true, result.Active);
            Assert.Equal(originalRule.Precedence, result.Precedence);
            AssertJsonEquivalent(originalRule.Conditions, result.Conditions);
            AssertJsonEquivalent(originalRule.Actions, result.Actions);
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

            var taskInfo = await _client.DeleteDynamicSearchRuleAsync(dynamicSearchRuleUid);
            await WaitForTaskSucceededAsync(taskInfo);

            var exception = await Assert.ThrowsAsync<MeilisearchApiError>(() =>
                _client.GetDynamicSearchRuleAsync(dynamicSearchRuleUid));
            Assert.Equal("dynamic_search_rule_not_found", exception.Code);
        }

        [Fact]
        public async Task DeleteNotExistingDynamicSearchRuleAsync()
        {
            var taskInfo = await _client.DeleteDynamicSearchRuleAsync(nameof(DeleteNotExistingDynamicSearchRuleAsync));
            await WaitForTaskSucceededAsync(taskInfo);
        }

        private async Task WaitForTaskSucceededAsync(TaskInfo taskInfo)
        {
            Assert.NotNull(taskInfo);
            Assert.True(taskInfo.TaskUid > 0);

            var finishedTask = await _client.WaitForTaskAsync(taskInfo.TaskUid, timeoutMs: 10000);
            Assert.Equal(TaskInfoStatus.Succeeded, finishedTask.Status);
        }

        private static PatchDynamicSearchRule CreateDynamicSearchRulePatch()
        {
            return new PatchDynamicSearchRule
            {
                Description = "Black Friday 2025 rules",
                Precedence = 10,
                Active = true,
                Conditions = new DynamicSearchRuleConditions
                {
                    Query = new QueryCondition { IsEmpty = false, Words = "black friday" },
                    Time = new TimeCondition
                    {
                        Start = new DateTimeOffset(2025, 11, 28, 0, 0, 0, TimeSpan.Zero),
                        End = new DateTimeOffset(2025, 11, 28, 23, 59, 59, TimeSpan.Zero)
                    }
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
        }

        private static void AssertPatchApplied(PatchDynamicSearchRule expected, string expectedUid, DynamicSearchRule actual)
        {
            Assert.Equal(expectedUid, actual.Uid);
            if (expected.Description.HasValue)
                Assert.Equal(expected.Description.Value, actual.Description);
            if (expected.Precedence.HasValue)
                Assert.Equal(expected.Precedence.Value, actual.Precedence);
            if (expected.Active.HasValue)
                Assert.Equal(expected.Active.Value ?? true, actual.Active);
            if (expected.Conditions.HasValue)
                Assert.Equivalent(expected.Conditions.Value, actual.Conditions);
            if (expected.Actions.HasValue)
                AssertJsonEquivalent(expected.Actions.Value, actual.Actions);
        }

        private static void AssertDynamicSearchRule(DynamicSearchRule expected, DynamicSearchRule actual)
        {
            Assert.Equal(expected.Uid, actual.Uid);
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.Precedence, actual.Precedence);
            Assert.Equal(expected.Active, actual.Active);
            Assert.Equivalent(expected.Conditions, actual.Conditions);
            AssertJsonEquivalent(expected.Actions, actual.Actions);
        }

        private static void AssertJsonEquivalent(object expected, object actual)
        {
            var expectedNode = JsonSerializer.SerializeToNode(expected, Constants.JsonSerializerOptionsRemoveNulls);
            var actualNode = JsonSerializer.SerializeToNode(actual, Constants.JsonSerializerOptionsRemoveNulls);

            Assert.True(
                JsonNode.DeepEquals(expectedNode, actualNode),
                $"Expected JSON: {expectedNode}{Environment.NewLine}Actual JSON: {actualNode}");
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
            if (patchRule.Precedence.HasValue)
                result.Precedence = patchRule.Precedence.Value;
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

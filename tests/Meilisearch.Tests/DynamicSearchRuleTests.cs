using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using Meilisearch.QueryParameters;
using Meilisearch.Tests.Fixtures;

using Xunit;

namespace Meilisearch.Tests
{
    public class DynamicSearchRuleSerializationTests
    {
        [Fact]
        public void SerializesObjectShapedConditionsAndArbitraryFilterValues()
        {
            var patch = new PatchDynamicSearchRule
            {
                Precedence = 10,
                Conditions = new DynamicSearchRuleConditions
                {
                    Query = new QueryCondition { IsEmpty = false, Words = "red shirt" },
                    Filter = new FilterCondition
                    {
                        Values = new Dictionary<string, object>
                        {
                            ["color"] = "red",
                            ["stock"] = 5,
                            ["featured"] = true,
                            ["category"] = new[] { "shirt", "sale" },
                            ["metadata"] = new Dictionary<string, object> { ["region"] = "jp" },
                            ["nullable"] = null,
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(patch, Constants.JsonSerializerOptionsRemoveNulls);
            using (var document = JsonDocument.Parse(json))
            {
                var root = document.RootElement;

                Assert.Equal(10UL, root.GetProperty("precedence").GetUInt64());
                Assert.False(root.TryGetProperty("priority", out _));

                var conditions = root.GetProperty("conditions");
                Assert.Equal(JsonValueKind.Object, conditions.ValueKind);

                var query = conditions.GetProperty("query");
                Assert.Equal(JsonValueKind.String, query.GetProperty("words").ValueKind);
                Assert.Equal("red shirt", query.GetProperty("words").GetString());
                Assert.False(query.TryGetProperty("scope", out _));

                var values = conditions.GetProperty("filter").GetProperty("values");
                Assert.Equal("red", values.GetProperty("color").GetString());
                Assert.Equal(5, values.GetProperty("stock").GetInt32());
                Assert.True(values.GetProperty("featured").GetBoolean());
                Assert.Equal(JsonValueKind.Array, values.GetProperty("category").ValueKind);
                Assert.Equal(JsonValueKind.Object, values.GetProperty("metadata").ValueKind);
                Assert.Equal(JsonValueKind.Null, values.GetProperty("nullable").ValueKind);
            }
        }

        [Fact]
        public void DistinguishesOmittedAndExplicitlyNullConditions()
        {
            var omittedJson = JsonSerializer.Serialize(
                new PatchDynamicSearchRule(),
                Constants.JsonSerializerOptionsRemoveNulls);
            using (var omittedDocument = JsonDocument.Parse(omittedJson))
            {
                Assert.False(omittedDocument.RootElement.TryGetProperty("conditions", out _));
            }

            var resetJson = JsonSerializer.Serialize(
                new PatchDynamicSearchRule { Conditions = (DynamicSearchRuleConditions)null },
                Constants.JsonSerializerOptionsRemoveNulls);
            using (var resetDocument = JsonDocument.Parse(resetJson))
            {
                Assert.Equal(JsonValueKind.Null, resetDocument.RootElement.GetProperty("conditions").ValueKind);
            }
        }

        [Fact]
        public void SerializesListFilterQuery()
        {
            var query = new DynamicSearchRulesQuery
            {
                Filter = new DynamicSearchRulesQuery.DSRQFilter
                {
                    Query = "black friday",
                    Active = true,
                }
            };

            var json = JsonSerializer.Serialize(query, Constants.JsonSerializerOptionsRemoveNulls);
            using (var document = JsonDocument.Parse(json))
            {
                var filter = document.RootElement.GetProperty("filter");

                Assert.Equal("black friday", filter.GetProperty("query").GetString());
                Assert.True(filter.GetProperty("active").GetBoolean());
                Assert.False(filter.TryGetProperty("attributePatterns", out _));
                Assert.False(filter.TryGetProperty("attribute_patterns", out _));
            }
        }

        [Fact]
        public void DeserializesLastUpdatedAtWithSubsecondPrecision()
        {
            const string json =
                "{\"uid\":\"black-friday\",\"lastUpdatedAt\":\"2026-07-27T06:47:12.123456789Z\"}";

            var rule = JsonSerializer.Deserialize<DynamicSearchRule>(json);

            var expected = new DateTimeOffset(2026, 7, 27, 6, 47, 12, TimeSpan.Zero).AddTicks(1234567);
            Assert.Equal(expected, rule.LastUpdatedAt);
        }

        [Theory]
        [InlineData("{\"uid\":\"missing\"}")]
        [InlineData("{\"uid\":\"null\",\"lastUpdatedAt\":null}")]
        public void DeserializesNullableLastUpdatedAt(string json)
        {
            var rule = JsonSerializer.Deserialize<DynamicSearchRule>(json);

            Assert.Null(rule.LastUpdatedAt);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void SelectorRequiresDocumentId(string id)
        {
            Assert.Throws<ArgumentNullException>(() => new DSRASelector("products", id));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void SelectorRequiresIndexUid(string indexUid)
        {
            Assert.Throws<ArgumentNullException>(() => new DSRASelector(indexUid, "123"));
        }
    }

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
            var dynamicSearchRule = CreateDynamicSearchRulePatch();

            var task = await _client.CreateOrUpdateDynamicSearchRuleAsync(dynamicSearchRuleUid, dynamicSearchRule);
            await AssertTaskSucceededAsync(task, TaskInfoType.DsrUpdate);
            var result = await _client.GetDynamicSearchRuleAsync(dynamicSearchRuleUid);

            AssertPatchApplied(dynamicSearchRule, dynamicSearchRuleUid, result);
            AssertLastUpdatedAtMatchesTask(result, task);
        }

        [Fact]
        public async Task CreateDynamicSearchRuleWithEmptyActionsAsync()
        {
            const string dynamicSearchRuleUid = nameof(CreateDynamicSearchRuleWithEmptyActionsAsync);
            var dynamicSearchRule = new PatchDynamicSearchRule
            {
                Actions = Array.Empty<DSRAction>(),
            };

            var task = await _client.CreateOrUpdateDynamicSearchRuleAsync(dynamicSearchRuleUid, dynamicSearchRule);
            await AssertTaskSucceededAsync(task, TaskInfoType.DsrUpdate);
            var result = await _client.GetDynamicSearchRuleAsync(dynamicSearchRuleUid);

            Assert.Empty(result.Actions);
            AssertLastUpdatedAtMatchesTask(result, task);
        }

        [Fact]
        public async Task UpdateDynamicSearchRuleAsync()
        {
            const string dynamicSearchRuleUid = nameof(UpdateDynamicSearchRuleAsync);
            var (_, originalRule) = await _fixture.SetUpDynamicSearchRuleExampleAsync(dynamicSearchRuleUid);
            var patchRule = new PatchDynamicSearchRule
            {
                Active = false,
                Description = "Some updated description",
            };

            var task = await _client.CreateOrUpdateDynamicSearchRuleAsync(dynamicSearchRuleUid, patchRule);
            await AssertTaskSucceededAsync(task, TaskInfoType.DsrUpdate);
            var result = await _client.GetDynamicSearchRuleAsync(dynamicSearchRuleUid);

            Assert.False(result.Active);
            Assert.Equal("Some updated description", result.Description);
            Assert.Equal(originalRule.Precedence, result.Precedence);
            AssertJsonEquivalent(originalRule.Conditions, result.Conditions);
            AssertJsonEquivalent(originalRule.Actions, result.Actions);
            Assert.NotEqual(originalRule.LastUpdatedAt, result.LastUpdatedAt);
            AssertLastUpdatedAtMatchesTask(result, task);
        }

        [Fact]
        public async Task UpdateDynamicSearchRuleFilterConditionAsync()
        {
            const string dynamicSearchRuleUid = nameof(UpdateDynamicSearchRuleFilterConditionAsync);
            await _fixture.SetUpDynamicSearchRuleExampleAsync(dynamicSearchRuleUid);
            var patchRule = new PatchDynamicSearchRule
            {
                Conditions = new DynamicSearchRuleConditions
                {
                    Filter = new FilterCondition
                    {
                        Values = new Dictionary<string, object>
                        {
                            ["color"] = "blue",
                            ["price"] = 19.99,
                        }
                    }
                }
            };

            var task = await _client.CreateOrUpdateDynamicSearchRuleAsync(dynamicSearchRuleUid, patchRule);
            await AssertTaskSucceededAsync(task, TaskInfoType.DsrUpdate);
            var result = await _client.GetDynamicSearchRuleAsync(dynamicSearchRuleUid);

            Assert.Null(result.Conditions.Query);
            Assert.Null(result.Conditions.Time);
            AssertJsonEquivalent(patchRule.Conditions.Value.Filter, result.Conditions.Filter);
            AssertLastUpdatedAtMatchesTask(result, task);
        }

        [Fact]
        public async Task ResetDynamicSearchRuleConditionsAsync()
        {
            const string dynamicSearchRuleUid = nameof(ResetDynamicSearchRuleConditionsAsync);
            await _fixture.SetUpDynamicSearchRuleExampleAsync(dynamicSearchRuleUid);
            var patchRule = new PatchDynamicSearchRule
            {
                Conditions = (DynamicSearchRuleConditions)null,
            };

            var task = await _client.CreateOrUpdateDynamicSearchRuleAsync(dynamicSearchRuleUid, patchRule);
            await AssertTaskSucceededAsync(task, TaskInfoType.DsrUpdate);
            var result = await _client.GetDynamicSearchRuleAsync(dynamicSearchRuleUid);

            Assert.NotNull(result.Conditions);
            Assert.Null(result.Conditions.Query);
            Assert.Null(result.Conditions.Time);
            Assert.Null(result.Conditions.Filter);
            AssertLastUpdatedAtMatchesTask(result, task);
        }

        [Theory]
        [MemberData(nameof(GetExistingDynamicSearchRuleCases))]
        public async Task GetExistingDynamicSearchRuleAsync(string dynamicSearchRuleJsonPath)
        {
            const string dynamicSearchRuleUid = nameof(GetExistingDynamicSearchRuleAsync);
            var (patchRule, dynamicSearchRule) = await _fixture.SetUpDynamicSearchRuleAsync(
                dynamicSearchRuleUid,
                dynamicSearchRuleJsonPath);

            var result = await _client.GetDynamicSearchRuleAsync(dynamicSearchRuleUid);

            AssertPatchApplied(patchRule, dynamicSearchRuleUid, result);
            AssertDynamicSearchRule(dynamicSearchRule, result);
            Assert.NotNull(result.LastUpdatedAt);
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
            AssertDynamicSearchRule(dynamicSearchRule, results[0]);
        }

        [Fact]
        public async Task ListDynamicSearchRulesWithQueryFilter()
        {
            const string dynamicSearchRuleUid = nameof(ListDynamicSearchRulesWithQueryFilter);
            const string nonMatchingUid = nameof(ListDynamicSearchRulesWithQueryFilter) + "-non-matching";
            await _fixture.SetUpDynamicSearchRuleExampleAsync(dynamicSearchRuleUid);
            var nonMatchingRule = new PatchDynamicSearchRule
            {
                Description = "Summer sale rules",
                Conditions = new DynamicSearchRuleConditions
                {
                    Query = new QueryCondition { IsEmpty = false, Words = "summer sale" }
                },
                Actions = Array.Empty<DSRAction>(),
            };
            var createTask = await _client.CreateOrUpdateDynamicSearchRuleAsync(nonMatchingUid, nonMatchingRule);
            await AssertTaskSucceededAsync(createTask, TaskInfoType.DsrUpdate);
            var query = new DynamicSearchRulesQuery
            {
                Filter = new DynamicSearchRulesQuery.DSRQFilter { Query = "black friday" }
            };

            var resourceResults = await _client.ListDynamicSearchRulesAsync(query);

            var result = Assert.Single(resourceResults.Results);
            Assert.Equal(1, resourceResults.Total);
            Assert.Equal(dynamicSearchRuleUid, result.Uid);
            Assert.NotNull(result.LastUpdatedAt);
        }

        [Fact]
        public async Task ListDynamicSearchRulesWithPagination()
        {
            const string dynamicSearchRuleUid = nameof(ListDynamicSearchRulesWithPagination);
            await _fixture.SetUpDynamicSearchRuleExampleAsync(dynamicSearchRuleUid);

            var resourceResults = await _client.ListDynamicSearchRulesAsync(
                new DynamicSearchRulesQuery { Offset = 1, Limit = 1 });

            Assert.Equal(1, resourceResults.Offset);
            Assert.Equal(1, resourceResults.Limit);
            Assert.Equal(1, resourceResults.Total);
            Assert.Empty(resourceResults.Results);
        }

        [Fact]
        public async Task DeleteExistingDynamicSearchRuleAsync()
        {
            const string dynamicSearchRuleUid = nameof(DeleteExistingDynamicSearchRuleAsync);
            await _fixture.SetUpDynamicSearchRuleExampleAsync(dynamicSearchRuleUid);

            var task = await _client.DeleteDynamicSearchRuleAsync(dynamicSearchRuleUid);
            await AssertTaskSucceededAsync(task, TaskInfoType.DsrUpdate);

            var exception = await Assert.ThrowsAsync<MeilisearchApiError>(() =>
                _client.GetDynamicSearchRuleAsync(dynamicSearchRuleUid));
            Assert.Equal("dynamic_search_rule_not_found", exception.Code);
        }

        [Fact]
        public async Task DeleteNotExistingDynamicSearchRuleAsync()
        {
            var task = await _client.DeleteDynamicSearchRuleAsync(
                nameof(DeleteNotExistingDynamicSearchRuleAsync));

            await AssertTaskSucceededAsync(task, TaskInfoType.DsrUpdate);
        }

        [Fact]
        public async Task DeleteAllDynamicSearchRulesAsync()
        {
            await _fixture.SetUpDynamicSearchRuleExampleAsync(
                $"{nameof(DeleteAllDynamicSearchRulesAsync)}-first");
            await _fixture.SetUpDynamicSearchRuleExampleAsync(
                $"{nameof(DeleteAllDynamicSearchRulesAsync)}-second");

            var task = await _client.DeleteAllDynamicSearchRulesAsync();
            await AssertTaskSucceededAsync(task, TaskInfoType.DsrClear);
            var result = await _client.ListDynamicSearchRulesAsync();

            Assert.Empty(result.Results);
            Assert.Equal(0, result.Total);
        }

        private async Task AssertTaskSucceededAsync(TaskInfo task, TaskInfoType expectedType)
        {
            Assert.Equal(expectedType, task.Type);

            var finishedTask = await _client.WaitForTaskAsync(task.TaskUid, timeoutMs: 10000);
            Assert.Equal(TaskInfoStatus.Succeeded, finishedTask.Status);
            Assert.Equal(expectedType, finishedTask.Type);
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
                    Query = new QueryCondition { IsEmpty = false, Words = "red shirt" },
                    Time = new TimeCondition
                    {
                        Start = new DateTimeOffset(2025, 11, 28, 0, 0, 0, TimeSpan.Zero),
                        End = new DateTimeOffset(2025, 11, 28, 23, 59, 59, TimeSpan.FromHours(3))
                    },
                    Filter = new FilterCondition
                    {
                        Values = new Dictionary<string, object>
                        {
                            ["color"] = "red",
                            ["stock"] = 5,
                            ["featured"] = true,
                            ["category"] = new[] { "shirt", "sale" },
                            ["metadata"] = new Dictionary<string, object> { ["region"] = "jp" },
                            ["nullable"] = null,
                        }
                    }
                },
                Actions = new[]
                {
                    new DSRAction
                    {
                        Selector = new DSRASelector("products", "123"),
                        Action = new PinAction { Position = 1 }
                    }
                }
            };
        }

        private static void AssertPatchApplied(
            PatchDynamicSearchRule expected,
            string expectedUid,
            DynamicSearchRule actual)
        {
            Assert.Equal(expectedUid, actual.Uid);
            if (expected.Description.HasValue)
                Assert.Equal(expected.Description.Value, actual.Description);
            if (expected.Precedence.HasValue)
                Assert.Equal(expected.Precedence.Value, actual.Precedence);
            if (expected.Active.HasValue)
                Assert.Equal(expected.Active.Value ?? true, actual.Active);
            if (expected.Conditions.HasValue)
                AssertJsonEquivalent(expected.Conditions.Value, actual.Conditions);
            if (expected.Actions.HasValue)
                AssertJsonEquivalent(expected.Actions.Value, actual.Actions);
        }

        private static void AssertDynamicSearchRule(DynamicSearchRule expected, DynamicSearchRule actual)
        {
            Assert.Equal(expected.Uid, actual.Uid);
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.LastUpdatedAt, actual.LastUpdatedAt);
            Assert.Equal(expected.Precedence, actual.Precedence);
            Assert.Equal(expected.Active, actual.Active);
            AssertJsonEquivalent(expected.Conditions, actual.Conditions);
            AssertJsonEquivalent(expected.Actions, actual.Actions);
        }

        private static void AssertLastUpdatedAtMatchesTask(DynamicSearchRule rule, TaskInfo task)
        {
            Assert.NotNull(rule.LastUpdatedAt);
            Assert.Equal(new DateTimeOffset(task.EnqueuedAt.ToUniversalTime()), rule.LastUpdatedAt);
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
}

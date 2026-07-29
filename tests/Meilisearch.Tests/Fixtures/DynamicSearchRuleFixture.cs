using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Xunit;

namespace Meilisearch.Tests.Fixtures
{
    public abstract class DynamicSearchRuleFixture : MeilisearchClientFixture
    {
        public override async Task InitializeAsync() =>
            Assert.True(await DefaultClient.EnableDynamicSearchRules());

        public override async Task DisposeAsync() => await DeleteAllDynamicSearchRulesAsync();

        public Task<(PatchDynamicSearchRule, DynamicSearchRule)> SetUpDynamicSearchRuleExampleAsync(string uid) =>
            SetUpDynamicSearchRuleAsync(uid, Datasets.DynamicSearchRuleJsonPath);

        public async Task<(PatchDynamicSearchRule, DynamicSearchRule)> SetUpDynamicSearchRuleAsync(string uid, string jsonPath)
        {
            var dsrJson = await File.ReadAllTextAsync(Datasets.GetDynamicSearchRuleJsonPath(jsonPath));
            var dynamicSearchRule = JsonSerializer.Deserialize<PatchDynamicSearchRule>(dsrJson, Constants.JsonSerializerOptionsRemoveNulls);
            var task = await DefaultClient.CreateOrUpdateDynamicSearchRuleAsync(uid, dynamicSearchRule);
            var finishedTask = await DefaultClient.WaitForTaskAsync(task.TaskUid, timeoutMs: 10000);
            Assert.Equal(TaskInfoStatus.Succeeded, finishedTask.Status);

            return (dynamicSearchRule, await DefaultClient.GetDynamicSearchRuleAsync(uid));
        }

        public async Task DeleteAllDynamicSearchRulesAsync()
        {
            var allDynamicSearchRules = await DefaultClient.ListDynamicSearchRulesAsync();
            if (allDynamicSearchRules.Results.Any())
            {
                var task = await DefaultClient.DeleteAllDynamicSearchRulesAsync();
                var finishedTask = await DefaultClient.WaitForTaskAsync(task.TaskUid, timeoutMs: 10000);
                Assert.Equal(TaskInfoStatus.Succeeded, finishedTask.Status);
            }
        }
    }
}

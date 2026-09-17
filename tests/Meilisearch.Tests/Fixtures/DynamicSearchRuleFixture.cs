using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Meilisearch.QueryParameters;

using Xunit;

namespace Meilisearch.Tests.Fixtures
{
    public abstract class DynamicSearchRuleFixture : MeilisearchClientFixture
    {
        public override async Task InitializeAsync() =>
            Assert.True(await DefaultClient.EnableDynamicSearchRules());

        public override async Task DisposeAsync() => await DeleteAllDynamicSearchRule();

        public Task<(PatchDynamicSearchRule, DynamicSearchRule)> SetUpDynamicSearchRuleExampleAsync(string uid) =>
            SetUpDynamicSearchRuleAsync(uid, Datasets.DynamicSearchRuleJsonPath);

        public async Task<(PatchDynamicSearchRule, DynamicSearchRule)> SetUpDynamicSearchRuleAsync(string uid, string jsonPath)
        {
            var dsrJson = await File.ReadAllTextAsync(Datasets.GetDynamicSearchRuleJsonPath(jsonPath));
            var patchRule = JsonSerializer.Deserialize<PatchDynamicSearchRule>(dsrJson, Constants.JsonSerializerOptionsRemoveNulls);
            await WaitForTaskSucceededAsync(await DefaultClient.CreateOrUpdateDynamicSearchRuleAsync(uid, patchRule));
            return (patchRule, await DefaultClient.GetDynamicSearchRuleAsync(uid));
        }

        public async Task DeleteAllDynamicSearchRule()
        {
            const int pageSize = 100;

            while (true)
            {
                var allDynamicSearchRules = await DefaultClient.ListDynamicSearchRulesAsync(new DynamicSearchRulesQuery { Limit = pageSize });
                if (!allDynamicSearchRules.Results.Any()) break;

                foreach (var dynamicSearchRule in allDynamicSearchRules.Results)
                    await WaitForTaskSucceededAsync(await DefaultClient.DeleteDynamicSearchRuleAsync(dynamicSearchRule.Uid));
            }
        }

        private async Task WaitForTaskSucceededAsync(TaskInfo taskInfo)
        {
            var finishedTask = await DefaultClient.WaitForTaskAsync(taskInfo.TaskUid, timeoutMs: 10000);
            if (finishedTask.Status != TaskInfoStatus.Succeeded)
            {
                throw new Exception($"Dynamic search rule task {taskInfo.TaskUid} finished with status {finishedTask.Status}.");
            }
        }
    }
}

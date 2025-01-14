using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Meilisearch.QueryParameters;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class TaskInfoTests<TFixture> : IAsyncLifetime where TFixture : IndexFixture
    {
        private readonly MeilisearchClient _client;
        private Index _index;
        private readonly TFixture _fixture;

        public TaskInfoTests(TFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.DefaultClient;
        }

        public async Task InitializeAsync()
        {
            await _fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]
            _index = await _fixture.SetUpBasicIndex("BasicIndex-TaskInfoTests");
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetMultipleTaskInfoFromClient()
        {
            await _index.AddDocumentsAsync(new[] { new Movie { Id = "1" } });
            var taskResponse = await _client.GetTasksAsync();
            var tasks = taskResponse.Results;
            tasks.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task GetMultipleTaskInfoFromIndex()
        {
            await _index.AddDocumentsAsync(new[] { new Movie { Id = "1" } });
            var taskResponse = await _index.GetTasksAsync();
            var tasks = taskResponse.Results.Where(t => t.IndexUid != _index.Uid);

            taskResponse.Results.Count().Should().BeGreaterOrEqualTo(1);
            Assert.Empty(tasks);
        }

        [Fact]
        public async Task GetMultipleTaskInfoWithLimitFromClient()
        {
            await _index.AddDocumentsAsync(new[] { new Movie { Id = "1" } });
            var tasks = await _client.GetTasksAsync(new TasksQuery { Limit = 1 });

            tasks.Results.Count().Should().BeGreaterOrEqualTo(1);
            Assert.Equal(1, tasks.Limit);
        }

        [Fact]
        public async Task GetMultipleTaskInfoWithQueryParameters()
        {
            await _index.AddDocumentsAsync(new[] { new Movie { Id = "1" } });
            var taskResponse = await _index.GetTasksAsync(new TasksQuery { Limit = 1, IndexUids = new List<string> { _index.Uid } });

            taskResponse.Results.Count().Should().BeGreaterOrEqualTo(1);
            taskResponse.Total.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetOneTaskInfo()
        {
            var task = await _index.AddDocumentsAsync(new[] { new Movie { Id = "2" } });
            var fetchedTask = await _index.GetTaskAsync(task.TaskUid);
            fetchedTask.Should().NotBeNull();
            fetchedTask.Uid.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task DefaultWaitForTask()
        {
            var task = await _index.AddDocumentsAsync(new[] { new Movie { Id = "3" } });
            var finishedTask = await _index.WaitForTaskAsync(task.TaskUid);
            Assert.Equal(finishedTask.Uid, task.TaskUid);
            Assert.Equal(TaskInfoStatus.Succeeded, finishedTask.Status);
        }

        [Fact]
        public async Task CustomWaitForTask()
        {
            var task = await _index.AddDocumentsAsync(new[] { new Movie { Id = "4" } });
            var finishedTask = await _index.WaitForTaskAsync(task.TaskUid, 10000.0, 20);
            Assert.Equal(finishedTask.Uid, task.TaskUid);
            Assert.Equal(TaskInfoStatus.Succeeded, finishedTask.Status);
        }

        [Fact]
        public async Task WaitForTaskWithException()
        {
            var task = await _index.AddDocumentsAsync(new[] { new Movie { Id = "5" } });
            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => _index.WaitForTaskAsync(task.TaskUid, 0.0, 20));
        }
    }
}

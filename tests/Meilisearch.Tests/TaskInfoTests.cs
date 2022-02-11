using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Xunit;

namespace Meilisearch.Tests
{
    [Collection("Sequential")]
    public class TaskInfoTests : IAsyncLifetime
    {
        private Index _index;
        private readonly IndexFixture _fixture;

        public TaskInfoTests(IndexFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]
            _index = await _fixture.SetUpBasicIndex("BasicIndex-TaskInfoTests");
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetAllTaskInfo()
        {
            await _index.AddDocumentsAsync(new[] { new Movie { Id = "1" } });
            var taskResponse = await _index.GetTasksAsync();
            var tasks = taskResponse.Results;
            tasks.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task GetOneTaskInfo()
        {
            var task = await _index.AddDocumentsAsync(new[] { new Movie { Id = "2" } });
            var fetchedTask = await _index.GetTaskAsync(task.Uid);
            fetchedTask.Should().NotBeNull();
            fetchedTask.Uid.Should().BeGreaterOrEqualTo(0);
            fetchedTask.Status.Should().NotBeNull();
        }

        [Fact]
        public async Task DefaultWaitForTask()
        {
            var task = await _index.AddDocumentsAsync(new[] { new Movie { Id = "3" } });
            var finishedTask = await _index.WaitForTaskAsync(task.Uid);
            Assert.Equal(finishedTask.Uid, task.Uid);
            Assert.Equal("succeeded", finishedTask.Status);
        }

        [Fact]
        public async Task CustomWaitForTask()
        {
            var task = await _index.AddDocumentsAsync(new[] { new Movie { Id = "4" } });
            var finishedTask = await _index.WaitForTaskAsync(task.Uid, 10000.0, 20);
            Assert.Equal(finishedTask.Uid, task.Uid);
            Assert.Equal("succeeded", finishedTask.Status);
        }

        [Fact]
        public async Task WaitForTaskWithException()
        {
            var task = await _index.AddDocumentsAsync(new[] { new Movie { Id = "5" } });
            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => _index.WaitForTaskAsync(task.Uid, 0.0, 20));
        }
    }
}
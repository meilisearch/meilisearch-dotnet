namespace Meilisearch.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    [Collection("Sequential")]
    public class TaskInfoTests : IAsyncLifetime
    {
        private Index index;
        private IndexFixture fixture;

        public TaskInfoTests(IndexFixture fixture)
        {
            this.fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await this.fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]
            this.index = await this.fixture.SetUpBasicIndex("BasicIndex-TaskInfoTests");
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetAllTaskInfo()
        {
            await this.index.AddDocumentsAsync(new[] { new Movie { Id = "1" } });
            var taskResponse = await this.index.GetTasksAsync();
            var tasks = taskResponse.Results;
            tasks.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task GetOneTaskInfo()
        {
            var task = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "2" } });
            TaskInfo fetchedTask = await this.index.GetTaskAsync(task.Uid);
            fetchedTask.Should().NotBeNull();
            fetchedTask.Uid.Should().BeGreaterOrEqualTo(0);
            fetchedTask.Status.Should().NotBeNull();
        }

        [Fact]
        public async Task DefaultWaitForTask()
        {
            var task = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "3" } });
            var finishedTask = await this.index.WaitForTaskAsync(task.Uid);
            Assert.Equal(finishedTask.Uid, task.Uid);
            Assert.Equal("succeeded", finishedTask.Status);
        }

        [Fact]
        public async Task CustomWaitForTask()
        {
            var task = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "4" } });
            var finishedTask = await this.index.WaitForTaskAsync(task.Uid, 10000.0, 20);
            Assert.Equal(finishedTask.Uid, task.Uid);
            Assert.Equal("succeeded", finishedTask.Status);
        }

        [Fact]
        public async Task WaitForTaskWithException()
        {
            var task = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "5" } });
            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => this.index.WaitForTaskAsync(task.Uid, 0.0, 20));
        }
    }
}

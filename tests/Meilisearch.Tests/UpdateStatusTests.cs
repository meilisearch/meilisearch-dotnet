namespace Meilisearch.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    [Collection("Sequential")]
    public class UpdateStatusTests : IAsyncLifetime
    {
        private Index index;
        private IndexFixture fixture;

        public UpdateStatusTests(IndexFixture fixture)
        {
            this.fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await this.fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]
            this.index = await this.fixture.SetUpBasicIndex("BasicIndex-UpdateStatusTests");
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetAllUpdateStatus()
        {
            await this.index.AddDocumentsAsync(new[] { new Movie { Id = "1" } });
            var taskResponse = await this.index.GetTasksAsync();
            var tasks = taskResponse.Results;
            tasks.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task GetOneUpdateStatus()
        {
            var status = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "2" } });
            UpdateStatus individualStatus = await this.index.GetTaskAsync(status.Uid);
            individualStatus.Should().NotBeNull();
            individualStatus.Uid.Should().BeGreaterOrEqualTo(0);
            individualStatus.Status.Should().NotBeNull();
        }

        [Fact]
        public async Task DefaultWaitForTask()
        {
            var status = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "3" } });
            var response = await this.index.WaitForTaskAsync(status.Uid);
            Assert.Equal(response.Uid, status.Uid);
            Assert.Equal("succeeded", response.Status);
        }

        [Fact]
        public async Task CustomWaitForTask()
        {
            var status = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "4" } });
            var response = await this.index.WaitForTaskAsync(status.Uid, 10000.0, 20);
            Assert.Equal(response.Uid, status.Uid);
            Assert.Equal("succeeded", response.Status);
        }

        [Fact]
        public async Task WaitForTaskWithException()
        {
            var status = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "5" } });
            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => this.index.WaitForTaskAsync(status.Uid, 0.0, 20));
        }
    }
}

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
            var allUpdates = await this.fixture.DefaultClient.Tasks.GetAllTaskStatusAsync();
            allUpdates.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task GetOneUpdateStatus()
        {
            var status = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "2" } });
            Meilisearch.TaskInfo individualInfo = await this.fixture.DefaultClient.Tasks.GetTaskStatusAsync(status.Uid);
            individualInfo.Should().NotBeNull();
            individualInfo.Uid.Should().BeGreaterOrEqualTo(0);
            individualInfo.Status.Should().NotBeNull();
        }

        [Fact]
        public async Task DefaultWaitForPendingUpdate()
        {
            var status = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "3" } });
            var response = await this.fixture.DefaultClient.Tasks.WaitForPendingTaskAsync(status.Uid);
            Assert.Equal(response.Uid, status.Uid);
            Assert.Equal("succeeded", response.Status);
        }

        [Fact]
        public async Task CustomWaitForPendingUpdate()
        {
            var status = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "4" } });
            var response = await this.fixture.DefaultClient.Tasks.WaitForPendingTaskAsync(status.Uid, 10000.0, 20);
            Assert.Equal(response.Uid, status.Uid);
            Assert.Equal("succeeded", response.Status);
        }

        [Fact]
        public async Task WaitForPendingUpdateWithException()
        {
            var status = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "5" } });
            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => this.fixture.DefaultClient.Tasks.WaitForPendingTaskAsync(status.Uid, 0.0, 20));
        }
    }
}

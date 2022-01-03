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
            var allUpdates = await this.index.GetAllUpdateStatusAsync();
            allUpdates.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task GetOneUpdateStatus()
        {
            var status = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "2" } });
            UpdateStatus individualStatus = await this.index.GetUpdateStatusAsync(status.UpdateId);
            individualStatus.Should().NotBeNull();
            individualStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
            individualStatus.Status.Should().NotBeNull();
        }

        [Fact]
        public async Task DefaultWaitForPendingUpdate()
        {
            var status = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "3" } });
            var response = await this.index.WaitForPendingUpdateAsync(status.UpdateId);
            Assert.Equal(response.UpdateId, status.UpdateId);
            Assert.Equal("processed", response.Status);
        }

        [Fact]
        public async Task CustomWaitForPendingUpdate()
        {
            var status = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "4" } });
            var response = await this.index.WaitForPendingUpdateAsync(status.UpdateId, 10000.0, 20);
            Assert.Equal(response.UpdateId, status.UpdateId);
            Assert.Equal("processed", response.Status);
        }

        [Fact]
        public async Task WaitForPendingUpdateWithException()
        {
            var status = await this.index.AddDocumentsAsync(new[] { new Movie { Id = "5" } });
            await Assert.ThrowsAsync<MeilisearchTimeoutError>(() => this.index.WaitForPendingUpdateAsync(status.UpdateId, 0.0, 20));
        }
    }
}

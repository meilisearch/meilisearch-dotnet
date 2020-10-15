namespace Meilisearch.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    [Collection("Sequential")]
    public class UpdateStatusTests
    {
        private readonly Meilisearch.Index index;

        public UpdateStatusTests(IndexFixture fixture)
        {
            fixture.DeleteAllIndexes().Wait(); // Test context cleaned for each [Fact]
            var client = fixture.DefaultClient;
            this.index = fixture.SetUpBasicIndex("BasicIndex-UpdateStatusTests").Result;
        }

        [Fact]
        public async Task GetAllUpdateStatus()
        {
            await this.index.AddDocuments(new[] { new Movie { Id = "1" } });
            var allUpdates = await this.index.GetAllUpdateStatus();
            allUpdates.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task GetOneUpdateStatus()
        {
            var status = await this.index.AddDocuments(new[] { new Movie { Id = "2" } });
            UpdateStatus individualStatus = await this.index.GetUpdateStatus(status.UpdateId);
            individualStatus.Should().NotBeNull();
            individualStatus.UpdateId.Should().BeGreaterOrEqualTo(0);
            individualStatus.Status.Should().NotBeNull();
        }

        [Fact]
        public async Task DefaultWaitForPendingUpdate()
        {
            var status = await this.index.AddDocuments(new[] { new Movie { Id = "3" } });
            var response = await this.index.WaitForPendingUpdate(status.UpdateId);
            Assert.Equal(response.UpdateId, status.UpdateId);
            Assert.Equal("processed", response.Status);
        }

        [Fact]
        public async Task CustomWaitForPendingUpdate()
        {
            var status = await this.index.AddDocuments(new[] { new Movie { Id = "4" } });
            var response = await this.index.WaitForPendingUpdate(status.UpdateId, 10000.0, 20);
            Assert.Equal(response.UpdateId, status.UpdateId);
            Assert.Equal("processed", response.Status);
        }

        [Fact]
        public async Task WaitForPendingUpdateWithException()
        {
            var status = await this.index.AddDocuments(new[] { new Movie { Id = "5" } });
            await Assert.ThrowsAsync<Exception>(() => this.index.WaitForPendingUpdate(status.UpdateId, 0.0, 20));
        }
    }
}

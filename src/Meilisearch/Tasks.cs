namespace Meilisearch
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Meilisearch tasks management.
    /// </summary>
    public class Tasks
    {
        private HttpClient http;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tasks"/> class.
        /// </summary>
        /// <param name="httpClient">HttpRequest instance used.</param>
        internal Tasks(HttpClient httpClient)
        {
            this.http = httpClient;
        }

        /// <summary>
        /// Gets the task status of all the asynchronous operations.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns a list of the operations status.</returns>
        public async Task<IEnumerable<TaskInfo>> GetAllTaskStatusAsync(CancellationToken cancellationToken = default)
        {
            var jsonStream = await this.http.GetStreamAsync("tasks").ConfigureAwait(false);
            var jsonDoc = await JsonDocument.ParseAsync(jsonStream, cancellationToken: cancellationToken).ConfigureAwait(false);
            var root = jsonDoc.RootElement.GetProperty("results");
            var allTaskStatus = root.Deserialize<IEnumerable<TaskInfo>>(MeilisearchClient.JsonSerializerOptions);
            return allTaskStatus;
        }

        /// <summary>
        /// Get Task Status by Status Id.
        /// </summary>
        /// <param name="taskUid">Task's Uid for the operation.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Return the current status of the operation.</returns>
        public async Task<TaskInfo> GetTaskStatusAsync(int taskUid, CancellationToken cancellationToken = default)
        {
            return await this.http.GetFromJsonAsync<TaskInfo>($"tasks/{taskUid}", cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Waits until the asynchronous task was done.
        /// </summary>
        /// <param name="taskUid">Unique identifier of the asynchronous task.</param>
        /// <param name="timeoutMs">Timeout in millisecond.</param>
        /// <param name="intervalMs">Interval in millisecond between each check.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the status of asynchronous task.</returns>
        public async Task<TaskInfo> WaitForPendingTaskAsync(
            int taskUid,
            double timeoutMs = 5000.0,
            int intervalMs = 50,
            CancellationToken cancellationToken = default)
        {
            DateTime endingTime = DateTime.Now.AddMilliseconds(timeoutMs);

            while (DateTime.Now < endingTime)
            {
                var response = await this.GetTaskStatusAsync(taskUid, cancellationToken).ConfigureAwait(false);

                if (response.Status != "enqueued" && response.Status != "processing")
                {
                    return response;
                }

                await Task.Delay(intervalMs, cancellationToken).ConfigureAwait(false);
            }

            throw new MeilisearchTimeoutError("The task " + taskUid.ToString() + " timed out.");
        }
    }
}

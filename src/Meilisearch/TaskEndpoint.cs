using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Meilisearch
{
    /// <summary>
    /// Meilisearch index to search and manage documents.
    /// </summary>
    public class TaskEndpoint
    {
        private HttpClient _http;

        /// <summary>
        /// Gets the tasks.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns a list of the tasks.</returns>
        public async Task<Result<IEnumerable<TaskInfo>>> GetTasksAsync(CancellationToken cancellationToken = default)
        {
            return await this._http.GetFromJsonAsync<Result<IEnumerable<TaskInfo>>>("/tasks", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets one task.
        /// </summary>
        /// <param name="taskUid">Uid of the index.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task.</returns>
        public async Task<TaskInfo> GetTaskAsync(int taskUid, CancellationToken cancellationToken = default)
        {
            return await this._http.GetFromJsonAsync<TaskInfo>($"/tasks/{taskUid}", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the tasks from an index.
        /// </summary>
        /// <param name="indexUid">Uid of the index.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns a list of tasks of an index.</returns>
        public async Task<Result<IEnumerable<TaskInfo>>> GetIndexTasksAsync(string indexUid, CancellationToken cancellationToken = default)
        {
            return await this._http.GetFromJsonAsync<Result<IEnumerable<TaskInfo>>>($"/indexes/{indexUid}/tasks", cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get one task from an index.
        /// </summary>
        /// <param name="indexUid">Uid of the index.</param>
        /// <param name="taskUid">Uid of the task.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Return the task of the index.</returns>
        public async Task<TaskInfo> GetIndexTaskAsync(string indexUid, int taskUid, CancellationToken cancellationToken = default)
        {
            return await this._http.GetFromJsonAsync<TaskInfo>($"/indexes/{indexUid}/tasks/{taskUid}", cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Waits until the asynchronous task was done.
        /// </summary>
        /// <param name="taskUid">Unique identifier of the asynchronous task.</param>
        /// <param name="timeoutMs">Timeout in millisecond.</param>
        /// <param name="intervalMs">Interval in millisecond between each check.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the task info of finished task.</returns>
        public async Task<TaskInfo> WaitForTaskAsync(
            int taskUid,
            double timeoutMs = 5000.0,
            int intervalMs = 50,
            CancellationToken cancellationToken = default)
        {
            var endingTime = DateTime.Now.AddMilliseconds(timeoutMs);

            while (DateTime.Now < endingTime)
            {
                var task = await this.GetTaskAsync(taskUid, cancellationToken).ConfigureAwait(false);

                if (task.Status != "enqueued" && task.Status != "processing")
                {
                    return task;
                }

                await Task.Delay(intervalMs, cancellationToken).ConfigureAwait(false);
            }

            throw new MeilisearchTimeoutError("The task " + taskUid.ToString() + " timed out.");
        }

        /// <summary>
        /// Initializes the Index with HTTP client. Only for internal usage.
        /// </summary>
        /// <param name="http">HttpRequest instance used.</param>
        /// <returns>The same object with the initialization.</returns>
        // internal Index WithHttpClient(HttpClient client)
        internal TaskEndpoint WithHttpClient(HttpClient http)
        {
            this._http = http;
            return this;
        }
    }
}

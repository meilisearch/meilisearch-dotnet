namespace Meilisearch
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Meilisearch dumps management.
    /// </summary>
    public class Dumps
    {
        private HttpClient http;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dumps"/> class.
        /// </summary>
        /// <param name="httpClient">HttpRequest instance used.</param>
        internal Dumps(HttpClient httpClient)
        {
            this.http = httpClient;
        }

        /// <summary>
        /// Creates Dump process.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns dump creation status with uid and processing status.</returns>
        public async Task<DumpStatus> CreateDumpAsync(CancellationToken cancellationToken = default)
        {
            var response = await this.http.PostAsync("/dumps", default, cancellationToken).ConfigureAwait(false);

            return await response.Content.ReadFromJsonAsync<DumpStatus>(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a dump creation status.
        /// </summary>
        /// <param name="uid">unique dump identifier.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the status of a dump creation process using the uid.</returns>
        public async Task<DumpStatus> GetDumpStatusAsync(string uid, CancellationToken cancellationToken = default)
        {
            return await this.http.GetFromJsonAsync<DumpStatus>($"dumps/{uid}/status", cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Wait for a dump creation process to finish.
        /// </summary>
        /// <param name="dumpUid">Unique identifier of the dump process.</param>
        /// <param name="timeoutMs">Timeout in millisecond.</param>
        /// <param name="intervalMs">Interval in millisecond between each check.</param>
        /// <param name="cancellationToken">The cancellation token for this call.</param>
        /// <returns>Returns the status of a dump creation process using the uid.</returns>
        /// <exception cref="MeilisearchTimeoutError">Will throw an exception after timeout.</exception>
        public async Task<DumpStatus> WaitForPendingDumpAsync(
            string dumpUid,
            double timeoutMs = 5000.0,
            int intervalMs = 50,
            CancellationToken cancellationToken = default)
        {
            DateTime endingTime = DateTime.Now.AddMilliseconds(timeoutMs);

            while (DateTime.Now < endingTime)
            {
                var response = await this.GetDumpStatusAsync(dumpUid, cancellationToken).ConfigureAwait(false);

                if (response.Status != "in_progress")
                {
                    return response;
                }

                await Task.Delay(intervalMs, cancellationToken).ConfigureAwait(false);
            }

            throw new MeilisearchTimeoutError("The dump " + dumpUid.ToString() + " timed out.");
        }
    }
}

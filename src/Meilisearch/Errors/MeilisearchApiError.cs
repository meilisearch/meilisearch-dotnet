namespace Meilisearch
{
    using System;
    using System.Net;

    /// <summary>
    /// Error sent by MeiliSearch API.
    /// </summary>
    public class MeilisearchApiError : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchApiError"/> class.
        /// Handler Exception received from MeiliSearch API.
        /// </summary>
        /// <param name="apiError">Specific error message from Meilisearch Api.</param>
        public MeilisearchApiError(MeilisearchApiErrorContent apiError)
            : base(string.Format("MeilisearchApiError, Message: {0}, Code: {1}, Type: {2}, Link: {3}", apiError.Message, apiError.Code, apiError.Type, apiError.Link))
        {
            this.Code = apiError.Code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchApiError"/> class.
        /// Handler Exception when Meilisearch API doesn't send a response message.
        /// </summary>
        /// <param name="statusCode">Status code from http response message.</param>
        /// <param name="reasonPhrase">Reason Phrase from http response message.</param>
        public MeilisearchApiError(HttpStatusCode statusCode, string reasonPhrase)
            : base(string.Format("MeilisearchApiError, Message: {0}, Code: {1}", reasonPhrase, (int)statusCode))
        {
        }

        /// <summary>
        /// Gets or sets the code return by MeilisearchApi.
        /// </summary>
        public string Code { get; set; }
    }
}

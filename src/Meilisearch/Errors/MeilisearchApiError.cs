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
        /// Handler Exception received from Meilisearch API.
        /// </summary>
        /// <param name="apiError">Specific error message from Meilisearch Api.</param>
        public MeilisearchApiError(MeilisearchApiErrorContent apiError)
            : base(string.Format("MeilisearchApiError, Message: {0}, ErrorCode: {1}, ErrorType: {2}, ErrorLink: {3}", apiError.Message, apiError.ErrorCode, apiError.ErrorType, apiError.ErrorLink))
        {
            this.ErrorCode = apiError.ErrorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchApiError"/> class.
        /// Handler Exception when Meilisearch API doesn't send a response message.
        /// </summary>
        /// <param name="statusCode">Status code from http response message.</param>
        /// <param name="reasonPhrase">Reason Phrase from http response message.</param>
        public MeilisearchApiError(HttpStatusCode statusCode, string reasonPhrase)
            : base(string.Format("MeilisearchApiError, Message: {0}, ErrorCode: {1}", reasonPhrase, (int)statusCode))
        {
        }

        /// <summary>
        /// Gets or sets the errorCode return by MeilisearchApi..
        /// </summary>
        public string ErrorCode { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace Meilisearch.QueryParameters
{
    public class DeleteDocumentsQuery
    {
        [JsonPropertyName("filter")]
        public object Filter { get; set; }
    }
}

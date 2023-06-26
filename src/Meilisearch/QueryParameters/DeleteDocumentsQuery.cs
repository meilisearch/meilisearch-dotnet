using System.Text.Json.Serialization;

namespace Meilisearch.QueryParameters
{
    public class DeleteDocumentsQuery
    {
        public DeleteDocumentsQuery(object filter)
        {
            Filter = filter;
        }
        [JsonPropertyName("filter")]
        public object Filter { get; set; }
    }
}

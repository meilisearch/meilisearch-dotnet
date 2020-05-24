using Newtonsoft.Json;

namespace Meilisearch
{
    public class UpdateStatus
    {
        [JsonProperty(PropertyName = "updateId")]
        public int UpdateId { get; set; }
    }
}
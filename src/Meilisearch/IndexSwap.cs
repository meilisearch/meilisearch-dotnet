using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Model for index swaps requests.
    /// </summary>
    public class IndexSwap
    {
        [JsonPropertyName("indexes")]
        public List<string> Indexes { get; private set; }

        public IndexSwap(string indexA, string indexB)
        {
            this.Indexes = new List<string> { indexA, indexB };
        }
    }
}

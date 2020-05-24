using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Meilisearch
{
    public class MeilisearchClient
    {
        private readonly HttpClient _client;

        public MeilisearchClient(HttpClient client)
        {
            _client = client;
        }

        public async  Task<MeiliSearchVersion> GetVersion()
        {
           var response = await _client.GetAsync("/version");
           response.EnsureSuccessStatusCode();
           var content = await response.Content.ReadAsStringAsync();
           return JsonConvert.DeserializeObject<MeiliSearchVersion>(content);
        }
    }

    public class MeiliSearchVersion
    {
        [JsonProperty(PropertyName = "commitSha")]
        public string CommitSha { get; set; }
        
        [JsonProperty(PropertyName = "buildDate")]
        public string BuildDate { get; set; }
        
        [JsonProperty(PropertyName = "pkgVersion")]
        public string Version { get; set; }
    }
}
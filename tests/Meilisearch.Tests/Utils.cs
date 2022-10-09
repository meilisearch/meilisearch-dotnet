using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Meilisearch.Tests
{
    public static class JsonFileReader
    {
        public static async Task<T> ReadAsync<T>(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                return await JsonSerializer.DeserializeAsync<T>(stream);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Information regarding an API key for the Meilisearch server.
    /// </summary>
    public class Key
    {
        /// <summary>
        /// Gets or sets unique identifier of the API key.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("key")]
        public string KeyUid { get; set; }

        /// <summary>
        /// Gets or sets unique identifier of the API key.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the name of the API key.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the API key.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets list of actions available for the API key.
        /// </summary>
        [JsonPropertyName("actions")]
        public IEnumerable<KeyAction> Actions { get; set; }

        /// <summary>
        /// Gets or sets the list of indexes the API key can access.
        /// </summary>
        [JsonPropertyName("indexes")]
        public IEnumerable<string> Indexes { get; set; }

        /// <summary>
        /// Gets or sets the date when the API key expires.
        /// </summary>
        [JsonPropertyName("expiresAt")]
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets the date when the API key was created.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date when the API key was updated.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }

    [JsonConverter(typeof(KeyActionJsonConverter))]
    public enum KeyAction
    {
        /// <summary>
        /// Gives all actions to the key.
        /// </summary>
        All,
        /// <summary>
        /// Provides access to all get endpoints.
        /// </summary>
        AllGet,
        /// <summary>
        /// Provides access to chat completions endpoint.
        /// </summary>
        ChatCompletions,
        /// <summary>
        /// Provides access to both POST and GET search endpoints.
        /// </summary>
        Search,
        /// <summary>
        /// Provides access to all documents endpoints.
        /// </summary>
        DocumentsAll,
        /// <summary>
        /// Provides access to the add documents and update documents endpoints.
        /// </summary>
        DocumentsAdd,
        /// <summary>
        /// Provides access to the get one document and get documents endpoints.
        /// </summary>
        DocumentsGet,
        /// <summary>
        /// Provides access to the delete one document, delete all documents, and batch delete endpoints.
        /// </summary>
        DocumentsDelete,
        /// <summary>
        /// Provides access to all indexes endpoint.
        /// </summary>
        IndexesAll,
        /// <summary>
        /// Provides access to the create index endpoint.
        /// </summary>
        IndexesCreate,
        /// <summary>
        /// Provides access to the get one index and list all indexes endpoints.
        /// Non-authorized indexes will be omitted from the response.
        /// </summary>
        IndexesGet,
        /// <summary>
        /// Provides access to the update index endpoint.
        /// </summary>
        IndexesUpdate,
        /// <summary>
        /// Provides access to the delete index endpoint.
        /// </summary>
        IndexesDelete,
        /// <summary>
        /// Provides access to the get one task and get tasks endpoints.
        /// Tasks from non-authorized indexes will be omitted from the response.
        /// </summary>
        TasksGet,
        /// <summary>
        /// Allows canceling tasks in the tasks endpoint.
        /// </summary>
        TasksCancel,
        /// <summary>
        /// Allows deleting tasks in the tasks endpoint.
        /// </summary>
        TasksDelete,
        /// <summary>
        /// Provides access to all settings endpoints and equivalents for all subroutes.
        /// </summary>
        SettingsAll,
        /// <summary>
        /// Provides access to the get settings endpoint and equivalents for all subroutes.
        /// </summary>
        SettingsGet,
        /// <summary>
        /// Provides access to the update settings and reset settings endpoints and equivalents for all subroutes.
        /// </summary>
        SettingsUpdate,
        /// <summary>
        /// Provides access to the get stats of an index endpoint and the get stats of all indexes endpoint.
        /// For the latter, non-authorized indexes are omitted from the response.
        /// </summary>
        StatsGet,
        /// <summary>
        /// Provides access to the create dump endpoint. Not restricted by indexes.
        /// </summary>
        DumpsCreate,
        /// <summary>
        /// Provides access to the get Meilisearch version endpoint.
        /// </summary>
        Version,
        /// <summary>
        /// Provides access to all keys endpoint.
        /// </summary>
        KeysAll,
        /// <summary>
        /// Provides access to the get all keys endpoint.
        /// </summary>
        KeysGet,
        /// <summary>
        /// Provides access to the create key endpoint.
        /// </summary>
        KeysCreate,
        /// <summary>
        /// Provides access to the update key endpoint.
        /// </summary>
        KeysUpdate,
        /// <summary>
        /// Provides access to the delete key endpoint.
        /// </summary>
        KeysDelete
    }


    internal class KeyActionJsonConverter : JsonConverter<KeyAction>
    {
        public override KeyAction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (KeyAction)Enum.Parse(typeof(KeyAction), ConvertFromDotCase(reader.GetString()), false);
        }

        public override void Write(Utf8JsonWriter writer, KeyAction value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(ConvertToDotCase(value));
        }

        private string ConvertFromDotCase(string inputRaw)
        {
            var input = inputRaw.Replace("*", "All");
            var sb = new StringBuilder();
            for (var i = 0; i < input.Length; i++)
            {
                if (i != 0 && input[i] != '.')
                {
                    sb.Append(input[i]);
                }
                else if (input[i] == '.')
                {
                    sb.Append(char.ToUpper(input[i + 1]));
                    ++i;
                }
                else
                {
                    sb.Append(char.ToUpper(input[i]));
                }

            }

            return sb.ToString();
        }

        private string ConvertToDotCase(KeyAction ka)
        {
            var input = ka.ToString().Replace("All", "*");
            var sb = new StringBuilder();
            for (var i = 0; i < input.Length; i++)
            {
                if (i != 0 && char.IsLower(input[i]))
                {
                    sb.Append(input[i]);
                }
                else if (i != 0)
                {
                    sb.Append($".{char.ToLower(input[i])}");
                }

                else
                {
                    sb.Append(char.ToLower(input[i]));
                }
            }
            return sb.ToString();
        }
    }
}

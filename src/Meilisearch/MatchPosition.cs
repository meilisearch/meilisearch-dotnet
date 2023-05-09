using System.Text.Json.Serialization;

namespace Meilisearch
{
    public class MatchPosition
    {
        public MatchPosition(int start, int length)
        {
            Start = start;
            Length = length;
        }

        /// <summary>
        /// The beginning of a matching term within a field.
        /// WARNING: This value is in bytes and not the number of characters. For example, ü represents two bytes but one character.
        /// </summary>
        [JsonPropertyName("start")]
        public int Start { get; }

        /// <summary>
        /// The length of a matching term within a field.
        /// WARNING: This value is in bytes and not the number of characters. For example, ü represents two bytes but one character.
        /// </summary>
        [JsonPropertyName("length")]
        public int Length { get; }
    }
}

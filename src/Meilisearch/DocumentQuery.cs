namespace Meilisearch
{
    public class DocumentQuery
    {
        public int? Offset { get; set; }

        public int? Limit { get; set; }

        public string AttributesToRetrieve { get; set; }
    }
}

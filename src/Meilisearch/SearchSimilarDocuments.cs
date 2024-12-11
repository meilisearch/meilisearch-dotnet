using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Meilisearch
{
    public class SearchSimilarDocuments
    {

        public int Id { get; set; }

        [DefaultValue("default")]
        public string Embedder { get; set; }

        public string[] AttributesToRetrieve { get; set; }

        [DefaultValue(0)]
        public int  Offset { get; set; }

        [DefaultValue(20)]
        public int Limit { get; set; }

        public string Filter { get; set; }

        [DefaultValue(false)]
        public bool ShowRankingScore { get; set; }

        [DefaultValue(false)]
        public bool ShowRankingScoreDetails { get; set; }

        public int RankingScoreThreshold { get; set; }

        [DefaultValue(false)]
        public bool RetrieveVectors { get; set; }
    }
}

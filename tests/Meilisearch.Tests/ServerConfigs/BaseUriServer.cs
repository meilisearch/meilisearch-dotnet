using System;

using Meilisearch.Tests.Fixtures;

using Xunit;

namespace Meilisearch.Tests.ServerConfigs
{
    public class BaseUriServer
    {
        const string CollectionFixtureName = nameof(BaseUriServer);
        private const string MeilisearchTestAddress = "http://localhost:7700/";

        public class IndexCollectionTests
        {
            const string IndexCollectionFixtureName = CollectionFixtureName + "Index";

            public class ConfigFixture : IndexFixture
            {
                public override string MeilisearchAddress()
                {
                    return Environment.GetEnvironmentVariable("MEILISEARCH_URL") ?? MeilisearchTestAddress;
                }
            }

            [CollectionDefinition(IndexCollectionFixtureName)]
            public class IndexCollection : ICollectionFixture<ConfigFixture>
            {
            }

            [Collection(IndexCollectionFixtureName)]
            public class DocumentTests : DocumentTests<ConfigFixture>
            {
                public DocumentTests(ConfigFixture fixture) : base(fixture)
                {
                }
            }

            [Collection(IndexCollectionFixtureName)]
            public class IndexTests : IndexTests<ConfigFixture>
            {
                public IndexTests(ConfigFixture fixture) : base(fixture)
                {
                }
            }

            [Collection(IndexCollectionFixtureName)]
            public class KeyTests : KeyTests<ConfigFixture>
            {
                public KeyTests(ConfigFixture fixture) : base(fixture)
                {
                }
            }

            [Collection(IndexCollectionFixtureName)]
            public class MeilisearchClientTests : MeilisearchClientTests<ConfigFixture>
            {
                public MeilisearchClientTests(ConfigFixture fixture) : base(fixture)
                {
                }
            }

            [Collection(IndexCollectionFixtureName)]
            public class SearchTests : SearchTests<ConfigFixture>
            {
                public SearchTests(ConfigFixture fixture) : base(fixture)
                {
                }
            }

            [Collection(IndexCollectionFixtureName)]
            public class MultiIndexSearchTests : MultiIndexSearchTests<ConfigFixture>
            {
                public MultiIndexSearchTests(ConfigFixture fixture) : base(fixture)
                {
                }
            }

            [Collection(IndexCollectionFixtureName)]
            public class FacetingSearchTests : FacetSearchTests<ConfigFixture>
            {
                public FacetingSearchTests(ConfigFixture fixture) : base(fixture)
                {
                }
            }

            [Collection(IndexCollectionFixtureName)]
            public class SettingsTests : SettingsTests<ConfigFixture>
            {
                public SettingsTests(ConfigFixture fixture) : base(fixture)
                {
                }
            }

            [Collection(IndexCollectionFixtureName)]
            public class TaskInfoTests : TaskInfoTests<ConfigFixture>
            {
                public TaskInfoTests(ConfigFixture fixture) : base(fixture)
                {
                }
            }

            [Collection(IndexCollectionFixtureName)]
            public class TenantTokenTests : TenantTokenTests<ConfigFixture>
            {
                public TenantTokenTests(ConfigFixture fixture) : base(fixture)
                {
                }
            }
        }

        public class DynamicSearchRuleCollectionTests
        {
            const string DsrCollectionFixtureName = CollectionFixtureName + "DynamicSearchReule";

            public class ConfigFixture : DynamicSearchRuleFixture
            {
                public override string MeilisearchAddress()
                {
                    return Environment.GetEnvironmentVariable("MEILISEARCH_URL") ?? MeilisearchTestAddress;
                }
            }

            [CollectionDefinition(DsrCollectionFixtureName)]
            public class DynamicSearchRuleCollection : ICollectionFixture<ConfigFixture>
            {
            }

            [Collection(DsrCollectionFixtureName)]
            public class DynamicSearchRuleTests : DynamicSearchRuleTests<ConfigFixture>
            {
                public DynamicSearchRuleTests(ConfigFixture fixture) : base(fixture)
                {
                }
            }
        }
    }
}

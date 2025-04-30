using System;

using Xunit;

namespace Meilisearch.Tests.ServerConfigs
{
    public class BaseUriServer
    {
        const string CollectionFixtureName = nameof(BaseUriServer);
        private const string MeilisearchTestAddress = "http://localhost:7700/";

        public class ConfigFixture : IndexFixture
        {
            public override string MeilisearchAddress()
            {
                return Environment.GetEnvironmentVariable("MEILISEARCH_URL") ?? MeilisearchTestAddress;
            }
        }

        [CollectionDefinition(CollectionFixtureName)]
        public class IndexCollection : ICollectionFixture<ConfigFixture>
        {
        }

        [Collection(CollectionFixtureName)]
        public class DocumentTests : DocumentTests<ConfigFixture>
        {
            public DocumentTests(ConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class IndexTests : IndexTests<ConfigFixture>
        {
            public IndexTests(ConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class KeyTests : KeyTests<ConfigFixture>
        {
            public KeyTests(ConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class MeilisearchClientTests : MeilisearchClientTests<ConfigFixture>
        {
            public MeilisearchClientTests(ConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class SearchSimilarDocumentsTests : SearchSimilarDocumentsTests<ConfigFixture>
        {
            public SearchSimilarDocumentsTests(ConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class SearchTests : SearchTests<ConfigFixture>
        {
            public SearchTests(ConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class MultiIndexSearchTests : MultiIndexSearchTests<ConfigFixture>
        {
            public MultiIndexSearchTests(ConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class FacetingSearchTests : FacetSearchTests<ConfigFixture>
        {
            public FacetingSearchTests(ConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class SettingsTests : SettingsTests<ConfigFixture>
        {
            public SettingsTests(ConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class TaskInfoTests : TaskInfoTests<ConfigFixture>
        {
            public TaskInfoTests(ConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class TenantTokenTests : TenantTokenTests<ConfigFixture>
        {
            public TenantTokenTests(ConfigFixture fixture) : base(fixture)
            {
            }
        }
    }
}

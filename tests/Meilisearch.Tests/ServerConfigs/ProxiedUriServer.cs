using System;

using Xunit;

namespace Meilisearch.Tests.ServerConfigs
{
    public class ProxiedUriServer
    {
        const string CollectionFixtureName = nameof(ProxiedUriServer);
        private const string MeilisearchTestAddress = "http://localhost:8080/api/";

        public class ConfigFixture : IndexFixture
        {
            public override string MeilisearchAddress()
            {
                var env = Environment.GetEnvironmentVariable("PROXIED_MEILISEARCH");

                return env == null ? MeilisearchTestAddress : env;
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
        public class SearchTests : SearchTests<ConfigFixture>
        {
            public SearchTests(ConfigFixture fixture) : base(fixture)
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

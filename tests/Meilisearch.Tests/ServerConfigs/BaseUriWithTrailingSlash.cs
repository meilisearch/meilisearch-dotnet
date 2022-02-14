using System.Runtime.InteropServices;

using Xunit;

namespace Meilisearch.Tests.ServerConfigs
{
    public class BaseUriWithTrailingSlash
    {
        const string CollectionFixtureName = nameof(BaseUriWithTrailingSlash);
        private const string MeilisearchTestAddress = "http://localhost:7700/";
        public class LocalConfigFixture : IndexFixture
        {
            public override string MeilisearchAddress => MeilisearchTestAddress;
        }

        [CollectionDefinition(CollectionFixtureName)]
        public class LocalIndexCollection : ICollectionFixture<LocalConfigFixture>
        {
        }

        [Collection(CollectionFixtureName)]
        public class LocalDocumentTests : DocumentTests<LocalConfigFixture>
        {
            public LocalDocumentTests(LocalConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class LocalIndexTests : IndexTests<LocalConfigFixture>
        {
            public LocalIndexTests(LocalConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class LocalKeyTests : KeyTests<LocalConfigFixture>
        {
            public LocalKeyTests(LocalConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class LocalMeilisearchClientTests : MeilisearchClientTests<LocalConfigFixture>
        {
            public LocalMeilisearchClientTests(LocalConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class LocalSearchTests : SearchTests<LocalConfigFixture>
        {
            public LocalSearchTests(LocalConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class LocalSettingsTests : SettingsTests<LocalConfigFixture>
        {
            public LocalSettingsTests(LocalConfigFixture fixture) : base(fixture)
            {
            }
        }

        [Collection(CollectionFixtureName)]
        public class LocalTaskInfoTests : TaskInfoTests<LocalConfigFixture>
        {
            public LocalTaskInfoTests(LocalConfigFixture fixture) : base(fixture)
            {
            }
        }
    }
}

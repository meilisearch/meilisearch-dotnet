namespace Meilisearch.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    [Collection("Sequential")]
    public class SettingsTests : IAsyncLifetime
    {
        private readonly Settings defaultSettings;
        private MeilisearchClient client;
        private Index index;
        private IndexFixture fixture;

        public SettingsTests(IndexFixture fixture)
        {
            this.fixture = fixture;
            this.client = fixture.DefaultClient;

            this.defaultSettings = new Settings
            {
                RankingRules = new string[]
                {
                    "words",
                    "typo",
                    "proximity",
                    "attribute",
                    "sort",
                    "exactness",
                },
                DistinctAttribute = null,
                SearchableAttributes = new string[] { "*" },
                DisplayedAttributes = new string[] { "*" },
                StopWords = new string[] { },
                Synonyms = new Dictionary<string, IEnumerable<string>> { },
                FilterableAttributes = new string[] { },
                SortableAttributes = new string[] { },
            };
        }

        private delegate Task<TValue> IndexGetMethod<TValue>();

        public async Task InitializeAsync()
        {
            await this.fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]
            this.index = await this.fixture.SetUpBasicIndex("BasicIndex-SettingsTests");
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private delegate Task<UpdateStatus> IndexUpdateMethod<TValue>(TValue newValue);

        private delegate Task<UpdateStatus> IndexResetMethod();

        [Fact]
        public async Task GetSettings()
        {
            await this.AssertGetEquality(this.index.GetSettings, this.defaultSettings);
        }

        [Fact]
        public async Task UpdateSettings()
        {
            var newSettings = new Settings
            {
                SearchableAttributes = new string[] { "name", "genre" },
                StopWords = new string[] { "of", "the" },
                DistinctAttribute = "name",
            };
            await this.AssertUpdateSuccess(this.index.UpdateSettings, newSettings);
            await this.AssertGetInequality(this.index.GetSettings, newSettings); // fields omitted in newSettings shouldn't have changed
            await this.AssertGetEquality(this.index.GetSettings, SettingsWithDefaultedNullFields(newSettings, this.defaultSettings));
        }

        [Fact]
        public async Task TwoStepUpdateSettings()
        {
            var newSettingsOne = new Settings
            {
                SearchableAttributes = new string[] { "name", "genre" },
                StopWords = new string[] { "of", "the" },
                DistinctAttribute = "name",
                Synonyms = new Dictionary<string, IEnumerable<string>>
                {
                    { "hp", new string[] { "harry potter" } },
                    { "harry potter", new string[] { "hp" } },
                },
            };
            await this.AssertUpdateSuccess(this.index.UpdateSettings, newSettingsOne);

            var expectedSettingsOne = SettingsWithDefaultedNullFields(newSettingsOne, this.defaultSettings);
            await this.AssertGetInequality(this.index.GetSettings, newSettingsOne); // fields omitted in newSettingsOne shouldn't have changed
            await this.AssertGetEquality(this.index.GetSettings, expectedSettingsOne);

            // Second update: this one should not overwritten StopWords and DistinctAttribute.
            var newSettingsTwo = new Settings
            {
                SearchableAttributes = new string[] { "name" },
            };
            await this.AssertUpdateSuccess(this.index.UpdateSettings, newSettingsTwo);

            var expectedSettingsTwo = SettingsWithDefaultedNullFields(newSettingsTwo, expectedSettingsOne);
            await this.AssertGetInequality(this.index.GetSettings, newSettingsTwo); // fields omitted in newSettingsTwo shouldn't have changed
            await this.AssertGetEquality(this.index.GetSettings, expectedSettingsTwo);
        }

        [Fact]
        public async Task ResetSettings()
        {
            var newSettings = new Settings
            {
                SearchableAttributes = new string[] { "name", "genre" },
                StopWords = new string[] { "of", "the" },
                DistinctAttribute = "name",
                DisplayedAttributes = new string[] { "name" },
                RankingRules = new string[] { "typo" },
                FilterableAttributes = new string[] { "genre" },
            };
            await this.AssertUpdateSuccess(this.index.UpdateSettings, newSettings);
            await this.AssertGetInequality(this.index.GetSettings, newSettings); // fields omitted in newSettings shouldn't have changed
            await this.AssertGetEquality(this.index.GetSettings, SettingsWithDefaultedNullFields(newSettings, this.defaultSettings));

            await this.AssertResetSuccess(this.index.ResetSettings);
            await this.AssertGetEquality(this.index.GetSettings, this.defaultSettings);
        }

        [Fact]
        public async Task GetDisplayedAttributes()
        {
            await this.AssertGetEquality(this.index.GetDisplayedAttributes, this.defaultSettings.DisplayedAttributes);
        }

        [Fact]
        public async Task UpdateDisplayedAttributes()
        {
            IEnumerable<string> newDisplayedAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this.index.UpdateDisplayedAttributes, newDisplayedAttributes);
            await this.AssertGetEquality(this.index.GetDisplayedAttributes, newDisplayedAttributes);
        }

        [Fact]
        public async Task ResetDisplayedAttributes()
        {
            IEnumerable<string> newDisplayedAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this.index.UpdateDisplayedAttributes, newDisplayedAttributes);
            await this.AssertGetEquality(this.index.GetDisplayedAttributes, newDisplayedAttributes);

            await this.AssertResetSuccess(this.index.ResetDisplayedAttributes);
            await this.AssertGetEquality(this.index.GetDisplayedAttributes, this.defaultSettings.DisplayedAttributes);
        }

        private static Settings SettingsWithDefaultedNullFields(Settings inputSettings, Settings defaultSettings)
        {
            return new Settings
            {
                RankingRules = inputSettings.RankingRules ?? defaultSettings.RankingRules,
                DistinctAttribute = inputSettings.DistinctAttribute ?? defaultSettings.DistinctAttribute,
                SearchableAttributes = inputSettings.SearchableAttributes ?? defaultSettings.SearchableAttributes,
                DisplayedAttributes = inputSettings.DisplayedAttributes ?? defaultSettings.DisplayedAttributes,
                StopWords = inputSettings.StopWords ?? defaultSettings.StopWords,
                Synonyms = inputSettings.Synonyms ?? defaultSettings.Synonyms,
                FilterableAttributes = inputSettings.FilterableAttributes ?? defaultSettings.FilterableAttributes,
                SortableAttributes = inputSettings.SortableAttributes ?? defaultSettings.SortableAttributes,
            };
        }

        private async Task AssertGetEquality<TValue>(IndexGetMethod<TValue> getMethod, TValue expectedValue)
        {
            var value = await getMethod();
            value.Should().BeEquivalentTo(expectedValue);
        }

        private async Task AssertGetInequality<TValue>(IndexGetMethod<TValue> getMethod, TValue expectedValue)
        {
            var value = await getMethod();
            value.Should().NotBeEquivalentTo(expectedValue);
        }

        private async Task AssertUpdateStatusProcessed(UpdateStatus updateStatus)
        {
            updateStatus.UpdateId.Should().BeGreaterThan(0);
            var updateWaitResponse = await this.index.WaitForPendingUpdate(updateStatus.UpdateId);
            updateWaitResponse.Status.Should().BeEquivalentTo("processed");
        }

        private async Task AssertUpdateSuccess<TValue>(IndexUpdateMethod<TValue> updateMethod, TValue newValue)
        {
            var updateStatus = await updateMethod(newValue);
            await this.AssertUpdateStatusProcessed(updateStatus);
        }

        private async Task AssertResetSuccess(IndexResetMethod resetMethod)
        {
            var updateStatus = await resetMethod();
            await this.AssertUpdateStatusProcessed(updateStatus);
        }
    }
}

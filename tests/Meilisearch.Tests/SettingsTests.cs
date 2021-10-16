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

        [Fact]
        public async Task GetDistinctAttribute()
        {
            await this.AssertGetEquality(this.index.GetDistinctAttribute, this.defaultSettings.DistinctAttribute);
        }

        [Fact]
        public async Task UpdateDistinctAttribute()
        {
            var newDistinctAttribute = "name";
            await this.AssertUpdateSuccess(this.index.UpdateDistinctAttribute, newDistinctAttribute);
            await this.AssertGetEquality(this.index.GetDistinctAttribute, newDistinctAttribute);
        }

        [Fact]
        public async Task ResetDistinctAttribute()
        {
            var newDistinctAttribute = "name";
            await this.AssertUpdateSuccess(this.index.UpdateDistinctAttribute, newDistinctAttribute);
            await this.AssertGetEquality(this.index.GetDistinctAttribute, newDistinctAttribute);

            await this.AssertResetSuccess(this.index.ResetDistinctAttribute);
            await this.AssertGetEquality(this.index.GetDistinctAttribute, this.defaultSettings.DistinctAttribute);
        }

        [Fact]
        public async Task GetFilterableAttributes()
        {
            await this.AssertGetEquality(this.index.GetFilterableAttributes, this.defaultSettings.FilterableAttributes);
        }

        [Fact]
        public async Task UpdateFilterableAttributes()
        {
            var newFilterableAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this.index.UpdateFilterableAttributes, newFilterableAttributes);
            await this.AssertGetEquality(this.index.GetFilterableAttributes, newFilterableAttributes);
        }

        [Fact]
        public async Task ResetFilterableAttributes()
        {
            var newFilterableAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this.index.UpdateFilterableAttributes, newFilterableAttributes);
            await this.AssertGetEquality(this.index.GetFilterableAttributes, newFilterableAttributes);

            await this.AssertResetSuccess(this.index.ResetFilterableAttributes);
            await this.AssertGetEquality(this.index.GetFilterableAttributes, this.defaultSettings.FilterableAttributes);
        }

        [Fact]
        public async Task GetRankingRules()
        {
            await this.AssertGetEquality(this.index.GetRankingRules, this.defaultSettings.RankingRules);
        }

        [Fact]
        public async Task UpdateRankingRules()
        {
            var newRankingRules = new string[] { "words", "typo" };
            await this.AssertUpdateSuccess(this.index.UpdateRankingRules, newRankingRules);
            await this.AssertGetEquality(this.index.GetRankingRules, newRankingRules);
        }

        [Fact]
        public async Task ResetRankingRules()
        {
            var newRankingRules = new string[] { "words", "typo" };
            await this.AssertUpdateSuccess(this.index.UpdateRankingRules, newRankingRules);
            await this.AssertGetEquality(this.index.GetRankingRules, newRankingRules);

            await this.AssertResetSuccess(this.index.ResetRankingRules);
            await this.AssertGetEquality(this.index.GetRankingRules, this.defaultSettings.RankingRules);
        }

        [Fact]
        public async Task GetSearchableAttributes()
        {
            await this.AssertGetEquality(this.index.GetSearchableAttributes, this.defaultSettings.SearchableAttributes);
        }

        [Fact]
        public async Task UpdateSearchableAttributes()
        {
            var newSearchableAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this.index.UpdateSearchableAttributes, newSearchableAttributes);
            await this.AssertGetEquality(this.index.GetSearchableAttributes, newSearchableAttributes);
        }

        [Fact]
        public async Task ResetSearchableAttributes()
        {
            var newSearchableAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this.index.UpdateSearchableAttributes, newSearchableAttributes);
            await this.AssertGetEquality(this.index.GetSearchableAttributes, newSearchableAttributes);

            await this.AssertResetSuccess(this.index.ResetSearchableAttributes);
            await this.AssertGetEquality(this.index.GetSearchableAttributes, this.defaultSettings.SearchableAttributes);
        }

        [Fact]
        public async Task GetSortableAttributes()
        {
            await this.AssertGetEquality(this.index.GetSortableAttributes, this.defaultSettings.SortableAttributes);
        }

        [Fact]
        public async Task UpdateSortableAttributes()
        {
            var newSortableAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this.index.UpdateSortableAttributes, newSortableAttributes);
            await this.AssertGetEquality(this.index.GetSortableAttributes, newSortableAttributes);
        }

        [Fact]
        public async Task ResetSortableAttributes()
        {
            var newSortableAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this.index.UpdateSortableAttributes, newSortableAttributes);
            await this.AssertGetEquality(this.index.GetSortableAttributes, newSortableAttributes);

            await this.AssertResetSuccess(this.index.ResetSortableAttributes);
            await this.AssertGetEquality(this.index.GetSortableAttributes, this.defaultSettings.SortableAttributes);
        }

        [Fact]
        public async Task GetStopWords()
        {
            await this.AssertGetEquality(this.index.GetStopWords, this.defaultSettings.StopWords);
        }

        [Fact]
        public async Task UpdateStopWords()
        {
            var newStopWords = new string[] { "the", "and", "of" };
            await this.AssertUpdateSuccess(this.index.UpdateStopWords, newStopWords);
            await this.AssertGetEquality(this.index.GetStopWords, newStopWords);
        }

        [Fact]
        public async Task ResetStopWords()
        {
            var newStopWords = new string[] { "the", "and", "of" };
            await this.AssertUpdateSuccess(this.index.UpdateStopWords, newStopWords);
            await this.AssertGetEquality(this.index.GetStopWords, newStopWords);

            await this.AssertResetSuccess(this.index.ResetStopWords);
            await this.AssertGetEquality(this.index.GetStopWords, this.defaultSettings.StopWords);
        }

        [Fact]
        public async Task GetSynonyms()
        {
            await this.AssertGetEquality(this.index.GetSynonyms, this.defaultSettings.Synonyms);
        }

        [Fact]
        public async Task UpdateSynonyms()
        {
            var newSynonyms = new Dictionary<string, IEnumerable<string>>
            {
                { "hp", new string[] { "harry potter" } },
                { "harry potter", new string[] { "hp" } },
            };
            await this.AssertUpdateSuccess(this.index.UpdateSynonyms, newSynonyms);
            await this.AssertGetEquality(this.index.GetSynonyms, newSynonyms);
        }

        [Fact]
        public async Task ResetSynonyms()
        {
            var newSynonyms = new Dictionary<string, IEnumerable<string>>
            {
                { "hp", new string[] { "harry potter" } },
                { "harry potter", new string[] { "hp" } },
            };
            await this.AssertUpdateSuccess(this.index.UpdateSynonyms, newSynonyms);
            await this.AssertGetEquality(this.index.GetSynonyms, newSynonyms);

            await this.AssertResetSuccess(this.index.ResetSynonyms);
            await this.AssertGetEquality(this.index.GetSynonyms, this.defaultSettings.Synonyms);
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

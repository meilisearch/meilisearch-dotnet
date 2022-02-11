using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Xunit;

namespace Meilisearch.Tests
{
    [Collection("Sequential")]
    public class SettingsTests : IAsyncLifetime
    {
        private readonly Settings _defaultSettings;
        private readonly MeilisearchClient _client;
        private Index _index;
        private readonly IndexFixture _fixture;

        public SettingsTests(IndexFixture fixture)
        {
            this._fixture = fixture;
            this._client = fixture.DefaultClient;

            this._defaultSettings = new Settings
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

        private delegate Task<TValue> IndexGetMethod<TValue>(CancellationToken cancellationToken = default);

        private delegate Task<TaskInfo> IndexUpdateMethod<TValue>(TValue newValue, CancellationToken cancellationToken = default);

        private delegate Task<TaskInfo> IndexResetMethod(CancellationToken cancellationToken = default);

        public async Task InitializeAsync()
        {
            await this._fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]
            this._index = await this._fixture.SetUpBasicIndex("BasicIndex-SettingsTests");
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetSettings()
        {
            await this.AssertGetEquality(this._index.GetSettingsAsync, this._defaultSettings);
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
            await this.AssertUpdateSuccess(this._index.UpdateSettingsAsync, newSettings);
            await this.AssertGetInequality(this._index.GetSettingsAsync, newSettings); // fields omitted in newSettings shouldn't have changed
            await this.AssertGetEquality(this._index.GetSettingsAsync, SettingsWithDefaultedNullFields(newSettings, this._defaultSettings));
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
            await this.AssertUpdateSuccess(this._index.UpdateSettingsAsync, newSettingsOne);

            var expectedSettingsOne = SettingsWithDefaultedNullFields(newSettingsOne, this._defaultSettings);
            await this.AssertGetInequality(this._index.GetSettingsAsync, newSettingsOne); // fields omitted in newSettingsOne shouldn't have changed
            await this.AssertGetEquality(this._index.GetSettingsAsync, expectedSettingsOne);

            // Second update: this one should not overwritten StopWords and DistinctAttribute.
            var newSettingsTwo = new Settings
            {
                SearchableAttributes = new string[] { "name" },
            };
            await this.AssertUpdateSuccess(this._index.UpdateSettingsAsync, newSettingsTwo);

            var expectedSettingsTwo = SettingsWithDefaultedNullFields(newSettingsTwo, expectedSettingsOne);
            await this.AssertGetInequality(this._index.GetSettingsAsync, newSettingsTwo); // fields omitted in newSettingsTwo shouldn't have changed
            await this.AssertGetEquality(this._index.GetSettingsAsync, expectedSettingsTwo);
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
            await this.AssertUpdateSuccess(this._index.UpdateSettingsAsync, newSettings);
            await this.AssertGetInequality(this._index.GetSettingsAsync, newSettings); // fields omitted in newSettings shouldn't have changed
            await this.AssertGetEquality(this._index.GetSettingsAsync, SettingsWithDefaultedNullFields(newSettings, this._defaultSettings));

            await this.AssertResetSuccess(this._index.ResetSettingsAsync);
            await this.AssertGetEquality(this._index.GetSettingsAsync, this._defaultSettings);
        }

        [Fact]
        public async Task GetDisplayedAttributes()
        {
            await this.AssertGetEquality(this._index.GetDisplayedAttributesAsync, this._defaultSettings.DisplayedAttributes);
        }

        [Fact]
        public async Task UpdateDisplayedAttributes()
        {
            IEnumerable<string> newDisplayedAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this._index.UpdateDisplayedAttributesAsync, newDisplayedAttributes);
            await this.AssertGetEquality(this._index.GetDisplayedAttributesAsync, newDisplayedAttributes);
        }

        [Fact]
        public async Task ResetDisplayedAttributes()
        {
            IEnumerable<string> newDisplayedAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this._index.UpdateDisplayedAttributesAsync, newDisplayedAttributes);
            await this.AssertGetEquality(this._index.GetDisplayedAttributesAsync, newDisplayedAttributes);

            await this.AssertResetSuccess(this._index.ResetDisplayedAttributesAsync);
            await this.AssertGetEquality(this._index.GetDisplayedAttributesAsync, this._defaultSettings.DisplayedAttributes);
        }

        [Fact]
        public async Task GetDistinctAttribute()
        {
            await this.AssertGetEquality(this._index.GetDistinctAttributeAsync, this._defaultSettings.DistinctAttribute);
        }

        [Fact]
        public async Task UpdateDistinctAttribute()
        {
            var newDistinctAttribute = "name";
            await this.AssertUpdateSuccess(this._index.UpdateDistinctAttributeAsync, newDistinctAttribute);
            await this.AssertGetEquality(this._index.GetDistinctAttributeAsync, newDistinctAttribute);
        }

        [Fact]
        public async Task ResetDistinctAttribute()
        {
            var newDistinctAttribute = "name";
            await this.AssertUpdateSuccess(this._index.UpdateDistinctAttributeAsync, newDistinctAttribute);
            await this.AssertGetEquality(this._index.GetDistinctAttributeAsync, newDistinctAttribute);

            await this.AssertResetSuccess(this._index.ResetDistinctAttributeAsync);
            await this.AssertGetEquality(this._index.GetDistinctAttributeAsync, this._defaultSettings.DistinctAttribute);
        }

        [Fact]
        public async Task GetFilterableAttributes()
        {
            await this.AssertGetEquality(this._index.GetFilterableAttributesAsync, this._defaultSettings.FilterableAttributes);
        }

        [Fact]
        public async Task UpdateFilterableAttributes()
        {
            var newFilterableAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this._index.UpdateFilterableAttributesAsync, newFilterableAttributes);
            await this.AssertGetEquality(this._index.GetFilterableAttributesAsync, newFilterableAttributes);
        }

        [Fact]
        public async Task ResetFilterableAttributes()
        {
            var newFilterableAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this._index.UpdateFilterableAttributesAsync, newFilterableAttributes);
            await this.AssertGetEquality(this._index.GetFilterableAttributesAsync, newFilterableAttributes);

            await this.AssertResetSuccess(this._index.ResetFilterableAttributesAsync);
            await this.AssertGetEquality(this._index.GetFilterableAttributesAsync, this._defaultSettings.FilterableAttributes);
        }

        [Fact]
        public async Task GetRankingRules()
        {
            await this.AssertGetEquality(this._index.GetRankingRulesAsync, this._defaultSettings.RankingRules);
        }

        [Fact]
        public async Task UpdateRankingRules()
        {
            var newRankingRules = new string[] { "words", "typo" };
            await this.AssertUpdateSuccess(this._index.UpdateRankingRulesAsync, newRankingRules);
            await this.AssertGetEquality(this._index.GetRankingRulesAsync, newRankingRules);
        }

        [Fact]
        public async Task ResetRankingRules()
        {
            var newRankingRules = new string[] { "words", "typo" };
            await this.AssertUpdateSuccess(this._index.UpdateRankingRulesAsync, newRankingRules);
            await this.AssertGetEquality(this._index.GetRankingRulesAsync, newRankingRules);

            await this.AssertResetSuccess(this._index.ResetRankingRulesAsync);
            await this.AssertGetEquality(this._index.GetRankingRulesAsync, this._defaultSettings.RankingRules);
        }

        [Fact]
        public async Task GetSearchableAttributes()
        {
            await this.AssertGetEquality(this._index.GetSearchableAttributesAsync, this._defaultSettings.SearchableAttributes);
        }

        [Fact]
        public async Task UpdateSearchableAttributes()
        {
            var newSearchableAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this._index.UpdateSearchableAttributesAsync, newSearchableAttributes);
            await this.AssertGetEquality(this._index.GetSearchableAttributesAsync, newSearchableAttributes);
        }

        [Fact]
        public async Task ResetSearchableAttributes()
        {
            var newSearchableAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this._index.UpdateSearchableAttributesAsync, newSearchableAttributes);
            await this.AssertGetEquality(this._index.GetSearchableAttributesAsync, newSearchableAttributes);

            await this.AssertResetSuccess(this._index.ResetSearchableAttributesAsync);
            await this.AssertGetEquality(this._index.GetSearchableAttributesAsync, this._defaultSettings.SearchableAttributes);
        }

        [Fact]
        public async Task GetSortableAttributes()
        {
            await this.AssertGetEquality(this._index.GetSortableAttributesAsync, this._defaultSettings.SortableAttributes);
        }

        [Fact]
        public async Task UpdateSortableAttributes()
        {
            var newSortableAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this._index.UpdateSortableAttributesAsync, newSortableAttributes);
            await this.AssertGetEquality(this._index.GetSortableAttributesAsync, newSortableAttributes);
        }

        [Fact]
        public async Task ResetSortableAttributes()
        {
            var newSortableAttributes = new string[] { "name", "genre" };
            await this.AssertUpdateSuccess(this._index.UpdateSortableAttributesAsync, newSortableAttributes);
            await this.AssertGetEquality(this._index.GetSortableAttributesAsync, newSortableAttributes);

            await this.AssertResetSuccess(this._index.ResetSortableAttributesAsync);
            await this.AssertGetEquality(this._index.GetSortableAttributesAsync, this._defaultSettings.SortableAttributes);
        }

        [Fact]
        public async Task GetStopWords()
        {
            await this.AssertGetEquality(this._index.GetStopWordsAsync, this._defaultSettings.StopWords);
        }

        [Fact]
        public async Task UpdateStopWords()
        {
            var newStopWords = new string[] { "the", "and", "of" };
            await this.AssertUpdateSuccess(this._index.UpdateStopWordsAsync, newStopWords);
            await this.AssertGetEquality(this._index.GetStopWordsAsync, newStopWords);
        }

        [Fact]
        public async Task ResetStopWords()
        {
            var newStopWords = new string[] { "the", "and", "of" };
            await this.AssertUpdateSuccess(this._index.UpdateStopWordsAsync, newStopWords);
            await this.AssertGetEquality(this._index.GetStopWordsAsync, newStopWords);

            await this.AssertResetSuccess(this._index.ResetStopWordsAsync);
            await this.AssertGetEquality(this._index.GetStopWordsAsync, this._defaultSettings.StopWords);
        }

        [Fact]
        public async Task GetSynonyms()
        {
            await this.AssertGetEquality(this._index.GetSynonymsAsync, this._defaultSettings.Synonyms);
        }

        [Fact]
        public async Task UpdateSynonyms()
        {
            var newSynonyms = new Dictionary<string, IEnumerable<string>>
            {
                { "hp", new string[] { "harry potter" } },
                { "harry potter", new string[] { "hp" } },
            };
            await this.AssertUpdateSuccess(this._index.UpdateSynonymsAsync, newSynonyms);
            await this.AssertGetEquality(this._index.GetSynonymsAsync, newSynonyms);
        }

        [Fact]
        public async Task ResetSynonyms()
        {
            var newSynonyms = new Dictionary<string, IEnumerable<string>>
            {
                { "hp", new string[] { "harry potter" } },
                { "harry potter", new string[] { "hp" } },
            };
            await this.AssertUpdateSuccess(this._index.UpdateSynonymsAsync, newSynonyms);
            await this.AssertGetEquality(this._index.GetSynonymsAsync, newSynonyms);

            await this.AssertResetSuccess(this._index.ResetSynonymsAsync);
            await this.AssertGetEquality(this._index.GetSynonymsAsync, this._defaultSettings.Synonyms);
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

        private async Task AssertTaskInfoSucceeded(TaskInfo task)
        {
            task.Uid.Should().BeGreaterThan(0);
            task = await this._index.WaitForTaskAsync(task.Uid);
            task.Status.Should().BeEquivalentTo("succeeded");
        }

        private async Task AssertUpdateSuccess<TValue>(IndexUpdateMethod<TValue> updateMethod, TValue newValue)
        {
            var task = await updateMethod(newValue);
            await this.AssertTaskInfoSucceeded(task);
        }

        private async Task AssertResetSuccess(IndexResetMethod resetMethod)
        {
            var task = await resetMethod();
            await this.AssertTaskInfoSucceeded(task);
        }
    }
}

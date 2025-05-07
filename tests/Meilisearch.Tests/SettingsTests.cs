using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Xunit;

namespace Meilisearch.Tests
{
    public abstract class SettingsTests<TFixture> : IAsyncLifetime where TFixture : IndexFixture
    {
        private readonly Settings _defaultSettings;
        private Index _index;
        private readonly TFixture _fixture;

        public SettingsTests(TFixture fixture)
        {
            _fixture = fixture;

            _defaultSettings = new Settings
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
                Dictionary = Array.Empty<string>(),
                StopWords = Array.Empty<string>(),
                SeparatorTokens = new List<string> { },
                NonSeparatorTokens = new List<string> { },
                Synonyms = new Dictionary<string, IEnumerable<string>> { },
                FilterableAttributes = Array.Empty<string>(),
                SortableAttributes = Array.Empty<string>(),
                ProximityPrecision = "byWord",
                TypoTolerance = new TypoTolerance
                {
                    Enabled = true,
                    DisableOnAttributes = Array.Empty<string>(),
                    DisableOnWords = Array.Empty<string>(),
                    MinWordSizeForTypos = new TypoTolerance.TypoSize
                    {
                        OneTypo = 5,
                        TwoTypos = 9
                    }
                },
                Faceting = new Faceting
                {
                    MaxValuesPerFacet = 100,
                    SortFacetValuesBy = new Dictionary<string, SortFacetValuesByType>()
                    {
                        ["*"] = SortFacetValuesByType.Alpha
                    }
                },
                Pagination = new Pagination
                {
                    MaxTotalHits = 1000
                }
            };
        }

        private delegate Task<TValue> IndexGetMethod<TValue>(CancellationToken cancellationToken = default);

        private delegate Task<TaskInfo> IndexUpdateMethod<TValue>(TValue newValue, CancellationToken cancellationToken = default);

        private delegate Task<TaskInfo> IndexResetMethod(CancellationToken cancellationToken = default);

        public async Task InitializeAsync()
        {
            await _fixture.DeleteAllIndexes(); // Test context cleaned for each [Fact]
            _index = await _fixture.SetUpBasicIndex("BasicIndex-SettingsTests");
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetSettings()
        {
            await AssertGetEquality(_index.GetSettingsAsync, _defaultSettings);
        }

        [Fact]
        public async Task UpdateSettings()
        {
            var newSettings = new Settings
            {
                SearchableAttributes = new string[] { "name", "genre" },
                StopWords = new string[] { "of", "the" },
                DistinctAttribute = "name",
                Dictionary = new string[] { "dictionary" },
                SearchCutoffMs = 1000,
                LocalizedAttributes = new LocalizedAttributeLocale[]
                {
                    new LocalizedAttributeLocale() {
                        Locales = new[] { "eng" },
                        AttributePatterns = new[] { "en_*" }
                    }
                }
            };
            await AssertUpdateSuccess(_index.UpdateSettingsAsync, newSettings);
            await AssertGetInequality(_index.GetSettingsAsync, newSettings); // fields omitted in newSettings shouldn't have changed
            await AssertGetEquality(_index.GetSettingsAsync, SettingsWithDefaultedNullFields(newSettings, _defaultSettings));
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
                Dictionary = new string[] { "dictionary" }
            };
            await AssertUpdateSuccess(_index.UpdateSettingsAsync, newSettingsOne);

            var expectedSettingsOne = SettingsWithDefaultedNullFields(newSettingsOne, _defaultSettings);
            await AssertGetInequality(_index.GetSettingsAsync, newSettingsOne); // fields omitted in newSettingsOne shouldn't have changed
            await AssertGetEquality(_index.GetSettingsAsync, expectedSettingsOne);

            // Second update: this one should not overwritten StopWords and DistinctAttribute.
            var newSettingsTwo = new Settings
            {
                SearchableAttributes = new string[] { "name" },
            };
            await AssertUpdateSuccess(_index.UpdateSettingsAsync, newSettingsTwo);

            var expectedSettingsTwo = SettingsWithDefaultedNullFields(newSettingsTwo, expectedSettingsOne);
            await AssertGetInequality(_index.GetSettingsAsync, newSettingsTwo); // fields omitted in newSettingsTwo shouldn't have changed
            await AssertGetEquality(_index.GetSettingsAsync, expectedSettingsTwo);
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
                Dictionary = new string[] { "dictionary" }
            };
            await AssertUpdateSuccess(_index.UpdateSettingsAsync, newSettings);
            await AssertGetInequality(_index.GetSettingsAsync, newSettings); // fields omitted in newSettings shouldn't have changed
            await AssertGetEquality(_index.GetSettingsAsync, SettingsWithDefaultedNullFields(newSettings, _defaultSettings));

            await AssertResetSuccess(_index.ResetSettingsAsync);
            await AssertGetEquality(_index.GetSettingsAsync, _defaultSettings);
        }

        [Fact]
        public async Task GetDisplayedAttributes()
        {
            await AssertGetEquality(_index.GetDisplayedAttributesAsync, _defaultSettings.DisplayedAttributes);
        }

        [Fact]
        public async Task UpdateDisplayedAttributes()
        {
            IEnumerable<string> newDisplayedAttributes = new string[] { "name", "genre" };
            await AssertUpdateSuccess(_index.UpdateDisplayedAttributesAsync, newDisplayedAttributes);
            await AssertGetEquality(_index.GetDisplayedAttributesAsync, newDisplayedAttributes);
        }

        [Fact]
        public async Task ResetDisplayedAttributes()
        {
            IEnumerable<string> newDisplayedAttributes = new string[] { "name", "genre" };
            await AssertUpdateSuccess(_index.UpdateDisplayedAttributesAsync, newDisplayedAttributes);
            await AssertGetEquality(_index.GetDisplayedAttributesAsync, newDisplayedAttributes);

            await AssertResetSuccess(_index.ResetDisplayedAttributesAsync);
            await AssertGetEquality(_index.GetDisplayedAttributesAsync, _defaultSettings.DisplayedAttributes);
        }

        [Fact]
        public async Task GetDistinctAttribute()
        {
            await AssertGetEquality(_index.GetDistinctAttributeAsync, _defaultSettings.DistinctAttribute);
        }

        [Fact]
        public async Task UpdateDistinctAttribute()
        {
            var newDistinctAttribute = "name";
            await AssertUpdateSuccess(_index.UpdateDistinctAttributeAsync, newDistinctAttribute);
            await AssertGetEquality(_index.GetDistinctAttributeAsync, newDistinctAttribute);
        }

        [Fact]
        public async Task ResetDistinctAttribute()
        {
            var newDistinctAttribute = "name";
            await AssertUpdateSuccess(_index.UpdateDistinctAttributeAsync, newDistinctAttribute);
            await AssertGetEquality(_index.GetDistinctAttributeAsync, newDistinctAttribute);

            await AssertResetSuccess(_index.ResetDistinctAttributeAsync);
            await AssertGetEquality(_index.GetDistinctAttributeAsync, _defaultSettings.DistinctAttribute);
        }

        [Fact]
        public async Task GetFilterableAttributes()
        {
            await AssertGetEquality(_index.GetFilterableAttributesAsync, _defaultSettings.FilterableAttributes);
        }

        [Fact]
        public async Task UpdateFilterableAttributes()
        {
            var newFilterableAttributes = new string[] { "name", "genre" };
            await AssertUpdateSuccess(_index.UpdateFilterableAttributesAsync, newFilterableAttributes);
            await AssertGetEquality(_index.GetFilterableAttributesAsync, newFilterableAttributes);
        }

        [Fact]
        public async Task ResetFilterableAttributes()
        {
            var newFilterableAttributes = new string[] { "name", "genre" };
            await AssertUpdateSuccess(_index.UpdateFilterableAttributesAsync, newFilterableAttributes);
            await AssertGetEquality(_index.GetFilterableAttributesAsync, newFilterableAttributes);

            await AssertResetSuccess(_index.ResetFilterableAttributesAsync);
            await AssertGetEquality(_index.GetFilterableAttributesAsync, _defaultSettings.FilterableAttributes);
        }

        [Fact]
        public async Task GetLocaliztedAttributes()
        {
            await AssertGetEquality(_index.GetLocalizedAttributesAsync, _defaultSettings.LocalizedAttributes);
        }

        [Fact]
        public async Task UpdateLocalizedAttributes()
        {
            var newLocalizedAttributes = new LocalizedAttributeLocale[]
            {
                new LocalizedAttributeLocale() {
                    Locales = new[] { "eng" },
                    AttributePatterns = new[] { "en_*" }
                }
            };

            await AssertUpdateSuccess(_index.UpdateLocalizedAttributesAsync, newLocalizedAttributes);
            await AssertGetEquality(_index.GetLocalizedAttributesAsync, newLocalizedAttributes);
        }

        [Fact]
        public async Task ResetLocalizedAttributes()
        {
            var newLocalizedAttributes = new LocalizedAttributeLocale[]
            {
                new LocalizedAttributeLocale() {
                    Locales = new[] { "eng" },
                    AttributePatterns = new[] { "en_*" }
                }
            };

            await AssertUpdateSuccess(_index.UpdateLocalizedAttributesAsync, newLocalizedAttributes);
            await AssertGetEquality(_index.GetLocalizedAttributesAsync, newLocalizedAttributes);

            await AssertResetSuccess(_index.ResetLocalizedAttributesAsync);
            await AssertGetEquality(_index.GetLocalizedAttributesAsync, _defaultSettings.LocalizedAttributes);
        }

        [Fact]
        public async Task GetRankingRules()
        {
            await AssertGetEquality(_index.GetRankingRulesAsync, _defaultSettings.RankingRules);
        }

        [Fact]
        public async Task UpdateRankingRules()
        {
            var newRankingRules = new string[] { "words", "typo" };
            await AssertUpdateSuccess(_index.UpdateRankingRulesAsync, newRankingRules);
            await AssertGetEquality(_index.GetRankingRulesAsync, newRankingRules);
        }

        [Fact]
        public async Task ResetRankingRules()
        {
            var newRankingRules = new string[] { "words", "typo" };
            await AssertUpdateSuccess(_index.UpdateRankingRulesAsync, newRankingRules);
            await AssertGetEquality(_index.GetRankingRulesAsync, newRankingRules);

            await AssertResetSuccess(_index.ResetRankingRulesAsync);
            await AssertGetEquality(_index.GetRankingRulesAsync, _defaultSettings.RankingRules);
        }

        [Fact]
        public async Task GetSearchableAttributes()
        {
            await AssertGetEquality(_index.GetSearchableAttributesAsync, _defaultSettings.SearchableAttributes);
        }

        [Fact]
        public async Task UpdateSearchableAttributes()
        {
            var newSearchableAttributes = new string[] { "name", "genre" };
            await AssertUpdateSuccess(_index.UpdateSearchableAttributesAsync, newSearchableAttributes);
            await AssertGetEquality(_index.GetSearchableAttributesAsync, newSearchableAttributes);
        }

        [Fact]
        public async Task ResetSearchableAttributes()
        {
            var newSearchableAttributes = new string[] { "name", "genre" };
            await AssertUpdateSuccess(_index.UpdateSearchableAttributesAsync, newSearchableAttributes);
            await AssertGetEquality(_index.GetSearchableAttributesAsync, newSearchableAttributes);

            await AssertResetSuccess(_index.ResetSearchableAttributesAsync);
            await AssertGetEquality(_index.GetSearchableAttributesAsync, _defaultSettings.SearchableAttributes);
        }

        [Fact]
        public async Task GetSortableAttributes()
        {
            await AssertGetEquality(_index.GetSortableAttributesAsync, _defaultSettings.SortableAttributes);
        }

        [Fact]
        public async Task UpdateSortableAttributes()
        {
            var newSortableAttributes = new string[] { "name", "genre" };
            await AssertUpdateSuccess(_index.UpdateSortableAttributesAsync, newSortableAttributes);
            await AssertGetEquality(_index.GetSortableAttributesAsync, newSortableAttributes);
        }

        [Fact]
        public async Task ResetSortableAttributes()
        {
            var newSortableAttributes = new string[] { "name", "genre" };
            await AssertUpdateSuccess(_index.UpdateSortableAttributesAsync, newSortableAttributes);
            await AssertGetEquality(_index.GetSortableAttributesAsync, newSortableAttributes);

            await AssertResetSuccess(_index.ResetSortableAttributesAsync);
            await AssertGetEquality(_index.GetSortableAttributesAsync, _defaultSettings.SortableAttributes);
        }

        [Fact]
        public async Task GetStopWords()
        {
            await AssertGetEquality(_index.GetStopWordsAsync, _defaultSettings.StopWords);
        }

        [Fact]
        public async Task UpdateStopWords()
        {
            var newStopWords = new string[] { "the", "and", "of" };
            await AssertUpdateSuccess(_index.UpdateStopWordsAsync, newStopWords);
            await AssertGetEquality(_index.GetStopWordsAsync, newStopWords);
        }

        [Fact]
        public async Task ResetStopWords()
        {
            var newStopWords = new string[] { "the", "and", "of" };
            await AssertUpdateSuccess(_index.UpdateStopWordsAsync, newStopWords);
            await AssertGetEquality(_index.GetStopWordsAsync, newStopWords);

            await AssertResetSuccess(_index.ResetStopWordsAsync);
            await AssertGetEquality(_index.GetStopWordsAsync, _defaultSettings.StopWords);
        }

        [Fact]
        public async Task GetSeparatorTokens()
        {
            await AssertGetEquality(_index.GetSeparatorTokensAsync, _defaultSettings.SeparatorTokens);
        }

        [Fact]
        public async Task UpdateSeparatorTokens()
        {
            var newSeparatorTokens = new List<string> { "-", "/", "&sep" };
            await AssertUpdateSuccess(_index.UpdateSeparatorTokensAsync, newSeparatorTokens);
            await AssertGetEquality(_index.GetSeparatorTokensAsync, newSeparatorTokens);
        }

        [Fact]
        public async Task ResetSeparatorTokens()
        {
            var newSeparatorTokens = new List<string> { "-", "/", "&sep" };
            await AssertUpdateSuccess(_index.UpdateSeparatorTokensAsync, newSeparatorTokens);
            await AssertGetEquality(_index.GetSeparatorTokensAsync, newSeparatorTokens);

            await AssertResetSuccess(_index.ResetSeparatorTokensAsync);
            await AssertGetEquality(_index.GetSeparatorTokensAsync, _defaultSettings.SeparatorTokens);
        }

        [Fact]
        public async Task GetNonSeparatorTokens()
        {
            await AssertGetEquality(_index.GetNonSeparatorTokensAsync, _defaultSettings.NonSeparatorTokens);
        }

        [Fact]
        public async Task UpdateNonSeparatorTokens()
        {
            var newNonSeparatorTokens = new List<string> { "@", "#" };
            await AssertUpdateSuccess(_index.UpdateNonSeparatorTokensAsync, newNonSeparatorTokens);
            await AssertGetEquality(_index.GetNonSeparatorTokensAsync, newNonSeparatorTokens);
        }

        [Fact]
        public async Task ResetNonSeparatorTokens()
        {
            var newNonSeparatorTokens = new List<string> { "@", "#" };
            await AssertUpdateSuccess(_index.UpdateNonSeparatorTokensAsync, newNonSeparatorTokens);
            await AssertGetEquality(_index.GetNonSeparatorTokensAsync, newNonSeparatorTokens);

            await AssertResetSuccess(_index.ResetNonSeparatorTokensAsync);
            await AssertGetEquality(_index.GetNonSeparatorTokensAsync, _defaultSettings.NonSeparatorTokens);
        }

        [Fact]
        public async Task GetSynonyms()
        {
            await AssertGetEquality(_index.GetSynonymsAsync, _defaultSettings.Synonyms);
        }

        [Fact]
        public async Task UpdateSynonyms()
        {
            var newSynonyms = new Dictionary<string, IEnumerable<string>>
            {
                { "hp", new string[] { "harry potter" } },
                { "harry potter", new string[] { "hp" } },
            };
            await AssertUpdateSuccess(_index.UpdateSynonymsAsync, newSynonyms);
            await AssertGetEquality(_index.GetSynonymsAsync, newSynonyms);
        }

        [Fact]
        public async Task ResetSynonyms()
        {
            var newSynonyms = new Dictionary<string, IEnumerable<string>>
            {
                { "hp", new string[] { "harry potter" } },
                { "harry potter", new string[] { "hp" } },
            };
            await AssertUpdateSuccess(_index.UpdateSynonymsAsync, newSynonyms);
            await AssertGetEquality(_index.GetSynonymsAsync, newSynonyms);

            await AssertResetSuccess(_index.ResetSynonymsAsync);
            await AssertGetEquality(_index.GetSynonymsAsync, _defaultSettings.Synonyms);
        }

        [Fact]
        public async Task GetTypoTolerance()
        {
            await AssertGetEquality(_index.GetTypoToleranceAsync, _defaultSettings.TypoTolerance);
        }

        [Fact]
        public async Task UpdateTypoTolerance()
        {
            var newTypoTolerance = new TypoTolerance
            {
                DisableOnWords = new string[] { "harry", "potter" },
                MinWordSizeForTypos = new TypoTolerance.TypoSize
                {
                    TwoTypos = 12
                }
            };

            var returnedTypoTolerance = new TypoTolerance
            {
                Enabled = true,
                DisableOnAttributes = Array.Empty<string>(),
                DisableOnWords = new string[] { "harry", "potter" },
                MinWordSizeForTypos = new TypoTolerance.TypoSize
                {
                    TwoTypos = 12,
                    OneTypo = 5
                }
            };

            await AssertUpdateSuccess(_index.UpdateTypoToleranceAsync, newTypoTolerance);
            await AssertGetEquality(_index.GetTypoToleranceAsync, returnedTypoTolerance);
        }

        [Fact]
        public async Task UpdateTypoTolerancePartially()
        {
            var newTypoTolerance = new TypoTolerance
            {
                DisableOnWords = new string[] { "harry", "potter" },
            };

            var otherUpdateTypoTolerance = new TypoTolerance
            {
                DisableOnAttributes = new string[] { "title" },
            };

            var returnedTypoTolerance = new TypoTolerance
            {
                Enabled = true,
                DisableOnAttributes = new string[] { "title" },
                DisableOnWords = new string[] { "harry", "potter" },
                MinWordSizeForTypos = new TypoTolerance.TypoSize
                {
                    TwoTypos = 9,
                    OneTypo = 5
                }
            };

            await AssertUpdateSuccess(_index.UpdateTypoToleranceAsync, newTypoTolerance);
            await AssertUpdateSuccess(_index.UpdateTypoToleranceAsync, otherUpdateTypoTolerance);
            await AssertGetEquality(_index.GetTypoToleranceAsync, returnedTypoTolerance);
        }

        [Fact]
        public async Task ResetTypoTolerance()
        {
            var newTypoTolerance = new TypoTolerance
            {
                DisableOnWords = new string[] { "harry", "potter" },
                MinWordSizeForTypos = new TypoTolerance.TypoSize
                {
                    TwoTypos = 12
                }
            };

            var returnedTypoTolerance = new TypoTolerance
            {
                Enabled = true,
                DisableOnAttributes = Array.Empty<string>(),
                DisableOnWords = new string[] { "harry", "potter" },
                MinWordSizeForTypos = new TypoTolerance.TypoSize
                {
                    TwoTypos = 12,
                    OneTypo = 5
                }
            };

            await AssertUpdateSuccess(_index.UpdateTypoToleranceAsync, newTypoTolerance);
            await AssertGetEquality(_index.GetTypoToleranceAsync, returnedTypoTolerance);

            await AssertResetSuccess(_index.ResetTypoToleranceAsync);
            await AssertGetEquality(_index.GetTypoToleranceAsync, _defaultSettings.TypoTolerance);
        }

        [Fact]
        public async Task GetFaceting()
        {
            await AssertGetEquality(_index.GetFacetingAsync, _defaultSettings.Faceting);
        }

        [Fact]
        public async Task UpdateFaceting()
        {
            var newFaceting = new Faceting
            {
                MaxValuesPerFacet = 20,
                SortFacetValuesBy = new Dictionary<string, SortFacetValuesByType>
                {
                    ["*"] = SortFacetValuesByType.Count
                }
            };

            await AssertUpdateSuccess(_index.UpdateFacetingAsync, newFaceting);
            await AssertGetEquality(_index.GetFacetingAsync, newFaceting);
        }

        [Fact]
        public async Task ResetFaceting()
        {
            var newFaceting = new Faceting
            {
                MaxValuesPerFacet = 30,
                SortFacetValuesBy = new Dictionary<string, SortFacetValuesByType>
                {
                    ["*"] = SortFacetValuesByType.Count
                }
            };

            await AssertUpdateSuccess(_index.UpdateFacetingAsync, newFaceting);
            await AssertGetEquality(_index.GetFacetingAsync, newFaceting);

            await AssertResetSuccess(_index.ResetFacetingAsync);
            await AssertGetEquality(_index.GetFacetingAsync, _defaultSettings.Faceting);
        }

        [Fact]
        public async Task GetPagination()
        {
            await AssertGetEquality(_index.GetPaginationAsync, _defaultSettings.Pagination);
        }

        [Fact]
        public async Task UpdatePagination()
        {
            var newPagination = new Pagination
            {
                MaxTotalHits = 20
            };

            await AssertUpdateSuccess(_index.UpdatePaginationAsync, newPagination);
            await AssertGetEquality(_index.GetPaginationAsync, newPagination);
        }

        [Fact]
        public async Task ResetPagination()
        {
            var newPagination = new Pagination
            {
                MaxTotalHits = 30
            };

            await AssertUpdateSuccess(_index.UpdatePaginationAsync, newPagination);
            await AssertGetEquality(_index.GetPaginationAsync, newPagination);

            await AssertResetSuccess(_index.ResetPaginationAsync);
            await AssertGetEquality(_index.GetPaginationAsync, _defaultSettings.Pagination);
        }

        [Fact]
        public async Task GetProximityPrecision()
        {
            await AssertGetEquality(_index.GetProximityPrecisionAsync, _defaultSettings.ProximityPrecision);
        }

        [Fact]
        public async Task UpdateProximityPrecision()
        {
            var newPrecision = "byAttribute";

            await AssertUpdateSuccess(_index.UpdateProximityPrecisionAsync, newPrecision);
            await AssertGetEquality(_index.GetProximityPrecisionAsync, newPrecision);
        }

        [Fact]
        public async Task ResetProximityPrecision()
        {
            var newPrecision = "byAttribute";

            await AssertUpdateSuccess(_index.UpdateProximityPrecisionAsync, newPrecision);
            await AssertGetEquality(_index.GetProximityPrecisionAsync, newPrecision);

            await AssertResetSuccess(_index.ResetProximityPrecisionAsync);
            await AssertGetEquality(_index.GetProximityPrecisionAsync, _defaultSettings.ProximityPrecision);
        }

        [Fact]
        public async Task GetDictionaryAsync()
        {
            await AssertGetEquality(_index.GetDictionaryAsync, _defaultSettings.Dictionary);
        }

        [Fact]
        public async Task UpdateDictionaryAsync()
        {
            var newDictionary = new string[] { "W. E. B.", "W.E.B." };

            await AssertUpdateSuccess(_index.UpdateDictionaryAsync, newDictionary);
            await AssertGetEquality(_index.GetDictionaryAsync, newDictionary);
        }

        [Fact]
        public async Task ResetDictionaryAsync()
        {
            var newDictionary = new string[] { "W. E. B.", "W.E.B." };

            await AssertUpdateSuccess(_index.UpdateDictionaryAsync, newDictionary);
            await AssertGetEquality(_index.GetDictionaryAsync, newDictionary);

            await AssertResetSuccess(_index.ResetDictionaryAsync);
            await AssertGetEquality(_index.GetDictionaryAsync, _defaultSettings.Dictionary);
        }

        [Fact]
        public async Task GetSearchCutoffMsAsync()
        {
            await AssertGetEquality(_index.GetSearchCutoffMsAsync, _defaultSettings.SearchCutoffMs);
        }

        [Fact]
        public async Task UpdateSearchCutoffMsAsync()
        {
            var newSearchCutoffMs = 2000;
            await AssertUpdateSuccess(_index.UpdateSearchCutoffMsAsync, newSearchCutoffMs);
            await AssertGetEquality(_index.GetSearchCutoffMsAsync, newSearchCutoffMs);
        }

        [Fact]
        public async Task ResetSearchCutoffMsAsync()
        {
            var newSearchCutoffMs = 2000;
            await AssertUpdateSuccess(_index.UpdateSearchCutoffMsAsync, newSearchCutoffMs);
            await AssertGetEquality(_index.GetSearchCutoffMsAsync, newSearchCutoffMs);

            await AssertResetSuccess(_index.ResetSearchCutoffMsAsync);
            await AssertGetEquality(_index.GetSearchCutoffMsAsync, _defaultSettings.SearchCutoffMs);
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
                SeparatorTokens = inputSettings.SeparatorTokens ?? defaultSettings.SeparatorTokens,
                NonSeparatorTokens = inputSettings.NonSeparatorTokens ?? defaultSettings.NonSeparatorTokens,
                Synonyms = inputSettings.Synonyms ?? defaultSettings.Synonyms,
                FilterableAttributes = inputSettings.FilterableAttributes ?? defaultSettings.FilterableAttributes,
                SortableAttributes = inputSettings.SortableAttributes ?? defaultSettings.SortableAttributes,
                TypoTolerance = inputSettings.TypoTolerance ?? defaultSettings.TypoTolerance,
                Faceting = inputSettings.Faceting ?? defaultSettings.Faceting,
                Pagination = inputSettings.Pagination ?? defaultSettings.Pagination,
                ProximityPrecision = inputSettings.ProximityPrecision ?? defaultSettings.ProximityPrecision,
                Dictionary = inputSettings.Dictionary ?? defaultSettings.Dictionary,
                SearchCutoffMs = inputSettings.SearchCutoffMs ?? defaultSettings.SearchCutoffMs,
                LocalizedAttributes = inputSettings.LocalizedAttributes ?? defaultSettings.LocalizedAttributes
            };
        }

        private static async Task AssertGetEquality<TValue>(IndexGetMethod<TValue> getMethod, TValue expectedValue)
        {
            var value = await getMethod();
            value.Should().BeEquivalentTo(expectedValue);
        }

        private static async Task AssertGetInequality<TValue>(IndexGetMethod<TValue> getMethod, TValue expectedValue)
        {
            var value = await getMethod();
            value.Should().NotBeEquivalentTo(expectedValue);
        }

        private async Task AssertTaskInfoSucceeded(TaskInfo task)
        {
            task.TaskUid.Should().BeGreaterThan(0);
            var taskResource = await _index.WaitForTaskAsync(task.TaskUid);
            taskResource.Status.Should().Be(TaskInfoStatus.Succeeded);
        }

        private async Task AssertUpdateSuccess<TValue>(IndexUpdateMethod<TValue> updateMethod, TValue newValue)
        {
            var task = await updateMethod(newValue);
            await AssertTaskInfoSucceeded(task);
        }

        private async Task AssertResetSuccess(IndexResetMethod resetMethod)
        {
            var task = await resetMethod();
            await AssertTaskInfoSucceeded(task);
        }
    }
}

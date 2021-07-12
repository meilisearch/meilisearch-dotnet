namespace Meilisearch.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    [Collection("Sequential")]
    public class SettingsTests
    {
        private MeilisearchClient client;
        private Index index;
        private IEnumerable<string> defaultRankingRules;
        private IEnumerable<string> defaultSearchableAndDisplayedAttributes;

        public SettingsTests(IndexFixture fixture)
        {
            fixture.DeleteAllIndexes().Wait(); // Test context cleaned for each [Fact]
            this.client = fixture.DefaultClient;
            this.defaultRankingRules = new string[]
            {
                "typo",
                "words",
                "proximity",
                "attribute",
                "wordsPosition",
                "exactness",
            };
            this.defaultSearchableAndDisplayedAttributes = new string[] { "*" };
            this.index = fixture.SetUpBasicIndex("BasicIndex-SettingsTests").Result;
        }

        [Fact]
        public async Task GetSettings()
        {
            Settings settings = await this.index.GetSettings();
            settings.Should().NotBeNull();
            Assert.Equal(settings.RankingRules, this.defaultRankingRules);
            settings.DistinctAttribute.Should().BeNull();
            Assert.Equal(settings.SearchableAttributes, this.defaultSearchableAndDisplayedAttributes);
            Assert.Equal(settings.DisplayedAttributes, this.defaultSearchableAndDisplayedAttributes);
            settings.StopWords.Should().BeNull();
            settings.Synonyms.Should().BeNull();
            settings.FilterableAttributes.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateSettings()
        {
            Settings newSettings = new Settings
            {
                SearchableAttributes = new string[] { "name", "genre" },
                StopWords = new string[] { "of", "the" },
                DistinctAttribute = "name",
            };
            UpdateStatus update = await this.index.UpdateSettings(newSettings);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await this.index.WaitForPendingUpdate(update.UpdateId);

            Settings response = await this.index.GetSettings();
            response.Should().NotBeNull();
            response.RankingRules.Should().Equals(this.defaultRankingRules);
            response.DistinctAttribute.Should().Equals("name");
            Assert.Equal(new string[] { "name", "genre" }, response.SearchableAttributes);
            Assert.Equal(this.defaultSearchableAndDisplayedAttributes, response.DisplayedAttributes);
            Assert.Equal(new string[] { "of", "the" }, response.StopWords);
            response.Synonyms.Should().BeEmpty();
            response.FilterableAttributes.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateSettingsWithoutOverwritting()
        {
            // First update
            var synonyms = new Dictionary<string, IEnumerable<string>>();
            synonyms.Add("HP", new[] { "Harry Potter" });
            synonyms.Add("Harry Potter", new[] { "HP" });
            Settings newSettings = new Settings
            {
                SearchableAttributes = new string[] { "name", "genre" },
                StopWords = new string[] { "of", "the" },
                DistinctAttribute = "name",
                Synonyms = synonyms,
            };
            UpdateStatus update = await this.index.UpdateSettings(newSettings);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await this.index.WaitForPendingUpdate(update.UpdateId);

            // Second update: this one should not overwritten StopWords and DistinctAttribute.
            newSettings = new Settings { SearchableAttributes = new string[] { "name" } };
            update = await this.index.UpdateSettings(newSettings);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await this.index.WaitForPendingUpdate(update.UpdateId);

            Settings response = await this.index.GetSettings();
            response.Should().NotBeNull();
            response.RankingRules.Should().Equals(this.defaultRankingRules);
            response.DistinctAttribute.Should().Equals("name");
            Assert.Equal(new string[] { "name" }, response.SearchableAttributes);
            Assert.Equal(this.defaultSearchableAndDisplayedAttributes, response.DisplayedAttributes);
            Assert.Equal(new string[] { "of", "the" }, response.StopWords);
            Assert.Equal(response.Synonyms["hp"], new[] { "harry potter" });
            Assert.Equal(response.Synonyms["harry potter"], new[] { "hp" });
            response.FilterableAttributes.Should().BeEmpty();
        }

        [Fact]
        public async Task ResetSettings()
        {
            // Update all settings
            Settings newSettings = new Settings
            {
                SearchableAttributes = new string[] { "name", "genre" },
                StopWords = new string[] { "of", "the" },
                DistinctAttribute = "name",
                DisplayedAttributes = new string[] { "name" },
                RankingRules = new string[] { "typo" },
                FilterableAttributes = new string[] { "genre" },
            };
            UpdateStatus update = await this.index.UpdateSettings(newSettings);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await this.index.WaitForPendingUpdate(update.UpdateId);
            Settings response = await this.index.GetSettings();
            Assert.Equal(new string[] { "typo" }, response.RankingRules);

            // Reset all settings
            update = await this.index.ResetSettings();
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await this.index.WaitForPendingUpdate(update.UpdateId);
            response = await this.index.GetSettings();
            response.Should().NotBeNull();
            Assert.Equal(response.RankingRules, this.defaultRankingRules);
            response.DistinctAttribute.Should().BeNull();
            Assert.Equal(response.SearchableAttributes, this.defaultSearchableAndDisplayedAttributes);
            Assert.Equal(response.DisplayedAttributes, this.defaultSearchableAndDisplayedAttributes);
            response.StopWords.Should().BeEmpty();
            response.Synonyms.Should().BeEmpty();
            response.FilterableAttributes.Should().BeEmpty();
        }
    }
}

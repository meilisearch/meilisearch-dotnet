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
        private IEnumerable<string> defaultRankingRules;
        private IEnumerable<string> defaultSearchableAndDisplayedAttributes;

        public SettingsTests()
        {
            this.client = new MeilisearchClient("http://localhost:7700", "masterKey");
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
        }

        [Fact]
        public async Task GetAllSettings()
        {
            var indexUID = "GetSettingsTests";
            var index = await this.client.GetOrCreateIndex(indexUID);
            Settings settings = await index.GetAllSettings();
            settings.Should().NotBeNull();
            Assert.Equal(settings.RankingRules, this.defaultRankingRules);
            settings.DistinctAttribute.Should().BeNull();
            Assert.Equal(settings.SearchableAttributes, this.defaultSearchableAndDisplayedAttributes);
            Assert.Equal(settings.DisplayedAttributes, this.defaultSearchableAndDisplayedAttributes);
            settings.StopWords.Should().BeEmpty();
            settings.Synonyms.Should().BeEmpty();
            settings.AttributesForFaceting.Should().BeEmpty();
            await index.Delete();
        }

        [Fact]
        public async Task UpdateAllSettings()
        {
            var indexUID = "UpdateSettingsTests1";
            var index = await this.client.GetOrCreateIndex(indexUID);
            Settings newSettings = new Settings
            {
                SearchableAttributes = new string[] { "name", "genre" },
                StopWords = new string[] { "of", "the" },
                DistinctAttribute = "name",
            };
            UpdateStatus update = await index.UpdateAllSettings(newSettings);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdate(update.UpdateId);

            Settings response = await index.GetAllSettings();
            response.Should().NotBeNull();
            response.RankingRules.Should().Equals(this.defaultRankingRules);
            response.DistinctAttribute.Should().Equals("name");
            Assert.Equal(new string[] { "name", "genre" }, response.SearchableAttributes);
            Assert.Equal(this.defaultSearchableAndDisplayedAttributes, response.DisplayedAttributes);
            Assert.Equal(new string[] { "of", "the" }, response.StopWords);
            response.Synonyms.Should().BeEmpty();
            response.AttributesForFaceting.Should().BeEmpty();

            await index.Delete();
        }

        [Fact]
        public async Task UpdateAllSettingsWithoutOverwritting()
        {
            var indexUID = "UpdateSettingsTests2";
            var index = await this.client.GetOrCreateIndex(indexUID);

            // First update
            Settings newSettings = new Settings
            {
                SearchableAttributes = new string[] { "name", "genre" },
                StopWords = new string[] { "of", "the" },
                DistinctAttribute = "name",
            };
            UpdateStatus update = await index.UpdateAllSettings(newSettings);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdate(update.UpdateId);

            // Second update: this one should not overwritten StopWords and DistinctAttribute.
            newSettings = new Settings { SearchableAttributes = new string[] { "name" } };
            update = await index.UpdateAllSettings(newSettings);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdate(update.UpdateId);

            Settings response = await index.GetAllSettings();
            response.Should().NotBeNull();
            response.RankingRules.Should().Equals(this.defaultRankingRules);
            response.DistinctAttribute.Should().Equals("name");
            Assert.Equal(new string[] { "name" }, response.SearchableAttributes);
            Assert.Equal(this.defaultSearchableAndDisplayedAttributes, response.DisplayedAttributes);
            Assert.Equal(new string[] { "of", "the" }, response.StopWords);
            response.Synonyms.Should().BeEmpty();
            response.AttributesForFaceting.Should().BeEmpty();

            await index.Delete();
        }

        [Fact]
        public async Task ResetAllSettings()
        {
            var indexUID = "ResetSettingsTests1";
            var index = await this.client.GetOrCreateIndex(indexUID);

            // Update all settings
            Settings newSettings = new Settings
            {
                SearchableAttributes = new string[] { "name", "genre" },
                StopWords = new string[] { "of", "the" },
                DistinctAttribute = "name",
                DisplayedAttributes = new string[] { "name" },
                RankingRules = new string[] { "typo" },
                AttributesForFaceting = new string[] { "genre" },
            };
            UpdateStatus update = await index.UpdateAllSettings(newSettings);
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdate(update.UpdateId);
            Settings response = await index.GetAllSettings();
            Assert.Equal(new string[] { "typo" }, response.RankingRules);

            // Reset all settings
            update = await index.ResetAllSettings();
            update.UpdateId.Should().BeGreaterOrEqualTo(0);
            await index.WaitForPendingUpdate(update.UpdateId);
            response = await index.GetAllSettings();
            response.Should().NotBeNull();
            Assert.Equal(response.RankingRules, this.defaultRankingRules);
            response.DistinctAttribute.Should().BeNull();
            Assert.Equal(response.SearchableAttributes, this.defaultSearchableAndDisplayedAttributes);
            Assert.Equal(response.DisplayedAttributes, this.defaultSearchableAndDisplayedAttributes);
            response.StopWords.Should().BeEmpty();
            response.Synonyms.Should().BeEmpty();
            response.AttributesForFaceting.Should().BeEmpty();

            await index.Delete();
        }
    }
}

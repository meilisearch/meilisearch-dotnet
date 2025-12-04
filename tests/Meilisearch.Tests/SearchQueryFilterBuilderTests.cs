using System;

using FluentAssertions;

using Xunit;

namespace Meilisearch.Tests
{
    public class SearchQueryFilterBuilderTests
    {
        [Fact]
        public void Where_WithStringValue_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.Where("genre", "SF");

            filter.Build().Should().Be("genre = SF");
        }

        [Fact]
        public void Where_WithStringValueContainingSpaces_ReturnsQuotedExpression()
        {
            var filter = SearchQueryFilterBuilder.Where("genre", "sci fi");

            filter.Build().Should().Be("genre = 'sci fi'");
        }

        [Fact]
        public void Where_WithIntValue_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.Where("id", 12);

            filter.Build().Should().Be("id = 12");
        }

        [Fact]
        public void Where_WithBoolValue_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.Where("isActive", true);

            filter.Build().Should().Be("isActive = true");
        }

        [Fact]
        public void WhereNot_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereNot("genre", "horror");

            filter.Build().Should().Be("genre != horror");
        }

        [Fact]
        public void WhereGreaterThan_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereGreaterThan("rating", 85);

            filter.Build().Should().Be("rating > 85");
        }

        [Fact]
        public void WhereGreaterThanOrEqual_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereGreaterThanOrEqual("rating", 85);

            filter.Build().Should().Be("rating >= 85");
        }

        [Fact]
        public void WhereLessThan_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereLessThan("rating", 50);

            filter.Build().Should().Be("rating < 50");
        }

        [Fact]
        public void WhereLessThanOrEqual_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereLessThanOrEqual("rating", 50);

            filter.Build().Should().Be("rating <= 50");
        }

        [Fact]
        public void WhereBetween_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereBetween("rating", 80, 89);

            filter.Build().Should().Be("rating 80 TO 89");
        }

        [Fact]
        public void WhereIn_WithParamsArray_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereIn("genre", "horror", "comedy", "action");

            filter.Build().Should().Be("genre IN [horror, comedy, action]");
        }

        [Fact]
        public void WhereIn_WithMixedValues_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereIn("id", 1, 2, 3);

            filter.Build().Should().Be("id IN [1, 2, 3]");
        }

        [Fact]
        public void WhereExists_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereExists("release_date");

            filter.Build().Should().Be("release_date EXISTS");
        }

        [Fact]
        public void WhereNotExists_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereNotExists("release_date");

            filter.Build().Should().Be("release_date NOT EXISTS");
        }

        [Fact]
        public void WhereEmpty_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereEmpty("overview");

            filter.Build().Should().Be("overview IS EMPTY");
        }

        [Fact]
        public void WhereNotEmpty_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereNotEmpty("overview");

            filter.Build().Should().Be("overview IS NOT EMPTY");
        }

        [Fact]
        public void WhereNull_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereNull("overview");

            filter.Build().Should().Be("overview IS NULL");
        }

        [Fact]
        public void WhereNotNull_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereNotNull("overview");

            filter.Build().Should().Be("overview IS NOT NULL");
        }

        [Fact]
        public void WhereGeoRadius_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereGeoRadius(45.4777599, 9.1967508, 2000);

            filter.Build().Should().Be("_geoRadius(45.4777599, 9.1967508, 2000)");
        }

        [Fact]
        public void WhereGeoBoundingBox_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereGeoBoundingBox(45.494181, 9.179175, 45.449484, 9.234175);

            filter.Build().Should().Be("_geoBoundingBox([45.494181, 9.179175], [45.449484, 9.234175])");
        }

        [Fact]
        public void And_CombinesTwoFilters()
        {
            var filter = SearchQueryFilterBuilder.Where("genre", "SF")
                .And(SearchQueryFilterBuilder.WhereGreaterThan("id", 12));

            filter.Build().Should().Be("genre = SF AND id > 12");
        }

        [Fact]
        public void Or_CombinesTwoFilters()
        {
            var filter = SearchQueryFilterBuilder.Where("genre", "horror")
                .Or(SearchQueryFilterBuilder.Where("genre", "comedy"));

            filter.Build().Should().Be("genre = horror OR genre = comedy");
        }

        [Fact]
        public void Not_NegatesFilter()
        {
            var filter = SearchQueryFilterBuilder.Where("genre", "horror").Not();

            filter.Build().Should().Be("NOT genre = horror");
        }

        [Fact]
        public void Group_WrapsFilterInParentheses()
        {
            var filter = SearchQueryFilterBuilder.Where("genre", "horror")
                .Or(SearchQueryFilterBuilder.Where("genre", "comedy"))
                .Group();

            filter.Build().Should().Be("(genre = horror OR genre = comedy)");
        }

        [Fact]
        public void GroupOr_CombinesFiltersWithOrAndGroups()
        {
            var filter = SearchQueryFilterBuilder.GroupOr(
                SearchQueryFilterBuilder.Where("genre", "horror"),
                SearchQueryFilterBuilder.Where("genre", "comedy")
            );

            filter.Build().Should().Be("(genre = horror OR genre = comedy)");
        }

        [Fact]
        public void GroupAnd_CombinesFiltersWithAndAndGroups()
        {
            var filter = SearchQueryFilterBuilder.GroupAnd(
                SearchQueryFilterBuilder.Where("genre", "SF"),
                SearchQueryFilterBuilder.WhereGreaterThan("rating", 85)
            );

            filter.Build().Should().Be("(genre = SF AND rating > 85)");
        }

        [Fact]
        public void ComplexExpression_WithAndOrAndGroup()
        {
            // (genres = horror OR genres = comedy) AND release_date > 795484800
            var filter = SearchQueryFilterBuilder.GroupOr(
                    SearchQueryFilterBuilder.Where("genres", "horror"),
                    SearchQueryFilterBuilder.Where("genres", "comedy"))
                .And(SearchQueryFilterBuilder.WhereGreaterThan("release_date", 795484800));

            filter.Build().Should().Be("(genres = horror OR genres = comedy) AND release_date > 795484800");
        }

        [Fact]
        public void ComplexExpression_WithMultipleConditions()
        {
            // genre = SF AND id > 12 AND rating >= 80
            var filter = SearchQueryFilterBuilder.Where("genre", "SF")
                .And(SearchQueryFilterBuilder.WhereGreaterThan("id", 12))
                .And(SearchQueryFilterBuilder.WhereGreaterThanOrEqual("rating", 80));

            filter.Build().Should().Be("genre = SF AND id > 12 AND rating >= 80");
        }

        [Fact]
        public void ImplicitStringConversion_Works()
        {
            string filter = SearchQueryFilterBuilder.Where("genre", "SF");

            filter.Should().Be("genre = SF");
        }

        [Fact]
        public void ToString_ReturnsExpression()
        {
            var filter = SearchQueryFilterBuilder.Where("genre", "SF");

            filter.ToString().Should().Be("genre = SF");
        }

        [Fact]
        public void Where_WithDateTime_ConvertsToUnixTimestamp()
        {
            var date = new DateTime(1995, 3, 18, 0, 0, 0, DateTimeKind.Utc);
            var filter = SearchQueryFilterBuilder.WhereGreaterThan("release_date", date);

            // 795484800 is the Unix timestamp for 1995-03-18
            filter.Build().Should().Be("release_date > 795484800");
        }

        [Fact]
        public void Where_WithDateTimeOffset_ConvertsToUnixTimestamp()
        {
            var date = new DateTimeOffset(1995, 3, 18, 0, 0, 0, TimeSpan.Zero);
            var filter = SearchQueryFilterBuilder.WhereGreaterThan("release_date", date);

            filter.Build().Should().Be("release_date > 795484800");
        }

        [Fact]
        public void Where_WithNestedAttribute_ReturnsCorrectExpression()
        {
            var filter = SearchQueryFilterBuilder.WhereGreaterThan("rating.users", 85);

            filter.Build().Should().Be("rating.users > 85");
        }

        [Fact]
        public void Where_WithSingleQuoteInValue_EscapesCorrectly()
        {
            var filter = SearchQueryFilterBuilder.Where("title", "Jordan's Movie");

            filter.Build().Should().Be("title = 'Jordan''s Movie'");
        }

        [Fact]
        public void EmptyFilterBuilder_And_ReturnsOther()
        {
            var empty = new SearchQueryFilterBuilder();
            var other = SearchQueryFilterBuilder.Where("genre", "SF");

            var result = empty.And(other);

            result.Build().Should().Be("genre = SF");
        }

        [Fact]
        public void EmptyFilterBuilder_Or_ReturnsOther()
        {
            var empty = new SearchQueryFilterBuilder();
            var other = SearchQueryFilterBuilder.Where("genre", "SF");

            var result = empty.Or(other);

            result.Build().Should().Be("genre = SF");
        }

        [Fact]
        public void FilterBuilder_CanBeUsedInSearchQuery()
        {
            var filter = SearchQueryFilterBuilder.Where("genre", "SF")
                .And(SearchQueryFilterBuilder.WhereGreaterThan("id", 12));

            var query = new SearchQuery
            {
                Q = "batman",
                Filter = filter.Build()
            };

            ((string)query.Filter).Should().Be("genre = SF AND id > 12");
        }
    }
}

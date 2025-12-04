using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Meilisearch
{
    /// <summary>
    /// A fluent builder for creating Meilisearch filter expressions.
    /// </summary>
    public class SearchQueryFilterBuilder
    {
        private readonly string _expression;

        private SearchQueryFilterBuilder(string expression)
        {
            _expression = expression;
        }

        /// <summary>
        /// Creates an empty filter builder.
        /// </summary>
        public SearchQueryFilterBuilder()
        {
            _expression = string.Empty;
        }

        /// <summary>
        /// Creates a filter expression where the attribute equals the specified value.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder Where(string attribute, object value)
        {
            return new SearchQueryFilterBuilder($"{attribute} = {FormatValue(value)}");
        }

        /// <summary>
        /// Creates a filter expression where the attribute does not equal the specified value.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereNot(string attribute, object value)
        {
            return new SearchQueryFilterBuilder($"{attribute} != {FormatValue(value)}");
        }

        /// <summary>
        /// Creates a filter expression where the attribute is greater than the specified value.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereGreaterThan(string attribute, object value)
        {
            return new SearchQueryFilterBuilder($"{attribute} > {FormatValue(value)}");
        }

        /// <summary>
        /// Creates a filter expression where the attribute is greater than or equal to the specified value.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereGreaterThanOrEqual(string attribute, object value)
        {
            return new SearchQueryFilterBuilder($"{attribute} >= {FormatValue(value)}");
        }

        /// <summary>
        /// Creates a filter expression where the attribute is less than the specified value.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereLessThan(string attribute, object value)
        {
            return new SearchQueryFilterBuilder($"{attribute} < {FormatValue(value)}");
        }

        /// <summary>
        /// Creates a filter expression where the attribute is less than or equal to the specified value.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereLessThanOrEqual(string attribute, object value)
        {
            return new SearchQueryFilterBuilder($"{attribute} <= {FormatValue(value)}");
        }

        /// <summary>
        /// Creates a filter expression where the attribute value is between two values (inclusive).
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="from">The lower bound value.</param>
        /// <param name="to">The upper bound value.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereBetween(string attribute, object from, object to)
        {
            return new SearchQueryFilterBuilder($"{attribute} {FormatValue(from)} TO {FormatValue(to)}");
        }

        /// <summary>
        /// Creates a filter expression where the attribute value is in the specified list.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="values">The list of values.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereIn(string attribute, params object[] values)
        {
            var formattedValues = string.Join(", ", values.Select(FormatValue));
            return new SearchQueryFilterBuilder($"{attribute} IN [{formattedValues}]");
        }

        /// <summary>
        /// Creates a filter expression where the attribute value is in the specified list.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="values">The list of values.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereIn(string attribute, IEnumerable<object> values)
        {
            return WhereIn(attribute, values.ToArray());
        }

        /// <summary>
        /// Creates a filter expression where the attribute exists.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereExists(string attribute)
        {
            return new SearchQueryFilterBuilder($"{attribute} EXISTS");
        }

        /// <summary>
        /// Creates a filter expression where the attribute does not exist.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereNotExists(string attribute)
        {
            return new SearchQueryFilterBuilder($"{attribute} NOT EXISTS");
        }

        /// <summary>
        /// Creates a filter expression where the attribute is empty.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereEmpty(string attribute)
        {
            return new SearchQueryFilterBuilder($"{attribute} IS EMPTY");
        }

        /// <summary>
        /// Creates a filter expression where the attribute is not empty.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereNotEmpty(string attribute)
        {
            return new SearchQueryFilterBuilder($"{attribute} IS NOT EMPTY");
        }

        /// <summary>
        /// Creates a filter expression where the attribute is null.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereNull(string attribute)
        {
            return new SearchQueryFilterBuilder($"{attribute} IS NULL");
        }

        /// <summary>
        /// Creates a filter expression where the attribute is not null.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereNotNull(string attribute)
        {
            return new SearchQueryFilterBuilder($"{attribute} IS NOT NULL");
        }

        /// <summary>
        /// Creates a geo radius filter expression.
        /// </summary>
        /// <param name="latitude">The latitude of the center point.</param>
        /// <param name="longitude">The longitude of the center point.</param>
        /// <param name="radiusInMeters">The radius in meters.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereGeoRadius(double latitude, double longitude, double radiusInMeters)
        {
            var lat = latitude.ToString(CultureInfo.InvariantCulture);
            var lng = longitude.ToString(CultureInfo.InvariantCulture);
            var radius = radiusInMeters.ToString(CultureInfo.InvariantCulture);
            return new SearchQueryFilterBuilder($"_geoRadius({lat}, {lng}, {radius})");
        }

        /// <summary>
        /// Creates a geo bounding box filter expression.
        /// </summary>
        /// <param name="topLeftLatitude">The latitude of the top-left corner.</param>
        /// <param name="topLeftLongitude">The longitude of the top-left corner.</param>
        /// <param name="bottomRightLatitude">The latitude of the bottom-right corner.</param>
        /// <param name="bottomRightLongitude">The longitude of the bottom-right corner.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder WhereGeoBoundingBox(
            double topLeftLatitude,
            double topLeftLongitude,
            double bottomRightLatitude,
            double bottomRightLongitude)
        {
            var topLeftLat = topLeftLatitude.ToString(CultureInfo.InvariantCulture);
            var topLeftLng = topLeftLongitude.ToString(CultureInfo.InvariantCulture);
            var bottomRightLat = bottomRightLatitude.ToString(CultureInfo.InvariantCulture);
            var bottomRightLng = bottomRightLongitude.ToString(CultureInfo.InvariantCulture);
            return new SearchQueryFilterBuilder($"_geoBoundingBox([{topLeftLat}, {topLeftLng}], [{bottomRightLat}, {bottomRightLng}])");
        }

        /// <summary>
        /// Combines this filter with another filter using AND.
        /// </summary>
        /// <param name="other">The other filter builder.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public SearchQueryFilterBuilder And(SearchQueryFilterBuilder other)
        {
            if (other == null)
            {
                return this;
            }

            if (string.IsNullOrEmpty(_expression))
            {
                return other;
            }

            if (string.IsNullOrEmpty(other._expression))
            {
                return this;
            }

            return new SearchQueryFilterBuilder($"{_expression} AND {other._expression}");
        }

        /// <summary>
        /// Combines this filter with another filter using OR.
        /// </summary>
        /// <param name="other">The other filter builder.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public SearchQueryFilterBuilder Or(SearchQueryFilterBuilder other)
        {
            if (other == null)
            {
                return this;
            }

            if (string.IsNullOrEmpty(_expression))
            {
                return other;
            }

            if (string.IsNullOrEmpty(other._expression))
            {
                return this;
            }

            return new SearchQueryFilterBuilder($"{_expression} OR {other._expression}");
        }

        /// <summary>
        /// Negates the current filter expression.
        /// </summary>
        /// <returns>A new FilterBuilder instance.</returns>
        public SearchQueryFilterBuilder Not()
        {
            if (string.IsNullOrEmpty(_expression))
            {
                return this;
            }

            return new SearchQueryFilterBuilder($"NOT {_expression}");
        }

        /// <summary>
        /// Groups the current filter expression with parentheses.
        /// </summary>
        /// <returns>A new FilterBuilder instance.</returns>
        public SearchQueryFilterBuilder Group()
        {
            if (string.IsNullOrEmpty(_expression))
            {
                return this;
            }

            return new SearchQueryFilterBuilder($"({_expression})");
        }

        /// <summary>
        /// Creates a grouped filter expression from multiple filters combined with AND.
        /// </summary>
        /// <param name="filters">The filters to combine.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder GroupAnd(params SearchQueryFilterBuilder[] filters)
        {
            var combined = filters.Aggregate(new SearchQueryFilterBuilder(), (current, filter) => current.And(filter));
            return combined.Group();
        }

        /// <summary>
        /// Creates a grouped filter expression from multiple filters combined with OR.
        /// </summary>
        /// <param name="filters">The filters to combine.</param>
        /// <returns>A new FilterBuilder instance.</returns>
        public static SearchQueryFilterBuilder GroupOr(params SearchQueryFilterBuilder[] filters)
        {
            var combined = filters.Aggregate(new SearchQueryFilterBuilder(), (current, filter) => current.Or(filter));
            return combined.Group();
        }

        /// <summary>
        /// Builds and returns the filter expression string.
        /// </summary>
        /// <returns>The filter expression string.</returns>
        public string Build()
        {
            return _expression;
        }

        /// <summary>
        /// Implicitly converts the FilterBuilder to a string.
        /// </summary>
        /// <param name="builder">The filter builder.</param>
        public static implicit operator string(SearchQueryFilterBuilder builder)
        {
            return builder?.Build() ?? string.Empty;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _expression;
        }

        private static string FormatValue(object value)
        {
            if (value == null)
            {
                return "null";
            }

            if (value is string s)
            {
                return FormatStringValue(s);
            }

            if (value is bool b)
            {
                return b ? "true" : "false";
            }

            if (value is int i)
            {
                return i.ToString(CultureInfo.InvariantCulture);
            }

            if (value is long l)
            {
                return l.ToString(CultureInfo.InvariantCulture);
            }

            if (value is float f)
            {
                return f.ToString(CultureInfo.InvariantCulture);
            }

            if (value is double d)
            {
                return d.ToString(CultureInfo.InvariantCulture);
            }

            if (value is decimal dec)
            {
                return dec.ToString(CultureInfo.InvariantCulture);
            }

            if (value is DateTime dt)
            {
                return ((DateTimeOffset)dt).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
            }

            if (value is DateTimeOffset dto)
            {
                return dto.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
            }

            return FormatStringValue(value.ToString());
        }

        private static string FormatStringValue(string value)
        {
            // If the value contains spaces or special characters, wrap in single quotes
            if (value.Contains(' ') || value.Contains('\'') || value.Contains('"') || value.Contains('\\'))
            {
                // Escape backslashes first, then single quotes (Meilisearch uses backslash escaping)
                var escaped = value.Replace("\\", "\\\\").Replace("'", "\\'");
                return $"'{escaped}'";
            }

            return value;
        }
    }
}

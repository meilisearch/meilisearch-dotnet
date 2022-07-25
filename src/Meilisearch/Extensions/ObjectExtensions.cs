using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Meilisearch.Extensions
{
    /// <summary>
    /// Meilisearch objects manipulation.
    /// </summary>
    internal static class ObjectExtensions
    {
        /// <summary>
        /// Transforms an Meilisearch object into a dictionary.
        /// </summary>
        /// <param name="source">Object to transform.</param>
        /// <param name="bindingAttr">Binding flags.</param>
        /// <returns>Returns a dictionary.</returns>
        internal static IDictionary<string, string> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).Where(p => p.GetValue(source, null) != null).ToDictionary(
                propInfo => char.ToLowerInvariant(propInfo.Name[0]) + propInfo.Name.Substring(1),
                propInfo => propInfo.GetValue(source, null).ToString());
        }

        /// <summary>
        /// Transforms a Meilisearch object into an URL encoded query string.
        /// </summary>
        /// <param name="source">Object to transform.</param>
        /// <param name="bindingAttr">Binding flags.</param>
        /// <returns>Returns an url encoded query string.</returns>
        internal static string ToQueryString(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            var values = source.GetType().GetProperties(bindingAttr)
            .Where(p => p.GetValue(source, null) != null)
            .Select(p =>
                Uri.EscapeDataString(char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1)) + "=" + Uri.EscapeDataString(p.GetValue(source, null).ToString()));
            var queryString = string.Join("&", values);
            return queryString;
        }

        /// <summary>
        /// Transforms a Meilisearch object containing Lists into an URL encoded query string.
        /// </summary>
        /// <param name="source">Object to transform.</param>
        /// <param name="bindingAttr">Binding flags.</param>
        /// <returns>Returns an url encoded query string.</returns>
        internal static string ToQueryStringWithList(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            var values = new List<string>(); ;
            foreach (var p in source.GetType().GetProperties(bindingAttr))
            {
                if (p.GetValue(source, null) != null)
                {
                    if (!(p.GetValue(source, null).GetType().IsGenericType && p.GetValue(source, null).GetType().GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        values.Add(Uri.EscapeDataString(char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1)) + "=" + Uri.EscapeDataString(p.GetValue(source, null).ToString()));
                    }
                    if (p.GetValue(source, null).GetType().IsGenericType && p.GetValue(source, null).GetType().GetGenericTypeDefinition() == typeof(List<>))
                    {
                        values.Add(Uri.EscapeDataString(char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1)) + "=" + string.Join(",", (List<string>)p.GetValue(source, null)));
                    }
                }
            }
            return string.Join("&", values);
        }
    }
}

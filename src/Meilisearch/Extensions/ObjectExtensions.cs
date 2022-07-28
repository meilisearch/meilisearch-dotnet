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
        /// Transforms a Meilisearch object containing Lists into an URL encoded query string.
        /// </summary>
        /// <param name="source">Object to transform.</param>
        /// <param name="bindingAttr">Binding flags.</param>
        /// <returns>Returns an url encoded query string.</returns>
        internal static string ToQueryString(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            var values = new List<string>();
            foreach (var field in source.GetType().GetProperties(bindingAttr))
            {
                if (field.GetValue(source, null) != null)
                {
                    var isList = field.GetValue(source, null).GetType().IsGenericType && field.GetValue(source, null).GetType().GetGenericTypeDefinition() == typeof(List<>);
                    if (isList)
                    {
                        values.Add(Uri.EscapeDataString(char.ToLowerInvariant(field.Name[0]) + field.Name.Substring(1)) + "=" + string.Join(",", (List<string>)field.GetValue(source, null)));
                    }
                    else
                    {
                        values.Add(Uri.EscapeDataString(char.ToLowerInvariant(field.Name[0]) + field.Name.Substring(1)) + "=" + Uri.EscapeDataString(field.GetValue(source, null).ToString()));
                    }
                }
            }
            return string.Join("&", values);
        }
    }
}

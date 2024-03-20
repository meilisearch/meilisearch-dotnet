using System;
using System.Collections.Generic;
using System.Globalization;
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
        /// Transforms any object fields into an URL encoded query string.
        /// </summary>
        /// <param name="source">Object to transform.</param>
        /// <param name="bindingAttr">Binding flags.</param>
        /// <param name="uri">Uri to prepend before query string.</param>
        /// <returns>Returns an url encoded query string.</returns>
        internal static string ToQueryString(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance, string uri = "")
        {
            var values = new List<string>();
            if (source != null)
            {
                foreach (var field in source.GetType().GetProperties(bindingAttr))
                {
                    var value = field.GetValue(source, null);
                    var key = Uri.EscapeDataString(char.ToLowerInvariant(field.Name[0]) + field.Name.Substring(1));

                    if (value != null)
                    {
                        var type = value.GetType();

                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var itemType = type.GetGenericArguments()[0];
                            if (itemType == typeof(string))
                            {
                                values.Add(key + "=" + string.Join(",", (List<string>)value));
                            }
                            else if (itemType == typeof(int))
                            {
                                values.Add(key + "=" + string.Join(",", (List<int>)value));
                            }
                            else if (itemType == typeof(TaskInfoStatus))
                            {
                                values.Add(key + "=" + string.Join(",", ((List<TaskInfoStatus>)value).Select(x => x.ToString())));
                            }
                            else if (itemType == typeof(TaskInfoType))
                            {
                                values.Add(key + "=" + string.Join(",", ((List<TaskInfoType>)value).Select(x => x.ToString())));
                            }
                        }
                        else if (value is DateTime)
                        {
                            values.Add(key + "=" + Uri.EscapeDataString(((DateTime)value).ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz")));
                        }
                        else
                        {
                            values.Add(key + "=" + Uri.EscapeDataString(value.ToString()));
                        }
                    }
                }
            }

            var queryString = string.Join("&", values);
            if (string.IsNullOrWhiteSpace(uri))
            {
                return queryString;
            }

            if (string.IsNullOrEmpty(queryString))
            {
                return uri;
            }

            return $"{uri}?{queryString}";
        }
    }
}

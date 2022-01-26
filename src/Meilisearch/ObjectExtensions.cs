namespace Meilisearch
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Meilisearch objects manipulation.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Transforms an Meilisearch object into a dictionary.
        /// </summary>
        /// <param name="source">Object to transform.</param>
        /// <param name="bindingAttr">Binding flags.</param>
        /// <returns>Returns a dictionary.</returns>
        public static IDictionary<string, string> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
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
        public static string ToQueryString(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            var values = source.GetType().GetProperties(bindingAttr)
            .Where(p => p.GetValue(source, null) != null)
            .Select(p =>
                Uri.EscapeDataString(char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1)) + "=" + Uri.EscapeDataString(p.GetValue(source, null).ToString()));
            var queryString = string.Join("&", values);
            return queryString;
        }

        /// <summary>
        /// Iterates through Ienumerable and removes null keys.
        /// </summary>
        /// <param name="objectToTransform">Object to transform.</param>
        /// <typeparam name="T">The element type of the IEnumerable.</typeparam>
        /// <returns>Returns the same IEnumerable with null keys removed. </returns>
        public static object RemoveNullValues<T>(this IEnumerable<T> objectToTransform)
        {
            var result = objectToTransform.Select(item =>
            {
                dynamic expando = new ExpandoObject();
                var x = expando as IDictionary<string, object>;
                foreach (var p in item.GetType().GetProperties().Where(p => p.GetValue(item) != null))
                {
                    x[p.Name.ToLower()] = p.GetValue(item, null);
                }

                return expando;
            }).ToList().AsEnumerable();
            return result;
        }
    }
}

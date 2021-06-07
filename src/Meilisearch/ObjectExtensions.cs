namespace Meilisearch
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// MeiliSearch objects manipulation.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Transforms an MeiliSearch object into a dictionary.
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
        /// Iterates through Ienumerable and removes null keys.
        /// </summary>
        /// <param name="objectToTransform">Object to transform.</param>
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

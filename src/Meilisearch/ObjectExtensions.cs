namespace Meilisearch
{
    using System.Collections.Generic;
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
        /// <returns>Returns a dictionary.</returns>
        public static IDictionary<string, string> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).Where(p => p.GetValue(source, null) != null).ToDictionary(
                propInfo => char.ToLowerInvariant(propInfo.Name[0]) + propInfo.Name.Substring(1),
                propInfo => propInfo.GetValue(source, null).ToString());
        }
    }
}

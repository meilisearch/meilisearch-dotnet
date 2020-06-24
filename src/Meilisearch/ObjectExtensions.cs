using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Meilisearch
{
    public static class ObjectExtensions
    {
        public static IDictionary<string, string> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).Where(p=>p.GetValue(source,null)!=null).ToDictionary
            (
                propInfo => Char.ToLowerInvariant(propInfo.Name[0]) + propInfo.Name.Substring(1),
                propInfo => propInfo.GetValue(source, null).ToString()
            );

        }
    }
}

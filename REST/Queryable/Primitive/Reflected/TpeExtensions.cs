using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    internal static class TpeExtensions
    {
        public static T TryGetAttribute<T>(this Type type)
        {
            var attr = (T)(type.GetCustomAttributes(typeof(T), true).FirstOrDefault());
            return attr;
        }

        public static T TryGetAttribute<T>(this System.Reflection.PropertyInfo property)
        {
            var attr = (T)(property.GetCustomAttributes(typeof(T), true).FirstOrDefault());
            return attr;
        }
    }
}

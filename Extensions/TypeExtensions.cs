using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Check if a String is in the Guid format
        /// </summary>
        /// <remarks>Gale String Type Extension</remarks>
        /// <param name="str">String to Check</param>
        /// <returns></returns>
        public static bool isGuid(this String str)
        {
            Guid guid = System.Guid.Empty;
            System.Guid.TryParse(str, out guid);

            return guid != System.Guid.Empty;
        }

        /// <summary>
        /// Check is a Type is a typical "primitive" Type 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSimpleType(this Type type)
        {
            return
                type.IsPrimitive || 
                new Type[] {
                    typeof(Enum),
                    typeof(String),
                    typeof(Decimal),
                    typeof(DateTime),
                    typeof(DateTimeOffset),
                    typeof(TimeSpan),
                    typeof(Guid)
                }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object ||
                (
                    type.IsGenericType && 
                    type.GetGenericTypeDefinition() == typeof(Nullable<>) && 
                    IsSimpleType(type.GetGenericArguments()[0])
                );
        }
    }
}

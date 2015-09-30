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
    }
}

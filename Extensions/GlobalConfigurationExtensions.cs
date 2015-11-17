using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class GlobalConfigurationExtensions
    {
        public static JsonMediaTypeFormatter KqlFormatter(this MediaTypeFormatterCollection collection)
        {
            return new Gale.REST.Http.Formatter.KqlFormatter();
        }
    }
}

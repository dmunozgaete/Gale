using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gale.REST.Http.Formatter
{
    /// <summary>
    /// Chrome Json Formatter Fix for IE
    /// </summary>
    public class JsonFormatter : JsonMediaTypeFormatter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public JsonFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            this.SerializerSettings.Formatting = Formatting.Indented;
        }

        /// <summary>
        /// Set default content
        /// </summary>
        /// <param name="type"></param>
        /// <param name="headers"></param>
        /// <param name="mediaType"></param>
        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
    }
}


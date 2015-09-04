using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace Swashbuckle.Swagger.Annotations
{

    /// <summary>
    /// Add in Swagger a Impliciti "header" parameter , (useful for testing in swagger-ui)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class HeaderParameter : System.Attribute
    {
        private string _name = null;
        private bool _required = true;

        /// <summary>
        /// Define a Header Parameter to send in swagger-ui
        /// </summary>
        /// <param name="header">Header name to send</param>
        /// <param name="required">define if a required parameter</param>
        public HeaderParameter(String header, bool required)
        {
            _name = header;
            _required = required;
        }

        /// <summary>
        /// Define a Header Parameter to send in swagger-ui (not required by default)
        /// </summary>
        /// <param name="header">Header name to send</param>
        public HeaderParameter(String header)
        {
            _name = header;
            _required = true;
        }

        /// <summary>
        /// Retrieves if the header parameter is required or not
        /// </summary>
        public Boolean Required
        {
            get
            {
                return _required;
            }
        }

        /// <summary>
        /// Retrieves the Header Name
        /// </summary>
        public String Name
        {
            get
            {
                return _name;
            }
        }
    }
}

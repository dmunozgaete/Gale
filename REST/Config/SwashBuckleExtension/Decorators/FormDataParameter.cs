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
    /// Add in Swagger a Implicit "formData" parameter , (useful for testing in swagger-ui)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class FormDataParameter : System.Attribute
    {
        private string _name = null;
        private bool _required = true;
        private string _default = String.Empty;

        /// <summary>
        /// Define a FormData Parameter to send in swagger-ui
        /// </summary>
        /// <param name="name">Header name to send</param>
        /// <param name="required">define if a required parameter</param>
        /// <param name="defaultValue">default value</param>
        public FormDataParameter(String name, bool required, String defaultValue)
        {
            _name = name;
            _required = required;
            _default = defaultValue;
        }

        /// <summary>
        /// Define a FormData Parameter to send in swagger-ui
        /// </summary>
        /// <param name="name">Header name to send</param>
        /// <param name="required">define if a required parameter</param>
        public FormDataParameter(String name, bool required)
        {
            _name = name;
            _required = required;
        }

        /// <summary>
        /// Define a FormData Parameter to send in swagger-ui (not required by default)
        /// </summary>
        /// <param name="name">Parameter name</param>
        public FormDataParameter(String header)
        {
            _name = header;
            _required = true;
        }

        /// <summary>
        /// Retrieves if the FormData parameter is required or not
        /// </summary>
        public Boolean Required
        {
            get
            {
                return _required;
            }
        }

        /// <summary>
        /// Retrieves the Parameter Name
        /// </summary>
        public String Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Retrieves the Default Value
        /// </summary>
        public String DefaultValue
        {
            get
            {
                return _default;
            }
        }
    }
}

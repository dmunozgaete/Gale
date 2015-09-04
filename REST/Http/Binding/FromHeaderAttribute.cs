using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace System.Web.Http
{
    /// <summary>
    /// Bind a Parameter to Header in Request
    /// </summary>
    public class FromHeaderAttribute : ParameterBindingAttribute
    {
        private string _name;

        /// <summary>
        /// Header Name
        /// </summary>
        internal String Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Define a Header Parameter
        /// </summary>
        public FromHeaderAttribute()
        {
            this._name = null;
        }


        /// <summary>
        /// Define a Header Parameter
        /// </summary>
        /// <param name="header">Header name to send</param>
        public FromHeaderAttribute(string header)
        {
            this._name = header;
        }

        /// <summary>
        /// Get Binding Process
        /// </summary>
        /// <param name="parameter">Parameter Descriptor</param>
        /// <returns></returns>
        public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
        {

            if(String.IsNullOrEmpty(this._name)){
                this._name = parameter.ParameterName;
            }

            return new Gale.REST.Http.Binding.FromHeaderBinding(parameter, this._name );
        }
    }
}

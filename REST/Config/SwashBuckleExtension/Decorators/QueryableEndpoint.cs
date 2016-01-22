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
    /// Add in Swagger a Explicit "Queryable Endpoint" , (useful for testing in swagger-ui)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class QueryableEndpoint: System.Attribute
    {
        private Type _queryableType = null;


        /// <summary>
        /// Defines a Queryable Endpoint
        /// </summary>
        /// <param name="header">Header name to send</param>
        public QueryableEndpoint()
        {}


        /// <summary>
        /// Defines a Queryable Endpoint ans hist Type
        /// </summary>
        /// <param name="type"></param>
        public QueryableEndpoint(Type type)
        {
            _queryableType = type;
        }

        /// <summary>
        /// Queryable Type Output
        /// </summary>
        public Type QueryableType
        {
            get
            {
                return _queryableType;
            }
        }
    }
}

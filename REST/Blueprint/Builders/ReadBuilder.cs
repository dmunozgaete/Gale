using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Gale.REST.Queryable.OData.Builders;

namespace Gale.REST.Blueprint.Builders
{
    /// <summary>
    /// Agnostic Database Builder for a Read Process
    /// </summary>
    public abstract class ReadBuilder : Gale.REST.Http.HttpBaseActionResult
    {
        private Type _modelType;
        private HttpRequestMessage _request;
        private GQLConfiguration _configuration;

        /// <summary>
        /// HttpRequest context associated with the request
        /// </summary>
        public HttpRequestMessage Request
        {
            get
            {
                return _request;
            }
        }

        /// <summary>
        /// Model Type associated with the request
        /// </summary>
        public Type modelType
        {
            get
            {

                return _modelType;
            }
        }

        /// <summary>
        /// Base Gale Query Language configuration
        /// </summary>
        public GQLConfiguration Configuration
        {
            get
            {

                return _configuration;
            }
        }

        /// <summary>
        /// Retrieves the Raw Result
        /// </summary>
        /// <returns></returns>
        public abstract Queryable.Primitive.Result GetRawResult();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Identifier Record in the Database</param>
        /// <param name="modelType">Model Type from the BluePrint Controller</param>
        public ReadBuilder(HttpRequestMessage request, GQLConfiguration configuration, Type modelType)
        {
            _modelType = modelType;
            _request = request;
            _configuration = configuration;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Blueprint.Builders
{
    /// <summary>
    /// Agnostic Database Builder for a Read Process
    /// </summary>
    public abstract class ReadBuilder : Gale.REST.Http.HttpBaseActionResult
    {
        private Type _modelType;
        private HttpRequestMessage _request;

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
        /// Constructor
        /// </summary>
        /// <param name="id">Identifier Record in the Database</param>
        /// <param name="modelType">Model Type from the BluePrint Controller</param>
        public ReadBuilder(HttpRequestMessage request, Type modelType)
        {
            _modelType = modelType;
            _request = request;
        }
    }
}

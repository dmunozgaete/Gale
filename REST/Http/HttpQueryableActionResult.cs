using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Gale.REST.Queryable.OData.Builders;

//CONTENT NEGOTATION: http://www.strathweb.com/2012/07/everything-you-want-to-know-about-asp-net-web-api-content-negotation/

namespace Gale.REST.Http
{
    /// <summary>
    /// Define a Queryable Http Result based in the ODATA Protocol (http://www.odata.org)
    /// </summary>
    /// <typeparam name="TModel">Model associated with the query</typeparam>
    public class HttpQueryableActionResult<TModel> : Gale.REST.Http.HttpBaseActionResult where TModel : class
    {
        HttpRequestMessage _request;
        GQLConfiguration _configuration;

        /// <summary>
        /// Create a Queryable Endpoint Action Result
        /// </summary>
        /// <param name="request">Http Request Context</param>
        public HttpQueryableActionResult(HttpRequestMessage request)
        {
            _request = request;
        }

        /// <summary>
        /// Create a Queryable Endpoint Action Result
        /// </summary>
        /// <param name="request">Http Request Context</param>
        /// <param name="configuration">Base Configuration</param>
        public HttpQueryableActionResult(HttpRequestMessage request, GQLConfiguration configuration)
        {
            _request = request;
            _configuration = configuration;
        }

        #region Caching Builder Types for Perfomance
        private static Type _readBuilderType;

        /// <summary>
        /// Find the Read Builder Type associated to the Database Factory Type
        /// </summary>
        private static Type ReadBuilderType
        {
            get
            {
                if (_readBuilderType == null)
                {
                    var types = Gale.Db.Factories.FactoryTarget.GetTypesByDatabaseTarget<Gale.REST.Blueprint.Builders.ReadBuilder>();
                    _readBuilderType = types.FirstOrDefault();
                }

                //Check if Builder Exist!
                Gale.Exception.RestException.Guard(() => _readBuilderType == null, "READBUILDER_NOTIMPLEMENTED_FOR_CURRENT_DATABASE_TYPE", Gale.Exception.Errors.ResourceManager);

                return _readBuilderType;
            }
        }
        #endregion

        /// <summary>
        /// Retrieves Raw Result
        /// </summary>
        /// <returns></returns>
        public Queryable.Primitive.Result GetRawResult()
        {
            var builder = ReadBuilderType.MakeGenericType(new Type[] { typeof(TModel) });
            return ((Gale.REST.Blueprint.Builders.ReadBuilder)Activator.CreateInstance(builder, new object[] { _request, _configuration })).GetRawResult();
        }

        /// <summary>
        /// Execute Async Process
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var builder = ReadBuilderType.MakeGenericType(new Type[] { typeof(TModel) });
            return ((IHttpActionResult)Activator.CreateInstance(builder, new object[] { _request, _configuration })).ExecuteAsync(cancellationToken);
        }
    }
}

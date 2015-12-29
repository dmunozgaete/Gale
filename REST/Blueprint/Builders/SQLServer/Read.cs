using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Gale.REST.Queryable.OData.Builders;

namespace Gale.REST.Blueprint.Builders.SQLServer
{
    /// <summary>
    /// Defines a Read Action to Database for SQL Server Factory
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    [Gale.Db.Factories.FactoryTarget(typeof(Gale.Db.Factories.SQLServerFactory))]
    internal class Read<TModel> : Gale.REST.Blueprint.Builders.ReadBuilder where TModel : class
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request">HttpRequest Message</param>
        /// <param name="configuration">Gale Query Language Setup</param>
        public Read(HttpRequestMessage request, GQLConfiguration configuration) : base(request, configuration, typeof(TModel)) { }



        /// <summary>
        /// Async Process
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var builder = new Gale.REST.Queryable.OData.Builders.SQLServer.HttpQueryBuilder<TModel>(this.Connection, this.Request, this.Configuration);
            return Task.FromResult(builder.GetResponse());
        }


        /// <summary>
        /// Retrieves The Raw Result
        /// </summary>
        /// <returns></returns>
        public override Queryable.Primitive.Result GetRawResult()
        {
            var builder = new Gale.REST.Queryable.OData.Builders.SQLServer.HttpQueryBuilder<TModel>(this.Connection, this.Request, this.Configuration);
            return builder.GetResult();
        }
    }
}

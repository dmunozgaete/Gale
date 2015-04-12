using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Karma.REST.Queryable.Blueprint
{
    internal class QueryableResult<TModel> : Karma.REST.Http.HttpActionResult where TModel : class
    {
        HttpRequestMessage _request;

        private SortedList<Type, Delegate> _descriptors = new SortedList<Type, Delegate>();

        public QueryableResult(HttpRequestMessage request)
        {
            _request = request;
        }

        public override Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            string query = _request.RequestUri.Query;
            Karma.REST.Queryable.OData.QueryBuilder<TModel> builder = new Karma.REST.Queryable.OData.QueryBuilder<TModel>(this.Connection, query);
            System.Reflection.MethodInfo methodbase = builder.GetType().GetMethod("RegisterForeignField");

            foreach (Type tableType in this._descriptors.Keys)
            {
                System.Reflection.MethodInfo method = methodbase.MakeGenericMethod(tableType);
                method.Invoke(builder, new object[] { this._descriptors[tableType] });
            }

            Karma.REST.Queryable.Primitive.IResponse odata_response = builder.Execute();

            return Task.FromResult(
                    _request.CreateResponse<Object>(odata_response.toPlainObject())
            );
        }

    }
}

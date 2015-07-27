using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Gale.REST.Queryable.Blueprint
{
    public class QueryableResult<TModel> : Gale.REST.Http.HttpActionResult where TModel : class
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
            Gale.REST.Queryable.OData.QueryBuilder<TModel> builder = new Gale.REST.Queryable.OData.QueryBuilder<TModel>(this.Connection, query);
            System.Reflection.MethodInfo methodbase = builder.GetType().GetMethod("RegisterForeignField");

            foreach (Type tableType in this._descriptors.Keys)
            {
                System.Reflection.MethodInfo method = methodbase.MakeGenericMethod(tableType);
                method.Invoke(builder, new object[] { this._descriptors[tableType] });
            }

            Gale.REST.Queryable.Primitive.IResponse odata_response = builder.Execute();

            return Task.FromResult(
                    _request.CreateResponse<Object>(odata_response.toPlainObject())
            );
        }

    }
}

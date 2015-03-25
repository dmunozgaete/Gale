﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Karma.REST.Queryable.Blueprint
{
    internal class QueryableResult<TModel> : IHttpActionResult where TModel : class
    {
        HttpRequestMessage _request;
        Karma.Db.IDataActions _iDataActions;

        private SortedList<Type, Delegate> _descriptors = new SortedList<Type, Delegate>();

        public QueryableResult(HttpRequestMessage request, Karma.Db.IDataActions connection)
        {
            _request = request;
            _iDataActions = connection;
        }
        
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                string query = _request.RequestUri.Query;
                Karma.REST.Queryable.OData.QueryBuilder<TModel> builder = new Karma.REST.Queryable.OData.QueryBuilder<TModel>(_iDataActions, query);
                System.Reflection.MethodInfo methodbase = builder.GetType().GetMethod("RegisterForeignField");

                foreach (Type tableType in this._descriptors.Keys)
                {
                    System.Reflection.MethodInfo method = methodbase.MakeGenericMethod(tableType);
                    method.Invoke(builder, new object[] { this._descriptors[tableType] });
                }

                Karma.REST.Queryable.Primitive.IResponse odata_response = builder.Execute();
                return Task.FromResult(_request.CreateResponse<Object>(
                    odata_response.toPlainObject()
                ));
            }
            catch (System.Exception ex)
            {
                if (ex.GetType() == typeof(System.Web.Http.HttpResponseException))
                {
                    throw ex;
                }
                else
                {
                    return Task.FromResult(_request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex.Message));
                }
            }

        }

    }
}
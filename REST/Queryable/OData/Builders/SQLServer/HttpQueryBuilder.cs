using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Gale.REST.Queryable.OData.Builders.SQLServer
{
    /// <summary>
    /// SQL SERVER QUERY BUILDER
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    [Gale.Db.Factories.FactoryTarget(typeof(Gale.Db.Factories.SQLServerFactory))]
    internal class HttpQueryBuilder<TModel> : Gale.REST.Queryable.OData.Builders.HttpQueryBuilder<TModel>
        where TModel : class
    {

        public HttpQueryBuilder(Gale.Db.IDataActions databaseFactory, HttpRequestMessage request, GQLConfiguration configuration)
            : base(databaseFactory, request, configuration)
        {
            System.Collections.Specialized.NameValueCollection query = System.Web.HttpUtility.ParseQueryString(request.RequestUri.Query);

            //------------------------------------------------------------------------------------------
            //--[ ORDER BY PARSER (FOR SQL SERVER WE NEED TO ENGAGE FIRST)
            this.RegisterParser<OData.Builders.SQLServer.Parsers.OrderBy>(this.Kql);
            //------------------------------------------------------------------------------------------

            //------------------------------------------------------------------------------------------
            //--[ SELECT PARSER
            this.RegisterParser<OData.Builders.SQLServer.Parsers.Select>(this.Kql);
            //------------------------------------------------------------------------------------------

            //------------------------------------------------------------------------------------------
            //--[ FROM PARSER (Send null query , the model , has converted automatically by constraint's)
            this.RegisterParser<OData.Builders.SQLServer.Parsers.From>(this.Kql);
            //------------------------------------------------------------------------------------------

            //------------------------------------------------------------------------------------------
            //--[ WHERE PARSER (Filter's)
            this.RegisterParser<OData.Builders.SQLServer.Parsers.Where>(this.Kql);
            //------------------------------------------------------------------------------------------

            //------------------------------------------------------------------------------------------
            //--[ APPEND THE ANSI SQL STATEMENTS (SELECT , FROM , WHERE, ORDER BY)
            String orderBy = "";
            this.OnExecutedParser += new ExecutedParserEventHandler((args) =>
            {
                #region OVERRIDE FRAGMENT
                //ORDER BY APPEND (CAPTURE AND SET NULL)
                if (args.Parser.GetType() == typeof(OData.Builders.SQLServer.Parsers.OrderBy))
                {
                    orderBy = args.ResultQueryFragment;
                    args.ResultQueryFragment = "";
                }

                //SELECT APPEND
                if (args.Parser.GetType() == typeof(OData.Builders.SQLServer.Parsers.Select))
                {
                    //INCLUDE PAGINATOR SQL SENTENCE
                    String format = String.Format("SELECT ROW_NUMBER() OVER (ORDER BY {0}) AS ROWNUM, {1}",
                        orderBy,
                        args.ResultQueryFragment
                    );
                    args.ResultQueryFragment = format; ;
                }

                //WHERE APPEND
                if (args.Parser.GetType() == typeof(OData.Builders.SQLServer.Parsers.Where))
                {
                    if (args.ResultQueryFragment.Length > 0)
                    {
                        args.ResultQueryFragment = String.Concat(" WHERE ", args.ResultQueryFragment);
                    }
                }
                #endregion
            });
            //------------------------------------------------------------------------------------------

            //------------------------------------------------------------------------------------------
            //--[ REGISTER OPERATOR'S (CAN BE EXECUTED IN ANY PARSER FOR THE "CALLOPERATOR" METHOD)
            IEnumerable<Type> operators = (from t in this.GetType().Assembly.GetTypes()
                                           where t.IsSubclassOf(typeof(OData.Builders.SQLServer.Operators.Operator))
                                           select t);

            foreach (Type Qoperator in operators)
            {
                System.Reflection.MethodInfo register = this.GetType().GetMethod("RegisterOperator").MakeGenericMethod(Qoperator);
                register.Invoke(this, new object[] { });
            }
            //------------------------------------------------------------------------------------------
        }

        /// <summary>
        /// PREPARE PAGINATION AND SQL DROP TABLE
        /// </summary>
        /// <param name="statements"></param>
        /// <returns></returns>
        internal override string PrepareCall(List<string> statements)
        {
            String temporalTable = String.Concat("##", System.Guid.NewGuid().ToString().Replace("-", String.Empty));

            //SELECT INTO CLAUSE 
            statements.Insert(0, String.Format("SELECT \n\n* \n\nINTO {0} \n\nFROM \n\n(", temporalTable));

            //CLOSE SELECT INTO
            statements.Add(") as TX; \n\n");

            //PAGINATION RETURN SQL CLAUSE
            statements.Add(String.Format("SELECT * FROM {0} TX WHERE TX.ROWNUM > {1} AND TX.ROWNUM <= {2}; \n\n",
                temporalTable,
                this.Kql.offset,
                this.Kql.offset + this.Kql.limit
            ));

            //PAGINATION COUNTER'S CLAUSE
            statements.Add(String.Format("SELECT (SELECT COUNT(*) FROM {0}) as total, {1} as offset, {2} as limit; \n\n",
                temporalTable,
                this.Kql.offset,
                this.Kql.limit
            ));

            //DROP TEMPORAL TABLE
            statements.Add(String.Format("DROP TABLE {0}; \n\n",
                temporalTable
            ));

            return base.PrepareCall(statements);
        }


        public override HttpResponseMessage GetResponse()
        {
            Gale.REST.Queryable.Primitive.Result response = base.Execute();

            Gale.REST.Queryable.OData.Results.Result odata_response;
            odata_response = new Gale.REST.Queryable.OData.Results.JsonResult(response, this.Kql.offset, this.Kql.limit);

            //TODO: CHECK VIA CONTENT-TYPE THE RESPONSE FORMMATER JsonResult or TableResult?

            return Request.CreateResponse(odata_response);
        }
    }
}

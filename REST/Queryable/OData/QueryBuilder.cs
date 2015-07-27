using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.REST.Queryable.OData
{
    internal class QueryBuilder<TModel> : Gale.REST.Queryable.Primitive.QueryBuilder<TModel>
        where TModel : class
    {

        private int _offset = 0;
        private int _limit = 10;
        private Format _format = Format.Primitive;

        public QueryBuilder(Gale.Db.IDataActions databaseFactoryType, string query)
            : base(databaseFactoryType)
        {
            System.Collections.Specialized.NameValueCollection fragments = System.Web.HttpUtility.ParseQueryString(query);

            //------------------------------------------------------------------------------------------
            //--[ ORDER BY PARSER (FOR SQL SERVER WE NEED TO ENGAGE FIRST)
            this.RegisterParser<OData.SQLServer.Parsers.OrderBy>(fragments["$orderBy"]);
            //------------------------------------------------------------------------------------------

            if (fragments.AllKeys.Contains("$offset"))
            {
                int.TryParse(fragments["$offset"], out  _offset);
            }

            if (fragments.AllKeys.Contains("$format"))
            {
                _format = fragments["$format"] == "table" ? Format.Table : Format.Primitive;
            }

            if (fragments.AllKeys.Contains("$limit"))
            {
                int.TryParse(fragments["$limit"], out  _limit);
                if (_limit < 1)
                {
                    _limit = 10;
                }
            }

            //------------------------------------------------------------------------------------------
            //--[ SELECT PARSER
            String selectedStatement = "*";
            if (fragments.AllKeys.Contains("$select"))
            {
                selectedStatement = fragments["$select"];
            }
            this.RegisterParser<OData.SQLServer.Parsers.Select>(selectedStatement);
            //------------------------------------------------------------------------------------------

            //------------------------------------------------------------------------------------------
            //--[ FROM PARSER (Send null query , the model , has converted automatically by constraint's)
            this.RegisterParser<OData.SQLServer.Parsers.From>("");
            //------------------------------------------------------------------------------------------

            //------------------------------------------------------------------------------------------
            //--[ WHERE PARSER (Filter's)
            this.RegisterParser<OData.SQLServer.Parsers.Where>(fragments["$filter"]);
            //------------------------------------------------------------------------------------------

            //------------------------------------------------------------------------------------------
            //--[ APPEND THE ANSI SQL STATEMENTS (SELECT , FROM , WHERE, ORDER BY)
            String orderBy = "";
            this.OnExecutedParser += new ExecutedParserEventHandler((args) =>
            {
                #region OVERRIDE FRAGMENT
                //ORDER BY APPEND (CAPTURE AND SET NULL)
                if (args.Parser.GetType() == typeof(OData.SQLServer.Parsers.OrderBy))
                {
                    orderBy = args.ResultQueryFragment;
                    args.ResultQueryFragment = "";
                }

                //SELECT APPEND
                if (args.Parser.GetType() == typeof(OData.SQLServer.Parsers.Select))
                {
                    //INCLUDE PAGINATOR SQL SENTENCE
                    String format = String.Format("SELECT ROW_NUMBER() OVER (ORDER BY {0}) AS ROWNUM, {1}",
                        orderBy,
                        args.ResultQueryFragment
                    );
                    args.ResultQueryFragment = format; ;
                }

                //WHERE APPEND
                if (args.Parser.GetType() == typeof(OData.SQLServer.Parsers.Where))
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
                                           where t.IsSubclassOf(typeof(OData.SQLServer.Operators.Operator))
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
        internal override string PrepareCall(List<string> statements, int offset, int limit)
        {
            String temporalTable = String.Concat("##", System.Guid.NewGuid().ToString().Replace("-", String.Empty));

            //SELECT INTO CLAUSE 
            statements.Insert(0, String.Format("SELECT * INTO {0} FROM (", temporalTable));

            //CLOSE SELECT INTO
            statements.Add(") as TX; ");

            //PAGINATION RETURN SQL CLAUSE
            statements.Add(String.Format("SELECT * FROM {0} TX WHERE TX.ROWNUM > {1} AND TX.ROWNUM <= {2};",
                temporalTable,
                offset,
                offset + limit
            ));

            //PAGINATION COUNTER'S CLAUSE
            statements.Add(String.Format("SELECT (SELECT COUNT(*) FROM {0}) as total, {1} as offset, {2} as limit;",
                temporalTable,
                _offset,
                _limit
            ));

            //DROP TEMPORAL TABLE
            statements.Add(String.Format("DROP TABLE {0};",
                temporalTable
            ));

            return base.PrepareCall(statements, offset, limit);
        }

        public Gale.REST.Queryable.Primitive.IResponse Execute()
        {
            return base.Execute(_offset, _limit, _format);
        }
    }
}

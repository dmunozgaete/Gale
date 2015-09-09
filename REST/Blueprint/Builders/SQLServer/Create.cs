using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Blueprint.Builders.SQLServer
{
    [Gale.Db.Factories.FactoryTarget(typeof(Gale.Db.Factories.SQLServerFactory))]
    internal class Create<TModel> : Gale.REST.Blueprint.Builders.CreateBuilder where TModel : class
    {
        TModel _payload;

        public Create(TModel payload): base(typeof(TModel))
        {
            _payload = payload;
        }

        public override Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            Gale.Exception.GaleException.Guard(() => this.modelType == null, System.Net.HttpStatusCode.BadRequest, "API_EMPTY_BODY");
            
            SortedDictionary<string, object> values = new SortedDictionary<string, object>();
            var table_name = this.modelType.Name;

            #region BIND DATA
            var fieldProperties = typeof(TModel).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(System.Data.Linq.Mapping.ColumnAttribute))).ToList();
            foreach (System.Reflection.PropertyInfo property in fieldProperties)
            {
                string db_name = property.Name;
                object value = null;

                if (property.GetValue(_payload) == null)
                {
                    continue;
                }

                var attr = property.TryGetAttribute<System.Data.Linq.Mapping.ColumnAttribute>();
                if (attr != null)
                {
                    System.Data.Linq.Mapping.ColumnAttribute column_attr = (attr as System.Data.Linq.Mapping.ColumnAttribute);
                    if (column_attr != null && column_attr.Name != null && column_attr.Name.Length > 0)
                    {
                        db_name = column_attr.Name;
                    }
                }

                try
                {
                    value = property.GetValue(_payload);
                }
                catch /*(System.Reflection.TargetInvocationException ex)*/
                {
                    throw new Gale.Exception.GaleException("API_CANT_SETVALUE", property.Name, table_name);
                }

                //Add as Data Value
                values.Add(db_name, value);

            }
            #endregion

            var table_attr = this.modelType.TryGetAttribute<System.Data.Linq.Mapping.TableAttribute>();
            if (table_attr != null && table_attr.Name != null && table_attr.Name.Length > 0)
            {
                table_name = table_attr.Name;
            }

            #region SQL Builder

            /*
             
                BEGIN
                    DECLARE @TABLE_NAME VARCHAR(200) = '{TABLE}';
                    DECLARE @OBJECT_ID INT = (SELECT object_id FROM sys.all_objects WHERE type_desc = 'USER_TABLE' AND name = @TABLE_NAME);
                    DECLARE @COUNT INT= (SELECT COUNT(*) FROM sys.identity_columns WHERE object_id = @OBJECT_ID);

                    INSERT INTO TABLE ( 
                        {FIELD} 
                    ) VALUES ( 
                        {VALUE}
                    ); 

                    IF(@COUNT = 1)
	                    BEGIN
		                    DECLARE @IDENTITY_COLUMN SYSNAME;
		                    SELECT TOP 1
			                    @IDENTITY_COLUMN = name
		                    FROM 
			                    sys.identity_columns WHERE object_id = @OBJECT_ID;

		                    DECLARE @GETLASTROWINSERTED NVARCHAR(4000) = 'SELECT ' + @TABLE_NAME + '.* FROM ' + @TABLE_NAME + ' WHERE ' + @IDENTITY_COLUMN + ' = ' + CONVERT(VARCHAR(200), SCOPE_IDENTITY());
		                    EXECUTE sp_executesql @GETLASTROWINSERTED;
	                    END
                    ELSE
	                    RAISERROR ('MORE_THAN_ONE_PK',12,1);

                END

             
             */


            System.Text.StringBuilder builder = new StringBuilder();
            builder.AppendFormat("INSERT INTO {0} ", table_name);
            builder.AppendFormat("( \n");
            bool isFirst = true;
            foreach (var value in values)
            {
                if (!isFirst)
                {
                    builder.Append(",");
                }
                builder.AppendFormat("{0} \n", value.Key);
                isFirst = false;
            }
            builder.Append(") VALUES ( \n");
            isFirst = true;
            foreach (var value in values)
            {
                if (!isFirst)
                {
                    builder.Append(",");
                }
                builder.AppendFormat("'{0}' \n", value.Value);
                isFirst = false;
            }
            builder.Append("); \n\n");

            string single_tableName = table_name.Substring(table_name.IndexOf(".") + 1);

            //GET COUNT FROM IDENTITY COLUMNS FOR A SPECIFIC TABLE
            builder.AppendFormat("SELECT \n");
            builder.AppendFormat("  COUNT(*) \n");
            builder.AppendFormat("FROM \n");
            builder.AppendFormat("  sys.identity_columns IDENT INNER JOIN \n");
            builder.AppendFormat("  sys.all_objects      TBLES ON IDENT.object_id = TBLES.object_id \n");
            builder.AppendFormat("  AND type_desc = 'USER_TABLE' \n");
            builder.AppendFormat("  AND TBLES.name = '{0}'; \n\n", single_tableName);

            //GET THE LAST INSERTED ROW (ONLY WORKS FOR A SINGLE PRIMARY KEY COLUMN)
            builder.AppendFormat("SELECT TOP 1 \n");
            builder.AppendFormat("  IDENT.name \n");
            builder.AppendFormat("FROM \n");
            builder.AppendFormat("  sys.identity_columns IDENT INNER JOIN \n");
            builder.AppendFormat("  sys.all_objects      TBLES ON IDENT.object_id = TBLES.object_id \n");
            builder.AppendFormat("  AND type_desc = 'USER_TABLE' \n");
            builder.AppendFormat("  AND TBLES.name = '{0}' ", single_tableName);

            string query = builder.ToString();
            #endregion

            //-------------------------------------------------------------------------------------
            //---[ DATABASE CALL
            using (Gale.Db.DataService svc = new Gale.Db.DataService(query))
            {
                try
                {
                    //Create the repository
                    this.ExecuteSql(svc);

                    HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.Created)
                    {
                        Content = new StringContent("Created")
                    };

                    return Task.FromResult(response);

                }
                catch (System.Exception ex)
                {
                    string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    throw new Gale.Exception.GaleException("API_DB_ERROR", message);
                }
            }
            //-------------------------------------------------------------------------------------
        }
    }
}

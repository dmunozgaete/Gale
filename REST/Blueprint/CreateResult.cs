using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Gale.REST.Blueprint
{

    internal class CreateResult<TModel> : Gale.REST.Http.HttpActionResult where TModel : class
    {
        Newtonsoft.Json.Linq.JToken _payload;

        public CreateResult(Newtonsoft.Json.Linq.JToken payload)
        {
            _payload = payload;
        }

        public override Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            Gale.Exception.GaleException.Guard(() => _payload == null, "API_EMPTY_BODY");


            var table_type = typeof(TModel);

            SortedDictionary<string, object> values = new SortedDictionary<string, object>();
            var table_name = table_type.Name;

            #region BIND DATA
            var fieldProperties = typeof(TModel).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(System.Data.Linq.Mapping.ColumnAttribute))).ToList();
            foreach (System.Reflection.PropertyInfo property in fieldProperties)
            {
                string db_name = property.Name;
                bool isPK = false;
                object value = null;

                if (_payload[property.Name] == null)
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
                        isPK = column_attr.IsPrimaryKey;
                    }
                }

                try
                {
                    value = _payload.GetType().GetMethod("Value").MakeGenericMethod(property.PropertyType).Invoke(_payload, new Object[]{
                        property.Name
                    });
                }
                catch /*(System.Reflection.TargetInvocationException ex)*/
                {
                    throw new Gale.Exception.GaleException("API_CANT_SETVALUE", property.Name, table_name);
                }

                //Add as Data Value
                values.Add(db_name, value);

            }
            #endregion

            var table_attr = table_type.TryGetAttribute<System.Data.Linq.Mapping.TableAttribute>();
            if (table_attr != null && table_attr.Name != null && table_attr.Name.Length > 0)
            {
                table_name = table_attr.Name;
            }

            #region SQL Builder
            System.Text.StringBuilder builder = new StringBuilder();
            builder.AppendFormat("INSERT INTO {0} ", table_name);
            builder.AppendFormat(" ( ");
            bool isFirst = true;
            foreach (var value in values)
            {
                if (!isFirst)
                {
                    builder.Append(",");
                }
                builder.AppendFormat("{0}", value.Key);
                isFirst = false;
            }
            builder.Append(" ) VALUES ( ");
            isFirst = true;
            foreach (var value in values)
            {
                if (!isFirst)
                {
                    builder.Append(",");
                }
                builder.AppendFormat("'{0}'", value.Value);
                isFirst = false;
            }
            builder.Append(" ) ");

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

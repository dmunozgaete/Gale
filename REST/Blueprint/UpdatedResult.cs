using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Karma.REST.Blueprint
{
    internal class UpdatedResult<TModel> : Karma.REST.Http.HttpActionResult where TModel : class
    {
        Newtonsoft.Json.Linq.JToken _payload;
        string _id;

        public UpdatedResult(string id, Newtonsoft.Json.Linq.JToken payload)
        {
            _id = id;
            _payload = payload;
        }

        public override Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            Karma.Exception.KarmaException.Guard(() => _payload == null, "API_EMPTY_BODY");

            var table_type = typeof(TModel);

            SortedDictionary<string, object> values = new SortedDictionary<string, object>();
            string table_name = table_type.Name;
            string primaryKey_name = null;

            #region BIND DATA
            var fieldProperties = typeof(TModel).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(System.Data.Linq.Mapping.ColumnAttribute))).ToList();
            foreach (System.Reflection.PropertyInfo property in fieldProperties)
            {
                string db_name = property.Name;
                object value = null;

                var attr = property.TryGetAttribute<System.Data.Linq.Mapping.ColumnAttribute>();
                if (attr != null)
                {
                    System.Data.Linq.Mapping.ColumnAttribute column_attr = (attr as System.Data.Linq.Mapping.ColumnAttribute);
                    if (column_attr != null && column_attr.Name != null && column_attr.Name.Length > 0)
                    {
                        db_name = column_attr.Name;

                        if (column_attr.IsPrimaryKey)
                        {
                            primaryKey_name = db_name;
                            continue;
                        }
                    }
                }

                if (_payload[property.Name] == null)
                {
                    continue;
                }

                try
                {
                    value = _payload.GetType().GetMethod("Value").MakeGenericMethod(property.PropertyType).Invoke(_payload, new Object[]{
                    property.Name
                });
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    throw new Karma.Exception.KarmaException("API_CANT_SETVALUE", property.Name, table_name);
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
            builder.AppendFormat("UPDATE {0} ", table_name);
            builder.AppendFormat("  SET");
            bool isFirst = true;
            foreach (var value in values)
            {
                if (!isFirst)
                {
                    builder.Append(",");
                }
                builder.AppendFormat("  {0} = '{1}'", value.Key, value.Value.ToString());
                isFirst = false;
            }
            builder.AppendFormat("  WHERE");
            builder.AppendFormat("  {0} = '{1}'", primaryKey_name, _id);


            string query = builder.ToString();
            #endregion

            //-------------------------------------------------------------------------------------
            //---[ DATABASE CALL
            using (Karma.Db.DataService svc = new Karma.Db.DataService(query))
            {
                try
                {
                    //Create the repository
                    this.ExecuteSql(svc);

                    HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.PartialContent)
                    {
                        Content = new StringContent("Updated")
                    };

                    return Task.FromResult(response);

                }
                catch (System.Exception ex)
                {
                    string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    throw new Karma.Exception.KarmaException("API_DB_ERROR", message);
                }
            }
            //-------------------------------------------------------------------------------------
        }
    }
}

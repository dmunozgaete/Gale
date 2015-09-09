using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Gale.REST.Blueprint.Builders.SQLServer
{
    [Gale.Db.Factories.FactoryTarget(typeof(Gale.Db.Factories.SQLServerFactory))]
    internal class Delete<TModel> : Gale.REST.Blueprint.Builders.DeleteBuilder where TModel : class
    {

        public Delete(string id) : base(id, typeof(TModel)) { }

        public override Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            var table_type = typeof(TModel);
            string table_name = table_type.Name;
            string primaryKey_name = null;

            #region FIND PRIMARY KEY
            var fieldProperties = typeof(TModel).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(System.Data.Linq.Mapping.ColumnAttribute))).ToList();
            foreach (System.Reflection.PropertyInfo property in fieldProperties)
            {
                string db_name = property.Name;
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
                            break;
                        }
                    }
                }
            }
            #endregion

            var table_attr = table_type.TryGetAttribute<System.Data.Linq.Mapping.TableAttribute>();
            if (table_attr != null && table_attr.Name != null && table_attr.Name.Length > 0)
            {
                table_name = table_attr.Name;
            }

            #region SQL Builder
            System.Text.StringBuilder builder = new StringBuilder();
            builder.AppendFormat("DELETE FROM {0} ", table_name);
            builder.AppendFormat("  WHERE");
            builder.AppendFormat("  {0} = '{1}'", primaryKey_name, this.id);


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

                    HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                    {
                        Content = new StringContent("Deleted")
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

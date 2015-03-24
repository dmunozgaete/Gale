using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace Karma.REST
{
    public class RestController : System.Web.Http.ApiController, Karma.Db.IDataActions
    {
        private static Karma.Db.IDataActions _connection = null;

        protected Karma.Db.IDataActions Connection
        {
            get
            {
                if (_connection == null)
                {

                    var cnx = System.Configuration.ConfigurationManager.ConnectionStrings["Application:default"];
                    if (cnx == null)
                    {
                        
                        throw new Karma.Exception.KarmaException("DB002");
                    }

                    try
                    {
                        Type factory_type = Type.GetType(cnx.ProviderName);
                        var factory = Activator.CreateInstance(factory_type, new object[]{ cnx.ConnectionString });
                        _connection = (Karma.Db.IDataActions)factory;
                    }
                    catch (System.Exception ex)
                    {
                        throw new Karma.Exception.KarmaException("DB001", ex.Message);
                    }

                }
                return _connection;
            }
        }

        #region IDataActions Members
        public Type DbConnection()
        {
            return Connection.DbConnection();
        }

        [NonAction]
        public Karma.Db.EntityRepository ExecuteQuery(Karma.Db.DataService Service)
        {
            return Connection.ExecuteQuery(Service);
        }

        [NonAction]
        public Karma.Db.EntityRepository ExecuteQuery(Karma.Db.DataService Service, int CommandTimeout)
        {
            return Connection.ExecuteQuery(Service, CommandTimeout);
        }

        [NonAction]
        public void ExecuteAction(Karma.Db.DataService Service)
        {
            Connection.ExecuteAction(Service);
        }

        [NonAction]
        public void ExecuteAction(Karma.Db.DataService Service, int CommandTimeout)
        {
            Connection.ExecuteAction(Service, CommandTimeout);
        }

        [NonAction]
        public object ExecuteScalar(Karma.Db.DataService Service)
        {
            return Connection.ExecuteScalar(Service);
        }

        [NonAction]
        public object ExecuteScalar(Karma.Db.DataService Service, int CommandTimeout)
        {
            return Connection.ExecuteScalar(Service, CommandTimeout);
        }

        [NonAction]
        public Karma.Db.EntityRepository ExecuteSql(Karma.Db.DataService Service)
        {
            return Connection.ExecuteSql(Service);
        }

        [NonAction]
        public Karma.Db.EntityRepository ExecuteSql(Karma.Db.DataService Service, int CommandTimeout)
        {
            return Connection.ExecuteSql(Service, CommandTimeout);
        }
        #endregion
    }
}

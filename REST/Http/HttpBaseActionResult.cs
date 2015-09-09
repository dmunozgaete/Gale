using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Gale.REST.Http
{
    /// <summary>
    /// Defines a command that asynchronously creates an System.Net.Http.HttpResponseMessage.
    /// </summary>
    public abstract class HttpBaseActionResult : System.Web.Http.IHttpActionResult, Gale.Db.IDataActions
    {
        private static Gale.Db.IDataActions _connection = null;

        /// <summary>
        /// Return the Connection DB Reference
        /// </summary>
        protected Gale.Db.IDataActions Connection
        {
            get
            {

                if (_connection == null)
                {

                    var cnx = System.Configuration.ConfigurationManager.ConnectionStrings[Gale.REST.Resources.GALE_CONNECTION_DEFAULT_KEY];
                    if (cnx == null)
                    {

                        throw new Gale.Exception.GaleException("DB002");
                    }

                    try
                    {
                        Type factory_type = Type.GetType(cnx.ProviderName);
                        var factory = Activator.CreateInstance(factory_type, new object[] { cnx.ConnectionString });
                        _connection = (Gale.Db.IDataActions)factory;
                    }
                    catch (System.Exception ex)
                    {
                        throw new Gale.Exception.GaleException("DB001", ex.Message);
                    }

                }
                return _connection;
            }
        }

        #region IDataActions Members
        [NonAction]
        public Type DbConnection()
        {
            return Connection.DbConnection();
        }

        /// <summary>
        /// Execute a Query and return the results
        /// </summary>
        /// <param name="Service"></param>
        /// <returns></returns>
        [NonAction]
        public Gale.Db.EntityRepository ExecuteQuery(Gale.Db.DataService Service)
        {
            return Connection.ExecuteQuery(Service);
        }

        /// <summary>
        /// Execute a Query and return the results
        /// </summary>
        /// <param name="Service"></param>
        /// <param name="CommandTimeout">Time to wait after a timeout throw Exception release</param>
        /// <returns></returns>
        [NonAction]
        public Gale.Db.EntityRepository ExecuteQuery(Gale.Db.DataService Service, int CommandTimeout)
        {
            return Connection.ExecuteQuery(Service, CommandTimeout);
        }


        /// <summary>
        /// Execute and Action Witouth Return Result
        /// </summary>
        /// <param name="Service"></param>
        [NonAction]
        public void ExecuteAction(Gale.Db.DataService Service)
        {
            Connection.ExecuteAction(Service);
        }


        /// <summary>
        /// Execute and Action Witouth Return Result
        /// </summary>
        /// <param name="Service"></param>
        /// <param name="CommandTimeout">Time to wait after a timeout throw Exception release</param>
        [NonAction]
        public void ExecuteAction(Gale.Db.DataService Service, int CommandTimeout)
        {
            Connection.ExecuteAction(Service, CommandTimeout);
        }

        /// <summary>
        /// Execute and Action Or Query and Return the First Column of the First Row on the first result
        /// </summary>
        /// <param name="Service"></param>
        /// <returns></returns>
        [NonAction]
        public object ExecuteScalar(Gale.Db.DataService Service)
        {
            return Connection.ExecuteScalar(Service);
        }

        /// <summary>
        /// Execute and Action Or Query and Return the First Column of the First Row on the first result
        /// </summary>
        /// <param name="Service"></param>
        /// <param name="CommandTimeout">Time to wait after a timeout throw Exception release</param>
        /// <returns></returns>
        [NonAction]
        public object ExecuteScalar(Gale.Db.DataService Service, int CommandTimeout)
        {
            return Connection.ExecuteScalar(Service, CommandTimeout);
        }

        [NonAction]
        public Gale.Db.EntityRepository ExecuteSql(Gale.Db.DataService Service)
        {
            return Connection.ExecuteSql(Service);
        }

        [NonAction]
        public Gale.Db.EntityRepository ExecuteSql(Gale.Db.DataService Service, int CommandTimeout)
        {
            return Connection.ExecuteSql(Service, CommandTimeout);
        }
        #endregion

        /// <summary>
        /// Creates an System.Net.Http.HttpResponseMessage asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <returns> A task that, when completed, contains the System.Net.Http.HttpResponseMessage.</returns>
        public abstract Task<System.Net.Http.HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken);
    }
}

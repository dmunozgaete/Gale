using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Karma.REST.Http
{
    /// <summary>
    /// Defines a command that asynchronously creates an System.Net.Http.HttpResponseMessage.
    /// </summary>
    public abstract class HttpActionResult : System.Web.Http.IHttpActionResult, Karma.Db.IDataActions
    {
        private static Karma.Db.IDataActions _connection = null;

        /// <summary>
        /// Return the Connection DB Reference
        /// </summary>
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
                        var factory = Activator.CreateInstance(factory_type, new object[] { cnx.ConnectionString });
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
        public Karma.Db.EntityRepository ExecuteQuery(Karma.Db.DataService Service)
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
        public Karma.Db.EntityRepository ExecuteQuery(Karma.Db.DataService Service, int CommandTimeout)
        {
            return Connection.ExecuteQuery(Service, CommandTimeout);
        }


        /// <summary>
        /// Execute and Action Witouth Return Result
        /// </summary>
        /// <param name="Service"></param>
        [NonAction]
        public void ExecuteAction(Karma.Db.DataService Service)
        {
            Connection.ExecuteAction(Service);
        }


        /// <summary>
        /// Execute and Action Witouth Return Result
        /// </summary>
        /// <param name="Service"></param>
        /// <param name="CommandTimeout">Time to wait after a timeout throw Exception release</param>
        [NonAction]
        public void ExecuteAction(Karma.Db.DataService Service, int CommandTimeout)
        {
            Connection.ExecuteAction(Service, CommandTimeout);
        }

        /// <summary>
        /// Execute and Action Or Query and Return the First Column of the First Row on the first result
        /// </summary>
        /// <param name="Service"></param>
        /// <returns></returns>
        [NonAction]
        public object ExecuteScalar(Karma.Db.DataService Service)
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

        /// <summary>
        /// Creates an System.Net.Http.HttpResponseMessage asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <returns> A task that, when completed, contains the System.Net.Http.HttpResponseMessage.</returns>
        public abstract Task<System.Net.Http.HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken);
    }
}

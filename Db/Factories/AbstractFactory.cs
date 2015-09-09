using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;
using Gale.Db;

namespace Gale.Db.Factories
{
    /// <summary>
    /// Defines a Database Factory, which can connect to a Specific Database
    /// </summary>
    /// <typeparam name="Tconnection"></typeparam>
    /// <typeparam name="TAdapter"></typeparam>
    public abstract class AbstractFactory<Tconnection, TAdapter> : IDataActions where Tconnection : System.Data.IDbConnection, new()
    {
        private string _connectionString = "";
        private const Int32 _defaultConnectionTimeout = 15;
        public AbstractFactory(string ConnectionString)
        {
            _connectionString = ConnectionString;
        }

        /// <summary>
        /// Connection Database Type
        /// </summary>
        /// <returns></returns>
        public Type DbConnection()
        {
            return typeof(Tconnection);
        }

        #region Build Method's

        /// <summary>
        /// Execute a Call to the database according to configuration
        /// </summary>
        /// <param name="Service">Database Service Information</param>
        /// <param name="Delegate">Database Callback</param>
        /// <param name="ConnectionTimeout">Maximum TimeOut before Throw TimeoutException</param>
        /// <returns>Return the Database Result</returns>
        private object Execute(DataService Service, Func<System.Data.IDbConnection, System.Data.IDbCommand, object> Delegate, Int32 ConnectionTimeout)
        {
            //Initialize the Component's will be Connect to the Database
            System.Data.IDbConnection DbConnection = new Tconnection();
            DbConnection.ConnectionString = _connectionString;


            System.Data.IDbCommand DbCommand = DbConnection.CreateCommand();
            if (Service != null)
            {
                DbCommand.CommandType = System.Data.CommandType.StoredProcedure;
                DbCommand.CommandText = Service.Command;
                DbCommand.CommandTimeout = ConnectionTimeout;
            }

            //Fill Parameters
            if (Service != null && Service.HasParameters())
            {
                Service.Parameters.ForEach((Parameter) =>
                {
                    string parameterName = Parameter.Name;

                    System.Data.IDbDataParameter dbParameter = DbCommand.CreateParameter();
                    dbParameter.ParameterName = parameterName;
                    dbParameter.Value = Parameter.Value;
                    dbParameter.Direction = Parameter.Direction;

                    //Special Threatment by Type
                    if (Parameter.Value != null)
                    {
                        ParameterParse(ref dbParameter, Parameter);
                    }

                    DbCommand.Parameters.Add(dbParameter);
                });
            }

            try
            {
                //Open The Connection
                if (DbConnection.State != System.Data.ConnectionState.Open)
                {
                    DbConnection.Open();
                }

                //Call Handler
                object ret = Delegate(DbConnection, DbCommand);
                return ret;
            }
            catch (System.Exception ex)
            {
                this.OnException(ex);   //Overridable method!

                //Throw Exception Between The Top Layers
                throw new System.Exception(String.Format("[{0}] Ex: {1}", Service.Command, ex.Message), ex);
            }
            finally
            {
                //Finally Close The Connection [Always]
                DbConnection.Close();
            }
        }

        /// <summary>
        /// Execute a Call to the database according to configuration
        /// </summary>
        /// <param name="Service">Database Service Information</param>
        /// <param name="Delegate">Database Callback</param>
        /// <returns>Return the Database Result</returns>
        private object Execute(DataService Service, Func<System.Data.IDbConnection, System.Data.IDbCommand, object> Delegate)
        {
            return Execute(Service, Delegate, _defaultConnectionTimeout);
        }

        /// <summary>
        /// Execute a Massive Call's to the database according to services configuration
        /// </summary>
        /// <param name="Service">Database Service Information</param>
        /// <param name="Delegate">Database Callback</param>
        /// <param name="ConnectionTimeout">Maximum TimeOut before Throw TimeoutException</param>
        /// <returns></returns>
        private void Execute(DataService[] Services, Action<System.Data.IDbCommand> Delegate, Int32 ConnectionTimeout)
        {
            //Initialize the Component's will be Connect to the Database
            System.Data.IDbConnection DbConnection = new Tconnection();
            DbConnection.ConnectionString = _connectionString;
            DbConnection.Open();
            System.Data.IDbTransaction _tran = DbConnection.BeginTransaction();
            System.Data.IDbCommand DbCommand = DbConnection.CreateCommand();
            DbCommand.CommandTimeout = ConnectionTimeout;
            DbCommand.Transaction = _tran;
            string _currentService = "";
            try
            {
                foreach (DataService Service in Services)
                {
                    DbCommand.Parameters.Clear();
                    if (Service != null)
                    {
                        DbCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        DbCommand.CommandText = _currentService = Service.Command;
                    }

                    //Fill Parameters
                    if (Service != null && Service.HasParameters())
                    {
                        Service.Parameters.ForEach((Parameter) =>
                        {
                            System.Data.IDbDataParameter dbParameter = DbCommand.CreateParameter();
                            dbParameter.ParameterName = Parameter.Name;
                            dbParameter.Value = Parameter.Value;
                            dbParameter.Direction = Parameter.Direction;

                            //Special Threatment by Type
                            if (Parameter.Value != null)
                            {
                                ParameterParse(ref dbParameter, Parameter);
                            }

                            DbCommand.Parameters.Add(dbParameter);
                        });
                    }


                    //Call Handler
                    Delegate(DbCommand);

                }


                //Call Handler
                _tran.Commit();
            }
            catch (System.Exception ex)
            {
                //Throw Exception Between The Top Layers
                _tran.Rollback();
                DbConnection.Close();

                throw new Gale.Exception.GaleException("ServiceTransactionError", _currentService, ex.Message);
            }
            finally
            {
                //Finally Close The Connection [Always]
                DbConnection.Close();
            }

        }

        /// <summary>
        /// Execute a Massive Call's to the database according to services configuration
        /// </summary>
        /// <param name="Service">Database Service Information</param>
        /// <param name="Delegate">Database Callback</param>
        /// <returns></returns>
        private void Execute(DataService[] Services, Action<System.Data.IDbCommand> Delegate)
        {
            Execute(Services, Delegate, _defaultConnectionTimeout);
        }


        /// <summary>
        /// Callback Method when a Exception has ocurred
        /// </summary>
        /// <param name="ex"></param>
        public virtual void OnException(System.Exception ex)
        {

        }


        #endregion

        #region IDataActions Members

        #region Execute Action
        /// <summary>
        /// Ejecuta una accion contra la DB sin devolver resultado alguno (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="Service">Configuracion Servicio de Datos basado en el Services.xml</param>
        /// <param name="CommandTimeout">Tiempo de espera para que la ejecucion retorne la respuesta</param>
        public void ExecuteAction(DataService Service, Int32 CommandTimeout)
        {
            Execute(Service, (DbConnection, DbCommand) =>
            {
                DbCommand.ExecuteNonQuery();
                return null;
            }, CommandTimeout);
        }

        /// <summary>
        /// Ejecuta una accion contra la DB sin devolver resultado alguno (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="Service">Configuracion Servicio de Datos basado en el Services.xml</param>
        public void ExecuteAction(DataService Service)
        {
            ExecuteAction(Service, _defaultConnectionTimeout);
        }
        #endregion

        #region Execute Query
        /// <summary>
        /// Ejecuta una accion contra la DB y devuelve el resultado de esta bajo un esquema de repositorio (configuracion a traves del Services.xml)
        /// </summary>
        /// <param name="Service">Servicio de Datos</param>
        /// <param name="CommandTimeout">Tiempo de espera para que la ejecucion retorne la respuesta</param>
        /// <returns>Repositorio de entidades</returns>
        public EntityRepository ExecuteQuery(DataService Service, Int32 CommandTimeout)
        {
            return (EntityRepository)Execute(Service, (DbConnection, DbCommand) =>
            {
                System.Data.IDbDataAdapter adapter = (System.Data.Common.DbDataAdapter)Activator.CreateInstance(typeof(TAdapter));

                System.Data.DataSet ds = new System.Data.DataSet();
                adapter.SelectCommand = DbCommand;
                adapter.Fill(ds);
                EntityRepository Repository = new EntityRepository(ds);

                return Repository;
            }, CommandTimeout);
        }

        /// <summary>
        /// Ejecuta una accion contra la DB y devuelve el resultado de esta bajo un esquema de repositorio (configuracion a traves del Services.xml)
        /// </summary>
        /// <param name="Service">Servicio de Datos</param>
        /// <returns>Repositorio de entidades</returns>
        public EntityRepository ExecuteQuery(DataService Service)
        {
            return ExecuteQuery(Service, _defaultConnectionTimeout);
        }
        #endregion

        #region Execute Scalar
        /// <summary>
        /// Ejecuta una accion contra la DB y devuelve el primer campo de respuesta que encuentre como resultado
        /// </summary>
        /// <param name="Service">Configuracion Servicio de Datos basado en el Services.xml</param>
        /// <param name="CommandTimeout">Tiempo de espera para que la ejecucion retorne la respuesta</param>
        public object ExecuteScalar(DataService Service, Int32 CommandTimeout)
        {
            return Execute(Service, (DbConnection, DbCommand) =>
            {
                return DbCommand.ExecuteScalar();
            }, CommandTimeout);
        }

        /// <summary>
        /// Ejecuta una accion contra la DB y devuelve el primer campo de respuesta que encuentre como resultado
        /// </summary>
        /// <param name="Service">Configuracion Servicio de Datos basado en el Services.xml</param>
        public object ExecuteScalar(DataService Service)
        {
            return ExecuteScalar(Service, _defaultConnectionTimeout);
        }
        #endregion

        #region Execute SQL
        public EntityRepository ExecuteSql(DataService Service)
        {
            return ExecuteSql(Service, _defaultConnectionTimeout);
        }

        public EntityRepository ExecuteSql(DataService Service, int CommandTimeout)
        {

            return (EntityRepository)Execute(Service, (DbConnection, DbCommand) =>
            {

                String sql = DbCommand.CommandText;

                Service.Parameters.ForEach((parameter) =>
                {
                    sql = sql.Replace("{" + parameter.Name + "}", parameter.Value.ToString());
                });
                DbCommand.CommandText = sql;


                DbCommand.CommandType = System.Data.CommandType.Text;

                System.Data.IDbDataAdapter adapter = (System.Data.Common.DbDataAdapter)Activator.CreateInstance(typeof(TAdapter));

                System.Data.DataSet ds = new System.Data.DataSet();
                adapter.SelectCommand = DbCommand;
                adapter.Fill(ds);
                EntityRepository Repository = new EntityRepository(ds);

                return Repository;
            }, CommandTimeout);
        }
        #endregion

        #endregion

        /// <summary>
        /// Parsing Step for each parameter in the service configuration (Useful for parsing c# values to DB Values)
        /// </summary>
        /// <param name="dbParameter">Database Referencial Parameter</param>
        /// <param name="serviceparameter">Service Configured Parameter</param>
        protected abstract void ParameterParse(ref System.Data.IDbDataParameter dbParameter, Gale.Db.DataParameter serviceparameter);

    }
}
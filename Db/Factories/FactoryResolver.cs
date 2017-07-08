using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gale.Db.Factories
{
    /// <summary>
    /// Database Factory Resolver
    /// </summary>
    public class FactoryResolver
    {
        /// <summary>
        /// Retrieves the Default Database Factory from the specified Connection String
        /// </summary>
        /// <returns></returns>
        public static Gale.Db.IDataActions Resolve()
        {
            var cnx = System.Configuration.ConfigurationManager.ConnectionStrings[Gale.REST.Resources.GALE_CONNECTION_DEFAULT_KEY];
            if (cnx == null)
            {
                throw new Gale.Exception.GaleException("DB002", Gale.REST.Resources.GALE_CONNECTION_DEFAULT_KEY);
            }

            return ResolveConnection(cnx);
        }

        /// <summary>
        /// Retrieves a Database Factory from the specified Connection String
        /// </summary>
        /// <param name="connectionString">Connection Key</param>
        /// <returns></returns>
        public static Gale.Db.IDataActions Resolve(String connectionKey)
        {
            var cnx = System.Configuration.ConfigurationManager.ConnectionStrings[connectionKey];
            if (cnx == null)
            {
                throw new Gale.Exception.GaleException("DB002", connectionKey);
            }

            return ResolveConnection(cnx);
        }

        /// <summary>
        /// Retrieves a Database Factory from the specified Connection String
        /// </summary>
        /// <param name="connectionString">Connection Key</param>
        /// <returns></returns>
        private static Gale.Db.IDataActions ResolveConnection(System.Configuration.ConnectionStringSettings connection)
        {
            try
            {
                Type factory_type = Type.GetType(connection.ProviderName);
                var factory = Activator.CreateInstance(factory_type, new object[] { connection.ConnectionString });
                return (Gale.Db.IDataActions)factory;
            }
            catch (System.Exception ex)
            {
                throw new Gale.Exception.GaleException("DB001", ex.Message);
            }
        }
    }
}

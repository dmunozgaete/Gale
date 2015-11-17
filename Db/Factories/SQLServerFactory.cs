using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.Db.Factories
{
    /// <summary>
    /// Defines a SQL Server Factory
    /// </summary>
    public class SQLServerFactory : Gale.Db.Factories.AbstractFactory<System.Data.SqlClient.SqlConnection, System.Data.SqlClient.SqlDataAdapter>
    {

        /// <summary>
        /// Base Constructor
        /// </summary>
        /// <param name="ConnectionString">Connection String</param>
        public SQLServerFactory(string ConnectionString)
            : base(ConnectionString)
        {

        }

        /// <summary>
        /// Callback Method when a Exception has ocurred
        /// </summary>
        /// <param name="ex"></param>
        public override void OnException(System.Exception ex)
        {
            if (ex is System.Data.SqlClient.SqlException)
            {
                //CUSTOM DB HANDLER ERROR && (ERROR NUMBER MUST BE > 50000)
                //https://msdn.microsoft.com/en-us/library/ms178592.aspx
                var DB_EX = (ex as System.Data.SqlClient.SqlException);
                if (DB_EX.Number > 50000)
                {
                    throw new Gale.Exception.SqlClient.CustomDatabaseException(DB_EX.Number.ToString(), DB_EX.Message);
                }
            }

        }

        /// <summary>
        /// Parsing Step for each parameter in the service configuration (Useful for parsing c# values to DB Values)
        /// </summary>
        /// <param name="dbParameter">Database Referencial Parameter</param>
        /// <param name="serviceparameter">Service Configured Parameter</param>
        protected override void ParameterParse(ref System.Data.IDbDataParameter dbParameter, DataParameter serviceparameter)
        {
            Type valueType = serviceparameter.Value.GetType();

            if (valueType == typeof(byte[]))
            {
                //Binary
                dbParameter.DbType = System.Data.DbType.Binary;
            }
            else if (valueType == typeof(DateTime))
            {
                /*
                //Date Time
                DateTime value = (DateTime)serviceparameter.Value;

                dbParameter.Value = (value).ToString("yyyy-MM-ddTHH:mm:ss.0");
                dbParameter.DbType = System.Data.DbType.String;
                */
                
                DateTime value = (DateTime)serviceparameter.Value;

                //http://www.hanselman.com/blog/OnTheNightmareThatIsJSONDatesPlusJSONNETAndASPNETWebAPI.aspx
                //http://thewebjedi.com/be/post/2014/02/22/dates-and-time-zones-in-javascript-c-and-sql-server.aspx

                //SET TO UTC Universal TIME, to Work with local times, and fix the Web API Nightmare of Date
                dbParameter.Value = value;
                dbParameter.DbType = System.Data.DbType.DateTime;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.Db.Factories
{
    public class SQLServerFactory : Gale.Db.Factories.AbstractFactory<System.Data.SqlClient.SqlConnection, System.Data.SqlClient.SqlDataAdapter>
    {
        public SQLServerFactory(string ConnectionString)
            : base(ConnectionString)
        {

        }

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
    }
}

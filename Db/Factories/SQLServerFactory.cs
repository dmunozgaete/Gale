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
    }
}

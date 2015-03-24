using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karma.Db
{
    public interface IDataActions
    {
        Type DbConnection();

        void ExecuteAction(Karma.Db.DataService Service);
        EntityRepository ExecuteQuery(Karma.Db.DataService Service);
        object ExecuteScalar(Karma.Db.DataService Service);

        void ExecuteAction(Karma.Db.DataService Service, Int32 CommandTimeout);
        EntityRepository ExecuteQuery(Karma.Db.DataService Service, Int32 CommandTimeout) ;
        object ExecuteScalar(Karma.Db.DataService Service, Int32 CommandTimeout);

        EntityRepository ExecuteSql(Karma.Db.DataService Service);
        EntityRepository ExecuteSql(Karma.Db.DataService Service, Int32 CommandTimeout);
    }
}

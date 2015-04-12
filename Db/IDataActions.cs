using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karma.Db
{
    public interface IDataActions
    {
        Type DbConnection();

        /// <summary>
        /// Execute and Action Witouth Return Result
        /// </summary>
        /// <param name="Service"></param>
        void ExecuteAction(Karma.Db.DataService Service);

        /// <summary>
        /// Execute a Query and return the results
        /// </summary>
        /// <param name="Service"></param>
        /// <returns></returns>
        EntityRepository ExecuteQuery(Karma.Db.DataService Service);

        /// <summary>
        /// Execute and Action Or Query and Return the First Column of the First Row on the first result
        /// </summary>
        /// <param name="Service"></param>
        /// <returns></returns>
        object ExecuteScalar(Karma.Db.DataService Service);

        /// <summary>
        /// Execute and Action Witouth Return Result
        /// </summary>
        /// <param name="Service"></param>
        /// <param name="CommandTimeout">Time to wait after a timeout throw Exception release</param>
        void ExecuteAction(Karma.Db.DataService Service, Int32 CommandTimeout);

        /// <summary>
        /// Execute a Query and return the results
        /// </summary>
        /// <param name="Service"></param>
        /// <param name="CommandTimeout">Time to wait after a timeout throw Exception release</param>
        /// <returns></returns>
        EntityRepository ExecuteQuery(Karma.Db.DataService Service, Int32 CommandTimeout) ;

        /// <summary>
        /// Execute and Action Or Query and Return the First Column of the First Row on the first result
        /// </summary>
        /// <param name="Service"></param>
        /// <param name="CommandTimeout">Time to wait after a timeout throw Exception release</param>
        /// <returns></returns>
        object ExecuteScalar(Karma.Db.DataService Service, Int32 CommandTimeout);

        EntityRepository ExecuteSql(Karma.Db.DataService Service);
        EntityRepository ExecuteSql(Karma.Db.DataService Service, Int32 CommandTimeout);
    }
}

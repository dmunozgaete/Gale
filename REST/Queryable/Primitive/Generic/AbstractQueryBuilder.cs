using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gale.REST.Queryable.Primitive.Reflected;

namespace Gale.REST.Queryable.Primitive.Generic
{
    public abstract class AbstractQueryBuilder<TModel> : Gale.REST.Queryable.Primitive.AbstractQueryBuilder
        where TModel : class
    {

        public AbstractQueryBuilder(Gale.Db.IDataActions databaseFactory)
            : base(databaseFactory, typeof(TModel))
        {

        }
    }
}

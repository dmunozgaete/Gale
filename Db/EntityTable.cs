using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karma.Db
{
    public sealed class EntityTable<T> : List<T>, IEntityTable, IEntityTable<T>
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karma.REST.Queryable.OData.SQLServer.Operators
{
    internal abstract class Operator : Karma.REST.Queryable.Primitive.Operator
    {
        public abstract string Parse(Karma.REST.Queryable.Primitive.Reflected.Field field, string value);

    }
}

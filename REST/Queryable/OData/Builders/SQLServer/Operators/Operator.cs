using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.REST.Queryable.OData.Builders.SQLServer.Operators
{
    internal abstract class Operator : Gale.REST.Queryable.Primitive.Operator
    {
        public abstract string Parse(Gale.REST.Queryable.Primitive.Reflected.Field field, string value);

    }
}

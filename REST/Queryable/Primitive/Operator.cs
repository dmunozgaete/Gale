using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gale.REST.Queryable.Primitive.Reflected;

namespace Gale.REST.Queryable.Primitive
{
    public interface Operator
    {
        String Parse(Field field, String value);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Karma.REST.Queryable.Primitive.Reflected;

namespace Karma.REST.Queryable.Primitive
{
    public interface Operator
    {
        String Parse(Field field, String value);
    }
}

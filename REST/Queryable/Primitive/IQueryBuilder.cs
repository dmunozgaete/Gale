using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Karma.REST.Queryable.Primitive.Reflected;

namespace Karma.REST.Queryable.Primitive
{
    public interface IQueryBuilder
    {
        Karma.REST.Queryable.Primitive.IResponse Execute(int offet, int limit);

        Model ReflectedModel();

        List<TypedItem> RegisteredParsers();
        SortedList<String,Type> RegisteredOperators();
    }
}

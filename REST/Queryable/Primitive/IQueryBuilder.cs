using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gale.REST.Queryable.Primitive.Reflected;

namespace Gale.REST.Queryable.Primitive
{
    public interface IQueryBuilder
    {
        Gale.REST.Queryable.Primitive.IResponse Execute(int offet, int limit);

        Model ReflectedModel();

        List<TypedItem> RegisteredParsers();
        SortedList<String,Type> RegisteredOperators();
    }
}

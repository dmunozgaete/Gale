using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karma.REST.Queryable.Primitive
{
    public class TypedItem
    {
        private Type _type;
        private String _queryFragment;

        public TypedItem(Type type, String queryFragment)
        {
            this._type = type;
            this._queryFragment = queryFragment;
        }
        public Type Type
        {
            get
            {
                return _type;
            }
        }

        public String QueryFragment
        {
            get
            {
                return _queryFragment;
            }
        }
    }
}

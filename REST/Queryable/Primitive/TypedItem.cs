using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gale.REST.Queryable.OData.Builders;

namespace Gale.REST.Queryable.Primitive
{
    public class TypedItem
    {
        private Type _type;
        private GQLConfiguration _configuration;

        public TypedItem(Type type, GQLConfiguration configuration)
        {
            this._type = type;
            this._configuration = configuration;
        }
        public Type Type
        {
            get
            {
                return _type;
            }
        }

        public GQLConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
        }
    }
}

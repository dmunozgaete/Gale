using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karma.REST.Queryable.OData.SQLServer.Operators
{
    [Karma.REST.Queryable.Primitive.Operator(Alias="eq")]
    internal class Equals : OData.SQLServer.Operators.Operator
    {
        public override string Parse(Karma.REST.Queryable.Primitive.Reflected.Field field, string value)
        {
            String format = "{0} = {1}";
            if (field.Type == typeof(String) || field.Type == typeof(System.Guid) || field.Type == typeof(System.DateTime))
            {
                format = "{0} = '{1}'";
            }
            return String.Format(format, field.Key, value);
        }
    }
}

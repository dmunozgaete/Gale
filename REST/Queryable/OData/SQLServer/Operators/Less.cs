using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karma.REST.Queryable.OData.SQLServer.Operators
{
    [Karma.REST.Queryable.Primitive.Operator(Alias = "<")]
    internal class Less : OData.SQLServer.Operators.Operator
    {
        public override string Parse(Karma.REST.Queryable.Primitive.Reflected.Field field, string value)
        {
            String format = "{0} < {1}";
            if (field.Type== typeof(String))
            {
                format = "LEN({0}) < {1}";
            }

            if (field.Type == typeof(DateTime) || field.Type == typeof(DateTime?))
            {
                format = "{0} < '{1}'";
            }
            return String.Format(format, field.Key, value);
        }
    }
}

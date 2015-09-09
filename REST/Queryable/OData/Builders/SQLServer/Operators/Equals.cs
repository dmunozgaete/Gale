using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.REST.Queryable.OData.Builders.SQLServer.Operators
{
    [Gale.REST.Queryable.Primitive.Operator(Alias="eq")]
    internal class Equals : OData.Builders.SQLServer.Operators.Operator
    {
        public override string Parse(Gale.REST.Queryable.Primitive.Reflected.Field field, string value)
        {
            String format = "{0} = {1}";
            
            if (
                field.Type == typeof(String) || 
                field.Type == typeof(System.Guid)  ||
                field.Type == typeof(System.Guid?) || 
                field.Type == typeof(System.DateTime) || 
                field.Type == typeof(System.DateTime?)
                )
            {
                format = "{0} = '{1}'";
            }
            return String.Format(format, field.Key, value);
        }
    }
}

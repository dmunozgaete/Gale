using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karma.REST.Queryable.OData.SQLServer.Operators
{
    [Karma.REST.Queryable.Primitive.Operator(Alias = "contains")]
    internal class Contains : OData.SQLServer.Operators.Operator
    {
        public override string Parse(Karma.REST.Queryable.Primitive.Reflected.Field field, string value)
        {
            if (field.Type == typeof(String) || field.Type == typeof(System.Guid))
            {
                return String.Format("{0} like '%{1}%'", field.Key, value);
            }
            else
            {
                throw new Exception.KarmaException("API018");
            }
            
        }
    }
}

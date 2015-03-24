using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karma.REST.Queryable.OData.SQLServer.Operators
{
    [Karma.REST.Queryable.Primitive.Operator(Alias = "in")]
    internal class In : OData.SQLServer.Operators.Operator
    {
        public override string Parse(Karma.REST.Queryable.Primitive.Reflected.Field field, string value)
        {
            if (field.Type == typeof(String) || field.Type == typeof(Int32) || field.Type == typeof(System.Guid))
            {
                List<String> values = value.Split('|').ToList();
                String in_values = string.Join("','", values);

                return String.Format("{0} IN ('{1}')", field.Key, in_values);
            }
            else
            {
                throw new Exception.KarmaException("API018");
            }
            
        }
    }
}

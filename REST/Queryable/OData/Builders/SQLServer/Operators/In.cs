using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.REST.Queryable.OData.Builders.SQLServer.Operators
{
    [Gale.REST.Queryable.Primitive.Operator(Alias = "in")]
    internal class In : OData.Builders.SQLServer.Operators.Operator
    {
        public override string Parse(Gale.REST.Queryable.Primitive.Reflected.Field field, string value)
        {
            if (field.Type == typeof(String) || field.Type == typeof(Int32) || field.Type == typeof(System.Guid))
            {
                List<String> values = value.Split('|').ToList();
                String in_values = string.Join("','", values);

                return String.Format("{0} IN ('{1}')", field.Key, in_values);
            }
            else
            {
                throw new Exception.GaleException("API018");
            }
            
        }
    }
}

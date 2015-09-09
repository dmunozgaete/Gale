using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.REST.Queryable.OData.Builders.SQLServer.Operators
{
    [Gale.REST.Queryable.Primitive.Operator(Alias = "contains")]
    internal class Contains : OData.Builders.SQLServer.Operators.Operator
    {
        public override string Parse(Gale.REST.Queryable.Primitive.Reflected.Field field, string value)
        {

            if (field.Type == typeof(String) || field.Type == typeof(System.Guid))
            {
                return String.Format("{0} like '%{1}%'", field.Key, value);
            }
            else if (field.Type == typeof(Int32))
            {
                //convert(varchar(10),StandardCost)
                return String.Format("CONVERT(VARCHAR(8000), {0}) like '%{1}%'", field.Key, value);
            }
            else
            {
                throw new Exception.GaleException("API018");
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.REST.Queryable.OData.Builders.SQLServer.Parsers
{
    internal class OrderBy : Gale.REST.Queryable.Primitive.Parser
    {
        public override string Parse(string query, Gale.REST.Queryable.Primitive.Reflected.Model model)
        {
            if (String.IsNullOrWhiteSpace(query) || String.IsNullOrEmpty(query))
            {
                return "SYSDATETIME() desc \n\n";
            }
            else
            {
                String[] values = query.Split(' ');
                String fieldName = values[0].ToLower();
                String order = values[1].Trim().ToLower();

                Gale.REST.Queryable.Primitive.Reflected.Field _field = model.Fields.FirstOrDefault((field) =>
                {
                    return field.Name == fieldName;
                });

                if (_field == null)
                {
                    throw new Exception.GaleException("API015", fieldName);
                }

                return String.Format("{0} {1} \n\n", _field.Key, values[1]);

            }
        }
    }
}

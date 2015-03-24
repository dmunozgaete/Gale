using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karma.REST.Queryable.OData.SQLServer.Parsers
{
    internal class OrderBy : Karma.REST.Queryable.Primitive.Parser
    {
        public override string Parse(string query, Karma.REST.Queryable.Primitive.Reflected.Model model)
        {
            if (String.IsNullOrWhiteSpace(query) || String.IsNullOrEmpty(query))
            {
                return "SYSDATETIME() desc";
            }
            else
            {
                String[] values = query.Split(' ');
                if (values.Length == 2)
                {
                    String fieldName = values[0].ToLower();
                    String order = values[1].Trim().ToLower();

                    Karma.REST.Queryable.Primitive.Reflected.Field _field = model.Fields.FirstOrDefault((field) =>
                    {
                        return field.Name == fieldName;
                    });

                    if (_field == null)
                    {
                        throw new Exception.KarmaException("API015", fieldName);
                    }

                    if (order != "asc" && order != "desc")
                    {
                        throw new Exception.KarmaException("API016");
                    }
                    return String.Format("{0} {1}", _field.Key, values[1]);
                }
                else
                {
                    throw new Exception.KarmaException("API017");
                }
            }
        }
    }
}

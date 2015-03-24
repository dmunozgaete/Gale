using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karma.REST.Queryable.OData.SQLServer.Parsers
{
    internal class Where : Karma.REST.Queryable.Primitive.Parser
    {
        public override string Parse(string query, Karma.REST.Queryable.Primitive.Reflected.Model model)
        {
            if (String.IsNullOrWhiteSpace(query) || String.IsNullOrEmpty(query))
            {
                return "";
            }

            String[] filters = query.Trim().Split(',');
            if (filters.Length > 0)
            {
                //WHERE PARSER QUERY
                List<String> builder = new List<string>();
                filters.ToList().ForEach((filter) =>
                {
                    //FK Constraint's Filter [ format: fk:(fk_column operator values) ]
                    if (filter.IndexOf(":(") > 0)
                    {
                        var foreignFieldMatch = String.Format("{0})", filter.Substring(0, filter.IndexOf(" ")));
                        var filteredField = (from field in model.Fields where field.Name == foreignFieldMatch select field).FirstOrDefault();
                        if (filteredField == null)
                        {
                            throw new Exception.KarmaException("API009", foreignFieldMatch);
                        }

                        //replace the first space with ")", so the format be "foreign:(foreignField) operator value"
                        String innerFilter = filter.Insert(filter.IndexOf(" "), ")").Substring(0, filter.Length);

                        String[] values = innerFilter.Trim().Split(' ');
                        if (values.Length != 3)
                        {
                            throw new Exception.KarmaException("API010", filter);
                        }


                        string _filter = String.Concat(
                            filteredField.Table.Key,
                            ".",
                            CallOperator(innerFilter, model)
                        );
                        builder.Add(_filter);
                    }
                    else
                    {
                        //Normal operator [ format: column operator values }
                        builder.Add(CallOperator(filter));
                    }
                });

                return String.Join(" AND ", builder);
            }
            else
            {
                return "";
            }
        }
    }
}

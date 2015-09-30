using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.REST.Queryable.OData.Builders.SQLServer.Parsers
{
    internal class Where : Gale.REST.Queryable.Primitive.Parser
    {
        public override string Parse(GQLConfiguration configuration, Gale.REST.Queryable.Primitive.Reflected.Model model)
        {
            if (configuration.filters.Count > 0)
            {
                //WHERE PARSER QUERY
                List<String> builder = new List<string>();
                configuration.filters.ForEach((filter) =>
                {

                    //FK Constraint's Filter [ format: fk:(fk_column operator values) ]
                    if (filter.field.IndexOf(":(") > 0)
                    {
                        var foreignFieldMatch = String.Format("{0})", filter.field.Substring(0, filter.field.IndexOf(" ")));
                        var filteredField = (from field in model.Fields where field.Name == foreignFieldMatch select field).FirstOrDefault();
                        if (filteredField == null)
                        {
                            throw new Exception.GaleException("API009", foreignFieldMatch);
                        }

                        //replace the first space with ")", so the format be "foreign:(foreignField) operator value"
                        String innerFilter = filter.field.Insert(filter.field.IndexOf(" "), ")").Substring(0, filter.field.Length);

                        String[] values = innerFilter.Trim().Split(' ');
                        if (values.Length != 3)
                        {
                            throw new Exception.GaleException("API010", filter.ToString());
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
                        builder.Add(CallOperator(filter.ToString()));
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

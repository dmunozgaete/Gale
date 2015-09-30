using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.REST.Queryable.OData.Builders.SQLServer.Parsers
{
    /// <summary>
    /// From Parser 
    /// </summary>
    internal class From : Gale.REST.Queryable.Primitive.Parser
    {

        public override string Parse(GQLConfiguration configuration, Gale.REST.Queryable.Primitive.Reflected.Model model)
        {
            //FROM PARSER QUERY
            System.Text.StringBuilder builder = new StringBuilder();

            List<String> SelectAppends = new List<string>();

            //Add Primary Table (First Position)
            SelectAppends.Add(String.Format(" FROM {0} T1 \n\n", model.Tables[0].Key));

            model.Constraints.ForEach((constraint) =>
            {
                SelectAppends.Insert(0, String.Concat(", ", constraint.Table.Key, ".* "));

                //LEFT JOIN TABLE
                builder.Append(String.Format(" LEFT JOIN {0} ON T1.{1} = {0}.{2} \n\n",
                        constraint.Table.Key,
                        constraint.ThisField.Key,
                        constraint.OtherField.Key
                    ));

            });


            return String.Concat(String.Join("", SelectAppends), builder.ToString());
        }
    }
}

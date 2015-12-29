using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.REST.Queryable.OData.Builders.SQLServer.Parsers
{
    /// <summary>
    /// Order By Parser
    /// </summary>
    internal class OrderBy : Gale.REST.Queryable.Primitive.Parser
    {
        /// <summary>
        /// Parse SQL String
        /// </summary>
        /// <param name="query"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public override string Parse(GQLConfiguration configuration, Gale.REST.Queryable.Primitive.Reflected.Model model)
        {
            if(configuration.orderBy == null)
            {
                return "SYSDATETIME() desc \n\n";
            }
            else
            {

                Gale.REST.Queryable.Primitive.Reflected.Field _field = model.Fields.FirstOrDefault((field) =>
                {
                    return field.Name.ToLower() == configuration.orderBy.name.ToLower();
                });

                if (_field == null)
                {
                    throw new Exception.GaleException("API015", configuration.orderBy.name);
                }

                return String.Format("{0} {1} \n\n", _field.Key, configuration.orderBy.order.ToString());

            }
        }
    }
}

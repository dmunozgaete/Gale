using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Gale.REST.Config.SwashBuckleExtension.Filters
{
    /// <summary>
    /// Add all header parameters defined with "HeaderParameter"
    /// </summary>
    internal class AddHeaderParameters : IOperationFilter
    {
        /// <summary>
        /// Apply Operation
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiDescription"></param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {

            if (
                apiDescription.ActionDescriptor.GetCustomAttributes<Swashbuckle.Swagger.Annotations.HeaderParameter>().Any()
                )
            {
                var attr = (Swashbuckle.Swagger.Annotations.HeaderParameter)apiDescription.ActionDescriptor.GetCustomAttributes<Swashbuckle.Swagger.Annotations.HeaderParameter>().First();

                if (operation.parameters == null)
                {
                    operation.parameters = new List<Parameter>();
                }

                operation.parameters.Add(new Parameter()
                {
                    name = attr.Name,
                    @in = "header",
                    type = "string",
                    minimum = attr.Required ? 1 : 0,
                    required = attr.Required
                });

            }
        }
    }
}

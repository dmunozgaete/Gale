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
    /// Add all form parameters defined with "FormDataParameter"
    /// </summary>
    internal class AddFormDataParameters : IOperationFilter
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
                apiDescription.ActionDescriptor.GetCustomAttributes<Swashbuckle.Swagger.Annotations.FormDataParameter>().Any()
                )
            {
                var attrs = apiDescription.ActionDescriptor.GetCustomAttributes<Swashbuckle.Swagger.Annotations.FormDataParameter>();

                foreach(Swashbuckle.Swagger.Annotations.FormDataParameter attribute in attrs)
                {

                    if (operation.parameters == null)
                    {
                        operation.parameters = new List<Parameter>();
                    }

                    operation.parameters.Add(new Parameter()
                    {
                        name = attribute.Name,
                        @in = "formData",
                        type = "string",
                        minimum = attribute.Required ? 1 : 0,
                        required = attribute.Required,
                        @default = attribute.DefaultValue
                    });


                    
                }


            }
        }
    }
}

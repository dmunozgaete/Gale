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
    /// Swagger Extension: Add header parameters for all parameten in action wich have [FromHeader] Attribute
    /// </summary>
    internal class AddFromHeaderParameters : IOperationFilter
    {
        /// <summary>
        /// Apply Operation
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiDescription"></param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var fromHeaderParams = apiDescription.ActionDescriptor.GetParameters()
                .Where(param => param.GetCustomAttributes<FromHeaderAttribute>().Any())
                .ToArray();

            //Exist Any??
            foreach (var param in fromHeaderParams)
            {
                var attr = param.GetCustomAttributes<FromHeaderAttribute>().First();
                var operationParam = operation.parameters.First(p => p.name == param.ParameterName);

                operationParam.name = attr.Name == null ? param.ParameterName : attr.Name;
                operationParam.description = "*" + param.ParameterName;
                operationParam.vendorExtensions.Add("action-name",param.ParameterName);
                operationParam.@in = "header";
                operationParam.type = "string";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Gale.REST.Config.SwashBuckleExtension.Filters
{
    /// <summary>
    /// Find the authorization Attribute and add the API Key
    /// </summary>
    internal class RemoveSwaggerDefaults : Swashbuckle.Swagger.IOperationFilter
    {


        /// <summary>
        /// Remove Swagger Defaults 
        /// </summary>
        /// <param name="operation">Operation</param>
        /// <param name="schemaRegistry">Schema</param>
        /// <param name="apiDescription">Description for the api</param>
        public void Apply(Swashbuckle.Swagger.Operation operation, Swashbuckle.Swagger.SchemaRegistry schemaRegistry, System.Web.Http.Description.ApiDescription apiDescription)
        {
            //not Work
            operation.responses.Clear();
        }
    }
}
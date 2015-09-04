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
    internal class AddAuthorizateSecurityDefinitions : Swashbuckle.Swagger.IOperationFilter
    {

        /// <summary>
        /// Security Schema Definition name, For Swagger UI
        /// </summary>
        internal const string _securityDefinitionNameSchema = "jwt";


        /// <summary>
        /// Filter and add Security JWT definition to all accion with has "AuthorizaAttribute"
        /// </summary>
        /// <param name="operation">Operation</param>
        /// <param name="schemaRegistry">Schema</param>
        /// <param name="apiDescription">Description for the api</param>
        public void Apply(Swashbuckle.Swagger.Operation operation, Swashbuckle.Swagger.SchemaRegistry schemaRegistry, System.Web.Http.Description.ApiDescription apiDescription)
        {
           
            if (
                apiDescription.ActionDescriptor.GetFilters().OfType<AuthorizeAttribute>().Any() ||
                apiDescription.ActionDescriptor.ControllerDescriptor.GetFilters().OfType<AuthorizeAttribute>().Any()
                )
            {
                if (operation.security == null)
                    operation.security = new List<IDictionary<string, IEnumerable<string>>>();

                var oAuthRequirements = new Dictionary<string, IEnumerable<string>>();
                oAuthRequirements.Add(_securityDefinitionNameSchema, new List<string>());

                operation.security.Add(oAuthRequirements);

            }
        }
    }
}
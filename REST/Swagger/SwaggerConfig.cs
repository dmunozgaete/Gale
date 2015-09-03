using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using Gale.REST.Swagger.SwashBuckleExtension;
using Swashbuckle.Application;

namespace Gale.REST.Swagger
{

    /// <summary>
    /// WEB API Global Configuration
    /// </summary>
    public static class SwaggerConfig
    {
        /// <summary>
        /// Security Schema Definition name, For Swagger UI
        /// </summary>
        internal const string _securityDefinitionNameSchema = "jwt";

        /// <summary>
        /// Register Config Variables
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            //--------------------------------------------------------------------------------------------------------------------------------------------
            //SWAGGER PROTOCOL AUTO-GENERATED DOC's
            //https://github.com/domaindrivendev/Swashbuckle

            string XMLComment = System.String.Format(
                                    @"{0}{1}bin{1}API.XML",
                                    System.AppDomain.CurrentDomain.BaseDirectory,
                                    System.IO.Path.DirectorySeparatorChar	/* Cross Platform */
                                );

            config.EnableSwagger((c) =>
            {
                //Ignote Obsolte Operations
                c.IgnoreObsoleteActions();

                //Action Conflict Resolver
                c.ResolveConflictingActions((apiDescriptions) =>
                {
                    return apiDescriptions.First();
                });

                //JWT Configuration
                c.ApiKey(_securityDefinitionNameSchema)
                    .Description("Bearer JWT Authorization")
                    .Name("Authorization")
                    .In("header");

                //Add JWT Api Key Scheme
                c.OperationFilter<AddAuthorizateSecurityDefinitions>();

                //Include XML
                c.IncludeXmlComments(XMLComment);


                //Basic Configuration
                c.SingleApiVersion(Gale.REST.Http.Routing.GaleApiControllerActionSelector.apiVersion, "API Explorer");

            })
            .EnableSwaggerUi((c) =>
            {
                c.DisableValidator();

                //Inject to correct authentication Bearer JWT Token into every Request
                c.InjectJavaScript(typeof(Gale.REST.Swagger.SwaggerConfig).Assembly, "Gale.REST.Swagger.SwashBuckleExtension.Swagger.js");
                c.InjectStylesheet(typeof(Gale.REST.Swagger.SwaggerConfig).Assembly, "Gale.REST.Swagger.SwashBuckleExtension.Swagger.css");
            });
            //--------------------------------------------------------------------------------------------------------------------------------------------
        }
    }
}

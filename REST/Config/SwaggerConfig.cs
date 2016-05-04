using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using Gale.REST.Config.SwashBuckleExtension;
using Swashbuckle.Application;

namespace Gale.REST.Config
{

    /// <summary>
    /// Swagger Global Configuration
    /// </summary>
    public static class SwaggerConfig
    {
        /// <summary>
        /// Store the swagger enabled
        /// </summary>
        private static bool _isSwaggerEnabled = false;

        /// <summary>
        /// Retrieves if Swagger was enabled in the API
        /// </summary>
        public static bool IsSwaggerEnabled
        {
            get
            {
                return _isSwaggerEnabled;
            }
        }

        /// <summary>
        /// Store the documentation file
        /// </summary>
        private static String _documentationFile = null;

        /// <summary>
        /// Retrieves the documentation file for getting the explorer
        /// </summary>
        public static String DocumentationFile
        {
            get
            {
                if (_documentationFile == null)
                {
                    /* Cross Platform */
                    _documentationFile = String.Format("API{0}xml",System.IO.Path.DirectorySeparatorChar);	
                }
                return _documentationFile;
            }
        }



        /// <summary>
        /// Security Schema Definition name, For Swagger UI
        /// </summary>
        internal const string _securityDefinitionNameSchema = "jwt";

        /// <summary>
        /// Register Config Variables
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration configuration, String documentationFilePath)
        {
            _isSwaggerEnabled = true;

            //Override the API Documentation File??
            if (documentationFilePath != null)
            {
                _documentationFile = documentationFilePath;
            }

            //--------------------------------------------------------------------------------------------------------------------------------------------
            //SWAGGER PROTOCOL AUTO-GENERATED DOC's
            //https://github.com/domaindrivendev/Swashbuckle

            string XMLComment = System.String.Format(
                                    @"{0}{1}",
                                    System.AppDomain.CurrentDomain.BaseDirectory,
                                    Gale.REST.Config.SwaggerConfig.DocumentationFile
                                );

            configuration.EnableSwagger((c) =>
            {
                //Ignote Obsolete Operations (Obsolete Attribute)
                c.IgnoreObsoleteActions();

                //Ignote Obsolete Properties (Obsolete Attribute)
                c.IgnoreObsoleteProperties();

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


                //Add ODATA Parameters to Queryable Endpoints =)
                c.OperationFilter<Gale.REST.Config.SwashBuckleExtension.Filters.AddBlueprintDocumentation>();

                //Add JWT Api Key Scheme
                c.OperationFilter<Gale.REST.Config.SwashBuckleExtension.Filters.AddAuthorizateSecurityDefinitions>();

                //Add implicit header parameter's       
                c.OperationFilter<Gale.REST.Config.SwashBuckleExtension.Filters.AddHeaderParameters>();

                //Add from header parameter's
                c.OperationFilter<Gale.REST.Config.SwashBuckleExtension.Filters.AddFromHeaderParameters>();

                //Remove Swagger Default's
                //c.OperationFilter<Gale.REST.Config.SwashBuckleExtension.Filters.RemoveSwaggerDefaults>();


                //Include XML
                c.IncludeXmlComments(XMLComment);

                //Basic Configuration
                c.SingleApiVersion("latest", "API Explorer");

            })
            .EnableSwaggerUi((c) =>
            {
                c.DisableValidator();

                //Inject to correct authentication Bearer JWT Token into every Request
                c.InjectJavaScript(typeof(Gale.REST.Config.SwaggerConfig).Assembly, "Gale.REST.Config.SwashBuckleExtension.Swagger.js");
                c.InjectStylesheet(typeof(Gale.REST.Config.SwaggerConfig).Assembly, "Gale.REST.Config.SwashBuckleExtension.Swagger.css");
            });
            //--------------------------------------------------------------------------------------------------------------------------------------------
        }
    }
}

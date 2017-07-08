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
                    _documentationFile = String.Format("API{0}xml", System.IO.Path.DirectorySeparatorChar);
                }
                return _documentationFile;
            }
        }



        /// <summary>
        /// Security Schema Definition name, For Swagger UI
        /// </summary>
        internal const string _securityDefinitionNameSchema = "jwt";

        private static string ResolveBasePath(HttpRequestMessage message)
        {
            var virtualPathRoot = message.GetRequestContext().VirtualPathRoot;

            var schemeAndHost = message.RequestUri.Scheme + "://" + message.RequestUri.Host + ":" + message.RequestUri.Port;
            return new Uri(new Uri(schemeAndHost, UriKind.Absolute), virtualPathRoot).AbsoluteUri;
        }

        /// <summary>
        /// Register Config Variables
        /// </summary>
        /// <param name="configuration">Configuration WebConfig</param>
        /// <param name="documentationFilePath">File to Documentation Path (documentation xml)</param>
        /// <param name="explorerTitle">Overrides the default Explorer Title</param>
        /// <param name="rootUrl">Root Url to override to find Swagger JSON File</param>
        public static void Register(
            HttpConfiguration configuration,
            String documentationFilePath,
            String explorerTitle,
            String rootUrl)
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

                //Add Strategy for Duplicadte IDS
                c.SchemaId((type) =>
                {
                    return type.FullName;
                });

                //Useful when the Swagger JSON is in another place
                if (String.IsNullOrEmpty(rootUrl))
                {
                    c.RootUrl(ResolveBasePath); //DEFAULT Resolver
                }
                else
                {
                    //Override the URL to another Place
                    c.RootUrl((msg) =>
                    {
                        return rootUrl;
                    });
                }


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

                //Add from header parameter's
                c.OperationFilter<Gale.REST.Config.SwashBuckleExtension.Filters.AddFormDataParameters>();

                //Remove Swagger Default's
                //c.OperationFilter<Gale.REST.Config.SwashBuckleExtension.Filters.RemoveSwaggerDefaults>();




                //Include XML
                c.IncludeXmlComments(XMLComment);

                if (String.IsNullOrEmpty(explorerTitle))
                {
                    explorerTitle = "API Explorer";
                }

                //Basic Configuration
                c.SingleApiVersion("latest", explorerTitle);

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

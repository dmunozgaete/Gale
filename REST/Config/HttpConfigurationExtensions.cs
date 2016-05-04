using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace System.Web.Http
{
    public static class HttpConfigurationExtensions
    {
        /// <summary>
        /// Enable Swagger "Live" Documentation in URL : {API}/swagger
        /// </summary>
        /// <param name="configuration"></param>
        public static void EnableSwagger(this HttpConfiguration configuration)
        {
            EnableSwagger(configuration, null);
        }

        /// <summary>
        /// Enable Swagger "Live" Documentation in URL : {API}/swagger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="documentationFilePath">override the default path for the XML Documentation File</param>
        public static void EnableSwagger(this HttpConfiguration configuration, String documentationFilePath)
        {
            
            Gale.REST.Config.SwaggerConfig.Register(configuration, documentationFilePath);
        }

        /// <summary>
        /// Enable Gale Routing for standard RESTful Protocols
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="apiVersion">Api Version to enable</param>
        public static void EnableGaleRoutes(this HttpConfiguration configuration, string apiVersion)
        {
            //Map Support for Route Attributes!!
            configuration.MapHttpAttributeRoutes();

            Gale.REST.Config.GaleConfig.Register(configuration, apiVersion);
        }

        /// <summary>
        /// Enable Gale Routing for standard RESTful Protocols (v1: default version)
        /// </summary>
        /// <param name="configuration"></param>
        public static void EnableGaleRoutes(this HttpConfiguration configuration)
        {
            EnableGaleRoutes(configuration, null);
        }

        /// <summary>
        /// change the default formatter from XML to JSON (Google Chrome Fix)
        /// </summary>
        /// <param name="configuration"></param>
        public static void SetJsonDefaultFormatter(this HttpConfiguration configuration)
        {
            configuration.Formatters.Add(new Gale.REST.Http.Formatter.JsonFormatter());
        }

    }

}
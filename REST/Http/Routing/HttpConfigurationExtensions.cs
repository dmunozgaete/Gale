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
            Gale.REST.Swagger.SwaggerConfig.Register(configuration);
        }

        /// <summary>
        /// Enable Gale Routing for standard RESTful Protocols
        /// </summary>
        /// <param name="configuration"></param>
        public static void MapGaleRoutes(this HttpConfiguration configuration)
        {
            MapGaleRoutes(configuration, null);
        }


        /// <summary>
        /// Enable Gale Routing for standard RESTful Protocols
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="apiVersion">Api Version to enable</param>
        public static void MapGaleRoutes(this HttpConfiguration configuration, string apiVersion)
        {
            configuration.Services.Replace(
                typeof(System.Web.Http.Controllers.IHttpActionSelector),
                new Gale.REST.Http.Routing.GaleApiControllerActionSelector(
                configuration,
                apiVersion
            ));



            var Route = configuration.Routes.MapHttpRoute(
               name: "GALE_Home_Page",
               routeTemplate: "{controller}",
               defaults: new
               {
                   controller = "Startup",
                   action = "Get"
               },

               constraints: new
               {
                   controller = @"Startup", //Default Controller ONLY
                   httpMethod = new HttpMethodConstraint(System.Net.Http.HttpMethod.Get)
               }
           );


        }

        /// <summary>
        /// change the default formatter from XML to JSON (Google Chrome Fix)
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="version"></param>
        public static void SetJsonDefaultFormatter(this HttpConfiguration configuration)
        {
            configuration.Formatters.Add(new Gale.REST.Http.Formatter.JsonFormatter());
        }
    }

}
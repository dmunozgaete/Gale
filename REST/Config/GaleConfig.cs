using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Gale.REST.Config
{
    /// <summary>
    /// Gale Global Configuration
    /// </summary>
    public static class GaleConfig
    {
        private static string _apiVersion = null;

        /// <summary>
        /// Retrieves the Api Version
        /// </summary>
        public static String apiVersion
        {
            get
            {
                return _apiVersion;
            }
        }


        /// <summary>
        /// Enable Gale Routing for standard RESTful Protocols (in "v1" version)
        /// </summary>
        /// <param name="configuration"></param>
        public static void Register(HttpConfiguration configuration)
        {
            Register(configuration, _apiVersion);
        }

        /// <summary>
        /// Enable Gale Routing for standard RESTful Protocols
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="apiVersion">Api Version to enable</param>
        public static void Register(HttpConfiguration configuration, string apiVersion)
        {
            //Set api Version if exists :P
            _apiVersion = apiVersion;

            //Add Child RESTful Service Action Selector
            configuration.Services.Replace(
                typeof(System.Web.Http.Controllers.IHttpActionSelector),
                new Gale.REST.Config.VerbHttpRouteMap(
                configuration
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

    }
}

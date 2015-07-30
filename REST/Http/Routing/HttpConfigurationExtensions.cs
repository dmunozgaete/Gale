using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Http
{
    public static class HttpConfigurationExtensions
    {
        public static void MapGaleRoutes(this HttpConfiguration configuration)
        {
            MapGaleRoutes(configuration, null);
        }

        public static void MapGaleRoutes(this HttpConfiguration configuration, string apiVersion)
        {
            configuration.Services.Replace(
                typeof(System.Web.Http.Controllers.IHttpActionSelector),
                new Gale.REST.Http.Routing.GaleApiControllerActionSelector(
                configuration,
                apiVersion
            ));
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

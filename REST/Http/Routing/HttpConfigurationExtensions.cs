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

        public static void MapGaleRoutes(this HttpConfiguration configuration, string version)
        {
            configuration.Services.Replace(
                typeof(System.Web.Http.Controllers.IHttpActionSelector),
                new Gale.REST.Http.Routing.GaleApiControllerActionSelector(
                configuration,
                version
            ));
        }
    }
}

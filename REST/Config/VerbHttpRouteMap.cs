using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Gale.REST.Config
{
    /// <summary>
    /// Set the HttpMethod as the Action , for RestFul principles
    /// </summary>
    public class VerbHttpRouteMap : ApiControllerActionSelector
    {
        public VerbHttpRouteMap(HttpConfiguration config)
        {
            String[] allowedMethods = new string[] { "Get", "Post", "Put", "Delete", "Options", "Patch", "Head" };
            string version = String.IsNullOrEmpty(Gale.REST.Config.GaleConfig.apiVersion) ? "" : Gale.REST.Config.GaleConfig.apiVersion + "/";
            string RestFulTemplate = version + "{controller}/{id}";

            foreach (string method in allowedMethods)
            {
                //-------------------------------------------------------
                //Only Enabled the ActionName Verbs
                config.Routes.MapHttpRoute(
                   name: "Gale_VerbHttpRouteMap_" + method,
                   routeTemplate: RestFulTemplate,
                   defaults: new
                   {
                       id = RouteParameter.Optional,
                       extension = RouteParameter.Optional,
                       action = method
                   },
                   constraints: new
                   {
                       HttpMethod = new HttpMethodConstraint(
                           new System.Net.Http.HttpMethod(method)
                       )
                   }
                );
                //-------------------------------------------------------
            }
        }

        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            return base.SelectAction(controllerContext);
        }
    }
}

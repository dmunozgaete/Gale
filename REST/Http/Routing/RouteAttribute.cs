using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace Gale.REST.Http.Routing
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class Route : Attribute, IDirectRouteFactory
    {
        private string _hierarchicalTemplate = null;

        public Route(string template)
        {
            //FORMAT: {version}/{template}
            string hierarchicalTemplate = String.Format("{0}" + (template.StartsWith("/") ? "" : "/") + "{1}",
                    Gale.REST.Http.Routing.GaleApiControllerActionSelector.apiVersion,
                    template
            );
            _hierarchicalTemplate = hierarchicalTemplate;
        }

        public RouteEntry CreateRoute(DirectRouteFactoryContext context)
        {


            return (new System.Web.Http.RouteAttribute(_hierarchicalTemplate)
            {

            } as IDirectRouteFactory).CreateRoute(context);
        }

        public string Template
        {
            get { return _hierarchicalTemplate; }
        }
    }
}

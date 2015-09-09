using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorEngine.Templating;

namespace Gale.REST.Config.Startup
{
    /// <summary>
    /// Startup Default Controller for GALE
    /// </summary>
    public class StartupController : System.Web.Http.ApiController
    {

        /// <summary>
        /// Retrieves the Home Gale Page
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public System.Net.Http.HttpResponseMessage Get()
        {
            string api_url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            string version = Gale.REST.Config.GaleConfig.apiVersion;

            //----------------------------------
            var response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new System.Net.Http.StringContent(
                    RenderView("Gale.REST.Config.Startup.Startup.cshtml",
                    new
                    {
                        GALE_DOCS_SITE = Gale.REST.Resources.GALE_DOCS_SITE,
                        isSwaggerEnabled = Gale.REST.Config.SwaggerConfig.IsSwaggerEnabled,
                        APIUrl =api_url +  (api_url.EndsWith("/") ? "" : "/") + version
                    })
                )
            };

            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
            return response;
            //----------------------------------

        }

        /// <summary>
        /// Render a Razor View
        /// </summary>
        /// <param name="resourceName">embedded resource</param>
        /// <param name="model">Model</param>
        /// <returns></returns>
        private string RenderView(string resourceName, Object model)
        {
            //----------------------------------
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            using (System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    return RazorEngine.Engine.Razor.RunCompile(reader.ReadToEnd(), resourceName, null, model);
                }
            }
            //----------------------------------
        }

    }
}
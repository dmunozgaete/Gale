using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorEngine.Templating;

namespace Gale.REST.Http.Routing.Home
{
    public class StartupController : System.Web.Http.ApiController
    {

        [Obsolete]
        public System.Net.Http.HttpResponseMessage Get()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resx = "Gale.REST.Http.Routing.Home.Startup.html";

            using (System.IO.Stream stream = assembly.GetManifestResourceStream(resx))
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    //----------------------------------
                    // RAZOR TEMPLATING
                    var model = new
                    {
                        isSwaggerEnabled = true
                    };
                    string html = RazorEngine.Engine.Razor.RunCompile(reader.ReadToEnd(), "StartupPage", null, model);
                    //----------------------------------
                    var response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)
                    {
                        Content = new System.Net.Http.StringContent(html)
                    };

                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");

                    return response;
                }
            }
        }
    }
}

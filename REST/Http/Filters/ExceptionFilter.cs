using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Karma.REST.Http.Filters
{

    /// <summary>
    /// Catch all Exception , to pre process and format into REST API Exception
    /// </summary>
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// On Exception Ocurred
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is Karma.Exception.KarmaException)
            {
                //DO NOTHING
            }
            else if (context.Exception is Karma.Exception.RestException)
            {
                //DO NOTHING
            }
            else if (context.Exception is System.Web.Http.HttpResponseException)
            {
                //DO NOTHING
            }
            else
            {
                //FORMAT THE UNHANDLEDEXCEPTION TO REST EXCEPTION FORMAT
                string type = context.Exception.GetType().Name;
                context.Exception = new Karma.Exception.RestException(type, context.Exception.Message);
            }
        }
    }
}

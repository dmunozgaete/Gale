using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Config
{
    class ExceptionMessageHandler : System.Web.Http.ExceptionHandling.ExceptionHandler
    {
        public override void Handle(System.Web.Http.ExceptionHandling.ExceptionHandlerContext context)
        {
            System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.InternalServerError;
            string message = "";
            string code = "RAW";


            if (context.Exception is Gale.Exception.GaleException)
            {
                var ex = (context.Exception as Gale.Exception.GaleException);
                message = ex.Message;
                code = ex.Code;
                statusCode = ex.StatusCode;
            }
            else if (context.Exception is Gale.Exception.RestException)
            {
                var ex = (context.Exception as Gale.Exception.RestException);
                message = ex.Message;
                code = ex.Code;
                statusCode = ex.StatusCode;
            }
            else
            {
                //FORMAT THE UNHANDLEDEXCEPTION TO REST EXCEPTION FORMAT
                code = context.Exception.GetType().Name;
                message = context.Exception.Message;
            }

            //WRAP THE EXCEPTION IN A STANDAR FORMAT
            context.Result = new System.Web.Http.Results.ResponseMessageResult(new System.Net.Http.HttpResponseMessage()
            {
                ReasonPhrase = code,
                StatusCode = statusCode,
                Content = new System.Net.Http.ObjectContent<Gale.Exception.RestException.ErrorContent>(new Gale.Exception.RestException.ErrorContent()
                {
                    error = code,
                    error_description = message,
                },
                new System.Net.Http.Formatting.JsonMediaTypeFormatter())
            });
        }
    }
}
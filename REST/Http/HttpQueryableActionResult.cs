using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

//CONTENT NEGOTATION: http://www.strathweb.com/2012/07/everything-you-want-to-know-about-asp-net-web-api-content-negotation/

namespace Gale.REST.Http
{
    /// <summary>
    /// Define a Queryable Http Result based in the ODATA Protocol (http://www.odata.org)
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class HttpQueryableActionResult<TModel> : Gale.REST.Http.HttpBaseActionResult where TModel : class
    {
        public override Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            // Gale.REST.Blueprint.Builders.SQLServer.Read<TModel>
            throw new NotImplementedException();
        }
    }
}

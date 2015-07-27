using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

//CONTENT NEGOTATION: http://www.strathweb.com/2012/07/everything-you-want-to-know-about-asp-net-web-api-content-negotation/

namespace Gale.REST.Http
{
    /// <summary>
    ///  Defines a Create command that asynchronously creates an System.Net.Http.HttpResponseMessage.    
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class HttpCreateActionResult<T> : Gale.REST.Http.Generic.HttpActionResult<T> where T : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model">T Model for the associated request (this.Model)</param>
        public HttpCreateActionResult(T model) : base(model) { }


        /// <summary>
        /// Creates an System.Net.Http.HttpResponseMessage asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <returns> A task that, when completed, contains the System.Net.Http.HttpResponseMessage.</returns>
        public override abstract Task<System.Net.Http.HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken);
    }
}

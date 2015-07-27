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
    ///  Defines a Update command that asynchronously creates an System.Net.Http.HttpResponseMessage.    
    /// </summary>
    /// <typeparam name="T">TModel</typeparam>
    public abstract class HttpUpdateActionResult<T> : Gale.REST.Http.Generic.HttpActionResult<T> where T : class
    {
        private string _token = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token">Identifier Token</param>
        /// <param name="model">T Model for the associated request (this.Model)</param>
        public HttpUpdateActionResult(string token , T model) : base(model) {
            _token = token;
        }

        /// <summary>
        /// Creates an System.Net.Http.HttpResponseMessage asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <returns> A task that, when completed, contains the System.Net.Http.HttpResponseMessage.</returns>
        public override Task<System.Net.Http.HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            return ExecuteAsync(_token, cancellationToken);
        }


        /// <summary>
        /// Creates an System.Net.Http.HttpResponseMessage asynchronously.
        /// </summary>
        /// <param name="token">Token Identifier</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <returns> A task that, when completed, contains the System.Net.Http.HttpResponseMessage.</returns>
        public abstract Task<System.Net.Http.HttpResponseMessage> ExecuteAsync(string token, System.Threading.CancellationToken cancellationToken);
    }
}

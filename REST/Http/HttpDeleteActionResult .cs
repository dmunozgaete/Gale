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
    ///  Defines a delete command that asynchronously creates an System.Net.Http.HttpResponseMessage.    
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class HttpDeleteActionResult : Gale.REST.Http.HttpBaseActionResult
    {
        private string _token = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token">Token which pass to the ExecuteAsync Method</param>
        public HttpDeleteActionResult(string token)
        {
            _token = token;
        }

        /// <summary>
        /// Creates an System.Net.Http.HttpResponseMessage asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <returns> A task that, when completed, contains the System.Net.Http.HttpResponseMessage.</returns>
        public override Task<System.Net.Http.HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            //------------------------------------------------------------------------------------------------------
            // GUARD EXCEPTIONS
            Gale.Exception.GaleException.Guard(() => String.IsNullOrEmpty(_token), "TOKEN_REQUIRED");
            //------------------------------------------------------------------------------------------------------

            return ExecuteAsync(_token, cancellationToken);
        }

        /// <summary>
        /// Creates an System.Net.Http.HttpResponseMessage asynchronously
        /// </summary>
        /// <param name="token">Token to Delete</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <returns>A task that, when completed, contains the System.Net.Http.HttpResponseMessage.</returns>
        public abstract Task<System.Net.Http.HttpResponseMessage> ExecuteAsync(string token, System.Threading.CancellationToken cancellationToken);
        
    }
}

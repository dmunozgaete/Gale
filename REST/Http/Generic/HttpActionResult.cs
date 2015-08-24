using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

//CONTENT NEGOTATION: http://www.strathweb.com/2012/07/everything-you-want-to-know-about-asp-net-web-api-content-negotation/

namespace Gale.REST.Http.Generic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class HttpActionResult<T> : Gale.REST.Http.HttpBaseActionResult where T : class
    {
        private T _model = null;

        /// <summary>
        /// Retrieve the Model associated with the Result
        /// </summary>
        public T Model
        {
            get
            {
                return _model;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model">T Model for the associated request (this.Model)</param>
        public HttpActionResult(T Model)
        {
            _model = Model;
        }
    }
}

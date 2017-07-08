using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Gale.Exception
{
    /// <summary>
    /// Rest Exception for a REST protocol
    /// </summary>
    public class RestException : System.Exception
    {

        System.Net.HttpStatusCode _statusCode;
        String _code;
        String _message;

        /// <summary>
        /// Get the Http Status Code for the Exception 
        /// </summary>
        public System.Net.HttpStatusCode StatusCode
        {
            get
            {
                return _statusCode;
            }
        }

        /// <summary>
        /// Get the Code for the Exception
        /// </summary>
        public String Code
        {
            get
            {
                return _code;
            }
        }

        /// <summary>
        /// Get the Message for the Exception
        /// </summary>
        public override String Message
        {
            get
            {
                return _message;
            }
        }

        /// <summary>
        /// Create a exception
        /// </summary>
        /// <param name="code">Identifier for the Error</param>
        /// <param name="message">Message</param>
        public RestException(string code, string message)
            : base()
        {
            _statusCode = System.Net.HttpStatusCode.InternalServerError;
            _code = code;
            _message = message;
        }

        /// <summary>
        /// Create a exception
        /// </summary>
        /// <param name="statusCode">Http Status Code</param>
        /// <param name="code">Identifier for the Error</param>
        /// <param name="message">Message</param>
        public RestException(System.Net.HttpStatusCode statusCode, string code, string message)
            : base()
        {
            _statusCode = statusCode;
            _code = code;
            _message = message;
        }


        /// <summary>
        /// Throw exception when condition evaluate true
        /// </summary>
        /// <param name="condition">Delegate to Evaluate</param>
        /// <param name="statusCode">Http Status Code</param>
        /// <param name="code">Identifier for the Error</param>
        /// <param name="message">Message</param>
        public static void Guard(Func<bool> condition, System.Net.HttpStatusCode statusCode, string code, string message)
        {
            if (condition())
            {
                if (String.IsNullOrEmpty(message))
                {
                    message = code;
                }

                throw new RestException(statusCode, code, message);
            }
        }

        /// <summary>
        /// Throw exception when condition evaluate true
        /// </summary>
        /// <param name="condition">Delegate to Evaluate</param>
        /// <param name="code">Identifier for the Error</param>
        /// <param name="message">Message</param>
        public static void Guard(Func<bool> condition, string code, string message)
        {
            Guard(condition, System.Net.HttpStatusCode.BadRequest, code, message);
        }

        /// <summary>
        /// Throw exception when condition evaluate true
        /// </summary>
        /// <param name="condition">Delegate to Evaluate</param>
        /// <param name="code">Identifier for the Error</param>
        /// <param name="resourceManager">Resource Manager for find the code</param>
        public static void Guard(Func<bool> condition, string code, System.Resources.ResourceManager resourceManager)
        {
            string message = resourceManager.GetString(code);
            Guard(condition, System.Net.HttpStatusCode.BadRequest, code, message);
        }


        /// <summary>
        /// Response Error Content
        /// </summary>
        internal class ErrorContent
        {
            /// <summary>
            /// Error Identifier
            /// </summary>
            public string error { get; set; }

            /// <summary>
            /// Description for the error
            /// </summary>
            public string error_description { get; set; }
        }
    }
}
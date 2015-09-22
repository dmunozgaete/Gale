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
    public class RestException: System.Exception
    {

        /// <summary>
        /// Create a exception
        /// </summary>
        /// <param name="code">Identifier for the Error</param>
        /// <param name="message">Message</param>
        public RestException(string code, string message)
            : base()
        {

            Build( System.Net.HttpStatusCode.InternalServerError, code, message);
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

            Build(statusCode, code, message);
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
                Build(statusCode, code, message);
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
        /// Build the exception
        /// </summary>
        /// <param name="statusCode">Http Status Code</param>
        /// <param name="code">Identifier for the Error</param>
        /// <param name="message">Message</param>
        private static void Build(System.Net.HttpStatusCode statusCode, String code, string message)
        {

            throw new HttpResponseException(new System.Net.Http.HttpResponseMessage()
            {
                ReasonPhrase = code,
                StatusCode = statusCode,
                Content = new System.Net.Http.ObjectContent<ErrorContent>(new ErrorContent()
                {
                    error = code,
                    error_description = message,
                },
                new System.Net.Http.Formatting.JsonMediaTypeFormatter())
            });

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

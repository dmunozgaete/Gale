using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.Exception
{
    /// <summary>
    /// Clase de excepción para el FrameWork
    /// </summary>
    internal sealed class GaleException : System.Exception
    {
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="ResourceString">Llave del recurso a emplear</param>
        /// <param name="Parameters">Arreglo de parámetros de tipo string</param>
        public GaleException(System.Net.HttpStatusCode statusCode, string resourceString, params string[] parameters)
            : base()
        {
            Build(statusCode, resourceString, parameters);
        }

        public GaleException(string resourceString, params string[] parameters)
            : base()
        {
            Build(System.Net.HttpStatusCode.InternalServerError, resourceString, parameters);
        }



        private static void Build(System.Net.HttpStatusCode statusCode, String code, params String[] parameters)
        {
            String resource = Gale.Exception.Errors.ResourceManager.GetString(code);

            string message = resource != null ? String.Format(resource, parameters) : code;
            //string message_code = resource != null ? code : "RAW";

            throw new System.Web.Http.HttpResponseException(new System.Net.Http.HttpResponseMessage()
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

        /// <summary>
        /// Delegado de función a evaluar
        /// </summary>
        /// <param name="ErrorCondition">Condición de error a evaluar</param>
        /// <param name="ResourceString">Llave del recurso a emplear</param>
        /// <param name="Parameters">Arreglo de objetos de tipo string</param>
        public static void Guard(Func<bool> ErrorCondition, string resourceString, params string[] parameters)
        {
            if (ErrorCondition())
            {
                Build(System.Net.HttpStatusCode.InternalServerError, resourceString, parameters);
            }
        }

        /// <summary>
        /// Delegado de función a evaluar
        /// </summary>
        /// <param name="ErrorCondition">Condición de error a evaluar</param>
        /// <param name="ResourceString">Llave del recurso a emplear</param>
        /// <param name="Parameters">Arreglo de objetos de tipo string</param>
        public static void Guard(Func<bool> ErrorCondition, System.Net.HttpStatusCode statusCode, string resourceString, params string[] parameters)
        {
            if (ErrorCondition())
            {
                Build(statusCode, resourceString, parameters);
            }
        }

    }
}

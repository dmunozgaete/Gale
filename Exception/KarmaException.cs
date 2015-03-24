using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karma.Exception
{
    /// <summary>
    /// Clase de excepción para el FrameWork
    /// </summary>
    internal sealed class KarmaException : System.Exception
    {
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="ResourceString">Llave del recurso a emplear</param>
        /// <param name="Parameters">Arreglo de parámetros de tipo string</param>
        public KarmaException(System.Net.HttpStatusCode statusCode, string resourceString, params string[] parameters)
            : base()
        {
            Build(statusCode, resourceString, parameters);
        }

        public KarmaException(string resourceString, params string[] parameters)
            : base()
        {

            Build(System.Net.HttpStatusCode.InternalServerError, resourceString, parameters);
        }



        private static void Build(System.Net.HttpStatusCode statusCode, String resourceString, params String[] parameters)
        {
            String resource = Karma.Exception.Errors.ResourceManager.GetString(resourceString);

            string message = resource != null ? String.Format(resource, parameters) : resourceString;
            string message_code = resource != null ? resourceString : "RAW";

            throw new System.Web.Http.HttpResponseException(new System.Net.Http.HttpResponseMessage()
            {
                StatusCode = statusCode,
                Content = new System.Net.Http.StringContent(String.Format("{0}: {1}", message_code, message)),
                ReasonPhrase = message_code
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

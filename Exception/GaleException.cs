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

        private void Build(System.Net.HttpStatusCode statusCode, String code, params String[] parameters)
        {
            String resource = Gale.Exception.Errors.ResourceManager.GetString(code);
            string message = resource != null ? String.Format(resource, parameters) : code;

            _statusCode = statusCode;
            _code = code;
            _message = message;
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
                Guard(ErrorCondition, System.Net.HttpStatusCode.InternalServerError, resourceString, parameters);
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
                throw new GaleException(statusCode, resourceString, parameters);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gale.Db
{
    /// <summary>
    /// Servicio de Datos
    /// </summary>
    public class DataService: System.IDisposable
    {
        private Gale.Db.Collections.DataParameterCollection _parameters;
        private string _command = null;

        public DataService(string command)
        {
            _command = command;
        }
        public DataService(string command, Gale.Db.Collections.DataParameterCollection parameters)
        {
            _command = command;
            _parameters = parameters;
        }

        #region Exposed Properties

        /// <summary>
        /// Nombre del Procedimiento de Almacenado
        /// </summary>
        public string Command
        {
            get
            {
                return _command;
            }
        }

        /// <summary>
        /// Obtiene el listado de parametros a enviar en la solicitud
        /// </summary>
        public Gale.Db.Collections.DataParameterCollection Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new Collections.DataParameterCollection();
                }
                return _parameters;
            }
        }

        /// <summary>
        /// Verifica que existan parametros en la solicitud
        /// </summary>
        /// <returns></returns>
        public bool HasParameters()
        {
            return (_parameters != null && _parameters.Count>0);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_parameters != null)
            {
                _parameters.Clear();
            }
            _parameters = null;
        }

        #endregion
    }
}
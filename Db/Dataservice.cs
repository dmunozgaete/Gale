using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Gale.Db
{
    /// <summary>
    /// Servicio de Datos
    /// </summary>
    public class DataService : System.IDisposable
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
        /// Add Parameter via the Model
        /// </summary>
        /// <param name="Model"></param>
        public void FromModel<T>(T Model)
        {
            Type EntityType = typeof(T);

            var properties = (
                from t in EntityType.GetProperties()
                where
                t.CanRead && t.GetIndexParameters().Count() == 0 &&
                (t.PropertyType.IsGenericType == false || (t.PropertyType.IsGenericType == true &&
                t.PropertyType.GetGenericTypeDefinition() != typeof(System.Data.Linq.EntitySet<>)))
                select t);

            //Add Each Parameter in the collection =)!
            foreach (var property in properties)
            {
                Object value = property.GetValue(Model);
                Type propertyType = value.GetType();
                if (value != null)
                {
                    if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType))
                    {
                        continue;
                    }

                    if (!propertyType.IsSimpleType())
                    {
                        //Loop Over the Model Class
                        MethodInfo method = this.GetType().GetMethod("FromModel");
                        MethodInfo generic = method.MakeGenericMethod(value.GetType());
                        generic.Invoke(this, new object[] { value });

                    }
                    else
                    {
                        String db_name = property.Name;
                        var attr = property.TryGetAttribute<System.Data.Linq.Mapping.ColumnAttribute>();
                        if (attr != null)
                        {
                            System.Data.Linq.Mapping.ColumnAttribute column_attr = (attr as System.Data.Linq.Mapping.ColumnAttribute);
                            if (column_attr != null && column_attr.Name != null && column_attr.Name.Length > 0)
                            {
                                db_name = column_attr.Name;
                            }
                        }

                        //Add Parameter
                        this.Parameters.Add(db_name, value);
                    }
                }
            }
        }

        /// <summary>
        /// Verifica que existan parametros en la solicitud
        /// </summary>
        /// <returns></returns>
        public bool HasParameters()
        {
            return (_parameters != null && _parameters.Count > 0);
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
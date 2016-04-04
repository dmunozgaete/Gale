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
        /// Add each parameter binded to a source in the Model
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
                if (value != null)
                {
                    Type propertyType = value.GetType();

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
                        else
                        {
                            continue;
                        }

                        //Add Parameter
                        this.Parameters.Add(db_name, value);
                    }
                }
            }
        }


        /// <summary>
        /// Add Table Type (SQL Structure) to a Store Procedure 
        /// https://msdn.microsoft.com/en-us/library/bb675163.aspx
        /// </summary>
        /// <typeparam name="T">Model Type</typeparam>
        /// <param name="parameterName">Parameter Name</param>
        /// <param name="model">Model List</param>
        /// <param name="tableType">Table Type in the Database</param>
        public void AddTableType<T>(String parameterName, List<T> model)
        {
            Type EntityType = typeof(T);

            //Reflect the Model Properies (Perform Pattern For Huge Data)
            List<ReflectedFieldCaching> MemoryOptimizer = (
                from t in
                    EntityType.GetProperties()
                where
                    t.CanRead && t.GetIndexParameters().Count() == 0 &&
                    (t.PropertyType.IsGenericType == false || (t.PropertyType.IsGenericType == true &&
                    t.PropertyType.GetGenericTypeDefinition() != typeof(System.Data.Linq.EntitySet<>))) &&
                    t.TryGetAttribute<System.Data.Linq.Mapping.ColumnAttribute>() != null
                select new ReflectedFieldCaching
                {
                    columnName = t.Name,
                    property = t,
                    columnAttribute = t.GetCustomAttribute<System.Data.Linq.Mapping.ColumnAttribute>()
                }
            ).ToList();

            //DataTable
            System.Data.DataTable table = new System.Data.DataTable();

            //----------------------------------------------------------------------
            //Create the Datatable Structure Columns
            foreach (var field in MemoryOptimizer)
            {
                string column_name = field.columnName;
                if (field.columnAttribute.Name != null)
                {
                    field.columnName = field.columnAttribute.Name;
                }

                table.Columns.Add(field.columnName);
            }
            //----------------------------------------------------------------------

            //----------------------------------------------------------------------
            //Create the Datatable Structure Columns
            foreach (var item in model)
            {
                System.Data.DataRow row = table.NewRow();
                foreach (var field in MemoryOptimizer)
                {
                    row[field.columnName] = field.property.GetValue(item);
                }

                table.Rows.Add(row);
            }
            //----------------------------------------------------------------------

            this.Parameters.Add(parameterName, table);
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


        #region Helper Class
        /// <summary>
        /// Internal Memory Caching 
        /// </summary>
        internal class ReflectedFieldCaching
        {
            /// <summary>
            /// Database Column Name
            /// </summary>
            public string columnName { get; set; }

            /// <summary>
            /// Property Model 
            /// </summary>
            public System.Reflection.PropertyInfo property { get; set; }

            /// <summary>
            /// ColumnAttribute Model 
            /// </summary>
            public System.Data.Linq.Mapping.ColumnAttribute columnAttribute { get; set; }
        }
        #endregion
    }
}
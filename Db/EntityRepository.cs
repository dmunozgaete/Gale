using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.Db
{
    /// <summary>
    /// Repositorio de Entidades fuertemente tipeada
    /// </summary>
    public class EntityRepository
    {
        private System.Data.DataSet _rawData;
        private List<object> _caching;  //Cache Pattern 

        public EntityRepository(System.Data.DataSet rawData)
        {
            _rawData = rawData;
        }

        /// <summary>
        /// Entity Caching (For Fast re-reading)
        /// </summary>
        private List<object> Caching
        {
            get
            {
                if (_caching == null)
                {
                    _caching = new List<object>();
                }
                return _caching;
            }
        }

        /// <summary>
        /// Obtiene los registros de la tabla 0 sin convertir a un modelo especifico
        /// </summary>
        /// <returns></returns>
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public System.Data.DataTable GetRawTable()
        {
            return GetRawTable(0);
        }

        /// <summary>
        /// Obtiene los registros de la tabla sin convertir a un modelo especifico
        /// </summary>
        /// <param name="index">Indice de tabla donde extraer los registros</param>
        /// <returns></returns>
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public System.Data.DataTable GetRawTable(int index)
        {
            return this._rawData.Tables[index];
        }

        /// <summary>
        /// Obtiene la tabla con registros de modelo especifico
        /// </summary>
        /// <typeparam name="T">Objeto (Modelo) a Extraer</typeparam>
        /// <param name="index">Indice de tabla donde extraer los registros</param>
        /// <returns></returns>
        public Gale.Db.EntityTable<T> GetModel<T>(int index) where T : class
        {
            Type EntityType = typeof(Gale.Db.EntityTable<T>);

            //Responsive Cache Pattern
            if (_caching != null && _caching.SingleOrDefault((obj) => { return obj.GetType() == typeof(T); }) != null)
            {
                return (Gale.Db.EntityTable<T>)(from t in Caching
                                                where t.GetType().GetGenericTypeDefinition() == EntityType
                                                select t).FirstOrDefault();
            }

            //Create the instance wich set the data
            Gale.Db.EntityTable<T> Model = (Gale.Db.EntityTable<T>)Activator.CreateInstance(EntityType);
            if (_rawData.Tables.Count > 0)
            {
                FillEntity<T>(ref Model, _rawData.Tables[index]);
            }

            //Set into the Cache
            Caching.Add(Model);

            return Model;
        }

        /// <summary>
        /// Obtiene la tabla con registros de modelo especifico
        /// </summary>
        /// <typeparam name="T">Objeto (Modelo) a Extraer</typeparam>
        /// <returns></returns>
        public Gale.Db.EntityTable<T> GetModel<T>() where T : class
        {
            return GetModel<T>(0);
        }

        /// <summary>
        /// Fill a T model with the Database Result Values
        /// </summary>
        /// <typeparam name="T">Type to fill</typeparam>
        /// <param name="model">Entity Table to Fill</param>
        /// <param name="table">DB Result Table</param>
        private void FillEntity<T>(ref Gale.Db.EntityTable<T> model, System.Data.DataTable table)
        {
            Type EntityType = typeof(T);

            //Perform Pattern For Huge Data =)
            List<MemoryFieldCaching> MemoryOptimizer = (from t in EntityType.GetProperties()
                                                        where
                                                        t.CanRead && t.GetIndexParameters().Count() == 0 &&
                                                        (t.PropertyType.IsGenericType == false || (t.PropertyType.IsGenericType == true &&
                                                        t.PropertyType.GetGenericTypeDefinition() != typeof(System.Data.Linq.EntitySet<>)))
                                                        select new MemoryFieldCaching
                                                        {
                                                            columnName = t.Name,
                                                            property = t
                                                        }).ToList();


            foreach (System.Data.DataRow row in table.Rows)
            {
                T Item = Activator.CreateInstance<T>();
                #region Transform Each Row
                foreach (MemoryFieldCaching Caching in MemoryOptimizer)
                {
                    //Perform Pattern For Huge Data =)

                    //Ordinal:
                    //  -1: Significa que tiene que ir a buscarlo
                    string Name = "";
                    if (Caching.ordinal == -1)
                    {
                        try
                        {
                            Name = Caching.columnName;

                            System.Data.Linq.Mapping.ColumnAttribute ColumnAttribute = (System.Data.Linq.Mapping.ColumnAttribute)(Caching.property.GetCustomAttributes(typeof(System.Data.Linq.Mapping.ColumnAttribute), true).FirstOrDefault());
                            if (ColumnAttribute != null && ColumnAttribute.Name != null && ColumnAttribute.Name.Length > 0)
                            {
                                Name = ColumnAttribute.Name;
                            }

                            if (ColumnAttribute == null)
                            {
                                Caching.ordinal = -2; //Campo Personalizado (no debe ser llenado por base de datos)
                            }
                            else
                            {
                                if (ColumnAttribute != null)
                                {
                                    //Si existe el atributo de columna y puede ser nulo, verifico que esta columna exista, de no existir esta columna, 
                                    //no se debe caer, sino que solo no debe establecerla 
                                    if (ColumnAttribute.CanBeNull || ColumnAttribute.DbType == null)
                                    {
                                        if (table.Columns.Contains(Name))
                                        {
                                            Caching.ordinal = table.Columns[Name].Ordinal;
                                        }
                                        else
                                        {
                                            Caching.ordinal = -2;
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            if (table.Columns.Contains(Name))
                                            {
                                                Caching.ordinal = table.Columns[Name].Ordinal;
                                            }
                                            else
                                            {
                                                Caching.ordinal = -2;
                                            }

                                        }
                                        catch (System.Exception ex)
                                        {
                                            //throw new Gale.Gale.Exception.GaleException("ColumnNameNotFoundInDataServiceAndIsNotNullable", Caching.columnName, Name, EntityType.Name);
                                            throw ex;
                                        }

                                    }
                                }
                                else
                                {
                                    Caching.ordinal = table.Columns[Name].Ordinal;
                                }

                            }

                        }
                        catch (System.Exception ex)
                        {
                            //---[ Guard Exception ]-------------------------------------------------------------------------------------------------------
                            Gale.Exception.GaleException.Guard(() => ex is IndexOutOfRangeException, "ColumnNameNotFoundInDataService", Caching.columnName, Name, EntityType.Name);
                            //-----------------------------------------------------------------------------------------------------------------------------
                            throw ex;
                        }
                    }
                    if (Caching.ordinal != -2)
                    {
                        object data = row[Caching.ordinal];

                        if (data is DateTime)
                        {
                            data = DateTime.SpecifyKind((DateTime)data, DateTimeKind.Local);
                        }

                        if (data is System.Guid && Caching.property.PropertyType == typeof(String))
                        {
                            data = data.ToString();
                        }

                        if (!(data is System.DBNull))
                        {

                            //Parse AnyWay for implicit Casting
                            if (Caching.property.PropertyType.IsGenericType == false && Caching.property.PropertyType != data.GetType())
                            {
                                data = Convert.ChangeType(data, Caching.property.PropertyType);
                            }


                            Caching.property.SetValue(Item, data, null);
                        }
                    }
                }
                #endregion
                model.Add(Item);
            }
        }

        /// <summary>
        /// Internal Memory Caching 
        /// </summary>
        internal class MemoryFieldCaching
        {
            /// <summary>
            /// Field Ordinal
            /// </summary>
            public int ordinal { get; set; }

            /// <summary>
            /// Database Column Name
            /// </summary>
            public string columnName { get; set; }

            /// <summary>
            /// Property Model 
            /// </summary>
            public System.Reflection.PropertyInfo property { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            public MemoryFieldCaching()
            {
                ordinal = -1;
            }
        }
    }
}

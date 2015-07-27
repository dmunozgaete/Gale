using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class DatasetExtension
    {
        /// <summary>
        /// Convierte una Tabla de datos (System.Data.Datatable) a una tabla de entidades del Framework CORFO del tipo T
        /// </summary>
        /// <typeparam name="T">Tipo de Entidad a Convertir</typeparam>
        /// <param name="TableSource">Extension de Datatable</param>
        /// <returns></returns>
        public static Gale.Db.EntityTable<T> ConvertToEntityTable<T>(this System.Data.DataTable TableSource) where T : class, new()
        {
            Type objectToConvertType = typeof(T);

            //Perform Pattern For Huge Data =)
            List<MemoryFieldCaching> MemoryOptimizer = null;

            Gale.Db.EntityTable<T> entityTable = new Gale.Db.EntityTable<T>();

            MemoryOptimizer = (from t in objectToConvertType.GetProperties()
                               where t.CanRead && t.GetIndexParameters().Count() == 0 && (t.PropertyType.IsGenericType == false || (t.PropertyType.IsGenericType == true && t.PropertyType.GetGenericTypeDefinition() != typeof(System.Data.Linq.EntitySet<>)))
                               select new MemoryFieldCaching
                               {
                                   columnName = t.Name,
                                   property = t,
                                   columnAttribute = (System.Data.Linq.Mapping.ColumnAttribute)t.GetCustomAttributes(typeof(System.Data.Linq.Mapping.ColumnAttribute), false).FirstOrDefault(),
                               }).ToList();

            foreach (System.Data.DataRow row in TableSource.Rows)
            {
                T objectMapped = new T();

                foreach (MemoryFieldCaching Caching in MemoryOptimizer)
                {
                    //Perform Pattern For Huge Data =)
                    if (TableSource.Columns.Contains(Caching.columnName))
                    {
                        if (Caching.ordinal == -1)
                        {
                            Caching.ordinal = TableSource.Columns[Caching.columnName].Ordinal;
                        }

                        object value = row[Caching.ordinal];
                        if (row[Caching.ordinal] != System.DBNull.Value)
                        {
                            //Cast To Type
                            if (Caching.property.PropertyType == typeof(char))  //Char
                            {
                                value = System.Char.Parse(value.ToString());
                            }
                            else if (Caching.property.PropertyType == typeof(Int16))  //Int16
                            {
                                value = System.Int16.Parse(value.ToString());
                            }
                            else if (Caching.property.PropertyType == typeof(Int32))  //Int32
                            {
                                value = System.Int32.Parse(value.ToString());
                            }
                            else if (Caching.property.PropertyType == typeof(Int64))  //Int64
                            {
                                value = System.Int64.Parse(value.ToString());
                            }
                            else if (Caching.property.PropertyType == typeof(Decimal))  //Decimal
                            {
                                value = System.Decimal.Parse(value.ToString());
                            }
                            else if (Caching.property.PropertyType == typeof(string))  //String
                            {
                                value = row[Caching.ordinal].ToString();
                            }
                            else if (Caching.property.PropertyType == typeof(DateTime))  //DateTime
                            {
                                value = System.DateTime.Parse(value.ToString());
                            }
                            else if (Caching.property.PropertyType == typeof(Byte))  //Byte
                            {
                                value = System.Byte.Parse(value.ToString());
                            }
                            else if (Caching.property.PropertyType == typeof(System.Guid))  //Byte
                            {
                                value = System.Guid.Parse(value.ToString());
                            }
                            else
                            {
                                value = row[Caching.ordinal];
                            }
                        }
                        else
                        {
                            value = null;
                        }
                        Caching.property.SetValue(objectMapped, value, null);
                    }
                }

                entityTable.Add(objectMapped);
            }

            return entityTable;
        }

        internal class MemoryFieldCaching
        {
            private string _columnName = "";
            public int ordinal { get; set; }
            public string columnName
            {
                get
                {
                    if (this.columnAttribute != null && this.columnAttribute.Name != null)
                    {
                        return this.columnAttribute.Name;
                    }
                    return _columnName;
                }
                set
                {
                    this._columnName = value;
                }
            }
            public System.Data.Linq.Mapping.ColumnAttribute columnAttribute { get; set; }
            public System.Reflection.PropertyInfo property { get; set; }

            public MemoryFieldCaching()
            {
                ordinal = -1;
            }
        }
    }
}
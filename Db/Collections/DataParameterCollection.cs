using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.Db.Collections
{
    public class DataParameterCollection : System.Collections.Generic.List<Gale.Db.DataParameter>, IDisposable
    {
        /// <summary>
        /// Insert the parameter to the service
        /// </summary>
        /// <typeparam name="T">Parameter Type</typeparam>
        /// <param name="ParameterName">Parameter Name</param>
        /// <param name="Value">Parameter Value</param>
        public void Add<T>(string ParameterName, T Value)
        {
            Gale.Db.DataParameter item = new Gale.Db.DataParameter(ParameterName, Value, typeof(T));
            base.Add(item);
        }

        /// <summary>
        /// Insert the parameter with null Value (Compatibilty to MYSQL wich not support optional parameters)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ParameterName">Nombre del Parametro</param>
        public void Add(string ParameterName)
        {
            Add(ParameterName, System.DBNull.Value);
        }


        /// <summary>
        /// Insert the parameter to the service with OUT Direction (Oracle DB Support)
        /// </summary>
        /// <typeparam name="T">Parameter Type</typeparam>
        /// <param name="ParameterName">Parameter Name</param>
        /// <param name="Value">Parameter Value</param>
        public void AddOut<T>(string ParameterName, T Value)
        {
            Gale.Db.DataParameter item = new Gale.Db.DataParameter(ParameterName, Value, typeof(T), System.Data.ParameterDirection.Output);
            base.Add(item);
        }

        /// <summary>
        /// Insert the parameter with null Value with OUT Direction (Oracle DB Support)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ParameterName">Nombre del Parametro</param>
        public void AddOut(string ParameterName)
        {
            AddOut(ParameterName, System.DBNull.Value);
        }

        /// <summary>
        /// Insert the parameter to the service with OUT Direction (Oracle DB Support)
        /// </summary>
        /// <typeparam name="T">Parameter Type</typeparam>
        /// <param name="ParameterName">Parameter Name</param>
        /// <param name="Value">Parameter Value</param>
        public void AddInOut<T>(string ParameterName, T Value)
        {
            Gale.Db.DataParameter item = new Gale.Db.DataParameter(ParameterName, Value, typeof(T), System.Data.ParameterDirection.InputOutput);
            base.Add(item);
        }

        /// <summary>
        /// Insert the parameter with null Value with OUT Direction (Oracle DB Support)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ParameterName">Nombre del Parametro</param>
        public void AddInOut(string ParameterName)
        {
            AddInOut(ParameterName, System.DBNull.Value);
        }


        /// <summary>
        /// Insert the parameter to the service with Return Value Direction (Oracle DB Support)
        /// </summary>
        /// <typeparam name="T">Parameter Type</typeparam>
        /// <param name="ParameterName">Parameter Name</param>
        /// <param name="Value">Parameter Value</param>
        public void AddReturnValue<T>(string ParameterName, T Value)
        {
            Gale.Db.DataParameter item = new Gale.Db.DataParameter(ParameterName, Value, typeof(T), System.Data.ParameterDirection.ReturnValue);
            base.Add(item);
        }

        /// <summary>
        /// Insert the parameter with null Value with Return Value Direction (Oracle DB Support)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ParameterName">Nombre del Parametro</param>
        public void AddReturnValue(string ParameterName)
        {
            AddReturnValue(ParameterName, System.DBNull.Value);
        }



        #region IDisposable Members

        public void Dispose()
        {
            //System.GC.Collect(System.GC.GetGeneration(this));
        }

        #endregion
    }
}

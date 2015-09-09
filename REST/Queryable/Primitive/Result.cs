using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gale.REST.Queryable.Primitive.Reflected;

namespace Gale.REST.Queryable.Primitive
{
    /// <summary>
    /// Encapsulate Records 
    /// </summary>
    public class Result
    {
        private int _total;
        private TimeSpan _elapsedTime;
        private List<Field> _fields;
        private List<List<Object>> _data;

        /// <summary>
        /// Total Record's in the Database
        /// </summary>
        public int total
        {
            get
            {
                return this._total;
            }
        }

        /// <summary>
        /// Elapsed Time 
        /// </summary>
        public TimeSpan elapsedTime
        {
            get
            {
                return this._elapsedTime;
            }
        }

        /// <summary>
        /// Fields Structure
        /// </summary>
        public List<Field> fields
        {
            get
            {
                return this._fields;
            }
        }

        /// <summary>
        /// Record Data
        /// </summary>
        public List<List<Object>> data
        {
            get
            {
                return this._data;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="total">Total Record's</param>
        /// <param name="elapsedTime">Elapsed Time</param>
        /// <param name="fields">Field's</param>
        /// <param name="data">Database Records</param>
        public Result(int total, TimeSpan elapsedTime, List<Field> fields, List<List<Object>> data)
        {
            this._total = total;
            this._elapsedTime = elapsedTime;
            this._fields = fields;
            this._data = data;
        }
    }
}

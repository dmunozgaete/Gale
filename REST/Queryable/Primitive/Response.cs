using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karma.REST.Queryable.Primitive.Reflected;
using Newtonsoft.Json;

namespace Karma.REST.Queryable.Primitive
{
    public class Response : IResponse
    {
        private int _offset;
        private int _limit;
        private int _total;
        private TimeSpan _elapsedTime;

        private List<Karma.REST.Queryable.Primitive.Reflected.Field> _fields;
        private List<List<String>> _data = new List<List<string>>();


        public Response(int offset, int limit, int total, TimeSpan elapsedTime, List<Field> fields, List<List<String>> data)
        {
            _offset = offset;
            _limit = limit;
            _total = total;
            _elapsedTime = elapsedTime;

            //Table Response
            this._fields = (from t in fields where t.IsSelected select t).ToList();
            this._data = data;
        }

        internal int offset
        {
            get
            {
                return _offset;
            }
        }
        internal int limit
        {
            get
            {
                return _limit;
            }
        }
        internal int total
        {
            get
            {
                return _total;
            }
        }

        internal TimeSpan elapsedTime
        {
            get
            {
                return _elapsedTime;
            }
        }

        /// <summary>
        /// Return the fields returned from the query
        /// </summary>
        internal List<Karma.REST.Queryable.Primitive.Reflected.Field> fields
        {
            get
            {
                return this._fields;
            }
        }

        /// <summary>
        /// Return data values
        /// </summary>
        internal List<List<String>> data
        {
            get
            {
                return this._data;
            }
        }


        public virtual object toPlainObject()
        {
            return new
            {
                offset = offset,
                limit = limit,
                total = total,
                elapsedTime = elapsedTime.ToString("t"),

                fiels = (from t in this._fields
                         select new
                         {
                             name = t.Name,
                             specification = t.Specification.ToString(),
                             type = (
                                 //---- Primary KEY
                                  t.Specification == Reflected.Field.SpecificationEnum.Pk ? "identifier" :

                                  //---- Foreign KEY
                                  t.Specification == Reflected.Field.SpecificationEnum.Fk ? "foreign" :

                                  //---- String's and GUID's
                                  t.Type == typeof(System.Guid) ||
                                  t.Type == typeof(System.String) ? "text" :

                                  //---- Date 
                                  t.Type == typeof(System.DateTime) ? "date" :

                                  //---- Boolean
                                  t.Type == typeof(System.Boolean) ? "boolean" :

                                  //---- Number's
                                  t.Type == typeof(System.Int16) ||
                                  t.Type == typeof(System.Int32) ||
                                  t.Type == typeof(System.Int64) ? "number" : "unknow"
                             )
                         }).ToList(),

                items = data
            };
        }

    }
}

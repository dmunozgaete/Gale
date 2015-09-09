using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Gale.REST.Queryable.Primitive;
using Gale.REST.Queryable.Primitive.Reflected;
using Newtonsoft.Json;

namespace Gale.REST.Queryable.OData.Results
{
    /// <summary>
    /// Response a JSON useful for Report Data
    /// </summary>
    public class TableResult : Gale.REST.Queryable.OData.Results.Result
    {
        private List<Gale.REST.Queryable.Primitive.Reflected.Field> _fields;

        public TableResult(Gale.REST.Queryable.Primitive.Result response)
        {
            this.total = response.total;
            this.elapsedTime = response.elapsedTime.ToString("t");

            //Table Response
            this._fields = (from t in response.fields where t.IsSelected select t).ToList();
            
            this.items = new List<Object>( response.data);
        }


        public List<Object> fields
        {
            get
            {
                List<Object> _fields = new List<object>();

                var fields = (from t in this._fields
                              select new
                              {
                                  name = t.Name,
                                  specification = t.Specification.ToString(),
                                  type = (
                                      //---- Primary KEY
                                       t.Specification == Gale.REST.Queryable.Primitive.Reflected.Field.SpecificationEnum.Pk ? "identifier" :

                                       //---- Foreign KEY
                                       t.Specification == Gale.REST.Queryable.Primitive.Reflected.Field.SpecificationEnum.Fk ? "foreign" :

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
                              });

                foreach (var item in fields)
                {
                    _fields.Add(item);
                }

                return _fields;
            }
        }

    }
}

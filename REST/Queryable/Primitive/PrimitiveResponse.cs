using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Queryable.Primitive
{
    public class PrimitiveResponse: IResponse
    {
        private Response _response = null;
        private List<object> _items = new List<object>();

        public PrimitiveResponse(Response response)
        {
            _response = response;

            _items  = new List<object>();
            foreach (var data in response.data)
            {
                int ordinal = 0;

                var _plainObject = new System.Dynamic.ExpandoObject();
                foreach (var field in response.fields)
                {
                    ((IDictionary<String, Object>)_plainObject).Add(field.Name,data[ordinal]);
                    ordinal++;
                }
                _items.Add(_plainObject);

                ordinal = 0;
            }

        }

        public object toPlainObject()
        {
            return new
            {
                offset = _response.offset,
                limit = _response.limit,
                total = _response.total,
                elapsedTime = _response.elapsedTime.ToString("t"),
                items = _items
            };
        }
    }
}

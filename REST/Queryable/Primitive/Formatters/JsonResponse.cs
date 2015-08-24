using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Queryable.Primitive.Formatters
{
    public class JsonResponse : IResponse
    {
        private TableResponse _response = null;
        private List<object> _items = new List<object>();

        public JsonResponse(TableResponse response)
        {
            _response = response;

            _items = new List<object>();
            foreach (var data in response.data)
            {
                int ordinal = 0;

                var plainObject = new System.Dynamic.ExpandoObject();
                var groupedFields = new SortedList<string, KeyValuePair<Reflected.Field, Object>>();

                foreach (var field in response.fields)
                {
                    if (field.Name.IndexOf("_") > 0)
                    {
                        groupedFields.Add(field.Name, new KeyValuePair<Reflected.Field, object>(field, data[ordinal]));
                    }
                    else
                    {
                        //Add Direct Property
                        ((IDictionary<String, Object>)plainObject).Add(field.Name, data[ordinal]);
                    }
                    ordinal++;
                }

                //Order the Grouped Fields
                var grouped = groupedFields.GroupBy((field) =>
                {
                    return field.Key.Substring(0, field.Key.IndexOf("_")); ;
                });

                foreach (var group in grouped)
                {
                    var diggedObject = new System.Dynamic.ExpandoObject();

                    foreach (var field in group)
                    {
                        var columnKey = field.Key.Substring(field.Key.IndexOf("_") + 1);

                        ((IDictionary<String, Object>)diggedObject).Add(columnKey, field.Value.Value);
                    }

                    //Add Digged Object
                    ((IDictionary<String, Object>)plainObject).Add(group.Key, diggedObject);
                }

                _items.Add(plainObject);

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

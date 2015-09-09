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
    /// Response a JSON object format
    /// </summary>
    public class JsonResult : Gale.REST.Queryable.OData.Results.Result
    {

        public JsonResult(Gale.REST.Queryable.Primitive.Result response, int offset, int limit)
        {
            List<object> _items = new List<object>();

            foreach (var data in response.data)
            {
                int ordinal = 0;

                var plainObject = new System.Dynamic.ExpandoObject();
                var groupedFields = new SortedList<string, KeyValuePair<Gale.REST.Queryable.Primitive.Reflected.Field, Object>>();

                foreach (var field in response.fields)
                {
                    if (field.Name.IndexOf("_") > 0)
                    {
                        groupedFields.Add(field.Name, new KeyValuePair<Gale.REST.Queryable.Primitive.Reflected.Field, object>(field, data[ordinal]));
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


            //Set Values into the Base Response
            base.offset = offset;
            base.limit = limit;
            base.total = response.total;
            base.items = _items;
            base.elapsedTime = response.elapsedTime.ToString("t");
        }
    }
}

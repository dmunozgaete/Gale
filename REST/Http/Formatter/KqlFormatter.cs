using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace Gale.REST.Http.Formatter
{

    /// <summary>
    /// Convert "_" model name to Grouped Object , for native JSON , sub-groups 
    /// <remarks>
    ///     Example Model:  
    ///         token={guid}
    ///         creator_user=David Gaete, 
    ///         creator_identifier=USU  
    ///         
    ///     => Json Output Model
    ///         {
    ///           token: {guid},
    ///           creator: {
    ///             user: 'David Gaete',
    ///             identifier: 'USU'
    ///           }
    ///         }
    /// </remarks>
    /// </summary>
    public class KqlFormatter : JsonMediaTypeFormatter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public KqlFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));

            this.SerializerSettings.Converters.Add(new EntityJsonConverter());
        }

        public class EntityJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType.IsClass;
                //ARRAY COLLECTION
                //(typeof(Gale.Db.IEntityTable)).IsAssignableFrom(objectType) ||
                //SIMPLE OBJECT
                //(typeof(System.ComponentModel.INotifyPropertyChanging)).IsAssignableFrom(objectType);
            }

            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var sobject = SerializedObject(value);
                writer.WriteRawValue(JsonConvert.SerializeObject(sobject));
            }


            private Object SerializedObject(Object value)
            {
                //NULL??
                if (value == null)
                {
                    return value;
                }

                //LIST COLLECTION???
                if (typeof(IList).IsAssignableFrom(value.GetType()))
                {
                    IEnumerable table = (IEnumerable)value;

                    List<object> _items = new List<object>();
                    foreach (Object item in table)
                    {
                        var expando = SerializedObject(item);
                        _items.Add(expando);
                    }

                    return _items;
                }
                else
                {
                    var expando = FlatObject(value);

                    return expando;
                }
            }

            private Object FlatObject(Object item)
            {
                bool isPrimitive = (
                                    item.GetType().IsPrimitive ||
                                    item.GetType() == typeof(DateTime) ||
                                    item.GetType() == typeof(String) ||
                                    item.GetType() == typeof(float) ||
                                    item.GetType() == typeof(Decimal) ||
                                    item.GetType() == typeof(System.Guid)
                                   );
                if (isPrimitive)
                {
                    return item;
                }

                var properties = item.GetType().GetProperties().ToList();
                var plainObject = new System.Dynamic.ExpandoObject();
                var groupedFields = new SortedList<string, KeyValuePair<System.Reflection.PropertyInfo, Object>>();

                foreach (var property in properties)
                {

                    if (property.Name.IndexOf("_") > 0)
                    {
                        groupedFields.Add(property.Name, new KeyValuePair<System.Reflection.PropertyInfo, Object>(property, SerializedObject(property.GetValue(item))));
                    }
                    else
                    {
                        //Add Direct Property
                        ((IDictionary<String, Object>)plainObject).Add(property.Name, SerializedObject(property.GetValue(item)));
                    }
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
                return plainObject;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

    }

}
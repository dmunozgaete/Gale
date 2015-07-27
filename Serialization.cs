using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Reflection;

namespace Gale
{
    public class Serialization
    {
        #region JSON Serialization
        public static string ToJson(object Target)
        {
            var s = new System.Web.Script.Serialization.JavaScriptSerializer();
            s.MaxJsonLength = int.MaxValue; //Change to Max Length  (Huge 4 MB!!!)

            if (Target != null && Target.GetType().Equals(typeof(System.Data.DataSet)))
            {
                #region Serializacion Especial para un Dataset
                System.Data.DataSet dsToFragment = (System.Data.DataSet)Target;

                List<JsonTable> JsonDataset = new List<JsonTable>();
                foreach (System.Data.DataTable dtable in dsToFragment.Tables)
                {
                    JsonTable _jsonTable = new JsonTable();

                    //--------------------------------------------------------------------------------------------------
                    //----[ Add Reference to Columns
                    _jsonTable.columns = new List<JsonTable.JsonColumn>();
                    foreach (System.Data.DataColumn col in dtable.Columns)
                    {
                        JsonTable.JsonColumn column = new JsonTable.JsonColumn();
                        column.datattype = col.DataType.ToString();
                        column.name = col.ColumnName;
                        _jsonTable.columns.Add(column);
                    }
                    //--------------------------------------------------------------------------------------------------


                    //--------------------------------------------------------------------------------------------------
                    //----[ Add rows data
                    _jsonTable.rows = new List<object[]>();
                    foreach (System.Data.DataRow row in dtable.Rows)
                    {
                        _jsonTable.rows.Add(row.ItemArray);
                    }
                    //--------------------------------------------------------------------------------------------------


                    JsonDataset.Add(_jsonTable);
                }
                return (s).Serialize(JsonDataset);
                #endregion
            }
            else
            {
                return (s).Serialize(Target);
            }
        }

        public static T FromJson<T>(string json)
        {

            var s = new System.Web.Script.Serialization.JavaScriptSerializer();
            s.MaxJsonLength = int.MaxValue; //Change to Max Length  (Huge 4 MB!!!)

            if (typeof(T).Equals(typeof(System.Data.DataSet)))
            {
                #region Serializacion Especial para un Dataset
                List<JsonTable> jsonDataset = s.Deserialize<List<JsonTable>>(json);

                System.Data.DataSet ds = new System.Data.DataSet();

                foreach (JsonTable jsonTable in jsonDataset)
                {
                    System.Data.DataTable table = ds.Tables.Add();
                    foreach (JsonTable.JsonColumn jcolumn in jsonTable.columns)
                    {
                        table.Columns.Add(jcolumn.name, Type.GetType(jcolumn.datattype));
                    }

                    foreach (object[] jrow in jsonTable.rows)
                    {
                        System.Data.DataRow row = table.NewRow();
                        row.ItemArray = jrow;
                        table.Rows.Add(row);
                    }
                }

                return (T)Convert.ChangeType(ds, typeof(T));
                #endregion
            }
            else
            {
                return s.Deserialize<T>(json);
            }
        }
        #endregion

        private const string cleanExpression = "(xmlns:\\w+=|xmlns=|xmlns:xsi=|xmlns:xsd=|xsi:type=|xsi:nil=)\"\\b(.*?)\"";
        #region XML Serialization

        public static string ToXML(object Target)
        {
            bool isCollection = typeof(System.Collections.IEnumerable).IsAssignableFrom(Target.GetType());
            return (isCollection ? ToXML(Target, null, true) : ToXML(Target, null, true));
        }
        public static string ToXML(object Target, XmlRootAttribute Root, bool cleanXML)
        {
            MemoryStream stream = new MemoryStream();
            XmlSerializer x = (Root == null ? new XmlSerializer(Target.GetType()) : new XmlSerializer(Target.GetType(), Root));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, Encoding.UTF8);

            x.Serialize(stream, Target);

            string xml = System.Text.Encoding.UTF8.GetString(((MemoryStream)xmlTextWriter.BaseStream).ToArray());

            if (cleanXML) //Clean The XML (remove the special Xml tags , like xmlns, xsi, etc..
            {
                System.Text.RegularExpressions.Regex Regex = new System.Text.RegularExpressions.Regex(cleanExpression);
                xml = Regex.Replace(xml, "");
            }

            return xml;
        }
        public static string ToXML(object Target, bool cleanXML)
        {
            return ToXML(Target, null, cleanXML);
        }
        public static string ToXML(object Target, XmlRootAttribute Root)
        {
            return ToXML(Target, Root, true);
        }

        public static T FromXML<T>(XmlNode Xml) where T : new()
        {
            return FromXML<T>(Xml, System.Threading.Thread.CurrentThread.CurrentCulture);
        }
        public static T FromXML<T>(string Xml) where T : new()
        {
            return FromXML<T>(Xml, System.Threading.Thread.CurrentThread.CurrentCulture);
        }
        public static T FromXML<T>(string Xml, IFormatProvider Provider) where T : new()
        {
            System.Text.RegularExpressions.Regex Regex = new System.Text.RegularExpressions.Regex(cleanExpression);

            XmlDocument DummyDoc = new XmlDocument();
            DummyDoc.InnerXml = Regex.Replace(Xml, "");

            //---[ Guard Exception ]-------------------------------------------------------------------------------------------------------
            Gale.Exception.GaleException.Guard(() => DummyDoc.ChildNodes.Count == 0, "InvalidInputXMLStringInXMLDeserialization", typeof(T).Name);
            //-----------------------------------------------------------------------------------------------------------------------------

            foreach (XmlNode node in DummyDoc.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.XmlDeclaration)
                {
                    return FromXML<T>(node, Provider);
                }
            }
            throw new Gale.Exception.GaleException("InvalidInputXMLStringInXMLDeserialization", typeof(T).Name);
        }
        public static T FromXML<T>(XmlNode Xml, IFormatProvider Provider) where T : new()
        {
            Type TypeEntity = typeof(T);

            T Entity = new T();


            //List<> of Types
            if (TypeEntity.IsGenericType && typeof(System.Collections.IList).IsAssignableFrom(TypeEntity.GetGenericTypeDefinition()))
            {
                foreach (XmlNode node in Xml.ChildNodes)
                {
                    object ret = node.InnerXml;
                    if (node.NodeType != XmlNodeType.XmlDeclaration)
                    {
                        Type UnderlyingType = TypeEntity.GetGenericArguments()[0];

                        Type[] BasicTypes = { typeof(String), typeof(Int16), typeof(Int32), typeof(Int64), typeof(Boolean), typeof(DateTime), typeof(System.Char), typeof(System.Decimal), typeof(System.Double), typeof(System.Single), typeof(System.TimeSpan), typeof(System.Byte) };


                        //If not a basic Type , try to deep more in the dom tree deserializing the inner XML Value
                        if (BasicTypes.SingleOrDefault((t) => { return t == UnderlyingType; }) == null)
                        {
                            //Otherwise of arrays convert to XML to send
                            var fromXMLMethods = typeof(Gale.Serialization).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                            System.Reflection.MethodInfo fromXMLMethodInfo = fromXMLMethods.FirstOrDefault(mi => mi.Name == "FromXML" && mi.GetGenericArguments().Count() == 1 && mi.GetParameters()[0].ParameterType == typeof(XmlNode));

                            System.Reflection.MethodInfo method = fromXMLMethodInfo.MakeGenericMethod(new Type[] { UnderlyingType });
                            ret = method.Invoke(null, new object[] { node });
                        }

                        (Entity as System.Collections.IList).Add(Convert.ChangeType(ret, UnderlyingType));
                    }
                }
                return (T)Entity;
            }

            List<PropertyInfo> setterProperties = (from PropertyInfo p in TypeEntity.GetProperties() where p.GetIndexParameters().Count() == 0 select p).ToList();

            //Delegate
            Action<PropertyInfo, object> setValue = (Property, Value) =>
            {
                try
                {
                    Property.SetValue(Entity, Value, null);
                }
                catch
                {
                    throw new Gale.Exception.GaleException("InvalidSetValueInXMLDeserialization", Value.ToString(), Property.Name, Property.PropertyType.ToString());
                }
            };

            //For Each
            setterProperties.ForEach((prop) =>
            {
                if (object.ReferenceEquals(prop.DeclaringType, TypeEntity))
                {
                    System.Xml.Serialization.XmlAttributeAttribute customAttribute = (System.Xml.Serialization.XmlAttributeAttribute)prop.GetCustomAttributes(typeof(System.Xml.Serialization.XmlAttributeAttribute), false).FirstOrDefault();
                    Type propertyType = prop.PropertyType;
                    string nodeValue = null;
                    System.Xml.XmlNode noderef = null;

                    if (customAttribute != null)
                    {
                        XmlAttribute attrib = Xml.Attributes[customAttribute.AttributeName];
                        if (attrib != null)
                        {
                            nodeValue = attrib.InnerXml;
                        }
                    }
                    else
                    {
                        System.Xml.Serialization.XmlElementAttribute xmlattrib = (System.Xml.Serialization.XmlElementAttribute)prop.GetCustomAttributes(typeof(System.Xml.Serialization.XmlElementAttribute), false).FirstOrDefault();
                        XmlNode propertyNode = Xml.SelectSingleNode((xmlattrib != null) ? xmlattrib.ElementName : prop.Name);
                        if (propertyNode != null)
                        {
                            noderef = propertyNode;
                            nodeValue = propertyNode.InnerXml;
                        }
                    }

                    if (nodeValue != null && nodeValue != String.Empty)
                    {
                        if (Type.Equals(propertyType, typeof(decimal)))
                        {
                            setValue(prop, System.Convert.ToDecimal(nodeValue, Provider));
                        }
                        else if (Type.Equals(propertyType, typeof(double)))
                        {
                            setValue(prop, System.Convert.ToDouble(nodeValue, Provider));
                        }
                        else if (Type.Equals(propertyType, typeof(Int16)))
                        {
                            setValue(prop, System.Convert.ToInt16(nodeValue, Provider));
                        }
                        else if (Type.Equals(propertyType, typeof(Int32)))
                        {
                            setValue(prop, System.Convert.ToInt32(nodeValue, Provider));
                        }
                        else if (Type.Equals(propertyType, typeof(Int64)))
                        {
                            setValue(prop, System.Convert.ToInt64(nodeValue, Provider));
                        }
                        else if (Type.Equals(propertyType, typeof(bool)))
                        {
                            setValue(prop, System.Convert.ToBoolean(nodeValue, Provider));
                        }
                        else if (Type.Equals(propertyType, typeof(DateTime)))
                        {
                            setValue(prop, System.Convert.ToDateTime(nodeValue, Provider));
                        }
                        else if (Type.Equals(propertyType, typeof(string)))
                        {
                            setValue(prop, System.Convert.ToString(nodeValue, Provider));
                        }
                        else if (propertyType.IsArray)
                        {

                            if (nodeValue != string.Empty)
                            {
                                Type UnderlyingType = propertyType.GetElementType();  //Array or a Generic List (Almost the same things)
                                // Creates and initializes a new Array of type Int32.
                                Array typedArray = Array.CreateInstance(UnderlyingType, noderef.ChildNodes.Count);

                                int arrayIndex = 0;
                                foreach (XmlNode node in noderef.ChildNodes)
                                {
                                    object ret = node.InnerXml;
                                    if (node.NodeType != XmlNodeType.XmlDeclaration)
                                    {
                                        //If not a basic Type , try to deep more in the dom tree deserializing the inner XML Value
                                        Type[] BasicTypes = { typeof(String), typeof(Int16), typeof(Int32), typeof(Int64), typeof(Boolean), typeof(DateTime), typeof(System.Char), typeof(System.Decimal), typeof(System.Double), typeof(System.Single), typeof(System.TimeSpan), typeof(System.Byte) };
                                        if (BasicTypes.SingleOrDefault((t) => { return t == UnderlyingType; }) == null)
                                        {
                                            //Otherwise of arrays convert to XML to send
                                            var fromXMLMethods = typeof(Gale.Serialization).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                                            System.Reflection.MethodInfo fromXMLMethodInfo = fromXMLMethods.FirstOrDefault(mi => mi.Name == "FromXML" && mi.GetGenericArguments().Count() == 1 && mi.GetParameters()[0].ParameterType == typeof(XmlNode));

                                            System.Reflection.MethodInfo method = fromXMLMethodInfo.MakeGenericMethod(new Type[] { UnderlyingType });
                                            ret = method.Invoke(null, new object[] { node });
                                        }

                                        typedArray.SetValue(Convert.ChangeType(ret, UnderlyingType), arrayIndex);
                                    }
                                    arrayIndex++;
                                }

                                setValue(prop, typedArray);
                            }
                        }
                        else if (propertyType.IsGenericType)
                        {
                            //If a Parameter Type is Nullable, create the nullable type dinamycally and set it's value
                            Type UnderlyingType = propertyType.GetGenericArguments()[0];
                            Type GenericDefinition = propertyType.GetGenericTypeDefinition();

                            Type GenericType = GenericDefinition.MakeGenericType(new Type[] { UnderlyingType });

                            object value = null;
                            if (nodeValue != string.Empty)
                            {
                                if (GenericDefinition == typeof(System.Nullable<>))
                                {
                                    value = Convert.ChangeType(nodeValue, UnderlyingType);
                                }
                                else
                                {
                                    //Complex Node Type, (Maybe it's a List of Other Complex Type and go on in the deepest level
                                    //So call the fromXML itself, again and again

                                    //Otherwise of arrays convert to XML to send
                                    var fromXMLMethods = typeof(Gale.Serialization).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                                    System.Reflection.MethodInfo fromXMLMethodInfo = fromXMLMethods.FirstOrDefault(mi => mi.Name == "FromXML" && mi.GetGenericArguments().Count() == 1 && mi.GetParameters()[0].ParameterType == typeof(XmlNode));

                                    System.Reflection.MethodInfo method = fromXMLMethodInfo.MakeGenericMethod(new Type[] { GenericType });

                                    value = method.Invoke(null, new object[] { noderef });

                                }
                                setValue(prop, Activator.CreateInstance(GenericType, new object[] { value }));
                            }
                        }
                        else
                        {
                            try
                            {
                                //Its a more complex type ( not generic but yes a object with properties and setter's)
                                if (noderef.ChildNodes.Count > 0)
                                {
                                    //Otherwise of arrays convert to XML to send
                                    var fromXMLMethods = typeof(Gale.Serialization).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                                    System.Reflection.MethodInfo fromXMLMethodInfo = fromXMLMethods.FirstOrDefault(mi => mi.Name == "FromXML" && mi.GetGenericArguments().Count() == 1 && mi.GetParameters()[0].ParameterType == typeof(XmlNode));

                                    System.Reflection.MethodInfo method = fromXMLMethodInfo.MakeGenericMethod(new Type[] { propertyType });

                                    var value = method.Invoke(null, new object[] { noderef });

                                    setValue(prop, Convert.ChangeType(value, propertyType));  //try to set into the variable anyways , if throw error then throw invalid cast exception
                                }
                                else
                                {
                                    setValue(prop, Convert.ChangeType(nodeValue, propertyType));  //try to set into the variable anyways , if throw error then throw invalid cast exception
                                }

                            }
                            catch
                            {
                                //TODO: Perform this Section, To Support Non Primitive Object Type
                                throw new Gale.Exception.GaleException("InvalidCastInXMLDeserialization", prop.Name);
                            }
                        }
                    }
                }
            });

            return (T)(Entity);
        }
        #endregion

        #region Base64 Serialization
        public static string ToBase64(string data)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(data);
            return System.Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string FromBase64(string data)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(data);
            return System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
        }
        #endregion
    }

    internal class JsonTable
    {
        public List<JsonColumn> columns { get; set; }
        public List<object[]> rows { get; set; }

        public class JsonColumn
        {

            public string datattype { get; set; }
            public string name { get; set; }
        }
    }
}

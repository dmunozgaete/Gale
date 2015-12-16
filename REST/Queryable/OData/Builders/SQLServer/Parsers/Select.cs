using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.REST.Queryable.OData.Builders.SQLServer.Parsers
{
    /// <summary>
    /// Select Parser
    /// </summary>
    internal class Select : Gale.REST.Queryable.Primitive.Parser
    {
        /// <summary>
        /// Parse SELECT 
        /// </summary>
        /// <param name="configuration">Gale Query Language Configuration</param>
        /// <param name="model">Model</param>
        /// <returns></returns>
        public override string Parse(GQLConfiguration configuration, Gale.REST.Queryable.Primitive.Reflected.Model model)
        {
            //SELECT PARSER QUERY
            List<String> builder = new List<string>();

            if (configuration.fields.Count == 0)
            {
                configuration.fields.Add("*");
            }

            #region SELECT FIELD
            //---- SELECT FIELD
            Action<Gale.REST.Queryable.Primitive.Reflected.Field> SelectField = new Action<Gale.REST.Queryable.Primitive.Reflected.Field>((field) =>
            {
                //The Main Primary Key , dont'need to add to the selection
                if (field.Specification == Gale.REST.Queryable.Primitive.Reflected.Field.SpecificationEnum.Pk)
                {
                    return;
                }

                //Only if, his, from the primary Table, add to selection, 
                //because all Foreign key Table, are Getting from the source (ForeignTable.*)
                if (field.Table.IsForeign == false)
                {
                    builder.Add(field.Key);
                }

                field.Select();

            });
            //---------------------------------
            #endregion

            //--- FIRST ADD THE PK
            builder.Insert(0, model.Tables.First().PrimaryKey.Key);

            //--- 
            foreach (String fieldName in configuration.fields)
            {
                string _fieldName = fieldName.Trim();

                //---[ Get all field from all tables :P
                if (_fieldName == "*.*")
                {
                    model.Fields.ForEach((f) =>
                    {
                        SelectField(f);
                    });
                    break;
                }

                //If query is * , bring all field's
                if (_fieldName.Contains("*"))
                {
                    Type searchedTable = null;

                    //Get all field from the Primary Table
                    if (_fieldName == "*")
                    {
                        searchedTable = model.Tables.First().Type; //Main Table
                    }
                    else
                    {
                        //try to get all field from a foreign table
                        if (!_fieldName.Contains(":("))
                        {
                            throw new Exception.GaleException("API012", _fieldName);
                        }
                        _fieldName = _fieldName.Substring(0, _fieldName.IndexOf(":("));
                        var fk = model.Constraints.FirstOrDefault(constraint => constraint.ThisField.Name == _fieldName);
                        if (fk == null)
                        {
                            throw new Exception.GaleException("API013", _fieldName);
                        }

                        searchedTable = fk.Table.Type;
                    }

                    model.Fields.ForEach((f) =>
                    {
                        if (f.Table.Type == searchedTable)
                        {
                            SelectField(f);
                        }
                    });

                    continue;
                }

                //Check if field exist's
                Gale.REST.Queryable.Primitive.Reflected.Field field = model.Fields.FirstOrDefault((f) =>
                {
                    return f.Name.ToLower() == _fieldName;
                });

                //If field is not exists, throw exception
                if (field == null)
                {
                    throw new Exception.GaleException("API013", _fieldName);
                }

                //Primary Key
                if (field.Specification == Gale.REST.Queryable.Primitive.Reflected.Field.SpecificationEnum.Pk)
                {
                    throw new Exception.GaleException("API014", _fieldName);
                }

                //Select Field
                SelectField(field);
            }

            return String.Join(",", builder);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.REST.Queryable.Primitive.Reflected
{
    public class Model
    {
        //-- VARIABLES
        private List<Field> _selectedFields;    //For Ordering's
        private List<Field> _fields;
        private List<Constraint> _constraints;
        private List<Table> _tables;

        public Model(List<Field> fields, List<Constraint> constraints, List<Table> tables)
        {
            this._fields = fields;
            this._constraints = constraints;
            this._tables = tables;

            //Attach event on every Field
            this._fields.ForEach((field) =>
            {
                if (field.IsSelected)
                {
                    SelectedFields.Add(field);
                }
                else
                {
                    field.OnSelectField += new Field.OnSelectFieldEventHandler((args) =>
                    {
                        SelectedFields.Add(field);
                    });
                }
            });
        }

        public List<Reflected.Field> Fields
        {
            get
            {
                return _fields;
            }
        }

        public List<Reflected.Field> SelectedFields
        {
            get
            {
                if (_selectedFields == null)
                {
                    _selectedFields = new List<Field>();
                }

                return _selectedFields;
            }
        }


        public List<Reflected.Constraint> Constraints
        {
            get
            {
                return _constraints;
            }
        }

        public List<Reflected.Table> Tables
        {
            get
            {
                return _tables;
            }
        }
    }
}

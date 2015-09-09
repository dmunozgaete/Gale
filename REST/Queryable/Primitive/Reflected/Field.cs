using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Gale.REST.Queryable.Primitive.Reflected
{
    public class Field
    {
        //-- VARIABLES
        private String _key;
        private String _name;
        private Type _type;
        private SpecificationEnum _specification;
        private Table _table;
        private Boolean _selected;

        //-- EVENT'S
        public delegate void OnSelectFieldEventHandler(EventArgs e);
        public event OnSelectFieldEventHandler OnSelectField;

        private void build(System.Reflection.PropertyInfo property, System.Data.Linq.Mapping.ColumnAttribute attribute, Table table, SpecificationEnum specification)
        {
            this._key = (attribute.Name != null ? attribute.Name : attribute.Storage.Substring(1)).Trim();

            if (this._key == property.Name)
            {
                throw new Gale.Exception.GaleException("API008",property.Name);
            }

            this._name = char.ToLower(property.Name[0]) + property.Name.Substring(1);
            this._type = property.PropertyType;
            this._table = table;
            this._specification = specification;
            this._selected = false;

            if (attribute.IsPrimaryKey)
            {
                _specification = SpecificationEnum.Pk;
                table.SetPrimaryKey(this);

                if (attribute.IsPrimaryKey && table.IsForeign == false)
                {
                    this._selected = true;
                }
            }
        }
     

        public Field(System.Reflection.PropertyInfo property, System.Data.Linq.Mapping.ColumnAttribute attribute, Table table)
        {
            build(property, attribute, table, SpecificationEnum.None);
        }

        public Field(System.Reflection.PropertyInfo property, System.Data.Linq.Mapping.ColumnAttribute attribute, Table table, SpecificationEnum specification)
        {
            build(property, attribute, table, specification);
        }

        /// <summary>
        /// Mark as Selected Field for the response
        /// </summary>

        internal void Select()
        {
            //If not selected, call handler if exist's
            if (_selected == false)
            {
                //------------------------------------------
                OnSelectFieldEventHandler handler = OnSelectField;
                if (handler != null)
                {
                    var e = new EventArgs();
                    handler(e);
                }
                //------------------------------------------
            }

            _selected = true;
        }

        [JsonIgnore]
        /// <summary>
        /// Return if the field is seelcted
        /// </summary>
        public Boolean IsSelected
        {
            get
            {
                return this._selected;
            }
        }

        [JsonIgnore]
        public String Key
        {
            get
            {
                return this._key;
            }
        }

        public String Name
        {
            get
            {
                if (this._table.IsForeign)
                {
                    return String.Format("{0}:({1})", this._table.Prefix, this._name);
                }
                return this._name;
            }
        }

        [JsonIgnore]
        public Type Type
        {
            get
            {
                return this._type;
            }
        }

        [JsonIgnore]
        public Table Table
        {
            get
            {
                return this._table;
            }
        }

        [JsonIgnore]
        public SpecificationEnum Specification
        {
            get
            {
                return _specification;
            }
        }

        public enum SpecificationEnum
        {
            Pk = 1,
            Fk = 2,
            Descriptor = 3,
            None = 4
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.REST.Queryable.Primitive.Reflected
{
    public class Table
    {
        //--------[ VARIABLES
        private String _key;
        private String _name;
        private String _prefix;
        private Type _type;
        private Reflected.Field _primaryKey;
        private Delegate _delegate;

        public Table(Type tableType)
        {
            build(tableType, null);
        }
        public Table(Type tableType, string prefix)
        {
            build(tableType, prefix);
        }

        private void build(Type tableType, String prefix)
        {
            //GET the Table Attribute
            var attr = tableType.TryGetAttribute<System.Data.Linq.Mapping.TableAttribute>();
            if (attr == null)
            {
                throw new Gale.Exception.GaleException("API007", tableType.Name);
            }

            //Set Table metadata
            this._type = tableType;
            this._name = tableType.Name;
            this._key = attr.Name;
            this._prefix = prefix;
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void SetPrimaryKey(Reflected.Field primary){
            this._primaryKey = primary;
        }

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void SetDescriptor(Delegate expression)
        {
            this._delegate = expression;
        }

        public String Key
        {
            get
            {
                return _key;
            }
        }

        public String Name
        {
            get
            {
                return _name;
            }
        }

        public Type Type
        {
            get
            {
                return _type;
            }
        }

        public Delegate Descriptor
        {
            get
            {
                return _delegate;
            }
        }
        
        public Reflected.Field PrimaryKey
        {
            get
            {
                return this._primaryKey;
            }
        }

        public String Prefix
        {
            get
            {
                return _prefix;
            }
        }
        public Boolean IsForeign
        {
            get
            {
                return _prefix != null;
            }
        }
    }
}

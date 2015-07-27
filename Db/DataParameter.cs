using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gale.Db
{
    public class DataParameter
    {
        private string _name;
        private object _value;
        private Type _type;
        private System.Data.ParameterDirection _direction = System.Data.ParameterDirection.Input;


        public DataParameter(string Name, object Value, Type Type)
        {
            _name = Name;
            _value = Value;
            _type = Type;
        }

        public DataParameter(string Name, object Value, Type Type, System.Data.ParameterDirection Direction)
        {
            _name = Name;
            _value = Value;
            _type = Type;
            _direction = Direction;
        }

        public System.Data.ParameterDirection Direction
        {
            get
            {
                return _direction;
            }
            private set
            {
                _direction = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            private set
            {
                _name = value;
            }
        }
        public object Value
        {
            get
            {
                return _value;
            }
            private set
            {
                _value = value;
            }
        }
        public Type Type
        {
            get
            {
                return _type;
            }
        }
    }
}
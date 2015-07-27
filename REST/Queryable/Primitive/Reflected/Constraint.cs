using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.REST.Queryable.Primitive.Reflected
{
    public class Constraint
    {
        private Reflected.Field _thisField;
        private Reflected.Field _otherField;
        private Table _table;

        public Constraint(Reflected.Field thisField, Reflected.Field otherField, Table table)
        {
            this._thisField = thisField;
            this._otherField = otherField;
            this._table = table;
        }

        public Reflected.Field ThisField
        {
            get
            {
                return _thisField;
            }
        }

        public Reflected.Field OtherField
        {
            get
            {
                return _otherField;
            }
        }

        public Table Table
        {
            get
            {
                return _table;
            }
        }
    }
}

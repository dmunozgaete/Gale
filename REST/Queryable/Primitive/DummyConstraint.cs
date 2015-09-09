using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gale.REST.Queryable.Primitive.Reflected;

namespace Gale.REST.Queryable.Primitive
{
    internal class DummyConstraint
    {
        private String _thisName;
        private String _otherName;
        private Table _table;

        public DummyConstraint(string thisName, string otherName, Table table)
        {
            this._thisName = thisName;
            this._otherName = otherName;
            this._table = table;
        }

        public String thisName
        {
            get
            {
                return _thisName;
            }
        }

        public String otherName
        {
            get
            {
                return _otherName;
            }
        }

        public Table table
        {
            get
            {
                return _table;
            }
        }
    }
}

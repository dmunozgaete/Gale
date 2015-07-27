using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gale.REST.Queryable.Primitive
{
    public class ExecutedParserEventArgs
    {
        private Gale.REST.Queryable.Primitive.Parser _parser;
        private String _resultQueryFragment;
        private Boolean _changed;

        public String ResultQueryFragment {
            get
            {
                return _resultQueryFragment;
            }
            set
            {
                _resultQueryFragment = value;
                _changed = true;
            }
        }

        public Gale.REST.Queryable.Primitive.Parser Parser
        {
            get {
                return _parser;
            }
        }

        public Boolean Changed
        {
            get
            {
                return _changed;
            }
        }

        public ExecutedParserEventArgs(Gale.REST.Queryable.Primitive.Parser parser, String queryFragment)
        {
            this._parser = parser;
            this._resultQueryFragment = queryFragment;
        }
    }
}

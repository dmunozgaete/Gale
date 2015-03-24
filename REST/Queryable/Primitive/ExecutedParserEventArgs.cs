using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karma.REST.Queryable.Primitive
{
    public class ExecutedParserEventArgs
    {
        private Karma.REST.Queryable.Primitive.Parser _parser;
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

        public Karma.REST.Queryable.Primitive.Parser Parser
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

        public ExecutedParserEventArgs(Karma.REST.Queryable.Primitive.Parser parser, String queryFragment)
        {
            this._parser = parser;
            this._resultQueryFragment = queryFragment;
        }
    }
}

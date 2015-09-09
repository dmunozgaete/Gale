using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Queryable.OData.Builders
{
    public class KQLConfiguration
    {

        public KQLConfiguration()
        {
            this.offset = 0;
            this.limit = 10;
            this.fields = new List<string>();
            this.filters = new List<Filter>();
        }

        public int offset { get; internal set; }
        public int limit { get; internal set; }

        public List<String> fields { get; internal set; }

        public OrderBy orderBy { get; internal set; }

        public List<Filter> filters { get; private set; }


        public class Filter
        {
            public string field { get; internal set; }
            public string operatorAlias { get; internal set; }
            public string value { get; internal set; }
        }

        public class OrderBy
        {
            public string name { get; internal set; }
            public orderEnum order { get; internal set; }

            public enum orderEnum
            {
                asc = 1,
                desc = 2
            }
        }
    }
}

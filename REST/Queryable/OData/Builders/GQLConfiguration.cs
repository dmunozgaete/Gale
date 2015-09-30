using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Queryable.OData.Builders
{
    public class GQLConfiguration
    {

        public GQLConfiguration()
        {
            this.offset = 0;
            this.limit = 10;
            this.fields = new List<string>();
            this.filters = new List<Filter>();
        }

        public int offset { get; set; }
        public int limit { get; set; }

        public List<String> fields { get; internal set; }

        public OrderBy orderBy { get; set; }

        public List<Filter> filters { get; private set; }


        public class Filter
        {
            public string field { get; set; }
            public string operatorAlias { get; set; }
            public string value { get; set; }

            public override string ToString()
            {
                return String.Format("{0} {1} {2}", this.field, this.operatorAlias, this.value);
            }
        }

        public class OrderBy
        {
            public string name { get; set; }
            public orderEnum order { get; set; }

            public enum orderEnum
            {
                asc = 1,
                desc = 2
            }
        }
    }
}

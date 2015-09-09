using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Queryable.OData.Results
{
    /// <summary>
    /// Convention OData Response
    /// </summary>
    public abstract class Result
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
        public string elapsedTime { get; set; }
        public List<Object> items { get; set; }

        public Result()
        {
        }
    }
}

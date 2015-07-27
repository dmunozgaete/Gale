using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Queryable.Primitive
{
    public interface IResponse
    {
        object toPlainObject();
    }
}

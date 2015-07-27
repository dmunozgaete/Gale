using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Queryable.Primitive.Mapping
{
    public class ModelAttribute : Attribute
    {
        private Type _modelType;

        public Type ModelType
        {
            get { return _modelType; }
        }


        public ModelAttribute(Type ModelType)
        {
            _modelType = ModelType;
        }

    }
}

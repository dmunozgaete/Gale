using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Blueprint.Builders
{
    /// <summary>
    /// Agnostic Database Builder for a Creation Process
    /// </summary>
    public abstract class CreateBuilder : Gale.REST.Http.HttpBaseActionResult
    {
        Type _modelType;

        /// <summary>
        /// Model Type
        /// </summary>
        public Type modelType
        {
            get
            {

                return _modelType;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="modelType">Model Type from the BluePrint Controller</param>
        public CreateBuilder(Type modelType)
        {
            _modelType = modelType;
        }

    }
}

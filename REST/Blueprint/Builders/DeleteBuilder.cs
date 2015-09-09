using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Blueprint.Builders
{
    /// <summary>
    /// Agnostic Database Builder for a Delete Process
    /// </summary>
    public abstract class DeleteBuilder : Gale.REST.Http.HttpBaseActionResult
    {
        private Type _modelType;
        private string _id;

        /// <summary>
        /// Identifier Record in the Database
        /// </summary>
        public String id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Model Type associated with the request
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
        /// <param name="id">Identifier Record in the Database</param>
        /// <param name="modelType">Model Type from the BluePrint Controller</param>
        public DeleteBuilder(string id, Type modelType)
        {
            _modelType = modelType;
            _id = id;
        }

    }
}

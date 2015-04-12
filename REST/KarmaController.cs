using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Karma.REST.Http.Filters;

namespace Karma.REST
{
    /// <summary>
    /// Create a fully Operational Controller for a Model
    /// </summary>
    /// <typeparam name="TModel">Model Class</typeparam>
    [ExceptionFilter]
    public class KarmaController<TModel> : System.Web.Http.ApiController where TModel : class
    {

        /// <summary>
        /// Get Data from the model with Odata Conventions (Queryable)
        /// </summary>
        /// <response code="201">Created</response>
        /// <response code="500">Internal Server Error</response>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]  //Avoid inling for reflection
        public virtual IHttpActionResult Get()
        {
            //string currentMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            //---------------------------------------------------------------------------
            // Get Methods from Caller Request
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame sf = st.GetFrame(1);
            string inheritedMethodName = sf.GetMethod().Name;
            //---------------------------------------------------------------------------
            
            Type ModelType = typeof(TModel);
            var attribute = this.GetType().GetMethod(inheritedMethodName).GetCustomAttributes(typeof(Karma.REST.Queryable.Primitive.Mapping.ModelAttribute), true).FirstOrDefault();

            //OVERRIDE THE CURRENT MODEL??
            if (attribute != null)
            {
                ModelType = (attribute as Karma.REST.Queryable.Primitive.Mapping.ModelAttribute).ModelType;
            }

            Type queryable_result = typeof(Karma.REST.Queryable.Blueprint.QueryableResult<>).MakeGenericType(ModelType);

            return (IHttpActionResult)Activator.CreateInstance(queryable_result, new object[] { Request});
        }

        /// <summary>
        /// Insert a record in the DB for the TModel
        /// </summary>
        /// <param name="payload">JSON data for the specified entity</param>
        /// <response code="201">Created</response>
        /// <response code="500">Internal Server Error</response>
        public virtual IHttpActionResult Post([FromBody]Newtonsoft.Json.Linq.JToken payload)
        {
            return new Karma.REST.Blueprint.CreateResult<TModel>(payload);
        }

        /// <summary>
        /// Update a record in the DB for the TModel
        /// </summary>
        /// <param name="id">identifier of the record to update</param>
        /// <param name="payload">JSON data for the specified entity</param>
        /// <response code="206">Updated</response>
        /// <response code="500">Internal Server Error</response>
        public virtual IHttpActionResult Put([FromUri]string id, [FromBody]Newtonsoft.Json.Linq.JToken payload)
        {
            return new Karma.REST.Blueprint.UpdatedResult<TModel>(id, payload);
        }

        /// <summary>
        /// Delete a record in the DB for the TModel
        /// </summary>
        /// <param name="id">identifier of the record to delete</param>
        /// <response code="200">Deleted</response>
        /// <response code="500">Internal Server Error</response>
        public virtual IHttpActionResult Delete(string id)
        {
            return new Karma.REST.Blueprint.DeleteResult<TModel>(id);
        }

    }
}

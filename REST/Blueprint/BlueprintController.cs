using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Gale.REST.Http.Filters;

namespace Gale.REST.Blueprint
{
    /// <summary>
    /// Create a fully Operational Controller for a Model
    /// </summary>
    /// <typeparam name="TModel">Model Class</typeparam>
    [ExceptionFilter]
    public class BlueprintController<TModel> : System.Web.Http.ApiController where TModel : class
    {
        #region Caching Builder Types for Perfomance
        private static Type _createBuilderType;
        private static Type _readBuilderType;
        private static Type _updateBuilderType;
        private static Type _deleteBuilderType;

        /// <summary>
        /// Find the Create Builder Type associated to the Database Factory Type
        /// </summary>
        private static Type CreateBuilderType
        {
            get
            {
                if (_createBuilderType == null)
                {
                    var types = Gale.Db.Factories.FactoryTarget.GetTypesByDatabaseTarget<Gale.REST.Blueprint.Builders.CreateBuilder>();
                    _createBuilderType = types.FirstOrDefault();
                }

                //Check if Builder Exist!
                Gale.Exception.RestException.Guard(() => _createBuilderType == null, "CREATEBUILDER_NOTIMPLEMENTED_FOR_CURRENT_DATABASE_TYPE", Gale.Exception.Errors.ResourceManager);

                return _createBuilderType;
            }
        }

        /// <summary>
        /// Find the Read Builder Type associated to the Database Factory Type
        /// </summary>
        private static Type ReadBuilderType
        {
            get
            {
                if (_readBuilderType == null)
                {
                    var types = Gale.Db.Factories.FactoryTarget.GetTypesByDatabaseTarget<Gale.REST.Blueprint.Builders.ReadBuilder>();
                    _readBuilderType = types.FirstOrDefault();
                }

                //Check if Builder Exist!
                Gale.Exception.RestException.Guard(() => _readBuilderType == null, "READBUILDER_NOTIMPLEMENTED_FOR_CURRENT_DATABASE_TYPE", Gale.Exception.Errors.ResourceManager);

                return _readBuilderType;
            }
        }

        /// <summary>
        /// Find the Updae Builder Type associated to the Database Factory Type
        /// </summary>
        private static Type UpdateBuilderType
        {
            get
            {
                if (_updateBuilderType == null)
                {
                    var types = Gale.Db.Factories.FactoryTarget.GetTypesByDatabaseTarget<Gale.REST.Blueprint.Builders.UpdateBuilder>();
                    _updateBuilderType = types.FirstOrDefault();
                }
                //Check if Builder Exist!

                Gale.Exception.RestException.Guard(() => _updateBuilderType == null, "UPDATEBUILDER_NOTIMPLEMENTED_FOR_CURRENT_DATABASE_TYPE", Gale.Exception.Errors.ResourceManager);

                return _updateBuilderType;
            }
        }

        /// <summary>
        /// Find the Delete Builder Type associated to the Database Factory Type
        /// </summary>
        private static Type DeleteBuilderType
        {
            get
            {
                if (_deleteBuilderType == null)
                {
                    var types = Gale.Db.Factories.FactoryTarget.GetTypesByDatabaseTarget<Gale.REST.Blueprint.Builders.DeleteBuilder>();
                    _deleteBuilderType = types.FirstOrDefault();
                }

                //Check if Builder Exist!
                Gale.Exception.RestException.Guard(() => _deleteBuilderType == null, "DELETEBUILDER_NOTIMPLEMENTED_FOR_CURRENT_DATABASE", Gale.Exception.Errors.ResourceManager);

                return _deleteBuilderType;
            }
        }
        #endregion


        /// <summary>
        /// Get Data from the model with Odata Conventions (Queryable)
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]  //Avoid inling for reflection
        [Swashbuckle.Swagger.Annotations.SwaggerResponseRemoveDefaults()]
        [Swashbuckle.Swagger.Annotations.SwaggerResponse(System.Net.HttpStatusCode.OK, type: typeof(Gale.REST.Queryable.OData.Results.Result))]
        [Swashbuckle.Swagger.Annotations.SwaggerResponse(System.Net.HttpStatusCode.BadRequest, type: typeof(Gale.Exception.RestException.ErrorContent))]
        [Swashbuckle.Swagger.Annotations.SwaggerResponse(System.Net.HttpStatusCode.InternalServerError, type: typeof(Gale.Exception.RestException.ErrorContent))]
        [Swashbuckle.Swagger.Annotations.SwaggerOperationFilter(typeof(Gale.REST.Config.SwashBuckleExtension.Filters.AddBlueprintDocumentation))]
        public virtual IHttpActionResult Get()
        {
            //---------------------------------------------------------------------------
            // Get Methods from Caller Request
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame sf = st.GetFrame(1);
            string inheritedMethodName = sf.GetMethod().Name;
            //---------------------------------------------------------------------------

            Type ModelType = typeof(TModel);
            if (this.GetType().GetMethod(inheritedMethodName) != null)
            {
                var attribute = this.GetType().GetMethod(inheritedMethodName).GetCustomAttributes(typeof(Gale.REST.Queryable.Primitive.Mapping.ModelAttribute), true).FirstOrDefault();

                //OVERRIDE THE CURRENT MODEL??
                if (attribute != null)
                {
                    ModelType = (attribute as Gale.REST.Queryable.Primitive.Mapping.ModelAttribute).ModelType;
                }
            }

            var builder = ReadBuilderType.MakeGenericType(new Type[] { typeof(TModel) });
            return (IHttpActionResult)Activator.CreateInstance(builder, new object[] { Request });
        }

        /// <summary>
        /// Insert a record in the DB for the TModel
        /// </summary>
        /// <param name="payload">JSON data for the specified entity</param>
        [Swashbuckle.Swagger.Annotations.SwaggerResponseRemoveDefaults()]
        [Swashbuckle.Swagger.Annotations.SwaggerResponse(System.Net.HttpStatusCode.Created)]
        [Swashbuckle.Swagger.Annotations.SwaggerResponse(System.Net.HttpStatusCode.BadRequest, type: typeof(Gale.Exception.RestException.ErrorContent))]
        [Swashbuckle.Swagger.Annotations.SwaggerResponse(System.Net.HttpStatusCode.InternalServerError, type: typeof(Gale.Exception.RestException.ErrorContent))]
        [Swashbuckle.Swagger.Annotations.SwaggerOperationFilter(typeof(Gale.REST.Config.SwashBuckleExtension.Filters.AddBlueprintDocumentation))]
        public virtual IHttpActionResult Post([FromBody]TModel payload)
        {
            var builder = CreateBuilderType.MakeGenericType(new Type[] { typeof(TModel) });
            return (IHttpActionResult)Activator.CreateInstance(builder, new object[] { payload });
        }

        /// <summary>
        /// Update a record in the DB for the TModel
        /// </summary>
        /// <param name="id">identifier of the record to update</param>
        /// <param name="payload">JSON data for the specified entity</param>
        [Swashbuckle.Swagger.Annotations.SwaggerResponseRemoveDefaults()]
        [Swashbuckle.Swagger.Annotations.SwaggerResponse(System.Net.HttpStatusCode.PartialContent)]
        [Swashbuckle.Swagger.Annotations.SwaggerResponse(System.Net.HttpStatusCode.BadRequest, type: typeof(Gale.Exception.RestException.ErrorContent))]
        [Swashbuckle.Swagger.Annotations.SwaggerResponse(System.Net.HttpStatusCode.InternalServerError, type: typeof(Gale.Exception.RestException.ErrorContent))]
        [Swashbuckle.Swagger.Annotations.SwaggerOperationFilter(typeof(Gale.REST.Config.SwashBuckleExtension.Filters.AddBlueprintDocumentation))]
        public virtual IHttpActionResult Put([FromUri]string id, [FromBody]TModel payload)
        {
            var builder = UpdateBuilderType.MakeGenericType(new Type[] { typeof(TModel) });
            return (IHttpActionResult)Activator.CreateInstance(builder, new object[] { id, payload });
        }

        /// <summary>
        /// Delete a record in the DB for the TModel
        /// </summary>
        /// <param name="id">identifier of the record to delete</param>
        [Swashbuckle.Swagger.Annotations.SwaggerResponseRemoveDefaults()]
        [Swashbuckle.Swagger.Annotations.SwaggerResponse(System.Net.HttpStatusCode.OK)]
        [Swashbuckle.Swagger.Annotations.SwaggerResponse(System.Net.HttpStatusCode.InternalServerError, type: typeof(Gale.Exception.RestException.ErrorContent))]
        [Swashbuckle.Swagger.Annotations.SwaggerOperationFilter(typeof(Gale.REST.Config.SwashBuckleExtension.Filters.AddBlueprintDocumentation))]
        public virtual IHttpActionResult Delete(string id)
        {
            var builder = DeleteBuilderType.MakeGenericType(new Type[] { typeof(TModel) });
            return (IHttpActionResult)Activator.CreateInstance(builder, new object[] { id });
        }

    }
}

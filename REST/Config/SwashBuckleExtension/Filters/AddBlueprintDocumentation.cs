using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Config.SwashBuckleExtension.Filters
{
    /// <summary>
    /// The Documentation of the baseClass Blueprint controller in the "User Controller" don't replicate
    /// </summary>
    class AddBlueprintDocumentation : Swashbuckle.Swagger.IOperationFilter
    {
        public void Apply(Swashbuckle.Swagger.Operation operation, Swashbuckle.Swagger.SchemaRegistry schemaRegistry, System.Web.Http.Description.ApiDescription apiDescription)
        {
            #region Add Documentation Nodes
            string TModel = null;
            bool isBlueprintEndpoint = false;


            Type BlueprintController = apiDescription.ActionDescriptor.ControllerDescriptor.ControllerType.BaseType;
            if (BlueprintController.IsGenericType)
            {
                Type modelType = BlueprintController.GetGenericArguments()[0];
                TModel = modelType.Name;

                //Remove all Auto DB generated Properties
                CleanModel(schemaRegistry, modelType);

                isBlueprintEndpoint = true;

            }

            //Has Queryable Endpoint Mark??
            if (apiDescription.ActionDescriptor.GetCustomAttributes<Swashbuckle.Swagger.Annotations.QueryableEndpoint>().Any())
            {
                var attr = apiDescription.ActionDescriptor.GetCustomAttributes<Swashbuckle.Swagger.Annotations.QueryableEndpoint>().First();

                if (attr.QueryableType != null)
                {
                    var EnumerableType = attr.QueryableType;

                    //Remove all Auto DB generated Properties
                    if (!typeof(System.Collections.IEnumerable).IsAssignableFrom(EnumerableType))
                    {
                        //Convert to List (Queryable ALWAYS Return a List)
                        EnumerableType = typeof(List<>).MakeGenericType(attr.QueryableType);
                    }

                    CleanModel(schemaRegistry, EnumerableType);
                    operation.responses.Clear();

                    var statusCode = "200";
                    operation.responses[statusCode] = new Swashbuckle.Swagger.Response
                    {
                        schema = (EnumerableType != null) ? schemaRegistry.GetOrRegister(EnumerableType) : null
                    };
                }

                isBlueprintEndpoint = true;
            }



            if (!isBlueprintEndpoint)
            {
                return;
            }

            if (apiDescription.HttpMethod == System.Net.Http.HttpMethod.Get)
            {
                operation.summary = String.Format(Gale.REST.Resources.SwasbuckleExtension_Blueprint_GET, TModel);
                operation.description = String.Format(Gale.REST.Resources.SwasbuckleExtension_Blueprint_GET_ImplementationNotes, Gale.REST.Resources.GALE_DOCS_SITE);

                #region OData Parameter's
                operation.parameters = new List<Swashbuckle.Swagger.Parameter>();
                operation.parameters.Add(new Swashbuckle.Swagger.Parameter()
                {
                    name = "$select",
                    description = "Fields selector (comma separated)",
                    @in = "query",
                    required = false,
                    type = "string"
                });

                operation.parameters.Add(new Swashbuckle.Swagger.Parameter()
                {
                    name = "$filter",
                    description = "collection of filter's (comma separated): {field} {operator} {value}",
                    @in = "query",
                    required = false,
                    type = "string"
                });

                operation.parameters.Add(new Swashbuckle.Swagger.Parameter()
                {
                    name = "$orderBy",
                    description = "Order by clause: {field} (asc|desc)",
                    @in = "query",
                    required = false,
                    type = "string"
                });

                operation.parameters.Add(new Swashbuckle.Swagger.Parameter()
                {
                    name = "$limit",
                    description = "Limit the number of records returned",
                    @in = "query",
                    required = false,
                    type = "number"
                });

                operation.parameters.Add(new Swashbuckle.Swagger.Parameter()
                {
                    name = "$offset",
                    description = "Skip records before returning anything",
                    @in = "query",
                    required = false,
                    type = "number"
                });
                #endregion
            }
            else if (apiDescription.HttpMethod == System.Net.Http.HttpMethod.Post)
            {
                operation.summary = String.Format(Gale.REST.Resources.SwasbuckleExtension_Blueprint_POST, TModel); ;
                operation.description = String.Format(Gale.REST.Resources.SwasbuckleExtension_Blueprint_POST_ImplementationNotes, TModel);
            }
            else if (apiDescription.HttpMethod == System.Net.Http.HttpMethod.Put)
            {
                operation.summary = String.Format(Gale.REST.Resources.SwasbuckleExtension_Blueprint_PUT, TModel); ;
            }
            else if (apiDescription.HttpMethod == System.Net.Http.HttpMethod.Delete)
            {
                operation.summary = String.Format(Gale.REST.Resources.SwasbuckleExtension_Blueprint_DELETE, TModel); ;
            }
            #endregion
        }

        /// <summary>
        /// Remove 
        /// </summary>
        /// <param name="schemaRegistry"></param>
        private void CleanModel(Swashbuckle.Swagger.SchemaRegistry schemaRegistry, Type TModel)
        {
            if (schemaRegistry.Definitions.Any((definition) => definition.Key == TModel.Name))
            {
                //Remove All Auto-Generated DB Properties
                var swaggerDefinition = schemaRegistry.Definitions[TModel.Name];
                var fieldProperties = TModel.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(System.Data.Linq.Mapping.ColumnAttribute))).ToList();
                foreach (System.Reflection.PropertyInfo property in fieldProperties)
                {
                    var attr = property.TryGetAttribute<System.Data.Linq.Mapping.ColumnAttribute>();

                    //Only if Auto DB Generated Key
                    if (attr.IsDbGenerated)
                    {
                        if (swaggerDefinition.properties.Any((prop) => prop.Key == property.Name))
                        {
                            swaggerDefinition.properties.Remove(property.Name);
                        }
                    }

                }
            }
        }
    }

}

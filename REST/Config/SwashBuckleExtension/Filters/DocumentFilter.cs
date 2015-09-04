using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gale.REST.Config.SwashBuckleExtension.Filters
{
    /// <summary>
    /// Documentation Swagger Fixing's
    /// </summary>
    internal class DocumentFilter: Swashbuckle.Swagger.IDocumentFilter
    {
        public void Apply(Swashbuckle.Swagger.SwaggerDocument swaggerDoc, Swashbuckle.Swagger.SchemaRegistry schemaRegistry, System.Web.Http.Description.IApiExplorer apiExplorer)
        {
           //Fix some Documentation Problem's??
        }
    }
}

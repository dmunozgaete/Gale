using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace Gale.REST.Http.Binding
{
    /// <summary>
    /// Bind a specific Header Name to a Parameter Name
    /// </summary>
    public class FromHeaderBinding : HttpParameterBinding
    {
        private string name;

        public FromHeaderBinding(HttpParameterDescriptor parameter, string headerName)
            : base(parameter)
        {
            if (string.IsNullOrEmpty(headerName))
            {
                throw new ArgumentNullException("Header Name");
            }

            this.name = headerName;
        }

        public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            IEnumerable<string> values;
            if (actionContext.Request.Headers.TryGetValues(this.name, out values))
            {
                actionContext.ActionArguments[this.Descriptor.ParameterName] = values.FirstOrDefault();
            }
            else
            {
                //Always bind a parameter, at least  null ;)
                actionContext.ActionArguments.Add(this.Descriptor.ParameterName, this.Descriptor.DefaultValue);
            }

            var taskSource = new TaskCompletionSource<object>();
            taskSource.SetResult(null);
            return taskSource.Task;
        }
    }
}

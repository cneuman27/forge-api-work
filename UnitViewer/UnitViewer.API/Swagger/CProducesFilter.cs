using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ApiExplorer;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UnitViewer.API.Swagger
{
    public class CProducesFilter : IOperationFilter
    {
        public void Apply(
            Operation op,
            OperationFilterContext ctx)
        {
            SwaggerProducesAttribute attr;

            attr = ctx.ControllerActionDescriptor.GetControllerAndActionAttributes(true)
                .OfType<SwaggerProducesAttribute>()
                .FirstOrDefault();

            if (attr == null) return;

            op.Produces.Clear();
            op.Produces = attr.ContentTypes.ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ApiExplorer;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UnitViewer.API.Swagger
{
    public class CConsumesFilter : IOperationFilter
    {
        public void Apply(
            Operation op, 
            OperationFilterContext ctx)
        {
            SwaggerConsumesAttribute attr;
            
            attr = ctx.ControllerActionDescriptor.GetControllerAndActionAttributes(true)
                .OfType<SwaggerConsumesAttribute>()
                .FirstOrDefault();

            if (attr == null) return;

            op.Consumes.Clear();
            op.Consumes = attr.ContentTypes.ToList();
        }
    }
}

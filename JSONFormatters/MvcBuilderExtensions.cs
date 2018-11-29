//
// Much help from:
// https://stackoverflow.com/questions/44828302/asp-net-core-api-json-serializersettings-per-request
// http://rovani.net/Explicit-Model-Constructor/
//

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace JSONFormatters
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddExpandedModelJSONFormatters(this IMvcBuilder me)
        {
            ServiceDescriptor descriptor;

            descriptor = ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, Expanded.CMVCOptionsSetup>();
          
            me.Services.TryAddEnumerable(descriptor);

            return me;
        }
        public static IMvcBuilder AddMinifiedModelJSONFormatters(this IMvcBuilder me)
        {
            ServiceDescriptor descriptor;

            descriptor = ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, Minified.CMVCOptionsSetup>();

            me.Services.TryAddEnumerable(descriptor);

            return me;
        }
    }
}

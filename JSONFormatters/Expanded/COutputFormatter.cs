//
// Much help from:
// https://stackoverflow.com/questions/44828302/asp-net-core-api-json-serializersettings-per-request
// http://rovani.net/Explicit-Model-Constructor/
//

using System;
using System.Buffers;

using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

using Newtonsoft.Json;

namespace JSONFormatters.Expanded
{
    public class COutputFormatter : JsonOutputFormatter
    {
        private static MediaTypeHeaderValue MEDIA_TYPE = MediaTypeHeaderValue.Parse(CConstants.MEDIA_TYPE_NAME__EXPANDED);
        private IServiceProvider m_Provider = null;

        public COutputFormatter(
            IServiceProvider provider,
            JsonSerializerSettings serializerSettings,
            ArrayPool<char> charPool)
            : base(
                  serializerSettings,
                  charPool)
        {
            m_Provider = provider;

            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(MEDIA_TYPE);
        }

        protected override JsonSerializer CreateJsonSerializer()
        {
            JsonSerializer serializer;

            serializer = base.CreateJsonSerializer();

            serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            serializer.TypeNameHandling = TypeNameHandling.Objects;
            serializer.DefaultValueHandling = DefaultValueHandling.Include;
            serializer.NullValueHandling = NullValueHandling.Include;

            serializer.ContractResolver = new Expanded.CContractResolver(m_Provider);

            return serializer;
        }
    }
}

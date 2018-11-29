//
// Much help from:
// https://stackoverflow.com/questions/44828302/asp-net-core-api-json-serializersettings-per-request
// http://rovani.net/Explicit-Model-Constructor/
//

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace JSONFormatters.Minified
{
    public class CMVCOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private IServiceProvider m_Provider; 

        private readonly ILoggerFactory m_LoggerFactory;
        private readonly JsonSerializerSettings m_JSONSerializerSettings;
        private readonly ArrayPool<char> m_CharPool;
        private readonly ObjectPoolProvider m_ObjectPoolProvider;

        public CMVCOptionsSetup(
            IServiceProvider provider,
            ILoggerFactory loggerFactory,
            IOptions<MvcJsonOptions> jsonOptions,
            ArrayPool<char> charPool,
            ObjectPoolProvider poolProvider)
        {
            m_Provider = provider;
            m_LoggerFactory = loggerFactory;
            m_JSONSerializerSettings = jsonOptions.Value.SerializerSettings;
            m_CharPool = charPool;
            m_ObjectPoolProvider = poolProvider;
        }

        public void Configure(MvcOptions options)
        {
            options.InputFormatters.Add(new CInputFormatter(
                m_Provider,
                m_LoggerFactory.CreateLogger<CInputFormatter>(),
                m_JSONSerializerSettings,
                m_CharPool,
                m_ObjectPoolProvider));

            options.OutputFormatters.Add(new COutputFormatter(
                m_Provider,
                m_JSONSerializerSettings,
                m_CharPool));
        }
    }
}

using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json.Serialization;

namespace ForgeAPI.AspNet
{
    public class JSONContractResolver : DefaultContractResolver
    {
        private IServiceProvider m_ServiceProvider = null;

        public JSONContractResolver(
            IServiceProvider provider)
        {
            m_ServiceProvider = provider;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            if (m_ServiceProvider.GetService(objectType) != null)
            {
                JsonObjectContract contract = base.CreateObjectContract(objectType);
                contract.DefaultCreator = () => m_ServiceProvider.GetService(objectType);

                return contract;
            }

            return base.CreateObjectContract(objectType);
        }
    }
}

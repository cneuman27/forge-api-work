using System;
using System.Collections.Generic;
using System.Text;

using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Newtonsoft.Json.Serialization;

namespace ForgeAPI.Autofac
{
    public class JSONContractResolver : DefaultContractResolver
    {
        private ILifetimeScope m_Scope = null;

        public JSONContractResolver(ILifetimeScope scope)
        {
            m_Scope = scope;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            if (m_Scope.IsRegistered(objectType))
            {
                JsonObjectContract contract = ResolveAutofacContract(objectType);
                contract.DefaultCreator = () => m_Scope.Resolve(objectType);

                return contract;
            }

            return base.CreateObjectContract(objectType);
        }

        private JsonObjectContract ResolveAutofacContract(Type objectType)
        {
            IComponentRegistration registration;

            if (m_Scope.ComponentRegistry.TryGetRegistration(new TypedService(objectType), out registration))
            {
                Type viewType = (registration.Activator as ReflectionActivator)?.LimitType;
                if (viewType != null)
                {
                    return base.CreateObjectContract(viewType);
                }
            }

            return base.CreateObjectContract(objectType);
        }
    }
}

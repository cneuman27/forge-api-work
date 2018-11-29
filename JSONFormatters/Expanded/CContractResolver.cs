using System;
using System.Collections.Generic;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JSONFormatters.Expanded
{
    public class CContractResolver : DefaultContractResolver
    {
        private IServiceProvider m_Provider = null;

        public CContractResolver(IServiceProvider provider)
        {
            m_Provider = provider;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            JsonObjectContract contract;
            Type[] interfaces;
            List<Type> interfaceList;

            interfaceList = new List<Type>();
            interfaceList.Add(objectType);

            interfaces = objectType.GetInterfaces();
            interfaceList.AddRange(interfaces);

            contract = base.CreateObjectContract(objectType);

            foreach (Type i in interfaceList)
            {
                if (m_Provider.GetService(i) != null)
                {
                    contract.DefaultCreator = () => m_Provider.GetService(i);
                    break;
                }
            }

            return contract;
        }

        protected override JsonProperty CreateProperty(
            MemberInfo member,
            MemberSerialization memberSerialization)
        {
            JsonProperty prop;

            prop = base.CreateProperty(member, memberSerialization);

            prop.PropertyName = member.Name;
            prop.DefaultValueHandling = DefaultValueHandling.Include;
            prop.ShouldSerialize = null;

            return prop;
        }
    }
}

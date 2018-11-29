using System;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeAPI.AspNet
{
    public class CFactory : ForgeAPI.Interface.IFactory
    {
        private IServiceProvider m_ServiceProvider = null;

        public CFactory(IServiceProvider provider)
        {
            m_ServiceProvider = provider;
        }

        T ForgeAPI.Interface.IFactory.Create<T>()
        {
            T item;

            item = m_ServiceProvider.GetService<T>();

            return item;
        }
        T ForgeAPI.Interface.IFactory.CreateInputs<T>(ForgeAPI.Interface.Authentication.IToken token)
        {
            T item;

            item = m_ServiceProvider.GetService<T>();
            item.AuthenticationToken = token;

            return item;
        }
        T ForgeAPI.Interface.IFactory.CreateOutputs<T>(ForgeAPI.Interface.REST.IResult result)
        {
            T item;

            item = m_ServiceProvider.GetService<T>();
            item.Result = result;

            return item;
        }
    }
}

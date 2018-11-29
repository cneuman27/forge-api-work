using Autofac;

namespace ForgeAPI.Autofac
{
    public class CFactory : ForgeAPI.Interface.IFactory
    {
        private ILifetimeScope m_Scope = null;

        public CFactory(ILifetimeScope scope)
        {   
            m_Scope = scope;
        }

        T ForgeAPI.Interface.IFactory.Create<T>()
        {
            T item;

            item = m_Scope.Resolve<T>();

            return item;
        }
        T ForgeAPI.Interface.IFactory.CreateInputs<T>(ForgeAPI.Interface.Authentication.IToken token)
        {
            T item;

            item = m_Scope.Resolve<T>();
            item.AuthenticationToken = token;

            return item;
        }
        T ForgeAPI.Interface.IFactory.CreateOutputs<T>(ForgeAPI.Interface.REST.IResult result)
        {
            T item;

            item = m_Scope.Resolve<T>();
            item.Result = result;

            return item;
        }
    }
}

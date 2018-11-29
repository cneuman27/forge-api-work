using System;
using System.Collections.Generic;
using System.Text;

using ForgeAPI.Interface.ModelDerivative.Derivatives;

namespace ForgeAPI.Implementation.ModelDerivative.Derivatives
{
    public partial class CService : IService
    {
        private ForgeAPI.Interface.IFactory m_Factory = null;
        private ForgeAPI.Interface.IForgeAPIConfiguration m_Configuration = null;
        private ForgeAPI.Interface.REST.IService m_RESTService = null;
        private ForgeAPI.Interface.Utility.IService m_UtilityService = null;
        private Newtonsoft.Json.Serialization.DefaultContractResolver m_Resolver = null;

        public CService(
            ForgeAPI.Interface.IFactory factory,
            ForgeAPI.Interface.IForgeAPIConfiguration configuration,
            ForgeAPI.Interface.REST.IService restService,
            ForgeAPI.Interface.Utility.IService utilityService,
            Newtonsoft.Json.Serialization.DefaultContractResolver contractResolver)
        {
            m_Factory = factory;
            m_Configuration = configuration;
            m_RESTService = restService;
            m_Resolver = contractResolver;
            m_UtilityService = utilityService;
        }
    }
}

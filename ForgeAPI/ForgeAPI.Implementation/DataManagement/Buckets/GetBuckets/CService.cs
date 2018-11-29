using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using ForgeAPI.Interface.DataManagement.Buckets.GetBuckets;

namespace ForgeAPI.Implementation.DataManagement.Buckets
{
    public partial class CService
    {
        public IOutputs GetBuckets(IInputs inputs)
        {
            IOutputs outputs;
            List<KeyValuePair<string, string>> headers;
            ForgeAPI.Interface.REST.IResult result;

            m_UtilityService.EnsureAuthenticationToken(inputs);

            headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(CConstants.HEADER__AUTHORIZATION, $"Bearer {inputs.AuthenticationToken.AccessToken}")
            };

            result = m_RESTService.Get(
                m_Configuration.APIURI_DataManagement_Buckets_GetBuckets,
                headers);

            outputs = m_Factory.CreateOutputs<IOutputs>(result);

            if (outputs.Success() == false)
            {
                return outputs;
            }

            JsonConvert.PopulateObject(
                result.ResponseData,
                outputs,
                new JsonSerializerSettings()
                {
                    ContractResolver = m_Resolver
                });

            return outputs;
        }
    }
}

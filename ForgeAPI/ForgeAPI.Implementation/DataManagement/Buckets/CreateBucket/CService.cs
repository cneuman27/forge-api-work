using System.Collections.Generic;
using Newtonsoft.Json;
using ForgeAPI.Interface.DataManagement.Buckets.CreateBucket;

namespace ForgeAPI.Implementation.DataManagement.Buckets
{
    public partial class CService
    {
        public IOutputs CreateBucket(IInputs inputs)
        {
            IOutputs outputs;
            List<KeyValuePair<string, string>> headers;
            ForgeAPI.Interface.REST.IResult result;

            m_UtilityService.EnsureAuthenticationToken(inputs);

            headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(CConstants.HEADER__CONTENT_TYPE, CConstants.MEDIA_TYPE__JSON),
                new KeyValuePair<string, string>(CConstants.HEADER__AUTHORIZATION, $"Bearer {inputs.AuthenticationToken.AccessToken}")
            };

            result = m_RESTService.Post(
                m_Configuration.APIURI_DataManagement_Buckets_CreateBucket,
                inputs,
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

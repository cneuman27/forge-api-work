using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob;

namespace ForgeAPI.Implementation.ModelDerivative.Derivatives
{
    public partial class CService 
    {
        public IOutputs AddJob(IInputs inputs)
        {
            IOutputs outputs;
            List<KeyValuePair<string, string>> headers;
            ForgeAPI.Interface.REST.IResult result;
           
            m_UtilityService.EnsureAuthenticationToken(inputs);

            headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(CConstants.HEADER__AUTHORIZATION, $"Bearer {inputs.AuthenticationToken.AccessToken}"),
                new KeyValuePair<string, string>(CConstants.HEADER__CONTENT_TYPE, CConstants.MEDIA_TYPE__JSON),
                new KeyValuePair<string, string>(CConstants.HEADER__ADS_FORCE, inputs.ReplaceExistingDerivatives.ToString().ToLower())
            };

            result = m_RESTService.Post(
                m_Configuration.APIURI_ModelDerivative_Derivatives_PostJob,
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

using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest;

namespace ForgeAPI.Implementation.ModelDerivative.Derivatives
{
    public partial class CService 
    {
        public IOutputs GetManifest(IInputs inputs)
        {
            IOutputs outputs;
            List<KeyValuePair<string, string>> headers;
            ForgeAPI.Interface.REST.IResult result;
            string encodedURN;

            m_UtilityService.EnsureAuthenticationToken(inputs);

            headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(CConstants.HEADER__AUTHORIZATION, $"Bearer {inputs.AuthenticationToken.AccessToken}")
            };

            encodedURN = inputs.URN;

            if (inputs.URNIsEncoded == false)
            {
                encodedURN = m_UtilityService.ConvertToBase64(encodedURN);
            }

            result = m_RESTService.Get(
                m_Configuration.APIURI_ModelDerivative_Derivatives_GetManifest
                    .Replace("{urn}", encodedURN),
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

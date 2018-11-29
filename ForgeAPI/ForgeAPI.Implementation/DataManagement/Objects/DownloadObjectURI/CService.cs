using System;
using System.Collections.Generic;
using System.Text;

using ForgeAPI.Interface.DataManagement.Objects.DownloadObjectURI;

namespace ForgeAPI.Implementation.DataManagement.Objects
{
    public partial class CService
    {
        public IOutputs DownloadObjectURI(IInputs inputs)
        {
            IOutputs outputs;
            List<KeyValuePair<string, string>> headers;
            ForgeAPI.Interface.REST.IResult result;

            m_UtilityService.EnsureAuthenticationToken(inputs);

            headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(CConstants.HEADER__AUTHORIZATION, $"Bearer {inputs.AuthenticationToken.AccessToken}")
            };

            result = m_RESTService.GetBinary(
                inputs.URI,
                headers);

            outputs = m_Factory.CreateOutputs<IOutputs>(result);
            outputs.ObjectData = result.ResponseBinaryData;

            return outputs;
        }
    }
}

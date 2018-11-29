using System;
using System.Collections.Generic;
using System.Text;

using ForgeAPI.Interface.DataManagement.Buckets.DeleteBucket;

namespace ForgeAPI.Implementation.DataManagement.Buckets
{
    public partial class CService
    {
        public IOutputs DeleteBucket(IInputs inputs)
        {
            IOutputs outputs;
            List<KeyValuePair<string, string>> headers;
            ForgeAPI.Interface.REST.IResult result;

            m_UtilityService.EnsureAuthenticationToken(inputs);

            headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(CConstants.HEADER__AUTHORIZATION, $"Bearer {inputs.AuthenticationToken.AccessToken}")
            };

            result = m_RESTService.Delete(
                m_Configuration.APIURI_DataManagement_Buckets_DeleteBucket.Replace("{bucketKey}", inputs.BucketKey),
                headers);

            outputs = m_Factory.CreateOutputs<IOutputs>(result);

            return outputs;
        }
    }
}

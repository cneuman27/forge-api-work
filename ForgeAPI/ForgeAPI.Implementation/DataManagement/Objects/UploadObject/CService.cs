using System.Collections.Generic;
using Newtonsoft.Json;
using ForgeAPI.Interface.DataManagement.Objects.UploadObject;

namespace ForgeAPI.Implementation.DataManagement.Objects
{
    public partial class CService
    {

        public IOutputs UploadObject(IInputs inputs)
        {
            IOutputs outputs;
            List<KeyValuePair<string, string>> headers;
            ForgeAPI.Interface.REST.IResult result;

            m_UtilityService.EnsureAuthenticationToken(inputs);

            headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(CConstants.HEADER__AUTHORIZATION, $"Bearer {inputs.AuthenticationToken.AccessToken}"),
                new KeyValuePair<string, string>(CConstants.HEADER__CONTENT_TYPE, inputs.ContentType),
                new KeyValuePair<string, string>(CConstants.HEADER__CONTENT_LENGTH, inputs.FileData.LongLength.ToString())
            };

            result = m_RESTService.Put(
                m_Configuration.APIURI_DataManagement_Objects_UploadObject
                    .Replace("{bucketKey}", inputs.BucketKey)
                    .Replace("{objectName}", inputs.ObjectName),
                inputs.FileData,
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

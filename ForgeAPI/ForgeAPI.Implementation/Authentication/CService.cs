using System.Collections.Generic;
using Newtonsoft.Json;
using ForgeAPI.Implementation.Extensions;
using ForgeAPI.Interface.Authentication;

namespace ForgeAPI.Implementation.Authentication
{
    public class CService : IService
    {
        private ForgeAPI.Interface.IForgeAPIConfiguration m_Configuration = null;
        private ForgeAPI.Interface.REST.IService m_RESTService = null;

        public CService(
            ForgeAPI.Interface.IForgeAPIConfiguration configuration,
            ForgeAPI.Interface.REST.IService restService)
        {
            m_Configuration = configuration;
            m_RESTService = restService;
        }

        public IToken Authenticate(List<ForgeAPI.Interface.Enums.E_AccessScope> scopes)
        {
            CToken token;
            ForgeAPI.Interface.REST.IResult result;
            List<KeyValuePair<string, string>> formData;
            string scopeString;

            #region Build Scope String

            scopeString = "";

            foreach (ForgeAPI.Interface.Enums.E_AccessScope tmp in scopes)
            {
                scopeString += (tmp.ConvertToJSON() + " ");
            }

            scopeString = scopeString.Trim();

            #endregion

            formData = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", m_Configuration.ClientID),
                new KeyValuePair<string, string>("client_secret", m_Configuration.Secret),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", scopeString)
            };

            result = m_RESTService.PostFormData(
                m_Configuration.APIURI_Authentication,
                formData);

            if (result.IsOK() == false)
            {
                throw new System.Exception(result.ResponseData);
            }

            token = JsonConvert.DeserializeObject<CToken>(result.ResponseData);
            token.AccessScopes = scopes;

            return token;
        }
    }
}

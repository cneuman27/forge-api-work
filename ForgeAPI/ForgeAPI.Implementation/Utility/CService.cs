using System;
using System.Collections.Generic;
using System.Text;

using ForgeAPI.Interface.Utility;

namespace ForgeAPI.Implementation.Utility
{
    public class CService : IService
    {
        private ForgeAPI.Interface.Authentication.IService m_AuthenticationService = null;

        public CService(
            ForgeAPI.Interface.Authentication.IService authService)
        {
            m_AuthenticationService = authService;
        }

        public string ConvertToBase64(string value)
        {
            byte[] bytes;
            string output;

            bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(value);

            output = Convert.ToBase64String(bytes);

            output = output.Split('=')[0];
            output = output.Replace('+', '-');
            output = output.Replace('/', '_');

            return output;
        }
        public void EnsureAuthenticationToken(ForgeAPI.Interface.IInputsCommon inputs)
        {
            if (inputs.AuthenticationToken == null)
            {
                inputs.AuthenticationToken = m_AuthenticationService.Authenticate(
                    CConstants.DEFAULT_ACCESS_SCOPES);
            }

            if (inputs.AuthenticationToken.IsExpired)
            {
                inputs.AuthenticationToken = m_AuthenticationService.Authenticate(
                    inputs.AuthenticationToken.AccessScopes);
            }
        }
    }
}

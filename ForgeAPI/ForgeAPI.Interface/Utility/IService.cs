using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.Utility
{
    public interface IService
    {
        string ConvertToBase64(string value);
        void EnsureAuthenticationToken(IInputsCommon inputs);
    }
}

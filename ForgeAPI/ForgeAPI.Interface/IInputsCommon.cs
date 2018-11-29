using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface
{
    public interface IInputsCommon
    {
        ForgeAPI.Interface.Authentication.IToken AuthenticationToken
        {
            get;
            set;
        }
    }
}

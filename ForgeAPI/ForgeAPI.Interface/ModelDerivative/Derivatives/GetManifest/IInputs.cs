using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest
{
    public interface IInputs : IInputsCommon
    {
        string URN
        {
            get;
            set;
        }

        bool URNIsEncoded
        {
            get;
            set;
        }
    }
}

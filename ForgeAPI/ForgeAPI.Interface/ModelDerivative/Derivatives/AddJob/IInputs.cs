using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob
{
    public interface IInputs : IInputsCommon
    {
        bool ReplaceExistingDerivatives
        {
            get;
            set;
        }

        IInputDefinition Input
        {
            get;
            set;
        }
        IOutputDefinition Output
        {
            get;
            set;
        }
    }
}

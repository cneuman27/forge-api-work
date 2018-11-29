using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob
{
    public interface IOutputs : IOutputsCommon
    {
        string APIStatus
        {
            get;
            set;
        }

        string URN
        {
            get;
            set;
        }
    }
}

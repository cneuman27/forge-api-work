using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob
{
    public interface IInputDefinition
    {
        string URNEncoded
        {
            get;
            set;
        }

        bool IsCompressed
        {
            get;
            set;
        }

        string CompressedRootFileName
        {
            get;
            set;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob
{
    public interface IOutputFormat
    {
        Enums.E_OutputFormatType Type
        {
            get;
            set;
        }
    }
}

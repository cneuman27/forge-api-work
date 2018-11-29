using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob
{
    public interface IDestinationSettings
    {
        Enums.E_Region Region
        {
            get;
            set;
        }
    }
}

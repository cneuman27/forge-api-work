using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob
{
    public interface IOutputDefinition
    {
        IDestinationSettings DestinationSettings
        {
            get;
            set;
        }
        List<IOutputFormat> FormatList
        {
            get;
            set;
        }
    }
}

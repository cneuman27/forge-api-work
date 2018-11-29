using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob
{
    public interface IOutputFormat_SVF : IOutputFormat
    {
        List<Enums.E_SVFViewType> ViewList
        {
            get;
            set;
        }
    }
}

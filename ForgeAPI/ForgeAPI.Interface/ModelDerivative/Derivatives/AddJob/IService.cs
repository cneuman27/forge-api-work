using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.ModelDerivative.Derivatives
{
    public partial interface IService
    {
        AddJob.IOutputs AddJob(AddJob.IInputs inputs);
    }
}

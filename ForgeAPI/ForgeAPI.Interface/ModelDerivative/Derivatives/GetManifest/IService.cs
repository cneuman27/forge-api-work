using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.ModelDerivative.Derivatives
{
    public partial interface IService
    {
        GetManifest.IOutputs GetManifest(GetManifest.IInputs inputs);
    }
}

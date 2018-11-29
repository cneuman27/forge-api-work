using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Buckets
{
    public partial interface IService
    {
        GetBuckets.IOutputs GetBuckets(GetBuckets.IInputs inputs);
    }
}

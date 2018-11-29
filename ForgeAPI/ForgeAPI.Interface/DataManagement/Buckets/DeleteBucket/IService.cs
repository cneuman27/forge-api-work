using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Buckets
{
    public partial interface IService
    {
        DeleteBucket.IOutputs DeleteBucket(DeleteBucket.IInputs inputs);
    }
}

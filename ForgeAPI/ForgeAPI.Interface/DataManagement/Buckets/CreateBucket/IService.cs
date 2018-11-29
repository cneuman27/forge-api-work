using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Buckets
{
    public partial interface IService
    {
        CreateBucket.IOutputs CreateBucket(CreateBucket.IInputs input);
    }
}

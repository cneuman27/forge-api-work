using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Buckets.DeleteBucket
{
    public interface IInputs : IInputsCommon
    {
        string BucketKey
        {
            get;
            set;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Buckets.GetBuckets
{
    public interface IInputs : IInputsCommon
    {
        int Limit
        {
            get;
            set;
        }

        string PaginationOffsetBucketKey
        {
            get;
            set;
        }
    }
}

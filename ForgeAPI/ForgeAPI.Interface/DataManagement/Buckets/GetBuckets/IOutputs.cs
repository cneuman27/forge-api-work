using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Buckets.GetBuckets
{
    public interface IOutputs : IOutputsCommon
    {
        List<IBucket> BucketList
        {
            get;
            set;
        }

        string NextPageURI
        {
            get;
            set;
        }
    }
}

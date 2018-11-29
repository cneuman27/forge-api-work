using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Buckets.GetBuckets
{
    public interface IBucket
    {
        string BucketKey
        {
            get;
            set;
        }

        long CreateDateEpoch
        {
            get;
            set;
        }

        Enums.E_RetentionPolicy RetentionPolicy
        {
            get;
            set;
        }
    }
}

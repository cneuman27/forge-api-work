using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Buckets.CreateBucket
{
    public interface IOutputs : IOutputsCommon
    {
        string BucketKey
        {
            get;
            set;
        }

        string BucketOwner
        {
            get;
            set;
        }

        long CreateDateEpoch
        {
            get;
            set;
        }

        List<IPermissions> Permissions
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

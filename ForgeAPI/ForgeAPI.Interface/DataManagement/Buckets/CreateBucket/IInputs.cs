using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Buckets.CreateBucket
{
    public interface IInputs : IInputsCommon
    {
        string BucketKey
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

using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Buckets.CreateBucket
{
    public interface IPermissions
    {
        string ApplicationID
        {
            get;
        }

        Enums.E_AccessType AccessType
        {
            get;
        }
    }
}

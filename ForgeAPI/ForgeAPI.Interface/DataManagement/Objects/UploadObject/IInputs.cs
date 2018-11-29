using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Objects.UploadObject
{
    public interface IInputs : IInputsCommon
    {
        string FileName
        {
            get;
            set;
        }

        byte[] FileData
        {
            get;
            set;
        }

        string ContentType
        {
            get;
            set;
        }

        string BucketKey
        {
            get;
            set;
        }

        string ObjectName
        {
            get;
            set;
        }
    }
}

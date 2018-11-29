using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Objects.UploadObject
{
    public interface IOutputs : IOutputsCommon
    {
        string BucketKey
        {
            get;
            set;
        }

        string ObjectID
        {
            get;
            set;
        }

        string ObjectKey
        {
            get;
            set;
        }

        string SHA1
        {
            get;
            set;
        }

        long ObjectSize
        {
            get;
            set;
        }

        string ContentType
        {
            get;
            set;
        }
        
        string DownloadURI
        {
            get;
            set;
        }
    }
}

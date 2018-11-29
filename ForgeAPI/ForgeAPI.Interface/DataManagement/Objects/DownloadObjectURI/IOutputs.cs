using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Objects.DownloadObjectURI
{
    public interface IOutputs : IOutputsCommon
    {
        byte[] ObjectData
        {
            get;
            set;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Objects
{
    public partial interface IService
    {
        DownloadObjectURI.IOutputs DownloadObjectURI(DownloadObjectURI.IInputs inputs);
    }
}

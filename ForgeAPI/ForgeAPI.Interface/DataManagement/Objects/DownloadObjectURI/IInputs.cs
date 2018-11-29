using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Objects.DownloadObjectURI
{
    public interface IInputs : IInputsCommon
    {
        string URI
        {
            get;
            set;
        }
    }
}

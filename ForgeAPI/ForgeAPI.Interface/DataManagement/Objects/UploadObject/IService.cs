using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.DataManagement.Objects
{
    public partial interface IService
    {
        UploadObject.IOutputs UploadObject(UploadObject.IInputs inputs);
    }
}

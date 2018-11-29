using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface
{
    public interface IOutputsCommon
    {
        ForgeAPI.Interface.REST.IResult Result
        {
            get;
            set;
        }

        bool Success();
        string FailureReason();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest
{
    public interface IMessage
    {
        string Type
        {
            get;
            set;
        }

        string Code
        {
            get;
            set;
        }

        string Message
        {
            get;
            set;
        }

    }
}

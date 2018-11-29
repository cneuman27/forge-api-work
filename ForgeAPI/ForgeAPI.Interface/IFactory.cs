using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface
{
    public interface IFactory
    {
        T Create<T>() where T : class;
        T CreateInputs<T>(ForgeAPI.Interface.Authentication.IToken token) where T : class, IInputsCommon;
        T CreateOutputs<T>(ForgeAPI.Interface.REST.IResult result) where T : class, IOutputsCommon;
    }
}

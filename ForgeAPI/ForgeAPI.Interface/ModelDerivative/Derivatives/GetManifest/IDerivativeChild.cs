using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest
{
    public interface IDerivativeChild
    {
        Guid ID
        {
            get;
            set;
        }

        string DerivativeType
        {
            get;
            set;
        }

        string Role
        {
            get;
            set;
        }

        string MIMEType
        {
            get;
            set;
        }

        string URN
        {
            get;
            set;
        }

        string Progress
        {
            get;
            set;
        }

        Enums.E_TranslationStatus Status
        {
            get;
            set;
        }

        List<IMessage> MessageList
        {
            get;
            set;
        }

        List<IDerivativeChild> Children
        {
            get;
            set;
        }
    }
}

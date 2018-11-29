using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest
{
    public interface IDerivative
    {
        string Name
        {
            get;
            set;
        }

        bool HasThumbnail
        {
            get;
            set;
        }

        Enums.E_OutputFormatType OutputFormat
        {
            get;
            set;
        }

        Enums.E_TranslationStatus Status
        {
            get;
            set;
        }

        string Progress
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

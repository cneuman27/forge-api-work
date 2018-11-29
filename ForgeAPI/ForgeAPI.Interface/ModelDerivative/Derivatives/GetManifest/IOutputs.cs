using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest
{
    public interface IOutputs : IOutputsCommon
    {
        string URN
        {
            get;
            set;
        }

        Enums.E_Region Region
        {
            get;
            set;
        }

        string Type
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

        bool HasThumbnail
        {
            get;
            set;
        }

        List<IDerivative> DerivativeList
        {
            get;
            set;
        }
    }
}

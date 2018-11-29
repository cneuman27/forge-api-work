using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest;

namespace ForgeAPI.Implementation.ModelDerivative.Derivatives.GetManifest
{
    public class COutputs : COutputsCommon, IOutputs
    {
        private string m_URN = "";
        private ForgeAPI.Interface.Enums.E_Region m_Region = Interface.Enums.E_Region.Undefined;
        private string m_Type = "";
        private string m_Progress = "";
        private ForgeAPI.Interface.Enums.E_TranslationStatus m_Status = Interface.Enums.E_TranslationStatus.Undefined;
        private bool m_HasThumbnail = false;
        private List<IDerivative> m_DerivativeList = new List<IDerivative>();

        public COutputs()
        {
        }

        [JsonProperty("urn")]
        public string URN
        {
            get { return m_URN; }
            set { m_URN = value; }
        }

        [JsonProperty("region")]
        [JsonConverter(typeof(JSONEnumConverters.CJSONConverter_Region))]
        public ForgeAPI.Interface.Enums.E_Region Region
        {
            get { return m_Region; }
            set { m_Region = value; }
        }

        [JsonProperty("type")]
        public string Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        [JsonProperty("progress")]
        public string Progress
        {
            get { return m_Progress; }
            set { m_Progress = value; }
        }

        [JsonProperty("status")]
        [JsonConverter(typeof(JSONEnumConverters.CJSONConverter_TranslationStatus))]
        public ForgeAPI.Interface.Enums.E_TranslationStatus Status
        {
            get { return m_Status; }
            set { m_Status = value; }
        }

        [JsonProperty("hasThumbnail")]
        public bool HasThumbnail
        {
            get { return m_HasThumbnail; }
            set { m_HasThumbnail = value; }
        }

        [JsonProperty("derivatives")]
        public List<IDerivative> DerivativeList
        {
            get { return m_DerivativeList; }
            set { m_DerivativeList = value; }
        }
    }
}

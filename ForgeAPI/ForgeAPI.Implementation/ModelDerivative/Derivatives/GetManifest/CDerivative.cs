using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest;

namespace ForgeAPI.Implementation.ModelDerivative.Derivatives.GetManifest
{
    public class CDerivative : IDerivative
    {
        private string m_Name = "";
        private bool m_HasThumbnail = false;
        private ForgeAPI.Interface.Enums.E_OutputFormatType m_OutputFormat = Interface.Enums.E_OutputFormatType.Undefined;
        private ForgeAPI.Interface.Enums.E_TranslationStatus m_Status = Interface.Enums.E_TranslationStatus.Undefined;
        private string m_Progress = "";
        private List<IMessage> m_MessageList = new List<IMessage>();
        private List<IDerivativeChild> m_Children = new List<IDerivativeChild>();

        public CDerivative()
        {
        }

        [JsonProperty("name")]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        [JsonProperty("hasThumbnail")]
        public bool HasThumbnail
        {
            get { return m_HasThumbnail; }
            set { m_HasThumbnail = value; }
        }

        [JsonProperty("role")]
        [JsonConverter(typeof(JSONEnumConverters.CJSONConverter_OutputFormatType))]
        public ForgeAPI.Interface.Enums.E_OutputFormatType OutputFormat
        {
            get { return m_OutputFormat; }
            set { m_OutputFormat = value; }
        }

        [JsonProperty("status")]
        [JsonConverter(typeof(JSONEnumConverters.CJSONConverter_TranslationStatus))]
        public ForgeAPI.Interface.Enums.E_TranslationStatus Status
        {
            get { return m_Status; }
            set { m_Status = value; }
        }
        
        [JsonProperty("progress")]
        public string Progress
        {
            get { return m_Progress; }
            set { m_Progress = value; }
        }

        [JsonProperty("messages")]
        public List<IMessage> MessageList
        {
            get { return m_MessageList; }
            set { m_MessageList = value; }
        }

        [JsonProperty("children")]
        public List<IDerivativeChild> Children
        {
            get { return m_Children; }
            set { m_Children = value; }
        }
    }
}

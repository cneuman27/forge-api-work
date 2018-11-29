using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest;

namespace ForgeAPI.Implementation.ModelDerivative.Derivatives.GetManifest
{
    public class CDerivativeChild : IDerivativeChild
    {
        private Guid m_ID = Guid.Empty;
        private string m_DerivativeType = "";
        private string m_Role = "";
        private string m_MIMEType = "";
        private string m_URN = "";
        private string m_Progress = "";
        private ForgeAPI.Interface.Enums.E_TranslationStatus m_Status = Interface.Enums.E_TranslationStatus.Undefined;
        private List<IMessage> m_MessageList = new List<IMessage>();
        private List<IDerivativeChild> m_Children = new List<IDerivativeChild>();

        public CDerivativeChild()
        {
        }

        [JsonProperty("id")]
        public Guid ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        [JsonProperty("type")]
        public string DerivativeType
        {
            get { return m_DerivativeType; }
            set { m_DerivativeType = value; }
        }
        
        [JsonProperty("role")]
        public string Role
        {
            get { return m_Role; }
            set { m_Role = value; }
        }

        [JsonProperty("mime")]
        public string MIMEType
        {
            get { return m_MIMEType; }
            set { m_MIMEType = value; }
        }

        [JsonProperty("urn")]
        public string URN
        {
            get { return m_URN; }
            set { m_URN = value; }
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

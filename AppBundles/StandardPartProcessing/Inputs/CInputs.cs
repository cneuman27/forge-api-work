using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace StandardPartProcessing.Inputs
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CInputs
    {
        private string m_Reference = "";
        private string m_PartNumber = "";
        private string m_Description = "";
        private string m_MaterialType = "";

        private string m_IPTFile = "";
        private string m_IDWFile = "";
        private string m_XLSFile = "";        

        private Dictionary<string, string> m_Parameters = new Dictionary<string, string>();

        public CInputs()
        {
        }

        [JsonProperty("Reference")]
        public string Reference
        {
            get { return m_Reference; }
            set { m_Reference = value; }
        }

        [JsonProperty("PartNumber")]
        public string PartNumber
        {
            get { return m_PartNumber; }
            set { m_PartNumber = value; }
        }

        [JsonProperty("Description")]
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }
        
        [JsonProperty("MaterialType")]
        public string MaterialType
        {
            get { return m_MaterialType; }
            set { m_MaterialType = value; }
        }

        [JsonProperty("IPTFile")]
        public string IPTFile
        {
            get { return m_IPTFile; }
            set { m_IPTFile = value; }
        }

        [JsonProperty("IDWFile")]
        public string IDWFile
        {
            get { return m_IDWFile; }
            set { m_IDWFile = value; }
        }

        [JsonProperty("XLSFile")]
        public string XLSFile
        {
            get { return m_XLSFile; }
            set { m_XLSFile = value; }
        }

        [JsonProperty("Parameters")]
        public Dictionary<string, string> Parameters
        {
            get { return m_Parameters; }
            set { m_Parameters = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace StandardPartProcessing.Outputs
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CArtifact
    {
        private Enums.E_ArtifactType m_Type = Enums.E_ArtifactType.Undefined;
        private string m_FileName = "";
        private string m_URN = "";
        private string m_URNEncoded = "";

        private string m_FileLocation = "";

        public CArtifact()
        {
        }

        [JsonProperty("Type")]
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public Enums.E_ArtifactType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        [JsonProperty("FileName")]
        public string FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        [JsonProperty("URN")]
        public string URN
        {
            get { return m_URN; }
            set { m_URN = value; }
        }

        [JsonProperty("URNEncoded")]
        public string URNEncoded
        {
            get { return m_URNEncoded; }
            set { m_URNEncoded = value; }
        }

        public string FileLocation
        {
            get { return m_FileLocation; }
            set { m_FileLocation = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace StandardPartProcessing.Outputs
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class COutputs
    {
        private string m_Reference = "";
        private string m_PartNumber = "";
        private TimeSpan m_RunDuration = TimeSpan.Zero;

        private List<CArtifact> m_ArtifactList = new List<CArtifact>();

        public COutputs()
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

        [JsonProperty("ArtifactList")]
        public List<CArtifact> ArtifactList
        {
            get { return m_ArtifactList; }
            set { m_ArtifactList = value; }
        }

        [JsonProperty("RunDuration")]
        public TimeSpan RunDuration
        {
            get { return m_RunDuration; }
            set { m_RunDuration = value; }
        }
    }
}

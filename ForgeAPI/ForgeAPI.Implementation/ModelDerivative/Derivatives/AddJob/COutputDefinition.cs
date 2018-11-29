using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob;

namespace ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class COutputDefinition : IOutputDefinition
    {
        private IDestinationSettings m_DestinationSettings = null;
        private List<IOutputFormat> m_FormatList = new List<IOutputFormat>();

        public COutputDefinition(
            IDestinationSettings destinationSettings)
        {
            m_DestinationSettings = destinationSettings;
        }

        [JsonProperty("destination")]
        public IDestinationSettings DestinationSettings
        {
            get { return m_DestinationSettings; }
            set { m_DestinationSettings = value; }
        }

        [JsonProperty("formats")]
        public List<IOutputFormat> FormatList
        {
            get { return m_FormatList; }
            set { m_FormatList = value; }
        }
    }
}

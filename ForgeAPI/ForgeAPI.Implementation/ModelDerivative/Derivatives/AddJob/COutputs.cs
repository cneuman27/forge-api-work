using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob;

namespace ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class COutputs : COutputsCommon, IOutputs
    {
        private string m_APIStatus = "";
        private string m_URN = "";

        [JsonProperty("result")]
        public string APIStatus
        {
            get { return m_APIStatus; }
            set { m_APIStatus = value; }
        }

        [JsonProperty("urn")]
        public string URN
        {
            get { return m_URN; }
            set { m_URN = value; }
        }
    }
}

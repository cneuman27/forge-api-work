using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob;

namespace ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class COutputFormat_SVF : COutputFormat, IOutputFormat_SVF
    {
        private List<ForgeAPI.Interface.Enums.E_SVFViewType> m_ViewList = new List<Interface.Enums.E_SVFViewType>();

        public COutputFormat_SVF()
        {
            Type = Interface.Enums.E_OutputFormatType.SVF;
        }

        [JsonProperty("views")]
        [JsonConverter(typeof(JSONEnumConverters.CJSONConverter_SVFViewType))]
        public List<ForgeAPI.Interface.Enums.E_SVFViewType> ViewList
        {
            get { return m_ViewList; }
            set { m_ViewList = value; }
        }            
    }
}

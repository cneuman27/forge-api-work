using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob;

namespace ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class COutputFormat : IOutputFormat
    {
        private ForgeAPI.Interface.Enums.E_OutputFormatType m_Type = Interface.Enums.E_OutputFormatType.Undefined;

        public COutputFormat()
        {
        }

        [JsonProperty("type")]
        [JsonConverter(typeof(JSONEnumConverters.CJSONConverter_OutputFormatType))]
        public ForgeAPI.Interface.Enums.E_OutputFormatType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }
    }
}

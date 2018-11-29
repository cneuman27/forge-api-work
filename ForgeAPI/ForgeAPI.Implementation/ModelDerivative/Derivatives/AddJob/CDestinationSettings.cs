using Newtonsoft.Json;
using ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob;

namespace ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CDestinationSettings : IDestinationSettings
    {
        private ForgeAPI.Interface.Enums.E_Region m_Region = Interface.Enums.E_Region.Undefined;

        public CDestinationSettings()
        {
        }

        [JsonProperty("region")]
        [JsonConverter(typeof(JSONEnumConverters.CJSONConverter_Region))]
        public ForgeAPI.Interface.Enums.E_Region Region
        {
            get { return m_Region; }
            set { m_Region = value; }
        }
    }
}

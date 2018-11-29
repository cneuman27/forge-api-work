using Newtonsoft.Json;
using ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob;

namespace ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CInputDefinition : IInputDefinition
    {
        private string m_URNEncoded = "";
        private bool m_IsCompressed = false;
        private string m_CompressedRootFileName = "";

        public CInputDefinition()
        {
        }

        [JsonProperty("urn")]
        public string URNEncoded
        {
            get { return m_URNEncoded; }
            set { m_URNEncoded = value; }
        }

        [JsonProperty("compressedUrn")]
        public bool IsCompressed
        {
            get { return m_IsCompressed; }
            set { m_IsCompressed = value; }
        }

        [JsonProperty("rootFilename")]
        public string CompressedRootFileName
        {
            get { return m_CompressedRootFileName; }
            set { m_CompressedRootFileName = value; }
        }

        public bool ShouldSerializeCompressedRootFileName()
        {
            return (string.IsNullOrWhiteSpace(CompressedRootFileName) == false);
        }
    }
}

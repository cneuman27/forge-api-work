using Newtonsoft.Json;
using ForgeAPI.Interface;
using ForgeAPI.Interface.DataManagement.Buckets.CreateBucket;

namespace ForgeAPI.Implementation.DataManagement.Buckets.CreateBucket 
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CPermissions : IPermissions
    {
        private string m_ApplicationID = "";
        private Enums.E_AccessType m_AccessType = Enums.E_AccessType.Undefined;

        public CPermissions()
        {
        }

        [JsonProperty("authId")]
        public string ApplicationID
        {
            get { return m_ApplicationID; }
            internal set { m_ApplicationID = value; }
        }

        [JsonProperty("access")]
        [JsonConverter(typeof(JSONEnumConverters.CJSONConverter_AccessType))]
        public Enums.E_AccessType AccessType
        {
            get { return m_AccessType; }
            internal set { m_AccessType = value; }
        }

        public bool ShouldSerializeAccessType()
        {
            return AccessType != Enums.E_AccessType.Undefined;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using ForgeAPI.Interface;
using ForgeAPI.Interface.DataManagement.Buckets.CreateBucket;

namespace ForgeAPI.Implementation.DataManagement.Buckets.CreateBucket
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CInputs : CInputsCommon, IInputs
    {
        private string m_BucketKey = "";
        private List<IPermissions> m_Permissions = new List<IPermissions>();
        private Enums.E_RetentionPolicy m_RetentionPolicy = Enums.E_RetentionPolicy.Undefined;

        public CInputs()
        {
        }

        [JsonProperty("bucketKey")]
        public string BucketKey
        {
            get { return m_BucketKey; }
            set { m_BucketKey = value; }
        }

        [JsonProperty("allow")]
        public List<IPermissions> Permissions
        {
            get { return m_Permissions; }
            set { m_Permissions = value; }
        }

        [JsonProperty("policyKey")]
        [JsonConverter(typeof(JSONEnumConverters.CJSONConverter_RetentionPolicy))]
        public Enums.E_RetentionPolicy RetentionPolicy
        {
            get { return m_RetentionPolicy; }
            set { m_RetentionPolicy = value; }
        }

        public bool ShouldSerializePermissions()
        {
            return (Permissions != null && Permissions.Count > 0);
        }
        public bool ShouldSerializeRetentionPolicy()
        {
            return RetentionPolicy != Enums.E_RetentionPolicy.Undefined;
        }
    }
}

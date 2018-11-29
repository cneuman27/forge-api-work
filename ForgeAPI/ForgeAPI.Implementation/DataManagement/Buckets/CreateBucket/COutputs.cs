using System.Collections.Generic;
using Newtonsoft.Json;
using ForgeAPI.Interface;
using ForgeAPI.Interface.DataManagement.Buckets.CreateBucket;

namespace ForgeAPI.Implementation.DataManagement.Buckets.CreateBucket
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class COutputs : COutputsCommon, IOutputs
    {
        private string m_BucketKey = "";
        private string m_BucketOwner = "";
        private long m_CreateDateEpoch = 0;
        private List<IPermissions> m_Permissions = new List<IPermissions>();
        private Enums.E_RetentionPolicy m_RetentionPolicy = Enums.E_RetentionPolicy.Undefined;

        public COutputs()
        {
        }

        [JsonProperty("bucketKey")]
        public string BucketKey
        {
            get { return m_BucketKey; }
            set { m_BucketKey = value; }
        }

        [JsonProperty("bucketOwner")]
        public string BucketOwner
        {
            get { return m_BucketOwner; }
            set { m_BucketOwner = value; }
        }

        [JsonProperty("createdDate")]
        public long CreateDateEpoch
        {
            get { return m_CreateDateEpoch; }
            set { m_CreateDateEpoch = value; }
        }

        [JsonProperty("permissions")]
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
    }
}

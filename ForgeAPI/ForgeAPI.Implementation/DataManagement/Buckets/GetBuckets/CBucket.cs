using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using ForgeAPI.Interface;
using ForgeAPI.Interface.DataManagement.Buckets.GetBuckets;

namespace ForgeAPI.Implementation.DataManagement.Buckets.GetBuckets
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CBucket : IBucket
    {
        private string m_BucketKey = "";
        private long m_CreateDateEpoch = 0;
        private Enums.E_RetentionPolicy m_RetentionPolicy = Enums.E_RetentionPolicy.Undefined;

        public CBucket()
        {
        }

        [JsonProperty("bucketKey")]
        public string BucketKey
        {
            get { return m_BucketKey; }
            set { m_BucketKey = value; }
        }

        [JsonProperty("createdDate")]
        public long CreateDateEpoch
        {
            get { return m_CreateDateEpoch; }
            set { m_CreateDateEpoch = value; }
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

using System.Collections.Generic;
using Newtonsoft.Json;
using ForgeAPI.Interface.DataManagement.Buckets.GetBuckets;

namespace ForgeAPI.Implementation.DataManagement.Buckets.GetBuckets
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class COutputs : COutputsCommon, IOutputs
    {
        private List<IBucket> m_BucketList = new List<IBucket>();
        private string m_NextPageURI = "";

        public COutputs()
            : base()
        {
        }
                
        [JsonProperty("items")]
        public List<IBucket> BucketList
        {
            get { return m_BucketList; }
            set { m_BucketList = value; }
        }

        [JsonProperty("next")]
        public string NextPageURI
        {
            get { return m_NextPageURI; }
            set { m_NextPageURI = value; }
        }
    }
}

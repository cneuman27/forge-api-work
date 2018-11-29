using Newtonsoft.Json;
using ForgeAPI.Interface.DataManagement.Objects.UploadObject;

namespace ForgeAPI.Implementation.DataManagement.Objects.UploadObject
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class COutputs : COutputsCommon, IOutputs
    {
        private string m_BucketKey = "";
        private string m_ObjectID = "";
        private string m_ObjectKey = "";
        private string m_SHA1 = "";
        private long m_ObjectSize = 0;
        private string m_ContentType = "";
        private string m_DownloadURI = "";

        public COutputs()
        {
        }

        [JsonProperty("bucketKey")]
        public string BucketKey
        {
            get { return m_BucketKey; }
            set { m_BucketKey = value; }
        }

        [JsonProperty("objectId")]
        public string ObjectID
        {
            get { return m_ObjectID; }
            set { m_ObjectID = value; }
        }

        [JsonProperty("objectKey")]
        public string ObjectKey
        {
            get { return m_ObjectKey; }
            set { m_ObjectKey = value; }
        }

        [JsonProperty("sha1")]
        public string SHA1
        {
            get { return m_SHA1; }
            set { m_SHA1 = value; }
        }

        [JsonProperty("size")]
        public long ObjectSize
        {
            get { return m_ObjectSize; }
            set { m_ObjectSize = value; }
        }

        [JsonProperty("contentType")]
        public string ContentType
        {
            get { return m_ContentType; }
            set { m_ContentType = value; }
        }

        [JsonProperty("location")]
        public string DownloadURI
        {
            get { return m_DownloadURI; }
            set { m_DownloadURI = value; }
        }
    }
}

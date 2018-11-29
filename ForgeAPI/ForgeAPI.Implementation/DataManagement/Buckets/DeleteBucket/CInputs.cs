using ForgeAPI.Interface.DataManagement.Buckets.DeleteBucket;

namespace ForgeAPI.Implementation.DataManagement.Buckets.DeleteBucket
{
    public class CInputs : CInputsCommon, IInputs
    {
        private string m_BucketKey = "";

        public CInputs()
        {
        }

        public string BucketKey
        {
            get { return m_BucketKey; }
            set { m_BucketKey = value; }
        }
    }
}

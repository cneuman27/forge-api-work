using ForgeAPI.Interface.DataManagement.Buckets.GetBuckets;

namespace ForgeAPI.Implementation.DataManagement.Buckets.GetBuckets
{
    public class CInputs : CInputsCommon, IInputs
    {
        private int m_Limit = 10;
        private string m_PaginationOffsetBucketKey = "";

        public CInputs()
        {
        }
                
        public int Limit
        {
            get { return m_Limit; }
            set { m_Limit = value; }
        }
        public string PaginationOffsetBucketKey
        {
            get { return m_PaginationOffsetBucketKey; }
            set { m_PaginationOffsetBucketKey = value; }
        }
    }
}

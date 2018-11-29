using ForgeAPI.Interface.DataManagement.Objects.UploadObject;

namespace ForgeAPI.Implementation.DataManagement.Objects.UploadObject
{
    public class CInputs : CInputsCommon, IInputs
    {
        private string m_FileName = "";
        private byte[] m_FileData = null;
        private string m_ContentType = "";
        private string m_BucketKey = "";
        private string m_ObjectName = "";

        public CInputs()
        {
        }

        public string FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }
        public byte[] FileData
        {
            get { return m_FileData; }
            set { m_FileData = value; }
        }
        public string ContentType
        {
            get { return m_ContentType; }
            set { m_ContentType = value; }
        }
        public string BucketKey
        {
            get { return m_BucketKey; }
            set { m_BucketKey = value; }
        }
        public string ObjectName
        {
            get { return m_ObjectName; }
            set { m_ObjectName = value; }
        }
    }
}

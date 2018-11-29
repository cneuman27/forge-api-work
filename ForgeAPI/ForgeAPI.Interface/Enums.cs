
namespace ForgeAPI.Interface
{
    public class Enums
    {
        public enum E_AccessScope
        {
            Undefined,

            UserProfile_Read,
            User_Read,
            User_Write,
            Viewables_Read,
            Data_Read,
            Data_Write,
            Data_Create,
            Data_Search,
            Bucket_Create,
            Bucket_Read,
            Bucket_Update,
            Bucket_Delete,
            Code_All,
            Account_Read,
            Account_Write
        }
        public enum E_AccessType
        {
            Undefined,

            Full,
            Read
        }
        public enum E_OutputFormatType
        {
            Undefined,

            DWG,
            FBX,
            IFC,
            IGES,
            OBJ,
            STEP,
            STL,
            SVF,
            Thumbnail                
        }
        public enum E_Region
        {
            Undefined,

            US,
            EMEA
        }
        public enum E_RetentionPolicy
        {
            Undefined,

            Transient,
            Temporary,
            Persistent
        }
        public enum E_SVFViewType
        {
            Undefined,

            _2D,
            _3D
        }
        public enum E_ThumbnailWidth
        {
            Undefined,
            _100,
            _200,
            _400
        }
        public enum E_ThumbnailHeight
        {
            Undefined,

            _100,
            _200,
            _400
        }
        public enum E_TranslationStatus
        {
            Undefined,

            Pending,
            Success,
            InProgress,
            Failed,
            Timeout
        }
    }
}

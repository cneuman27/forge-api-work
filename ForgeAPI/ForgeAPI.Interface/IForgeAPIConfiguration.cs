
namespace ForgeAPI.Interface
{
    public interface IForgeAPIConfiguration
    {
        string ClientID
        {
            get;
        }
        string Secret
        {
            get;
        }

        string APIURI_Authentication
        {
            get;
        }

        string APIURI_DataManagement_Buckets_CreateBucket
        {
            get;
        }
        string APIURI_DataManagement_Buckets_GetBuckets
        {
            get;
        }
        string APIURI_DataManagement_Buckets_DeleteBucket
        {
            get;
        }

        string APIURI_DataManagement_Objects_UploadObject
        {
            get;
        }

        string APIURI_ModelDerivative_Derivatives_PostJob
        {
            get;
        }
        string APIURI_ModelDerivative_Derivatives_GetManifest
        {
            get;
        }
    }
}

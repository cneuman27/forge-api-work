using System;
using System.Collections.Generic;
using System.Text;

namespace ForgeAPI.Implementation
{
    public static class CConstants
    {
        public const string HEADER__CONTENT_TYPE = "Content-Type";
        public const string HEADER__CONTENT_LENGTH = "Content-Length";
        public const string HEADER__CONTENT_DISPOSITION = "Content-Disposition";

        public const string HEADER__AUTHORIZATION = "Authorization";

        public const string HEADER__ADS_FORCE = "x-ads-force";

        public const string MEDIA_TYPE__JSON = "application/json";

        public const string FORGE_API_CONFIG = "ForgeAPI";

        public const string FORGE_API_CONFIG__CLIENT_ID = "ClientID";
        public const string FORGE_API_CONFIG__SECRET = "Secret";

        public const string FORGE_API_CONFIG__API_URI__AUTHENTICATION = "APIURI_Authentication";

        public const string FORGE_API_CONFIG__API_URI__DATA_MANAGEMENT__BUCKETS__CREATE_BUCKET = "APIURI_DataManagement_Buckets_CreateBucket";
        public const string FORGE_API_CONFIG__API_URI__DATA_MANAGEMENT__BUCKETS__GET_BUCKETS = "APIURI_DataManagement_Buckets_GetBuckets";
        public const string FORGE_API_CONFIG__API_URI__DATA_MANAGEMENT__BUCKETS__GET_BUCKET_DETAILS = "APIURI_DataManagement_Buckets_GetBucketDetails";
        public const string FORGE_API_CONFIG__API_URI__DATA_MANAGEMENT__BUCKETS__DELETE_BUCKET = "APIURI_DataManagement_Buckets_DeleteBucket";

        public const string FORGE_API_CONFIG__API_URI__DATA_MANAGEMENT__OBJECTS__UPLOAD_OBJECT = "APIURI_DataManagement_Objects_UploadObject";

        public const string FORGE_API_CONFIG__API_URI__MODEL_DERIVATIVE__DERIVATIVES__POST_JOB = "APIURI_ModelDerivative_Derivatives_PostJob";
        public const string FORGE_API_CONFIG__API_URI__MODEL_DERIVATIVE__DERIVATIVES__GET_MANIFEST = "APIURI_ModelDerivative_Derivatives_GetManifest";

        public static List<ForgeAPI.Interface.Enums.E_AccessScope> DEFAULT_ACCESS_SCOPES = new List<Interface.Enums.E_AccessScope>()
        {
            ForgeAPI.Interface.Enums.E_AccessScope.Data_Read
        };
    }
}

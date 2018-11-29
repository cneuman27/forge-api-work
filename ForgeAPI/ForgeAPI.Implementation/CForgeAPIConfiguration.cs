using Microsoft.Extensions.Configuration;
using ForgeAPI.Interface;

namespace ForgeAPI.Implementation
{
    public class CForgeAPIConfiguration : IForgeAPIConfiguration
    {
        private IConfiguration m_Configuration = null;

        public CForgeAPIConfiguration(IConfiguration configuration)
        {
            m_Configuration = configuration;
        }

        public string ClientID
        {
            get
            {
                return m_Configuration
                    .GetSection(CConstants.FORGE_API_CONFIG)[CConstants.FORGE_API_CONFIG__CLIENT_ID];                    
            }
        }
        public string Secret
        {
            get
            {
                return m_Configuration
                    .GetSection(CConstants.FORGE_API_CONFIG)[CConstants.FORGE_API_CONFIG__SECRET];
            }
        }

        public string APIURI_Authentication
        {
            get
            {
                return m_Configuration
                    .GetSection(CConstants.FORGE_API_CONFIG)[CConstants.FORGE_API_CONFIG__API_URI__AUTHENTICATION];
            }
        }

        public string APIURI_DataManagement_Buckets_CreateBucket
        {
            get
            {
                return m_Configuration
                    .GetSection(CConstants.FORGE_API_CONFIG)[CConstants.FORGE_API_CONFIG__API_URI__DATA_MANAGEMENT__BUCKETS__CREATE_BUCKET];
            }
        }
        public string APIURI_DataManagement_Buckets_GetBuckets
        {
            get
            {
                return m_Configuration
                    .GetSection(CConstants.FORGE_API_CONFIG)[CConstants.FORGE_API_CONFIG__API_URI__DATA_MANAGEMENT__BUCKETS__GET_BUCKETS];
            }
        }
        public string APIURI_DataManagement_Buckets_DeleteBucket
        {
            get
            {
                return m_Configuration
                    .GetSection(CConstants.FORGE_API_CONFIG)[CConstants.FORGE_API_CONFIG__API_URI__DATA_MANAGEMENT__BUCKETS__DELETE_BUCKET];
            }
        }

        public string APIURI_DataManagement_Objects_UploadObject
        {
            get
            {
                return m_Configuration
                    .GetSection(CConstants.FORGE_API_CONFIG)[CConstants.FORGE_API_CONFIG__API_URI__DATA_MANAGEMENT__OBJECTS__UPLOAD_OBJECT];
            }
        }

        public string APIURI_ModelDerivative_Derivatives_PostJob
        {
            get
            {
                return m_Configuration
                    .GetSection(CConstants.FORGE_API_CONFIG)[CConstants.FORGE_API_CONFIG__API_URI__MODEL_DERIVATIVE__DERIVATIVES__POST_JOB];
            }
        }
        public string APIURI_ModelDerivative_Derivatives_GetManifest
        {
            get
            {
                return m_Configuration
                    .GetSection(CConstants.FORGE_API_CONFIG)[CConstants.FORGE_API_CONFIG__API_URI__MODEL_DERIVATIVE__DERIVATIVES__GET_MANIFEST];
            }
        }
    }
}

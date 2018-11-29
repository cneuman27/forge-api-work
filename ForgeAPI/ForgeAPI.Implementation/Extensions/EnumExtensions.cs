using System;
using System.Collections.Generic;

using ForgeAPI.Interface;

namespace ForgeAPI.Implementation.Extensions
{
    public static class EnumExtensions
    {
        #region E_AccessScope

        private static List<Tuple<Enums.E_AccessScope, string>> ACCESS_SCOPE_LOOKUP = new List<Tuple<Enums.E_AccessScope, string>>()
        {
            new Tuple<Enums.E_AccessScope, string>(Enums.E_AccessScope.UserProfile_Read, "user-profile:read"),
            new Tuple<Enums.E_AccessScope, string>(Enums.E_AccessScope.User_Read, "user:read"),
            new Tuple<Enums.E_AccessScope, string>(Enums.E_AccessScope.User_Write, "user:write"),
            new Tuple<Enums.E_AccessScope, string>(Enums.E_AccessScope.Viewables_Read, "viewables:read"),
            new Tuple<Enums.E_AccessScope, string>(Enums.E_AccessScope.Data_Read, "data:read"),
            new Tuple<Enums.E_AccessScope, string>(Enums.E_AccessScope.Data_Write, "data:write"),
            new Tuple<Enums.E_AccessScope, string>(Enums.E_AccessScope.Data_Create, "data:create"),
            new Tuple<Enums.E_AccessScope, string>(Enums.E_AccessScope.Data_Search, "data:search"),
            new Tuple<Enums.E_AccessScope, string>(Enums.E_AccessScope.Bucket_Create, "bucket:create"),
            new Tuple<Enums.E_AccessScope, string>(Enums.E_AccessScope.Bucket_Read, "bucket:read"),
            new Tuple<Enums.E_AccessScope, string>(Enums.E_AccessScope.Bucket_Update, "bucket:update"),
            new Tuple<Enums.E_AccessScope, string>(Enums.E_AccessScope.Bucket_Delete, "bucket:delete"),
            new Tuple<Enums.E_AccessScope, string>(Enums.E_AccessScope.Code_All, "code:all"),
            new Tuple<Enums.E_AccessScope, string>(Enums.E_AccessScope.Account_Read, "account:read"),
            new Tuple<Enums.E_AccessScope, string>(Enums.E_AccessScope.Account_Write, "account:write")
        };

        public static string ConvertToJSON(this Enums.E_AccessScope type)
        {
            Tuple<Enums.E_AccessScope, string> item;

            item = ACCESS_SCOPE_LOOKUP.Find(i => i.Item1 == type);
            if (item != null)
            {
                return item.Item2;
            }

            return "";
        }
        public static Enums.E_AccessScope ConvertToAccessScope(this string json)
        {
            Tuple<Enums.E_AccessScope, string> item;

            item = ACCESS_SCOPE_LOOKUP.Find(i => i.Item2 == json);
            if (item != null)
            {
                return item.Item1;
            }

            return Enums.E_AccessScope.Undefined;
        }

        #endregion

        #region E_AccessType

        private static List<Tuple<Enums.E_AccessType, string>> ACCESS_TYPE_LOOKUP = new List<Tuple<Enums.E_AccessType, string>>()
        {
            new Tuple<Enums.E_AccessType, string>(Enums.E_AccessType.Full, "full"),
            new Tuple<Enums.E_AccessType, string>(Enums.E_AccessType.Read, "read")
        };

        public static string ConvertToJSON(this Enums.E_AccessType type)
        {
            Tuple<Enums.E_AccessType, string> item;

            item = ACCESS_TYPE_LOOKUP.Find(i => i.Item1 == type);
            if (item != null)
            {
                return item.Item2;
            }

            return "";
        }
        public static Enums.E_AccessType ConvertToAccessType(this string json)
        {
            Tuple<Enums.E_AccessType, string> item;

            item = ACCESS_TYPE_LOOKUP.Find(i => i.Item2 == json);
            if (item != null)
            {
                return item.Item1;
            }

            return Enums.E_AccessType.Undefined;
        }

        #endregion

        #region E_OutputFormatType

        private static List<Tuple<Enums.E_OutputFormatType, string>> OUTPUT_FORMAT_TYPE_LOOKUP = new List<Tuple<Enums.E_OutputFormatType, string>>()
        {
            new Tuple<Enums.E_OutputFormatType, string>(Enums.E_OutputFormatType.DWG, "DWG"),
            new Tuple<Enums.E_OutputFormatType, string>(Enums.E_OutputFormatType.FBX, "FBX"),
            new Tuple<Enums.E_OutputFormatType, string>(Enums.E_OutputFormatType.IFC, "IFC"),
            new Tuple<Enums.E_OutputFormatType, string>(Enums.E_OutputFormatType.IGES, "IGES"),
            new Tuple<Enums.E_OutputFormatType, string>(Enums.E_OutputFormatType.OBJ, "OBJ"),
            new Tuple<Enums.E_OutputFormatType, string>(Enums.E_OutputFormatType.STEP, "STEP"),
            new Tuple<Enums.E_OutputFormatType, string>(Enums.E_OutputFormatType.STL, "STL"),
            new Tuple<Enums.E_OutputFormatType, string>(Enums.E_OutputFormatType.SVF, "SVF"),
            new Tuple<Enums.E_OutputFormatType, string>(Enums.E_OutputFormatType.Thumbnail, "thumbnail"),
        };

        public static string ConvertToJSON(this Enums.E_OutputFormatType type)
        {
            Tuple<Enums.E_OutputFormatType, string> item;

            item = OUTPUT_FORMAT_TYPE_LOOKUP.Find(i => i.Item1 == type);
            if (item != null)
            {
                return item.Item2;
            }

            return "";
        }
        public static Enums.E_OutputFormatType ConvertToOrderFormatType(this string json)
        {
            Tuple<Enums.E_OutputFormatType, string> item;

            item = OUTPUT_FORMAT_TYPE_LOOKUP.Find(i => i.Item2 == json);
            if (item != null)
            {
                return item.Item1;
            }

            return Enums.E_OutputFormatType.Undefined;
        }

        #endregion

        #region E_Region

        private static List<Tuple<Enums.E_Region, string>> REGION_LOOKUP = new List<Tuple<Enums.E_Region, string>>()
        {
            new Tuple<Enums.E_Region, string>(Enums.E_Region.US, "US"),
            new Tuple<Enums.E_Region, string>(Enums.E_Region.EMEA, "EMEA")
        };

        public static string ConvertToJSON(this Enums.E_Region type)
        {
            Tuple<Enums.E_Region, string> item;

            item = REGION_LOOKUP.Find(i => i.Item1 == type);
            if (item != null)
            {
                return item.Item2;
            }

            return "";
        }
        public static Enums.E_Region ConvertToRegion(this string json)
        {
            Tuple<Enums.E_Region, string> item;

            item = REGION_LOOKUP.Find(i => i.Item2 == json);
            if (item != null)
            {
                return item.Item1;
            }

            return Enums.E_Region.Undefined;
        }

        #endregion

        #region E_RetentionPolicy

        private static List<Tuple<Enums.E_RetentionPolicy, string>> RETENTION_POLICY_LOOKUP = new List<Tuple<Enums.E_RetentionPolicy, string>>()
        {
            new Tuple<Enums.E_RetentionPolicy, string>(Enums.E_RetentionPolicy.Transient, "transient"),
            new Tuple<Enums.E_RetentionPolicy, string>(Enums.E_RetentionPolicy.Temporary, "temporary"),
            new Tuple<Enums.E_RetentionPolicy, string>(Enums.E_RetentionPolicy.Persistent, "persistent")
        };

        public static string ConvertToJSON(this Enums.E_RetentionPolicy type)
        {
            Tuple<Enums.E_RetentionPolicy, string> item;

            item = RETENTION_POLICY_LOOKUP.Find(i => i.Item1 == type);
            if (item != null)
            {
                return item.Item2;
            }

            return "";
        }
        public static Enums.E_RetentionPolicy ConvertToRetentionPolicy(this string json)
        {
            Tuple<Enums.E_RetentionPolicy, string> item;

            item = RETENTION_POLICY_LOOKUP.Find(i => i.Item2 == json);
            if (item != null)
            {
                return item.Item1;
            }

            return Enums.E_RetentionPolicy.Undefined;
        }

        #endregion

        #region E_SVFViewType

        private static List<Tuple<Enums.E_SVFViewType, string>> SVF_VIEW_TYPE_LOOKUP = new List<Tuple<Enums.E_SVFViewType, string>>()
        {
            new Tuple<Enums.E_SVFViewType, string>(Enums.E_SVFViewType._2D, "2d"),
            new Tuple<Enums.E_SVFViewType, string>(Enums.E_SVFViewType._3D, "3d")
        };

        public static string ConvertToJSON(this Enums.E_SVFViewType type)
        {
            Tuple<Enums.E_SVFViewType, string> item;

            item = SVF_VIEW_TYPE_LOOKUP.Find(i => i.Item1 == type);
            if (item != null)
            {
                return item.Item2;
            }

            return "";
        }
        public static Enums.E_SVFViewType ConvertToSVFViewType(this string json)
        {
            Tuple<Enums.E_SVFViewType, string> item;

            item = SVF_VIEW_TYPE_LOOKUP.Find(i => i.Item2 == json);
            if (item != null)
            {
                return item.Item1;
            }

            return Enums.E_SVFViewType.Undefined;
        }

        #endregion

        #region E_TranslationStatus

        private static List<Tuple<Enums.E_TranslationStatus, string>> TRANSLATION_STATUS_LOOKUP = new List<Tuple<Enums.E_TranslationStatus, string>>()
        {
            new Tuple<Enums.E_TranslationStatus, string>(Enums.E_TranslationStatus.Failed, "failed"),
            new Tuple<Enums.E_TranslationStatus, string>(Enums.E_TranslationStatus.InProgress, "inprogress"),
            new Tuple<Enums.E_TranslationStatus, string>(Enums.E_TranslationStatus.Pending, "pending"),
            new Tuple<Enums.E_TranslationStatus, string>(Enums.E_TranslationStatus.Success, "success"),
            new Tuple<Enums.E_TranslationStatus, string>(Enums.E_TranslationStatus.Timeout, "timeout")
        };

        public static string ConvertToJSON(this Enums.E_TranslationStatus type)
        {
            Tuple<Enums.E_TranslationStatus, string> item;

            item = TRANSLATION_STATUS_LOOKUP.Find(i => i.Item1 == type);
            if (item != null)
            {
                return item.Item2;
            }

            return "";
        }
        public static Enums.E_TranslationStatus ConvertToTranslationStatus(this string json)
        {
            Tuple<Enums.E_TranslationStatus, string> item;

            item = TRANSLATION_STATUS_LOOKUP.Find(i => i.Item2 == json);
            if (item != null)
            {
                return item.Item1;
            }

            return Enums.E_TranslationStatus.Undefined;
        }

        #endregion
    }
}

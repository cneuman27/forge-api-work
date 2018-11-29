using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeTest.Types
{
    public class CBucketWrapper : MOMShared.CNotifier
    {
        private ForgeAPI.Interface.DataManagement.Buckets.GetBuckets.IBucket m_Data = null;

        public CBucketWrapper(ForgeAPI.Interface.DataManagement.Buckets.GetBuckets.IBucket data)
        {
            m_Data = data;
        }

        public string BucketKey
        {
            get
            {
                if (m_Data != null &&
                    string.IsNullOrWhiteSpace(m_Data.BucketKey) == false)
                {
                    return m_Data.BucketKey.Trim();
                }

                return "N/A";
            }
        }
        public string CreateDate
        {
            get
            {
                if (m_Data != null &&
                    m_Data.CreateDateEpoch > 0)
                {
                    return
                        new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(m_Data.CreateDateEpoch).ToLocalTime().ToString();
                }

                return "N/A";
            }
        }
        public string RetentionPolicy
        {
            get
            {
                if (m_Data.RetentionPolicy != ForgeAPI.Interface.Enums.E_RetentionPolicy.Undefined)
                {
                    return m_Data.RetentionPolicy.ToString();
                }

                return "N/A";
            }
        }

        public override void NotifyAll()
        {
            Notify(nameof(BucketKey));
            Notify(nameof(CreateDate));
            Notify(nameof(RetentionPolicy));
        }
    }
}

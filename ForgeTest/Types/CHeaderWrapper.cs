using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeTest.Types
{
    public class CHeaderWrapper : MOMShared.CNotifier
    {
        private KeyValuePair<string, string> m_Data;

        public CHeaderWrapper(KeyValuePair<string, string> data)
        {
            m_Data = data;
        }

        public string HeaderKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(m_Data.Key))
                {
                    return "N/A";
                }

                return m_Data.Key;
            }
        }
        public string HeaderValue
        {
            get
            {
                if (string.IsNullOrWhiteSpace(m_Data.Value))
                {
                    return "N/A";
                }

                return m_Data.Value;
            }
        }

        public override void NotifyAll()
        {
            Notify(nameof(HeaderKey));
            Notify(nameof(HeaderValue));
        }
    }
}

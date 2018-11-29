using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Inventor;

namespace StandardPartProcessing
{
    [Guid("575A7C50-A78F-4FE3-AEF9-2C16B37D7924")]
    public class Server : ApplicationAddInServer
    {
        private InventorServer m_InventorServer = null;
        private CProcessor m_Processor = null;

        public dynamic Automation
        {
            get { return m_Processor; }
        }

        public void Activate(
            ApplicationAddInSite site,
            bool firstTime)
        {
            m_InventorServer = site.InventorServer;
            m_Processor = new CProcessor(m_InventorServer);
        }

        public void Deactivate()
        {
            Marshal.ReleaseComObject(m_InventorServer);
            m_InventorServer = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int commandID)
        {
        }
    }
}

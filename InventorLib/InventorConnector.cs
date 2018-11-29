using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using Inventor;

namespace InventorLib
{
    public class InventorConnector : IDisposable
    {
        #region DLL Imports

        [DllImport("oleaut32.dll", PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [System.Security.SecurityCritical]  
        private static extern void GetActiveObject(
            ref Guid rclsid, 
            IntPtr reserved, 
            [MarshalAs(UnmanagedType.Interface)] out Object ppunk);

        [DllImport("oleaut32.dll", PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [System.Security.SecurityCritical]
        private static extern void CLSIDFromProgID(
            [MarshalAs(UnmanagedType.LPWStr)]
            String progId, out Guid clsid);

        [DllImport("oleaut32.dll", PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [System.Security.SecurityCritical]  
        private static extern void CLSIDFromProgIDEx(
            [MarshalAs(UnmanagedType.LPWStr)] String progId, 
            out Guid clsid);

        #endregion

        [System.Security.SecurityCritical]  
        public static Object GetActiveObject(String progID)
        {
            Object obj = null;
            Guid clsid;

            try
            {
                CLSIDFromProgIDEx(progID, out clsid);
            }
            catch (Exception)
            {
                CLSIDFromProgID(progID, out clsid);
            }

            GetActiveObject(ref clsid, IntPtr.Zero, out obj);
            return obj;
        }

        Application _Instance;
        bool _CreatedByUs;
        const string PROG_ID = "Inventor.Application";

        public InventorConnector()
        {
        }

        public InventorServer GetInventorServer()
        {
            Connect();
            return _Instance as InventorServer;
        }

        private void Connect()
        {
            if (_Instance == null)
            {
                _Instance = TryConnectToRunningInstance();

                if (_Instance == null)
                {
                    _Instance = TryCreateInstance();
                    _CreatedByUs = _Instance != null;
                }

                if (_Instance == null)
                {
                    throw new ApplicationException("Could not connect to Inventor.");
                }
            }
        }

        private static Application TryCreateInstance()
        {
            Console.WriteLine("Trying to create instance of Inventor...");
            Application app = null;
            try
            {
                Type type = Type.GetTypeFromProgID(PROG_ID);
                app = Activator.CreateInstance(type) as Application;
                Console.WriteLine($"Connected to Inventor {app.SoftwareVersion.DisplayName}");

                // show Inventor UI
                app.Visible = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"No running Inventor instance... ({e.Message})");
            }
            return app;
        }
        private static Application TryConnectToRunningInstance()
        {
            Console.WriteLine("Trying to connect to Inventor...");
            Application app = null;
            try
            {
                app = GetActiveObject(PROG_ID) as Application;
                Console.WriteLine($"Connected to Inventor {app.SoftwareVersion.DisplayName}");
            }
            catch /*(Exception e)*/
            {
                //Console.WriteLine($"Could not connect to running Inventor Instance... ({e.Message})");
            }
            return app;
        }

        public void Dispose()
        {
            if (_Instance != null)
            {
                Console.WriteLine("Closing all documents...");
                _Instance.Documents.CloseAll(UnreferencedOnly: false);

                if (_CreatedByUs)
                {
                    // Uncomment to close the Inventor instance
                    //_Instance.Quit();
                }

                Console.WriteLine("Detaching from Inventor...");
                Marshal.ReleaseComObject(_Instance);
                _Instance = null;
            }
        }
    }
}

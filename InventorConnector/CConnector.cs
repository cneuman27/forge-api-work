using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InventorConnector
{
    public class CConnector : IDisposable
    {
        private Inventor.Application _Instance;
        private bool _CreatedByUs;
        private const string PROG_ID = "Inventor.Application";

        public CConnector()
        {
        }

        public Inventor.InventorServer GetInventorServer()
        {
            Connect();

            return _Instance as Inventor.InventorServer;
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
                    throw new ApplicationException("Could not connect to Inventor.");
            }
        }
        private static Inventor.Application TryCreateInstance()
        {
            Console.WriteLine("Trying to create instance of Inventor...");

            Inventor.Application app = null;

            try
            {
                Type type = Type.GetTypeFromProgID(PROG_ID);
                app = Activator.CreateInstance(type) as Inventor.Application;

                Console.WriteLine($"Connected to Inventor {app.SoftwareVersion.DisplayName}");

                app.Visible = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"No running Inventor instance... ({e.Message})");
            }

            return app;
        }
        private static Inventor.Application TryConnectToRunningInstance()
        {
            Console.WriteLine("Trying to connect to Inventor...");
            Inventor.Application app = null;
            try
            {
                app = Marshal.GetActiveObject(PROG_ID) as Inventor.Application;

                Console.WriteLine($"Connected to Inventor {app.SoftwareVersion.DisplayName}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not connect to running Inventor Instance... ({e.Message})");
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

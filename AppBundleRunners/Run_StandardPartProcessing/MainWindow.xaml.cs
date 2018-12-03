using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO.Compression;

using Newtonsoft.Json;

using MOMControls;
using MOMShared;

namespace Run_StandardPartProcessing
{
    public partial class MainWindow : CMOMForm
    {
        private bool m_InProcess = true;
        private string m_InputFileLocation = "";
        private string m_OutputFolderLocation = "";

        private StandardPartProcessing.CProcessor m_Processor = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void DoButtons()
        {
            if (m_InProcess == true)
            {
                txtInputFile.DisableControl();
                btnSelectInputFile.DisableControl();

                txtOutputFolder.DisableControl();
                btnSelectOutputFolder.DisableControl();

                btnAPILog.DisableControl();
                btnRun.DisableControl();
            }
            else
            {
                txtInputFile.EnableControl();
                btnSelectInputFile.EnableControl();

                txtOutputFolder.EnableControl();
                btnSelectOutputFolder.EnableControl();

                if (m_Processor != null &&
                    m_Processor.APILog != null &&
                    m_Processor.APILog.Count > 0)
                {
                    btnAPILog.EnableControl();
                }
                else
                {
                    btnAPILog.DisableControl();
                }

                if (System.IO.File.Exists(m_InputFileLocation) == false ||
                    string.IsNullOrWhiteSpace(m_OutputFolderLocation))
                {
                    btnRun.DisableControl();
                }
                else
                {
                    btnRun.EnableControl();
                }
            }
        }

        private void DoRun(object state)
        {
            StandardPartProcessing.Inputs.CInputs inputs;
            string tmpDir = "";
            string workingDir;

            try
            {
                m_InProcess = true;
                DoButtons();

                lblStatus.SetStatus("Initializing");
                lblStatus.ShowStatus();

                workingDir = System.IO.Path.GetDirectoryName(m_InputFileLocation);

                inputs = JsonConvert.DeserializeObject<StandardPartProcessing.Inputs.CInputs>(
                    System.IO.File.ReadAllText(m_InputFileLocation));

                #region Sanity Checks

                if (System.IO.File.Exists(System.IO.Path.Combine(workingDir, inputs.IPTFile)) == false)
                {
                    throw new Exception("Unable To Locate IPT File - Must be in same folder as Input File");
                }

                if (System.IO.File.Exists(System.IO.Path.Combine(workingDir, inputs.XLSFile)) == false)
                {
                    throw new Exception("Unable To Locate XLS File - Must be in same folder as Input File");
                }

                if (System.IO.File.Exists(System.IO.Path.Combine(workingDir, inputs.IDWFile)) == false)
                {
                    throw new Exception("Unable To Locate IDW File - Must be in same folder as Input File");
                }

                #endregion

                #region Copy Files To Temp Folder

                lblStatus.SetStatus("Copying Files To Temp Folder");

                tmpDir = GetTempFolder();

                System.IO.File.Copy(
                    m_InputFileLocation,
                    System.IO.Path.Combine(tmpDir, StandardPartProcessing.CConstants.INPUT_FILE_NAME));

                System.IO.File.Copy(
                    System.IO.Path.Combine(workingDir, inputs.IPTFile),
                    System.IO.Path.Combine(tmpDir, inputs.IPTFile));

                System.IO.File.Copy(
                    System.IO.Path.Combine(workingDir, inputs.XLSFile),
                    System.IO.Path.Combine(tmpDir, inputs.XLSFile));

                System.IO.File.Copy(
                    System.IO.Path.Combine(workingDir, inputs.IDWFile),
                    System.IO.Path.Combine(tmpDir, inputs.IDWFile));

                #endregion

                #region Create Zip File

                lblStatus.SetStatus("Creating Zip File");

                if (System.IO.Directory.Exists(m_OutputFolderLocation) == false)
                {
                    System.IO.Directory.CreateDirectory(m_OutputFolderLocation);
                }

                ZipFile.CreateFromDirectory(
                    tmpDir,
                    System.IO.Path.Combine(m_OutputFolderLocation, $"{inputs.PartNumber}.zip"));

                #endregion

                lblStatus.SetStatus("Starting Processor");

                using (var connector = new InventorConnector.CConnector())
                {
                    Inventor.InventorServer server;
                    Inventor.NameValueMap args;
                    
                    server = connector.GetInventorServer();

                    args = server.TransientObjects.CreateNameValueMap();
                    args.Add(
                        "_1",
                        System.IO.Path.Combine(m_OutputFolderLocation, $"{inputs.PartNumber}.zip"));

                    m_Processor = new StandardPartProcessing.CProcessor(
                        server,
                        new StandardPartProcessing.CProcessor.DelegateSetStatus(SetStatus));

                    m_Processor.RunWithArguments(null, args);
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            finally
            {
                lblStatus.SetStatus("Cleaning");

                if (string.IsNullOrWhiteSpace(tmpDir) == false &&
                    System.IO.Directory.Exists(tmpDir))
                {
                    System.IO.Directory.Delete(tmpDir, true);
                }

                lblStatus.HideStatus();

                m_InProcess = false;
                DoButtons();
            }
        }
        
        private string GetTempFolder()
        {
            string tmp = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                System.IO.Path.GetRandomFileName());

            System.IO.Directory.CreateDirectory(tmp);

            return tmp;
        }
        private void SetStatus(string msg)
        {
            lblStatus.SetStatus(msg);
        }

        #region Control Events

        private void btnSelectInputFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fd;

            m_InProcess = true;
            DoButtons();

            fd = new System.Windows.Forms.OpenFileDialog();
            fd.FileName = m_InputFileLocation;
            fd.Filter = "Input Files (*.json)|*.json";
            fd.Multiselect = false;
            fd.RestoreDirectory = false;
            fd.Title = "Select Input File";

            if (fd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                m_InputFileLocation = fd.FileName;
                txtInputFile.SetText(m_InputFileLocation);
            }

            m_InProcess = false;
            DoButtons();
        }
        private void btnSelectOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fd;

            m_InProcess = true;
            DoButtons();

            fd = new System.Windows.Forms.FolderBrowserDialog();
            fd.Description = "Select Output Folder";
            fd.SelectedPath = m_OutputFolderLocation;
            fd.ShowNewFolderButton = true;

            if (fd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                m_OutputFolderLocation = fd.SelectedPath;
                txtOutputFolder.SetText(m_OutputFolderLocation);
            }

            m_InProcess = false;
            DoButtons();
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            m_InProcess = true;
            DoButtons();

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoRun));
        }
        private void btnAPILog_Click(object sender, RoutedEventArgs e)
        {
            APILog.CFrmAPILog frm;

            m_InProcess = true;
            DoButtons();

            frm = new APILog.CFrmAPILog();
            frm.Start(
                this,
                m_Processor.APILog);

            m_InProcess = false;
            DoButtons();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseForm();
        }

        private void CMOMForm_Loaded(object sender, RoutedEventArgs e)
        {
            txtInputFile.SetText("N/A");
            txtOutputFolder.SetText("N/A");

            lblStatus.HideStatus();

            m_InProcess = false;
            DoButtons();
        }

        #endregion
    }
}

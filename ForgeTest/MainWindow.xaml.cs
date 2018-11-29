using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Net.Http;

using Autofac;
using Newtonsoft.Json.Linq;
using Infragistics.Windows.DataPresenter;

using MOMControls;
using MOMShared;
using ForgeAPI.Autofac;

namespace ForgeTest
{
    public partial class MainWindow : CMOMForm
    {
        private bool m_InProcess = true;
        private bool m_InProcess_Derivative_Model = false;
        private bool m_InProcess_Derivative_Drawing = false;

        private ObservableCollection<Types.CResultWrapper> m_APILogList = new ObservableCollection<Types.CResultWrapper>();

        private IContainer m_IoC = null;

        private string m_Upload_IPTLocation = "";
        private string m_Upload_XLSLocation = "";
        private string m_Upload_IDWLocation = "";

        private string m_UploadIPT_DownloadURI = "";
        private string m_UploadIDW_DownloadURI = "";

        private PartDB.CPart m_ViewerPart = null;

        private ForgeAPI.Interface.Authentication.IToken m_AuthenticationToken = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void DoButtons()
        {
            if (m_InProcess == true)
            {
                #region Bucket Management

                btnRefreshBucketList.DisableControl();
                btnNewBucket.DisableControl();
                btnDeleteBucket.DisableControl();

                #endregion

                #region Upload

                txtUploadPartNumber.DisableControl();
                cmbBuckets.DisableControl();
                btnRefreshUploadBuckets.DisableControl();
                txtIPTFile.DisableControl();
                btnSelectIPT.DisableControl();
                txtXLSFile.DisableControl();
                btnSelectXLS.DisableControl();
                txtIDWFile.DisableControl();
                btnSelectIDW.DisableControl();
                btnUploadToBucket.DisableControl();

                txtUploadResultsIPT_BucketKey.DisableControl();
                txtUploadResultsIPT_ObjectID.DisableControl();
                txtUploadResultsIPT_ObjectKey.DisableControl();
                txtUploadResultsIPT_SHA1.DisableControl();
                txtUploadResultsIPT_ObjectSize.DisableControl();
                txtUploadResultsIPT_ContentType.DisableControl();
                txtUploadResultsIPT_DownloadURI.DisableControl();
                btnUploadResultsIPT_DownloadFile.DisableControl();

                txtUploadResultsIDW_BucketKey.DisableControl();
                txtUploadResultsIDW_ObjectID.DisableControl();
                txtUploadResultsIDW_ObjectKey.DisableControl();
                txtUploadResultsIDW_SHA1.DisableControl();
                txtUploadResultsIDW_ObjectSize.DisableControl();
                txtUploadResultsIDW_ContentType.DisableControl();
                txtUploadResultsIDW_DownloadURI.DisableControl();
                btnUploadResultsIDW_DownloadFile.DisableControl();

                #endregion

                #region Derivative Processing

                txtDerivativeURN_Model.DisableControl();
                txtDerivativeEncodedURN_Model.DisableControl();
                btnDerivativeRun_Model.DisableControl();
                txtDerivativeStatus_Model.DisableControl();
                btnDerivativeRefeshStatus_Model.DisableControl();

                txtDerivativeURN_Drawing.DisableControl();
                txtDerivativeEncodedURN_Drawing.DisableControl();
                btnDerivativeRun_Drawing.DisableControl();
                txtDerivativeStatus_Drawing.DisableControl();
                btnDerivativeRefeshStatus_Drawing.DisableControl();

                #endregion

                #region Viewer

                txtViewerPartNumber.DisableControl();
                btnViewerSearch.DisableControl();

                btnExportHTML_Model.DisableControl();
                btnExportHTML_Drawing.DisableControl();

                #endregion

                txtPartDB.DisableControl();
            }
            else
            {
                #region Bucket Management

                btnRefreshBucketList.EnableControl();
                btnNewBucket.EnableControl();

                if (gridBucketManagement.CountSelectedItems() > 0)
                {
                    btnDeleteBucket.EnableControl();
                }
                else
                {
                    btnDeleteBucket.DisableControl();
                }

                #endregion

                #region Upload

                txtUploadPartNumber.EnableControl();
                cmbBuckets.EnableControl();
                btnRefreshUploadBuckets.EnableControl();

                txtIPTFile.EnableControl();
                btnSelectIPT.EnableControl();
                txtXLSFile.EnableControl();
                btnSelectXLS.EnableControl();
                txtIDWFile.EnableControl();
                btnSelectIDW.EnableControl();

                if (string.IsNullOrWhiteSpace(txtUploadPartNumber.GetText()) == false &&
                    cmbBuckets.GetSelectedItem() != null &&
                    System.IO.File.Exists(m_Upload_IPTLocation) &&
                    System.IO.File.Exists(m_Upload_XLSLocation) &&
                    System.IO.File.Exists(m_Upload_IDWLocation))
                {
                    btnUploadToBucket.EnableControl();
                }
                else
                {
                    btnUploadToBucket.DisableControl();
                }

                #region IPT Results

                txtUploadResultsIPT_BucketKey.EnableControl();
                txtUploadResultsIPT_ObjectID.EnableControl();
                txtUploadResultsIPT_ObjectKey.EnableControl();
                txtUploadResultsIPT_SHA1.EnableControl();
                txtUploadResultsIPT_ObjectSize.EnableControl();
                txtUploadResultsIPT_ContentType.EnableControl();
                txtUploadResultsIPT_DownloadURI.EnableControl();

                if (string.IsNullOrWhiteSpace(m_UploadIPT_DownloadURI))
                {
                    btnUploadResultsIPT_DownloadFile.DisableControl();
                }
                else
                {
                    btnUploadResultsIPT_DownloadFile.EnableControl();
                }

                #endregion

                #region IDW Results

                txtUploadResultsIDW_BucketKey.EnableControl();
                txtUploadResultsIDW_ObjectID.EnableControl();
                txtUploadResultsIDW_ObjectKey.EnableControl();
                txtUploadResultsIDW_SHA1.EnableControl();
                txtUploadResultsIDW_ObjectSize.EnableControl();
                txtUploadResultsIDW_ContentType.EnableControl();
                txtUploadResultsIDW_DownloadURI.EnableControl();

                if (string.IsNullOrWhiteSpace(m_UploadIDW_DownloadURI))
                {
                    btnUploadResultsIDW_DownloadFile.DisableControl();
                }
                else
                {
                    btnUploadResultsIDW_DownloadFile.EnableControl();
                }

                #endregion

                #endregion

                #region Derivative Processing

                #region Model

                txtDerivativeURN_Model.EnableControl();
                txtDerivativeEncodedURN_Model.EnableControl();
                txtDerivativeStatus_Model.EnableControl();

                if (m_InProcess_Derivative_Model)
                {
                    btnDerivativeRun_Model.DisableControl();
                    btnDerivativeRefeshStatus_Model.EnableControl();
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(txtDerivativeURN_Model.GetText()) ||
                        txtDerivativeURN_Model.GetText() == "N/A")
                    {
                        btnDerivativeRun_Model.DisableControl();
                    }
                    else
                    {
                        btnDerivativeRun_Model.EnableControl();
                    }

                    btnDerivativeRefeshStatus_Model.DisableControl();
                }

                #endregion

                #region Drawing

                txtDerivativeURN_Drawing.EnableControl();
                txtDerivativeEncodedURN_Drawing.EnableControl();
                txtDerivativeStatus_Drawing.EnableControl();

                if (m_InProcess_Derivative_Drawing)
                {
                    btnDerivativeRun_Drawing.DisableControl();
                    btnDerivativeRefeshStatus_Drawing.EnableControl();
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(txtDerivativeURN_Drawing.GetText()) ||
                        txtDerivativeURN_Drawing.GetText() == "N/A")
                    {
                        btnDerivativeRun_Drawing.DisableControl();
                    }
                    else
                    {
                        btnDerivativeRun_Drawing.EnableControl();
                    }

                    btnDerivativeRefeshStatus_Drawing.DisableControl();
                }

                #endregion

                #endregion

                #region Viewer

                txtViewerPartNumber.EnableControl();

                if (txtViewerPartNumber.GetText().Trim().Length > 0)
                {
                    btnViewerSearch.EnableControl();
                }
                else
                {
                    btnViewerSearch.DisableControl();
                }

                if (m_ViewerPart != null)
                {
                    btnExportHTML_Model.EnableControl();
                    btnExportHTML_Drawing.EnableControl();
                }
                else
                {
                    btnExportHTML_Model.DisableControl();
                    btnExportHTML_Drawing.DisableControl();
                }

                #endregion

                txtPartDB.EnableControl();
            }
        }
        private void AddAPIResult(ForgeAPI.Interface.REST.IResult result)
        {
            if (gridAPILog.Dispatcher.CheckAccess() == false)
            {
                Action<ForgeAPI.Interface.REST.IResult> action = new Action<ForgeAPI.Interface.REST.IResult>(AddAPIResult);

                gridAPILog.Dispatcher.Invoke(action, result);

                return;
            }

            Types.CResultWrapper item;

            item = new Types.CResultWrapper(result);

            m_APILogList.Add(item);

            gridAPILog.RefreshSort();
            gridAPILog.SelectAndActivateItem(item, null);
        }

        private void DoLoad(object state)
        {
            ContainerBuilder builder;
            ForgeAPI.Interface.Authentication.IService authService;

            try
            {
                m_InProcess = true;
                DoButtons();

                lblStatus.SetStatus("Loading");
                lblStatus.ShowStatus();

                lblStatus.SetStatus("Generating IoC Container");

                builder = new ContainerBuilder();
                builder.AddForgeAPI();

                m_IoC = builder.Build();

                lblStatus.SetStatus("Authorizing");

                authService = m_IoC.Resolve<ForgeAPI.Interface.Authentication.IService>();

                m_AuthenticationToken = authService.Authenticate(
                    new List<ForgeAPI.Interface.Enums.E_AccessScope>()
                    {
                        ForgeAPI.Interface.Enums.E_AccessScope.Bucket_Create,
                        ForgeAPI.Interface.Enums.E_AccessScope.Bucket_Delete,
                        ForgeAPI.Interface.Enums.E_AccessScope.Bucket_Read,
                        ForgeAPI.Interface.Enums.E_AccessScope.Data_Read,
                        ForgeAPI.Interface.Enums.E_AccessScope.Data_Write
                    });

                viewerModel.SetAccessToken(m_AuthenticationToken.AccessToken);
                viewerDrawing.SetAccessToken(m_AuthenticationToken.AccessToken);

                lblStatus.SetStatus("Initializing Lists");

                gridAPILog.SetDataSource(m_APILogList);
                gridBucketManagement.SetDataSource(new ObservableCollection<Types.CBucketWrapper>());

                lblStatus.SetStatus("Loading Part DB");

                PartDB.PartDB.LoadPartDB();
                LoadPartDBTab();
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
                lblStatus.CollapseStatus();

                m_InProcess = false;
                DoButtons();
            }
        }
        private void DoRefreshBucketLists(object state)
        {
            ForgeAPI.Interface.IFactory factory;
            ForgeAPI.Interface.DataManagement.Buckets.GetBuckets.IInputs inputs;
            ForgeAPI.Interface.DataManagement.Buckets.GetBuckets.IOutputs outputs;
            ForgeAPI.Interface.DataManagement.Buckets.IService service;
            Types.CBucketWrapper selected;

            ObservableCollection<Types.CBucketWrapper> list;

            try
            {
                m_InProcess = true;
                DoButtons();

                lblStatus.SetStatus("Refreshing Bucket List");
                lblStatus.ShowStatus();

                factory = m_IoC.Resolve<ForgeAPI.Interface.IFactory>();
                service = m_IoC.Resolve<ForgeAPI.Interface.DataManagement.Buckets.IService>();

                inputs = factory.CreateInputs<ForgeAPI.Interface.DataManagement.Buckets.GetBuckets.IInputs>(m_AuthenticationToken);

                inputs.Limit = 10;
                inputs.PaginationOffsetBucketKey = "";

                outputs = service.GetBuckets(inputs);

                AddAPIResult(outputs.Result);

                if (outputs.Success() == false)
                {
                    throw new Exception(outputs.FailureReason());
                }

                list = new ObservableCollection<Types.CBucketWrapper>();

                outputs.BucketList.ForEach(i => list.Add(new Types.CBucketWrapper(i)));

                gridBucketManagement.SetDataSource(list);

                selected = cmbBuckets.GetSelectedItem() as Types.CBucketWrapper;
                cmbBuckets.SetItemsSource(list);
                if (selected != null)
                {
                    selected = list.ToList().Find(i => i.BucketKey == selected.BucketKey);
                }
                cmbBuckets.SetSelectedItem(selected);

                lblStatus.SetStatus("Bucket List Refresh Complete");
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
                lblStatus.CollapseStatus();

                m_InProcess = false;
                DoButtons();
            }
        }
        private void DoDeleteBucket(object state)
        {
            Types.CBucketWrapper bucket;
            ForgeAPI.Interface.IFactory factory;
            ForgeAPI.Interface.DataManagement.Buckets.DeleteBucket.IInputs inputs;
            ForgeAPI.Interface.DataManagement.Buckets.DeleteBucket.IOutputs outputs;
            ForgeAPI.Interface.DataManagement.Buckets.IService service;

            try
            {
                bucket = gridBucketManagement.GetSelectedItems<Types.CBucketWrapper>()[0];

                lblStatus.SetStatus($"Deleting Bucket [{bucket.BucketKey}]");
                lblStatus.ShowStatus();

                factory = m_IoC.Resolve<ForgeAPI.Interface.IFactory>();
                service = m_IoC.Resolve<ForgeAPI.Interface.DataManagement.Buckets.IService>();

                inputs = factory.CreateInputs<ForgeAPI.Interface.DataManagement.Buckets.DeleteBucket.IInputs>(m_AuthenticationToken);
                inputs.BucketKey = bucket.BucketKey;

                outputs = service.DeleteBucket(inputs);

                AddAPIResult(outputs.Result);

                if (!outputs.Success())
                {
                    throw new Exception(outputs.FailureReason());
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
                DoRefreshBucketLists(state);
            }
        }
        private void DoUpload(object state)
        {
            string tmpFolderSource = "";
            string tmpFolderZipIPT = "";
            string tmpFolderZipIDW = "";
            Types.CBucketWrapper bucket;

            ForgeAPI.Interface.IFactory factory;
            ForgeAPI.Interface.DataManagement.Objects.UploadObject.IInputs inputs;
            ForgeAPI.Interface.DataManagement.Objects.UploadObject.IOutputs outputs;
            ForgeAPI.Interface.DataManagement.Objects.IService service;
            ForgeAPI.Interface.Utility.IService utility;

            try
            {
                m_InProcess = true;
                DoButtons();

                lblStatus.SetStatus("Starting Upload");
                lblStatus.ShowStatus();

                ResetUploadResults();

                #region Copy Files To Temp Folder

                lblStatus.SetStatus("Copying Files To Temp Folder");

                tmpFolderSource = CreateTempFolder();

                System.IO.File.Copy(
                    m_Upload_IPTLocation,
                    tmpFolderSource + System.IO.Path.GetFileName(m_Upload_IPTLocation));

                System.IO.File.Copy(
                    m_Upload_XLSLocation,
                    tmpFolderSource + System.IO.Path.GetFileName(m_Upload_XLSLocation));

                System.IO.File.Copy(
                    m_Upload_IDWLocation,
                    tmpFolderSource + System.IO.Path.GetFileName(m_Upload_IDWLocation));

                #endregion

                #region Model

                #region Create Zip File 

                lblStatus.SetStatus("Creating Zip File [MODEL]");

                tmpFolderZipIPT = CreateTempFolder();

                System.IO.Compression.ZipFile.CreateFromDirectory(
                    tmpFolderSource,
                    tmpFolderZipIPT + System.IO.Path.GetFileNameWithoutExtension(m_Upload_IPTLocation) + "_MODEL.zip");

                #endregion

                #region Upload

                lblStatus.SetStatus("Uploading Zip File To Bucket [MODEL]");

                bucket = cmbBuckets.GetSelectedItem() as Types.CBucketWrapper;

                factory = m_IoC.Resolve<ForgeAPI.Interface.IFactory>();
                inputs = factory.CreateInputs<ForgeAPI.Interface.DataManagement.Objects.UploadObject.IInputs>(m_AuthenticationToken);

                //inputs.FileName = System.IO.Path.GetFileNameWithoutExtension(m_Upload_IPTLocation) + "_MODEL.zip";
                //inputs.FileData = System.IO.File.ReadAllBytes(tmpFolderZipIPT + inputs.FileName);
                inputs.FileName = System.IO.Path.GetFileName(m_Upload_IPTLocation);
                inputs.FileData = System.IO.File.ReadAllBytes(m_Upload_IPTLocation);
                inputs.ContentType = "application/octet-stream";
                inputs.BucketKey = bucket.BucketKey;
                inputs.ObjectName = inputs.FileName;

                service = m_IoC.Resolve<ForgeAPI.Interface.DataManagement.Objects.IService>();

                outputs = service.UploadObject(inputs);

                AddAPIResult(outputs.Result);

                if (outputs.Success() == false)
                {
                    throw new Exception(outputs.FailureReason());
                }

                txtUploadResultsIPT_BucketKey.SetText(outputs.BucketKey.Trim());
                txtUploadResultsIPT_ObjectID.SetText(outputs.ObjectID.Trim());
                txtUploadResultsIPT_ObjectKey.SetText(outputs.ObjectKey.Trim());
                txtUploadResultsIPT_SHA1.SetText(outputs.SHA1.Trim());
                txtUploadResultsIPT_ObjectSize.SetText(outputs.ObjectSize.ToString());
                txtUploadResultsIPT_ContentType.SetText(outputs.ContentType.Trim());
                txtUploadResultsIPT_DownloadURI.SetText(outputs.DownloadURI.Trim());

                m_UploadIPT_DownloadURI = outputs.DownloadURI.Trim();

                #endregion

                utility = m_IoC.Resolve<ForgeAPI.Interface.Utility.IService>();

                txtDerivativeURN_Model.SetText(outputs.ObjectID);
                txtDerivativeEncodedURN_Model.SetText(utility.ConvertToBase64(outputs.ObjectID));

                #endregion

                #region Drawing

                #region Create Zip File 

                lblStatus.SetStatus("Creating Zip File [DRAWING]");

                tmpFolderZipIDW = CreateTempFolder();

                System.IO.Compression.ZipFile.CreateFromDirectory(
                    tmpFolderSource,
                    tmpFolderZipIDW + System.IO.Path.GetFileNameWithoutExtension(m_Upload_IPTLocation) + "_DRAWING.zip");

                #endregion

                #region Upload

                lblStatus.SetStatus("Uploading Zip File To Bucket [DRAWING]");

                bucket = cmbBuckets.GetSelectedItem() as Types.CBucketWrapper;

                factory = m_IoC.Resolve<ForgeAPI.Interface.IFactory>();
                inputs = factory.CreateInputs<ForgeAPI.Interface.DataManagement.Objects.UploadObject.IInputs>(m_AuthenticationToken);

                inputs.FileName = System.IO.Path.GetFileNameWithoutExtension(m_Upload_IPTLocation) + "_DRAWING.zip";
                inputs.FileData = System.IO.File.ReadAllBytes(tmpFolderZipIDW + inputs.FileName);
                inputs.ContentType = "application/octet-stream";
                inputs.BucketKey = bucket.BucketKey;
                inputs.ObjectName = inputs.FileName;

                service = m_IoC.Resolve<ForgeAPI.Interface.DataManagement.Objects.IService>();

                outputs = service.UploadObject(inputs);

                AddAPIResult(outputs.Result);

                if (outputs.Success() == false)
                {
                    throw new Exception(outputs.FailureReason());
                }

                txtUploadResultsIDW_BucketKey.SetText(outputs.BucketKey.Trim());
                txtUploadResultsIDW_ObjectID.SetText(outputs.ObjectID.Trim());
                txtUploadResultsIDW_ObjectKey.SetText(outputs.ObjectKey.Trim());
                txtUploadResultsIDW_SHA1.SetText(outputs.SHA1.Trim());
                txtUploadResultsIDW_ObjectSize.SetText(outputs.ObjectSize.ToString());
                txtUploadResultsIDW_ContentType.SetText(outputs.ContentType.Trim());
                txtUploadResultsIDW_DownloadURI.SetText(outputs.DownloadURI.Trim());

                m_UploadIDW_DownloadURI = outputs.DownloadURI.Trim();

                #endregion

                utility = m_IoC.Resolve<ForgeAPI.Interface.Utility.IService>();

                txtDerivativeURN_Drawing.SetText(outputs.ObjectID);
                txtDerivativeEncodedURN_Drawing.SetText(utility.ConvertToBase64(outputs.ObjectID));

                #endregion

                lblStatus.SetStatus("Saving Part Record");

                PartDB.CPart part;

                part = new PartDB.CPart();
                part.PartNumber = txtUploadPartNumber.GetText();
                part.ModelURN = txtUploadResultsIPT_ObjectID.GetText();
                part.DrawingURN = txtUploadResultsIDW_ObjectID.GetText();

                PartDB.PartDB.SavePart(part);

                lblStatus.SetStatus("Saving Part Database");

                PartDB.PartDB.SavePartDB();
                LoadPartDBTab();

                lblStatus.SetStatus("Upload Complete");
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
                if (string.IsNullOrWhiteSpace(tmpFolderSource) == false &&
                    System.IO.Directory.Exists(tmpFolderSource))
                {
                    System.IO.Directory.Delete(tmpFolderSource, true);
                }

                if (string.IsNullOrWhiteSpace(tmpFolderZipIPT) == false &&
                    System.IO.Directory.Exists(tmpFolderZipIPT))
                {
                    System.IO.Directory.Delete(tmpFolderZipIPT, true);
                }

                if (string.IsNullOrWhiteSpace(tmpFolderZipIDW) == false &&
                    System.IO.Directory.Exists(tmpFolderZipIDW))
                {
                    System.IO.Directory.Delete(tmpFolderZipIDW, true);
                }

                lblStatus.CollapseStatus();

                m_InProcess = false;
                DoButtons();
            }
        }
        private void DoDownloadObjectURI_Model(object state)
        {
            string loc;

            ForgeAPI.Interface.IFactory factory;
            ForgeAPI.Interface.DataManagement.Objects.DownloadObjectURI.IInputs inputs;
            ForgeAPI.Interface.DataManagement.Objects.DownloadObjectURI.IOutputs outputs;
            ForgeAPI.Interface.DataManagement.Objects.IService service;

            try
            {
                m_InProcess = true;
                DoButtons();

                loc = state as string;

                lblStatus.SetStatus("Downloading From Object URI [MODEL]");
                lblStatus.ShowStatus();

                factory = m_IoC.Resolve<ForgeAPI.Interface.IFactory>();
                inputs = factory.CreateInputs<ForgeAPI.Interface.DataManagement.Objects.DownloadObjectURI.IInputs>(m_AuthenticationToken);
                service = m_IoC.Resolve<ForgeAPI.Interface.DataManagement.Objects.IService>();

                inputs.URI = m_UploadIPT_DownloadURI;

                outputs = service.DownloadObjectURI(inputs);

                AddAPIResult(outputs.Result);

                if (outputs.Success() == false)
                {
                    throw new Exception(outputs.FailureReason());
                }

                lblStatus.SetStatus("Saving File Data");

                System.IO.File.WriteAllBytes(
                    loc,
                    outputs.ObjectData);

                lblStatus.SetStatus("Object Download Complete");
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
                lblStatus.CollapseStatus();

                m_InProcess = false;
                DoButtons();
            }
        }
        private void DoDownloadObjectURI_Drawing(object state)
        {
            string loc;

            ForgeAPI.Interface.IFactory factory;
            ForgeAPI.Interface.DataManagement.Objects.DownloadObjectURI.IInputs inputs;
            ForgeAPI.Interface.DataManagement.Objects.DownloadObjectURI.IOutputs outputs;
            ForgeAPI.Interface.DataManagement.Objects.IService service;

            try
            {
                m_InProcess = true;
                DoButtons();

                loc = state as string;

                lblStatus.SetStatus("Downloading From Object URI [DRAWING]");
                lblStatus.ShowStatus();

                factory = m_IoC.Resolve<ForgeAPI.Interface.IFactory>();
                inputs = factory.CreateInputs<ForgeAPI.Interface.DataManagement.Objects.DownloadObjectURI.IInputs>(m_AuthenticationToken);
                service = m_IoC.Resolve<ForgeAPI.Interface.DataManagement.Objects.IService>();

                inputs.URI = m_UploadIDW_DownloadURI;

                outputs = service.DownloadObjectURI(inputs);

                AddAPIResult(outputs.Result);

                if (outputs.Success() == false)
                {
                    throw new Exception(outputs.FailureReason());
                }

                lblStatus.SetStatus("Saving File Data");

                System.IO.File.WriteAllBytes(
                    loc,
                    outputs.ObjectData);

                lblStatus.SetStatus("Object Download Complete");
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
                lblStatus.CollapseStatus();

                m_InProcess = false;
                DoButtons();
            }
        }
        private void DoRunDerivative_Model(object state)
        {
            ForgeAPI.Interface.IFactory factory;
            ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IInputs inputs;
            ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IOutputFormat_SVF svfFormat;
            ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IOutputs outputs;
            ForgeAPI.Interface.ModelDerivative.Derivatives.IService service;

            try
            {
                m_InProcess = true;
                DoButtons();

                lblStatus.SetStatus("Starting Derivative Processing [MODEL]");
                lblStatus.ShowStatus();

                factory = m_IoC.Resolve<ForgeAPI.Interface.IFactory>();
                service = m_IoC.Resolve<ForgeAPI.Interface.ModelDerivative.Derivatives.IService>();

                inputs = factory.CreateInputs<ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IInputs>(
                    m_AuthenticationToken);

                inputs.ReplaceExistingDerivatives = true;

                inputs.Input.URNEncoded = txtDerivativeEncodedURN_Model.GetText();
                inputs.Input.IsCompressed = false;
                //inputs.Input.IsCompressed = true;
                //inputs.Input.CompressedRootFileName = System.IO.Path.GetFileName(m_Upload_IPTLocation);

                inputs.Output.DestinationSettings.Region = ForgeAPI.Interface.Enums.E_Region.US;

                svfFormat = factory.Create<ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IOutputFormat_SVF>();
                svfFormat.ViewList.Add(ForgeAPI.Interface.Enums.E_SVFViewType._3D);

                inputs.Output.FormatList.Add(svfFormat);

                lblStatus.SetStatus("Adding Derivative Job [MODEL]");

                outputs = service.AddJob(inputs);

                AddAPIResult(outputs.Result);

                if (outputs.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    m_InProcess_Derivative_Model = true;
                    if (outputs.APIStatus.ToUpper() == "SUCCESS")
                    {
                        txtDerivativeStatus_Model.SetText("Derivative Job Added");
                    }
                    else
                    {
                        txtDerivativeStatus_Model.SetText(outputs.APIStatus);
                    }
                }
                else if (outputs.Result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    txtDerivativeStatus_Model.SetText("Derivative Already Exists");
                }
                else
                {
                    txtDerivativeStatus_Model.SetText("API Failure - See API Log");
                }

                lblStatus.SetStatus("Derivative Job Add Complete [MODEL]");
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
                lblStatus.CollapseStatus();

                m_InProcess = false;
                DoButtons();
            }
        }
        private void DoRunDerivative_Drawing(object state)
        {
            ForgeAPI.Interface.IFactory factory;
            ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IInputs inputs;
            ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IOutputFormat_SVF svfFormat;
            ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IOutputs outputs;
            ForgeAPI.Interface.ModelDerivative.Derivatives.IService service;

            try
            {
                m_InProcess = true;
                DoButtons();

                lblStatus.SetStatus("Starting Derivative Processing [DRAWING]");
                lblStatus.ShowStatus();

                factory = m_IoC.Resolve<ForgeAPI.Interface.IFactory>();
                service = m_IoC.Resolve<ForgeAPI.Interface.ModelDerivative.Derivatives.IService>();

                inputs = factory.CreateInputs<ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IInputs>(
                    m_AuthenticationToken);

                inputs.ReplaceExistingDerivatives = true;

                inputs.Input.URNEncoded = txtDerivativeEncodedURN_Drawing.GetText();
                inputs.Input.IsCompressed = true;
                inputs.Input.CompressedRootFileName = System.IO.Path.GetFileName(m_Upload_IDWLocation);

                inputs.Output.DestinationSettings.Region = ForgeAPI.Interface.Enums.E_Region.US;

                svfFormat = factory.Create<ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IOutputFormat_SVF>();
                svfFormat.ViewList.Add(ForgeAPI.Interface.Enums.E_SVFViewType._2D);

                inputs.Output.FormatList.Add(svfFormat);

                lblStatus.SetStatus("Adding Derivative Job [DRAWING]");

                outputs = service.AddJob(inputs);

                AddAPIResult(outputs.Result);

                if (outputs.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    m_InProcess_Derivative_Drawing = true;
                    if (outputs.APIStatus.ToUpper() == "SUCCESS")
                    {
                        txtDerivativeStatus_Drawing.SetText("Derivative Job Added");
                    }
                    else
                    {
                        txtDerivativeStatus_Drawing.SetText(outputs.APIStatus);
                    }
                }
                else if (outputs.Result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    txtDerivativeStatus_Drawing.SetText("Derivative Already Exists");
                }
                else
                {
                    txtDerivativeStatus_Drawing.SetText("API Failure - See API Log");
                }

                lblStatus.SetStatus("Derivative Job Add Complete [DRAWING]");
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
                lblStatus.CollapseStatus();

                m_InProcess = false;
                DoButtons();
            }
        }
        private void DoRefreshDerivative_Model(object state)
        {
            ForgeAPI.Interface.ModelDerivative.Derivatives.IService service;
            ForgeAPI.Interface.IFactory factory;
            ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IInputs inputs;
            ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IOutputs outputs;

            try
            {
                m_InProcess = true;
                DoButtons();

                lblStatus.SetStatus("Starting Derivative Status Refresh [MODEL]");
                lblStatus.ShowStatus();

                factory = m_IoC.Resolve<ForgeAPI.Interface.IFactory>();

                inputs = factory.CreateInputs<ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IInputs>(m_AuthenticationToken);
                inputs.URN = txtDerivativeURN_Model.GetText();
                inputs.URNIsEncoded = false;

                service = m_IoC.Resolve<ForgeAPI.Interface.ModelDerivative.Derivatives.IService>();

                outputs = service.GetManifest(inputs);

                AddAPIResult(outputs.Result);

                if (outputs.Success())
                {
                    if (outputs.Status == ForgeAPI.Interface.Enums.E_TranslationStatus.InProgress &&
                        string.IsNullOrWhiteSpace(outputs.Progress) == false)
                    {
                        txtDerivativeStatus_Model.SetText(outputs.Progress);
                    }
                    else
                    {
                        txtDerivativeStatus_Model.SetText(outputs.Status.ToString());
                    }

                    if (outputs.Status == ForgeAPI.Interface.Enums.E_TranslationStatus.Failed ||
                        outputs.Status == ForgeAPI.Interface.Enums.E_TranslationStatus.Success ||
                        outputs.Status == ForgeAPI.Interface.Enums.E_TranslationStatus.Timeout)
                    {
                        m_InProcess_Derivative_Model = false;
                    }
                }
                else
                {
                    txtDerivativeStatus_Model.SetText("API Failure - See API Log");
                }

                lblStatus.SetStatus("Derivative Status Refresh Complete [MODEL]");
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
                lblStatus.CollapseStatus();

                m_InProcess = false;
                DoButtons();
            }
        }
        private void DoRefreshDerivative_Drawing(object state)
        {
            ForgeAPI.Interface.ModelDerivative.Derivatives.IService service;
            ForgeAPI.Interface.IFactory factory;
            ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IInputs inputs;
            ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IOutputs outputs;

            try
            {
                m_InProcess = true;
                DoButtons();

                lblStatus.SetStatus("Starting Derivative Status Refresh [DRAWING]");
                lblStatus.ShowStatus();

                factory = m_IoC.Resolve<ForgeAPI.Interface.IFactory>();

                inputs = factory.CreateInputs<ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IInputs>(m_AuthenticationToken);
                inputs.URN = txtDerivativeURN_Drawing.GetText();
                inputs.URNIsEncoded = false;

                service = m_IoC.Resolve<ForgeAPI.Interface.ModelDerivative.Derivatives.IService>();

                outputs = service.GetManifest(inputs);

                AddAPIResult(outputs.Result);

                if (outputs.Success())
                {
                    if (outputs.Status == ForgeAPI.Interface.Enums.E_TranslationStatus.InProgress &&
                        string.IsNullOrWhiteSpace(outputs.Progress) == false)
                    {
                        txtDerivativeStatus_Drawing.SetText(outputs.Progress);
                    }
                    else
                    {
                        txtDerivativeStatus_Drawing.SetText(outputs.Status.ToString());
                    }

                    if (outputs.Status == ForgeAPI.Interface.Enums.E_TranslationStatus.Failed ||
                        outputs.Status == ForgeAPI.Interface.Enums.E_TranslationStatus.Success ||
                        outputs.Status == ForgeAPI.Interface.Enums.E_TranslationStatus.Timeout)
                    {
                        m_InProcess_Derivative_Drawing = false;
                    }
                }
                else
                {
                    txtDerivativeStatus_Drawing.SetText("API Failure - See API Log");
                }

                lblStatus.SetStatus("Derivative Status Refresh Complete [DRAWING]");
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
                lblStatus.CollapseStatus();

                m_InProcess = false;
                DoButtons();
            }
        }
        private void DoViewerSearch(object state)
        {
            PartDB.CPart part;
            ForgeAPI.Interface.Utility.IService utility;

            try
            {
                m_InProcess = true;
                DoButtons();

                lblStatus.SetStatus("Searching");
                lblStatus.ShowStatus();

                part = PartDB.PartDB.GetPart(txtViewerPartNumber.GetText());
                if (part == null)
                {
                    throw new Exception("Part Not Found");
                }

                m_ViewerPart = part;

                utility = m_IoC.Resolve<ForgeAPI.Interface.Utility.IService>();

                lblStatus.SetStatus("Loading Model Viewer");

                SetModelViewerURN(utility.ConvertToBase64(m_ViewerPart.ModelURN));

                lblStatus.SetStatus("Loading Drawing Viewer");

                SetDrawingViewerURN(utility.ConvertToBase64(m_ViewerPart.DrawingURN));
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
                lblStatus.CollapseStatus();

                m_InProcess = false;
                DoButtons();
            }
        }
        private void DoExportHTML_Model(object state)
        {
            string loc;
            string html;

            try
            {
                m_InProcess = true;
                DoButtons();

                lblStatus.SetStatus("Exporting Model Viewer HTML");
                lblStatus.ShowStatus();

                loc = (string)state;

                html = viewerModel.GetHTML();

                System.IO.File.WriteAllText(
                    loc,
                    html);
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
                lblStatus.CollapseStatus();

                m_InProcess = false;
                DoButtons();
            }
        }
        private void DoExportHTML_Drawing(object state)
        {
            string loc;
            string html;

            try
            {
                m_InProcess = true;
                DoButtons();

                lblStatus.SetStatus("Exporting Drawing Viewer HTML");
                lblStatus.ShowStatus();

                loc = (string)state;

                html = viewerDrawing.GetHTML();

                System.IO.File.WriteAllText(
                    loc,
                    html);
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
                lblStatus.CollapseStatus();

                m_InProcess = false;
                DoButtons();
            }
        }

        private string CreateTempFolder()
        {
            string tmp;

            tmp = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                System.IO.Path.GetRandomFileName());

            System.IO.Directory.CreateDirectory(tmp);

            if (tmp.EndsWith("\\") == false)
            {
                tmp += "\\";
            }

            return tmp;
        }
        private void ResetUploadResults()
        {
            #region Model

            m_UploadIPT_DownloadURI = "";

            txtUploadResultsIPT_BucketKey.SetText("N/A");
            txtUploadResultsIPT_ObjectID.SetText("N/A");
            txtUploadResultsIPT_ObjectKey.SetText("N/A");
            txtUploadResultsIPT_SHA1.SetText("N/A");
            txtUploadResultsIPT_ObjectSize.SetText("N/A");
            txtUploadResultsIPT_ContentType.SetText("N/A");
            txtUploadResultsIPT_DownloadURI.SetText("N/A");

            #endregion

            #region Drawing

            m_UploadIDW_DownloadURI = "";

            txtUploadResultsIDW_BucketKey.SetText("N/A");
            txtUploadResultsIDW_ObjectID.SetText("N/A");
            txtUploadResultsIDW_ObjectKey.SetText("N/A");
            txtUploadResultsIDW_SHA1.SetText("N/A");
            txtUploadResultsIDW_ObjectSize.SetText("N/A");
            txtUploadResultsIDW_ContentType.SetText("N/A");
            txtUploadResultsIDW_DownloadURI.SetText("N/A");

            #endregion
        }
        private void SetModelViewerURN(string urn)
        {
            if (viewerModel.Dispatcher.CheckAccess() == false)
            {
                Action<string> action = new Action<string>(SetModelViewerURN);

                viewerModel.Dispatcher.Invoke(action, urn);

                return;
            }

            viewerModel.LoadURN(urn);
        }
        private void SetDrawingViewerURN(string urn)
        {
            if (viewerDrawing.Dispatcher.CheckAccess() == false)
            {
                Action<string> action = new Action<string>(SetDrawingViewerURN);

                viewerDrawing.Dispatcher.Invoke(action, urn);

                return;
            }

            viewerDrawing.LoadURN(urn);
        }
        private void LoadPartDBTab()
        {
            string json;

            json = System.IO.File.ReadAllText(PartDB.PartDB.PART_DB_FILE_NAME);

            txtPartDB.SetText(FormatJSON(json));
        }
        private string FormatJSON(string txt)
        {
            try
            {
                return Newtonsoft.Json.Linq.JValue.Parse(txt).ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception)
            {
                return txt;
            }
        }

        #region Control Events

        #region Bucket Management

        private void gridBucketManagement_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
        {
            foreach (Record tmp in gridBucketManagement.SelectedItems.Records)
            {
                if (tmp.RecordType == RecordType.DataRecord &&
                    (tmp as DataRecord).DataItem is CNotifier)
                {
                    ((tmp as DataRecord).DataItem as CNotifier).NotifyAll();
                }
            }

            DoButtons();
        }

        private void btnRefreshBucketList_Click(object sender, RoutedEventArgs e)
        {
            m_InProcess = true;
            DoButtons();

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoRefreshBucketLists));
        }
        private void btnNewBucket_Click(object sender, RoutedEventArgs e)
        {
            m_InProcess = true;
            DoButtons();

            BucketUI.CFrmNewBucket frm;

            frm = new BucketUI.CFrmNewBucket();
            frm.Start(
                this,
                m_AuthenticationToken,
                m_IoC);

            if (frm.Cancelled == false)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(DoRefreshBucketLists));
            }
            else
            {
                m_InProcess = false;
                DoButtons();
            }
        }
        private void btnDeleteBucket_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res;

            m_InProcess = true;
            DoButtons();

            res = ShowMessageBox(
                "Really Delete Selected Bucket?",
                "Please Confirm",
                MessageBoxImage.Question,
                MessageBoxButton.YesNo,
                MessageBoxResult.No);

            if (res == MessageBoxResult.Yes)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(DoDeleteBucket));
            }
            else
            {
                m_InProcess = false;
                DoButtons();
            }
        }

        #endregion

        #region Upload

        private void txtUploadPartNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (m_InProcess == false)
            {
                DoButtons();
            }
        }

        private void cmbBuckets_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (m_InProcess == false)
            {
                DoButtons();
            }
        }
        private void btnRefreshUploadBuckets_Click(object sender, RoutedEventArgs e)
        {
            m_InProcess = true;
            DoButtons();

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoRefreshBucketLists));
        }

        private void btnSelectIPT_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fd;

            m_InProcess = true;
            DoButtons();

            fd = new System.Windows.Forms.OpenFileDialog();
            fd.FileName = m_Upload_IPTLocation;
            fd.Filter = "Inventor Part Files (*.ipt)|*.ipt";
            fd.Multiselect = false;
            fd.RestoreDirectory = false;
            fd.Title = "Select Inventor Part File";

            if (fd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                m_Upload_IPTLocation = fd.FileName;
                txtIPTFile.SetText(m_Upload_IPTLocation);
            }

            m_InProcess = false;
            DoButtons();
        }
        private void btnSelectXLS_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fd;

            m_InProcess = true;
            DoButtons();

            fd = new System.Windows.Forms.OpenFileDialog();
            fd.FileName = m_Upload_XLSLocation;
            fd.Filter = "Excel Files (*.xls,*.xlsx)|*.xls;*.xlsx";
            fd.Multiselect = false;
            fd.RestoreDirectory = false;
            fd.Title = "Select Parameter Sheet";

            if (fd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                m_Upload_XLSLocation = fd.FileName;
                txtXLSFile.SetText(m_Upload_XLSLocation);
            }

            m_InProcess = false;
            DoButtons();
        }
        private void btnSelectIDW_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fd;

            m_InProcess = true;
            DoButtons();

            fd = new System.Windows.Forms.OpenFileDialog();
            fd.FileName = m_Upload_IDWLocation;
            fd.Filter = "Inventor Drawing Files (*.idw)|*.idw";
            fd.Multiselect = false;
            fd.RestoreDirectory = false;
            fd.Title = "Select Inventor Drawing File";

            if (fd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                m_Upload_IDWLocation = fd.FileName;
                txtIDWFile.SetText(m_Upload_IDWLocation);
            }

            m_InProcess = false;
            DoButtons();
        }

        private void btnUploadToBucket_Click(object sender, RoutedEventArgs e)
        {
            m_InProcess = true;
            DoButtons();

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoUpload));
        }

        private void btnUploadResultsIPT_DownloadFile_Click(object sender, RoutedEventArgs e)
        {
            m_InProcess = false;
            DoButtons();

            System.Windows.Forms.SaveFileDialog fd;

            string[] parts;
            string fname;

            parts = m_UploadIPT_DownloadURI.Split('/');
            fname = parts[parts.Length - 1];

            fd = new System.Windows.Forms.SaveFileDialog();
            fd.FileName = fname;
            fd.RestoreDirectory = false;
            fd.Title = "Select Save File";

            if (fd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(DoDownloadObjectURI_Model), fd.FileName);
            }
            else
            {
                m_InProcess = false;
                DoButtons();
            }
        }
        private void btnUploadResultsIDW_DownloadFile_Click(object sender, RoutedEventArgs e)
        {
            m_InProcess = false;
            DoButtons();

            System.Windows.Forms.SaveFileDialog fd;

            string[] parts;
            string fname;

            parts = m_UploadIDW_DownloadURI.Split('/');
            fname = parts[parts.Length - 1];

            fd = new System.Windows.Forms.SaveFileDialog();
            fd.FileName = fname;
            fd.RestoreDirectory = false;
            fd.Title = "Select Save File";

            if (fd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(DoDownloadObjectURI_Drawing), fd.FileName);
            }
            else
            {
                m_InProcess = false;
                DoButtons();
            }
        }

        #endregion

        #region Derivative Processing

        private void btnDerivativeRun_Model_Click(object sender, RoutedEventArgs e)
        {
            m_InProcess = true;
            DoButtons();

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoRunDerivative_Model));
        }
        private void btnDerivativeRefeshStatus_Model_Click(object sender, RoutedEventArgs e)
        {
            m_InProcess = true;
            DoButtons();

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoRefreshDerivative_Model));
        }

        private void btnDerivativeRun_Drawing_Click(object sender, RoutedEventArgs e)
        {
            m_InProcess = true;
            DoButtons();

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoRunDerivative_Drawing));
        }
        private void btnDerivativeRefeshStatus_Drawing_Click(object sender, RoutedEventArgs e)
        {
            m_InProcess = true;
            DoButtons();

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoRefreshDerivative_Drawing));
        }

        #endregion

        #region Viewer

        private void txtViewerPartNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (m_InProcess == false)
            {
                DoButtons();
            }
        }
        private void txtViewerPartNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (m_InProcess == false &&
                string.IsNullOrWhiteSpace(txtViewerPartNumber.GetText()) == false &&
                (e.Key == Key.Enter ||
                 e.Key == Key.Return))
            {
                btnViewerSearch.PerformClick();
            }
        }
        private void btnViewerSearch_Click(object sender, RoutedEventArgs e)
        {
            m_InProcess = true;
            DoButtons();

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoViewerSearch));
        }

        private void btnExportHTML_Model_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog fd;

            m_InProcess = true;
            DoButtons();

            fd = new System.Windows.Forms.SaveFileDialog();
            fd.FileName = $"VIEWER_{m_ViewerPart.PartNumber}_MODEL.HTML";
            fd.RestoreDirectory = false;
            fd.Title = "Select Model HTML Export Location";

            if (fd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(DoExportHTML_Model), fd.FileName);
            }
            else
            {
                m_InProcess = false;
                DoButtons();
            }
        }
        private void btnExportHTML_Drawing_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog fd;

            m_InProcess = true;
            DoButtons();

            fd = new System.Windows.Forms.SaveFileDialog();
            fd.FileName = $"VIEWER_{m_ViewerPart.PartNumber}_DRAWING.HTML";
            fd.RestoreDirectory = false;
            fd.Title = "Select Drawing HTML Export Location";

            if (fd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(DoExportHTML_Drawing), fd.FileName);
            }
            else
            {
                m_InProcess = false;
                DoButtons();
            }
        }

        #endregion

        #region API Log

        private void gridAPILog_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
        {
            foreach (Record tmp in gridAPILog.SelectedItems.Records)
            {
                if (tmp.RecordType == RecordType.DataRecord &&
                    (tmp as DataRecord).DataItem is CNotifier)
                {
                    ((tmp as DataRecord).DataItem as CNotifier).NotifyAll();
                }
            }

            DoButtons();
        }
        private void gridAPILog_RecordMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Types.CResultWrapper selected;
            LogUI.CFrmLogDetails frm;

            if (m_InProcess == false &&                 
                CUtility.IsDataRecord(sender))
            {
                m_InProcess = true;
                DoButtons();

                selected = gridAPILog.GetSelectedItems<Types.CResultWrapper>()[0];

                frm = new LogUI.CFrmLogDetails();
                frm.Start(
                    this,
                    selected.Data);

                m_InProcess = false;
                DoButtons();
            }
        }

        #endregion

        private void CMOMForm_Loaded(object sender, RoutedEventArgs e)
        {
            m_InProcess = true;
            DoButtons();

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoLoad));
        }

        #endregion

    }
}

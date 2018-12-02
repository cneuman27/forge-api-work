using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using Autofac;
using Inventor;
using Newtonsoft.Json;

using ForgeAPI.Autofac;

namespace StandardPartProcessing
{
    [ComVisible(true)]
    public class CProcessor
    {
        public delegate void DelegateSetStatus(string msg);

        private InventorLib.CInventorInstance m_ServerInstance = null;
        private DelegateSetStatus m_HandlerSetStatus = null;

        private Inputs.CInputs m_Inputs = null;
        private Outputs.COutputs m_Outputs = null;

        private string m_InputsPath = "";
        private string m_OutputsPath = "";

        public CProcessor(
            InventorServer server,
            DelegateSetStatus handlerSetStatus = null)
        {
            m_ServerInstance = new InventorLib.CInventorInstance(server);
            m_HandlerSetStatus = handlerSetStatus;
        }
        
        public void Run(Document doc)
        {
        }
        public void RunWithArguments(
            Document doc,
            NameValueMap args)
        {
            DateTime start = DateTime.Now;

            using (CHeartBeat heartBeat = new CHeartBeat())
            {
                try
                {
                    m_Outputs = new Outputs.COutputs();

                    m_ServerInstance.CloseAllDocuments();

                    ProcessArguments(args);
                    LoadInputs();
                    SetParameters();
                    ProcessPartModel();
                    ProcessPartDrawing();
                    UpdateForge();

                    m_Outputs.Reference = m_Inputs.Reference;
                    m_Outputs.PartNumber = m_Inputs.PartNumber;
                    m_Outputs.RunDuration = DateTime.Now - start;

                    System.IO.File.WriteAllText(
                        System.IO.Path.Combine(m_OutputsPath, CConstants.OUTPUT_FILE_NAME),
                        JsonConvert.SerializeObject(m_Outputs));
                }
                catch (Exception e)
                {
                    // TODO: Log This - For Now Just Rethrow

                    throw e;
                }
            }
        }

        private void ProcessArguments(NameValueMap args)
        {
            string zipPath;

            SetStatus("Processing Folder Structure and Zip File Data");

            zipPath = (string)args.Value["_1"];

            if (string.IsNullOrWhiteSpace(zipPath))
            {
                throw new Exception("INPUT ZIP FILE PARAMETER NOT FOUND");
            }

            if (System.IO.File.Exists(zipPath) == false)
            {
                throw new Exception("INPUT ZIP FILE DOES NOT EXIST");
            }

            m_InputsPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(zipPath),
                "INPUTS");

            if (System.IO.Directory.Exists(m_InputsPath) == false)
            {
                System.IO.Directory.CreateDirectory(m_InputsPath);
            }

            m_OutputsPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(zipPath),
                "OUTPUTS");

            if (System.IO.Directory.Exists(m_OutputsPath) == false)
            {
                System.IO.Directory.CreateDirectory(m_OutputsPath);
            }

            System.IO.Compression.ZipFile.ExtractToDirectory(
                zipPath,
                m_InputsPath);
        }
        private void LoadInputs()
        {
            string inputsJSONPath;

            SetStatus("Loading Input Data");

            inputsJSONPath = System.IO.Path.Combine(
                m_InputsPath,
                CConstants.INPUT_FILE_NAME);

            if (System.IO.File.Exists(inputsJSONPath) == false)
            {
                throw new Exception(
                    $"INPUT FILE [{inputsJSONPath}] NOT FOUND - ABORTING");
            }

            m_Inputs = Newtonsoft.Json.JsonConvert.DeserializeObject<Inputs.CInputs>(
                System.IO.File.ReadAllText(inputsJSONPath));
        }
        private void SetParameters()
        {
            string xlsPath;
            SpreadsheetGear.IWorkbook workbook = null;
            SpreadsheetGear.IWorksheet sheet = null;
            SpreadsheetGear.IRange range = null;

            SetStatus("Setting Parameter Data");

            try
            {
                xlsPath = EnsureXLSFile();

                workbook = SpreadsheetGear.Factory.GetWorkbook(xlsPath);
                sheet = workbook.Worksheets["Data"];

                foreach (string key in m_Inputs.Parameters.Keys)
                {
                    range = null;
                    try
                    {
                        range = sheet.Cells[key];
                    }
                    catch (Exception)
                    {
                    }

                    if (range != null)
                    {
                        range.Value = m_Inputs.Parameters[key];
                    }
                }

                workbook.WorkbookSet.CalculateFull();
                workbook.Save();
            }
            finally
            {
                workbook?.Close();
            }
        }
        private void ProcessPartModel()
        {
            string iptPath;
            InventorLib.Documents.CPartDocument partDoc = null;

            iptPath = EnsureIPTFile();

            SetStatus("Opening IPT File");

            using (partDoc = m_ServerInstance.OpenPartDocument(iptPath))
            {
                SetStatus("Setting Part Property Data");

                partDoc.SetProperty("PART NUMBER", m_Inputs.PartNumber);
                partDoc.SetProperty("DESCRIPTION", m_Inputs.Description);
                partDoc.SetProperty("MATERIAL", m_Inputs.MaterialType);
                partDoc.SetMaterial(m_Inputs.MaterialType);
                partDoc.ReduceThicknessPrecision(3);

                SetStatus("Updating and Saving");

                partDoc.Update();
                partDoc.Save();
                               
                #region DXF

                {
                    SetStatus("Processing DXF");

                    string tmpPath1;
                    string tmpPath2;
                    string finalPath;
                    DXFProcessor.CDXFProcessor processor;

                    tmpPath1 = System.IO.Path.Combine(
                        m_InputsPath,
                        $"{m_Inputs.PartNumber}_RAW.dxf");

                    tmpPath2 = System.IO.Path.Combine(
                        m_InputsPath,
                        $"{m_Inputs.PartNumber}_INTERMEDIATE.dxf");

                    finalPath = System.IO.Path.Combine(
                        m_OutputsPath,
                        $"{m_Inputs.PartNumber}.dxf");

                    partDoc.ExportDXF(tmpPath1);

                    processor = new DXFProcessor.CDXFProcessor();

                    processor.ProcessDXFFile(
                        tmpPath1,
                        tmpPath2,
                        m_Inputs.PartNumber,
                        m_Inputs.MaterialType,
                        false,
                        false,
                        false,
                        false);

                    processor.CalculatePartLaborAndRouting(
                        tmpPath2,
                        finalPath);

                    AddArtifact(finalPath, Enums.E_ArtifactType.DXF);
                }

                #endregion

                //#region SAT

                //{
                //    SetStatus("Processing SAT");

                //    SpatialSATLib.CSAT sat;
                //    string tmpPath;

                //    tmpPath = System.IO.Path.Combine(
                //        m_OutputsPath,
                //        $"{m_Inputs.PartNumber}.sat");

                //    partDoc.ExportSAT(tmpPath);

                //    sat = new SpatialSATLib.CSAT(tmpPath);
                //    if (sat.LoadOK() == false)
                //    {
                //        throw new Exception(sat.LoadErrorMessage());
                //    }

                //    sat.SetMaterialThickness(partDoc.GetSheetMetalThickness());
                //    sat.FixOrientation();
                //    sat.Save();

                //    tmpPath = System.IO.Path.Combine(
                //        m_OutputsPath,
                //        $"{m_Inputs.PartNumber}_V7.sat");

                //    sat.SaveAs(
                //        tmpPath,
                //        new Version("7.0.0.0"));

                //    AddArtifact(
                //        System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}.sat"),
                //        Enums.E_ArtifactType.SAT);

                //    AddArtifact(
                //        System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}_V7.sat"),
                //        Enums.E_ArtifactType.SAT);
                //}

                //#endregion

                SetStatus("Processing DWFx");

                partDoc.ExportDWFx(
                    System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}_MODEL.dwfx"));

                AddArtifact(
                    System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}_MODEL.dwfx"),
                    Enums.E_ArtifactType.DWF_Model);

                SetStatus("Saving To Output Path");

                partDoc.SaveAs(
                    System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}.ipt"));

                AddArtifact(
                    System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}.ipt"),
                    Enums.E_ArtifactType.IPT);

                SetStatus("Closing IPT");

                partDoc.Close();                  
            }
        }
        private void ProcessPartDrawing()
        {
            string idwPath;
            InventorLib.Documents.CDrawingDocument drawingDoc = null;

            idwPath = EnsureIDWFile();

            SetStatus("Opening IDW File");
            
            using (drawingDoc = m_ServerInstance.OpenDrawingDocument(idwPath))
            {
                SetStatus("Setting Drawing Property Data");

                drawingDoc.SetProperty("PART NUMBER", m_Inputs.PartNumber);
                drawingDoc.SetProperty("DESCRIPTION", m_Inputs.Description);
                drawingDoc.SetProperty("MATERIAL", m_Inputs.MaterialType);

                SetStatus("Updating and Saving Drawing");

                drawingDoc.Update();
                drawingDoc.Save();

                SetStatus("Processing Drawing DWF File");

                drawingDoc.ExportDWF(
                    System.IO.Path.Combine(m_OutputsPath, m_Inputs.PartNumber + "_DRAWING.dwf"));

                AddArtifact(
                    System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}_DRAWING.dwf"),
                    Enums.E_ArtifactType.DWF_Drawing);

                SetStatus("Saving Drawing To Output Folder");

                drawingDoc.SaveAs(
                    System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}.idw"));

                AddArtifact(
                    System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}.idw"),
                    Enums.E_ArtifactType.IDW);

                SetStatus("Closing Drawing");

                drawingDoc.Close();
            }
        }
        private void UpdateForge()
        {
            ContainerBuilder builder;
            ForgeAPI.Interface.Authentication.IService authService;
            ForgeAPI.Interface.Authentication.IToken authToken;
            ForgeAPI.Interface.Utility.IService utility;
            ForgeAPI.Interface.IFactory factory;

            builder = new ContainerBuilder();
            builder.AddForgeAPI();

            using (var container = builder.Build())
            {
                SetStatus("Getting Forge Authentication Token");

                #region Authenticate

                authService = container.Resolve<ForgeAPI.Interface.Authentication.IService>();
                authToken = authService.Authenticate(
                    new List<ForgeAPI.Interface.Enums.E_AccessScope>()
                    {
                        ForgeAPI.Interface.Enums.E_AccessScope.Bucket_Create,
                        ForgeAPI.Interface.Enums.E_AccessScope.Bucket_Delete,
                        ForgeAPI.Interface.Enums.E_AccessScope.Bucket_Read,
                        ForgeAPI.Interface.Enums.E_AccessScope.Data_Read,
                        ForgeAPI.Interface.Enums.E_AccessScope.Data_Write
                    });

                #endregion

                factory = container.Resolve<ForgeAPI.Interface.IFactory>();
                utility = container.Resolve<ForgeAPI.Interface.Utility.IService>();

                int index = 1;
                foreach (Outputs.CArtifact artifact in m_Outputs.ArtifactList)
                {
                    string prefix;

                    prefix =
                        $"Processing {index} of {m_Outputs.ArtifactList.Count} [{artifact.FileName}]";

                    #region Upload To Data Management

                    SetStatus($"{prefix} - Uploading To Data Management");

                    {
                        ForgeAPI.Interface.DataManagement.Objects.UploadObject.IInputs inputs;
                        ForgeAPI.Interface.DataManagement.Objects.UploadObject.IOutputs outputs;
                        ForgeAPI.Interface.DataManagement.Objects.IService service;

                        inputs = factory.CreateInputs<ForgeAPI.Interface.DataManagement.Objects.UploadObject.IInputs>(authToken);
                        service = container.Resolve<ForgeAPI.Interface.DataManagement.Objects.IService>();

                        inputs.FileName = System.IO.Path.GetFileName(artifact.FileLocation);
                        inputs.FileData = System.IO.File.ReadAllBytes(artifact.FileLocation);
                        inputs.ContentType = "application/octet-stream";
                        inputs.BucketKey = m_Inputs.BucketKey;
                        inputs.ObjectName = inputs.FileName;

                        outputs = service.UploadObject(inputs);

                        if (outputs.Success() == false)
                        {
                            throw new Exception(outputs.FailureReason());
                        }

                        artifact.URN = outputs.ObjectID;
                        artifact.URNEncoded = utility.ConvertToBase64(artifact.URN);
                    }

                    #endregion

                    #region Add Derivative Job

                    SetStatus($"{prefix} - Adding Derivative Job");

                    {
                        ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IInputs inputs;
                        ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IOutputFormat_SVF svfFormat;
                        ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IOutputs outputs;
                        ForgeAPI.Interface.ModelDerivative.Derivatives.IService service;

                        service = container.Resolve<ForgeAPI.Interface.ModelDerivative.Derivatives.IService>();

                        inputs = factory.CreateInputs<ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IInputs>(
                            authToken);

                        inputs.ReplaceExistingDerivatives = true;
                        inputs.Input.URNEncoded = utility.ConvertToBase64(artifact.URN);
                        inputs.Input.IsCompressed = false;
                        inputs.Output.DestinationSettings.Region = ForgeAPI.Interface.Enums.E_Region.US;

                        svfFormat = factory.Create<ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IOutputFormat_SVF>();

                        #region Set View Types

                        if (artifact.Type == Enums.E_ArtifactType.DWF_Drawing ||
                            artifact.Type == Enums.E_ArtifactType.DXF ||
                            artifact.Type == Enums.E_ArtifactType.IDW)
                        {
                            svfFormat.ViewList.Add(ForgeAPI.Interface.Enums.E_SVFViewType._2D);
                        }
                        else if (
                            artifact.Type == Enums.E_ArtifactType.DWF_Model ||
                            artifact.Type == Enums.E_ArtifactType.IPT ||
                            artifact.Type == Enums.E_ArtifactType.SAT ||
                            artifact.Type == Enums.E_ArtifactType.SAT_V7)
                        {
                            svfFormat.ViewList.Add(ForgeAPI.Interface.Enums.E_SVFViewType._3D);
                        }

                        #endregion

                        inputs.Output.FormatList.Add(svfFormat);

                        outputs = service.AddJob(inputs);

                        if (outputs.Success() == false)
                        {
                            throw new Exception(outputs.FailureReason());
                        }
                    }

                    #endregion

                    #region Derivative Status Loop

                    SetStatus($"{prefix} - Waiting For Derivative Job Completion");

                    while (true)
                    {
                        ForgeAPI.Interface.ModelDerivative.Derivatives.IService service;
                        ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IInputs inputs;
                        ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IOutputs outputs;

                        inputs = factory.CreateInputs<ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IInputs>(authToken);
                        inputs.URN = artifact.URN;
                        inputs.URNIsEncoded = false;

                        service = container.Resolve<ForgeAPI.Interface.ModelDerivative.Derivatives.IService>();

                        outputs = service.GetManifest(inputs);

                        if (outputs.Success())
                        {
                            if (outputs.Status == ForgeAPI.Interface.Enums.E_TranslationStatus.Failed ||
                                outputs.Status == ForgeAPI.Interface.Enums.E_TranslationStatus.Timeout)
                            {
                                throw new Exception($"Derivative Processing Failed [{artifact.FileName}]");
                            }

                            if (outputs.Status == ForgeAPI.Interface.Enums.E_TranslationStatus.Success)
                            {
                                break;
                            }
                        }
                        else
                        {
                            throw new Exception(outputs.FailureReason());
                        }

                        Thread.Sleep(1000);
                    }

                    #endregion

                    index++;
                }
            }
        }

        private string EnsureXLSFile()
        {
            string xlsPath;

            if (string.IsNullOrWhiteSpace(m_Inputs?.XLSFile))
            {
                throw new Exception("XLS FILE IS NOT SET");
            }

            xlsPath = System.IO.Path.Combine(
                m_InputsPath,
                m_Inputs.XLSFile);

            if (System.IO.File.Exists(xlsPath) == false)
            {
                throw new Exception(
                    $"XLS FILE [{xlsPath}] NOT FOUND - ABORTING");
            }

            return xlsPath;
        }
        private string EnsureIPTFile()
        {
            string iptPath;

            if (string.IsNullOrWhiteSpace(m_Inputs?.IPTFile))
            {
                throw new Exception("IPT FILE IS NOT SET - ABORTING");
            }
            
            iptPath = System.IO.Path.Combine(
                m_InputsPath,
                m_Inputs.IPTFile);

            if (System.IO.File.Exists(iptPath) == false)
            {
                throw new Exception(
                    $"IPT FILE [{iptPath}] NOT FOUND - ABORTING");
            }

            return iptPath;
        }
        private string EnsureIDWFile()
        {
            string idwPath;
            
            if (string.IsNullOrWhiteSpace(m_Inputs?.IDWFile))
            {
                throw new Exception("IDW FILE IS NOT SET");
            }

            idwPath = System.IO.Path.Combine(
                m_InputsPath,
                m_Inputs.IDWFile);

            if (System.IO.File.Exists(idwPath) == false)
            {
                throw new Exception(
                    $"IDW FILE [{idwPath}] NOT FOUND - ABORTING");
            }

            return idwPath;
        }

        private void AddArtifact(
            string fileLocation,
            Enums.E_ArtifactType type)
        {
            Outputs.CArtifact artifact;

            artifact = new Outputs.CArtifact();

            artifact.FileLocation = fileLocation;
            artifact.FileName = System.IO.Path.GetFileName(fileLocation);
            artifact.Type = type;

            m_Outputs.ArtifactList.Add(artifact);
        }

        private void SetStatus(string msg)
        {
            if (m_HandlerSetStatus != null)
            {
                m_HandlerSetStatus(msg);
            }
        }
    }
}

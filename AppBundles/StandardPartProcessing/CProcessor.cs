using System;
using System.Runtime.InteropServices;

using Inventor;
using Newtonsoft.Json;

namespace StandardPartProcessing
{
    [ComVisible(true)]
    public class CProcessor
    {
        private InventorLib.CInventorInstance m_ServerInstance = null;

        private Inputs.CInputs m_Inputs = null;
        private Outputs.COutputs m_Outputs = null;

        private string m_InputsPath = "";
        private string m_OutputsPath = "";

        public CProcessor(InventorServer server)
        {
            m_ServerInstance = new InventorLib.CInventorInstance(server);
        }

        public void Run(Document doc)
        {
            RunWithArguments(doc, null);
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

            m_OutputsPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(zipPath),
                "OUTPUTS");

            System.IO.Compression.ZipFile.ExtractToDirectory(
                zipPath,
                m_InputsPath);
        }
        private void LoadInputs()
        {
            string inputsJSONPath;

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

            using (partDoc = m_ServerInstance.OpenPartDocument(iptPath))
            {
                partDoc.SetProperty("PART NUMBER", m_Inputs.PartNumber);
                partDoc.SetProperty("DESCRIPTION", m_Inputs.Description);
                partDoc.SetProperty("MATERIAL", m_Inputs.MaterialType);
                partDoc.SetMaterial(m_Inputs.MaterialType);
                partDoc.ReduceThicknessPrecision(3);
                partDoc.Update();
                partDoc.Save();
                               
                #region DXF

                {
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

                #region SAT

                {
                    SpatialSATLib.CSAT sat;
                    string tmpPath;

                    tmpPath = System.IO.Path.Combine(
                        m_OutputsPath,
                        $"{m_Inputs.PartNumber}.sat");

                    partDoc.ExportSAT(tmpPath);

                    sat = new SpatialSATLib.CSAT(tmpPath);
                    if (sat.LoadOK() == false)
                    {
                        throw new Exception(sat.LoadErrorMessage());
                    }

                    sat.SetMaterialThickness(partDoc.GetSheetMetalThickness());
                    sat.FixOrientation();
                    sat.Save();

                    tmpPath = System.IO.Path.Combine(
                        m_OutputsPath,
                        $"{m_Inputs.PartNumber}_V7.sat");

                    sat.SaveAs(
                        tmpPath,
                        new Version("7.0.0.0"));

                    AddArtifact(
                        System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}.sat"),
                        Enums.E_ArtifactType.SAT);

                    AddArtifact(
                        System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}_V7.sat"),
                        Enums.E_ArtifactType.SAT);
                }

                #endregion

                partDoc.ExportDWFx(
                    System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}_MODEL.dwfx"));

                AddArtifact(
                    System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}_MODEL.dwfx"),
                    Enums.E_ArtifactType.DWF_Model);

                partDoc.SaveAs(
                    System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}.ipt"));

                AddArtifact(
                    System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}.ipt"),
                    Enums.E_ArtifactType.IPT);

                partDoc.Close();                  
            }
        }
        private void ProcessPartDrawing()
        {
            string idwPath;
            InventorLib.Documents.CDrawingDocument drawingDoc = null;

            idwPath = EnsureIDWFile();

            using (drawingDoc = m_ServerInstance.OpenDrawingDocument(idwPath))
            {
                drawingDoc.SetProperty("PART NUMBER", m_Inputs.PartNumber);
                drawingDoc.SetProperty("DESCRIPTION", m_Inputs.Description);
                drawingDoc.SetProperty("MATERIAL", m_Inputs.MaterialType);
                drawingDoc.Update();
                drawingDoc.Save();

                drawingDoc.ExportDWF(
                    System.IO.Path.Combine(m_OutputsPath, m_Inputs.PartNumber + "_DRAWING.dwf"));

                AddArtifact(
                    System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}_DRAWING.dwf"),
                    Enums.E_ArtifactType.DWF_Drawing);

                drawingDoc.SaveAs(
                    System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}.idw"));

                AddArtifact(
                    System.IO.Path.Combine(m_OutputsPath, $"{m_Inputs.PartNumber}.idw"),
                    Enums.E_ArtifactType.IDW);

                drawingDoc.Close();
            }
        }
        private void UpdateForge()
        {
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
    }
}

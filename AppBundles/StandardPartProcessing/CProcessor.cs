using System;
using System.Runtime.InteropServices;

using Inventor;

namespace StandardPartProcessing
{
    [ComVisible(true)]
    public class CProcessor
    {
        private InventorServer m_InventorServer = null;
        private Inputs.CInputs m_Inputs = null;

        public CProcessor(InventorServer server)
        {
            m_InventorServer = server;
        }

        public void Run(Document doc)
        {
            RunWithArguments(doc, null);
        }
        public void RunWithArguments(
            Document doc,
            NameValueMap map)
        {
            using (CHeartBeat heartBeat = new CHeartBeat())
            {
                try
                {
                    LoadInputs();
                    SetParameters();
                    ProcessPartModel();
                    ProcessPartDrawing();
                    ProcessDXF();
                    ProcessSAT();
                    UpdateForge();
                    WriteOutputFile();
                }
                catch (Exception e)
                {
                    // TODO: Log This - For Now Just Rethrow

                    throw e;
                }
            }
        }

        private void LoadInputs()
        {
            if (System.IO.File.Exists(CConstants.INPUT_FILE_NAME) == false)
            {
                throw new Exception(
                    $"INPUT FILE [{CConstants.INPUT_FILE_NAME}] NOT FOUND - ABORTING");
            }

            m_Inputs = Newtonsoft.Json.JsonConvert.DeserializeObject<Inputs.CInputs>(
                System.IO.File.ReadAllText(CConstants.INPUT_FILE_NAME));
        }
        private void SetParameters()
        {
            SpreadsheetGear.IWorkbook workbook = null;
            SpreadsheetGear.IWorksheet sheet = null;
            SpreadsheetGear.IRange range = null;
            
            if (string.IsNullOrWhiteSpace(m_Inputs?.XLSFile))
            {
                throw new Exception("XLS FILE IS NOT SET");
            }

            try
            {
                workbook = SpreadsheetGear.Factory.GetWorkbook(m_Inputs.XLSFile);
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


            try
            {
            }
            finally
            {
            }
        }
        private void ProcessPartDrawing()
        {
        }
        private void ProcessDXF()
        {
        }
        private void ProcessSAT()
        {
        }
        private void UpdateForge()
        {
        }
        private void WriteOutputFile()
        {
        }
    }
}

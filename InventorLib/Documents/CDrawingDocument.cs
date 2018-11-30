using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using Inventor;

namespace InventorLib.Documents
{
    public class CDrawingDocument : IDisposable
    {
        private DrawingDocument m_Document = null;
        private InventorServer m_InventorServerObject = null;

        internal CDrawingDocument(
            DrawingDocument document,
            InventorServer server)
        {
            m_Document = document;
            m_InventorServerObject = server;
        }

        public void Update()
        {
            m_Document.Update();
            m_Document.MakeAllViewsPrecise();
        }
        public void Save()
        {
            m_Document.Save();
        }
        public void SaveAs(string outputLocation)
        {
            m_Document.SaveAs(
                outputLocation,
                true);
        }
        public void Close()
        {
            m_Document.Close();
        }

        public void ExportDWF(string outputLocation)
        {
            Inventor.TranslatorAddIn translator;
            Inventor.TranslationContext context;
            Inventor.NameValueMap options;
            Inventor.DataMedium medium;
            Inventor.NameValueMap sheets;
            Inventor.NameValueMap sheetOptions;

            translator = m_InventorServerObject.ApplicationAddIns.ItemById[CConstants.DWF_TRANSLATOR_ID] as Inventor.TranslatorAddIn;
            translator.Activate();

            context = m_InventorServerObject.TransientObjects.CreateTranslationContext();
            context.Type = IOMechanismEnum.kFileBrowseIOMechanism;

            medium = m_InventorServerObject.TransientObjects.CreateDataMedium();
            medium.FileName = outputLocation;

            options = m_InventorServerObject.TransientObjects.CreateNameValueMap();

            if (translator.HasSaveCopyAsOptions[m_Document, context, options])
            {
                options.Value["Publish_Mode"] = DWFPublishModeEnum.kCustomDWFPublish;
                options.Value["Publish_All_Sheets"] = 0;
                options.Value["Launch_Viewer"] = 0;

                sheets = m_InventorServerObject.TransientObjects.CreateNameValueMap();

                int index = 1;
                foreach (Inventor.Sheet tmp in m_Document.Sheets)
                {
                    sheetOptions = m_InventorServerObject.TransientObjects.CreateNameValueMap();
                    sheetOptions.Add("Name", tmp.Name);
                    sheetOptions.Add("3DModel", false);

                    sheets.Add($"Sheet{index}", sheetOptions);

                    index++;
                }

                options.Value["Sheets"] = sheets;
            }

            translator.SaveCopyAs(
                m_Document,
                context,
                options,
                medium);
        }

        public void SetProperty(
            string propName,
            string propValue)
        {
            PropertySets sets;

            sets = m_Document.PropertySets;

            foreach (PropertySet set in sets)
            {
                foreach (Property prop in set)
                {
                    if (prop.Name.ToUpper() == propName.ToUpper())
                    {
                        prop.Value = propValue;
                        return;
                    }
                }
            }
        }
               
        public void Dispose()
        {
            if (m_Document != null)
            {
                Marshal.ReleaseComObject(m_Document);
                m_Document = null;
            }
        }
    }
}

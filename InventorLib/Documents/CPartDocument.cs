using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using Inventor;

namespace InventorLib.Documents
{
    public class CPartDocument : IDisposable
    {
        private PartDocument m_Document = null;
        private InventorServer m_InventorServerObject = null;

        internal CPartDocument(
            PartDocument document,
            InventorServer server)
        {
            m_Document = document;
            m_InventorServerObject = server;
        }

        public void Update()
        {
            m_Document.Update();
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
            m_Document.Close(true);
        }

        public void ExportDXF(string outputLocation)
        {
            string dxfFormat;

            dxfFormat = "FLAT PATTERN DXF?AcadVersion=R12&" +
                        "TangentLayer=tangent&" +
                        "BendLayer=bend&" +
                        "ToolCenterLayer=tool_center&" +
                        "ArcCentersLayer=arc_center&" +
                        "OuterProfileLayer=outer_profile&" +
                        "FeatureProfilesLayer=feature_profile&" +
                        "InteriorProfilesLayer=interior_profile";

            m_Document.ComponentDefinition.DataIO.WriteDataToFile(
                dxfFormat,
                outputLocation);
        }
        public void ExportSAT(string outputLocation)
        {
            if (System.IO.Path.GetExtension(outputLocation).ToUpper().EndsWith("SAT") == false)
            {
                outputLocation += ".sat"; 
            }

            m_Document.SaveAs(
                outputLocation, 
                true);
        }
        public void ExportDWFx(string outputLocation)
        {
            Inventor.TranslatorAddIn translator;
            Inventor.TranslationContext context;
            Inventor.NameValueMap options;
            Inventor.DataMedium medium;

            translator = m_InventorServerObject.ApplicationAddIns.ItemById[CConstants.DWFX_TRANSLATOR_ID] as Inventor.TranslatorAddIn;
            options = m_InventorServerObject.TransientObjects.CreateNameValueMap();

            context = m_InventorServerObject.TransientObjects.CreateTranslationContext();
            context.Type = IOMechanismEnum.kFileBrowseIOMechanism;

            medium = m_InventorServerObject.TransientObjects.CreateDataMedium();
            medium.FileName = outputLocation;

            if (translator.HasSaveCopyAsOptions[m_Document, context, options])
            {
                options.Value["Publish_All_Sheets"] = 0;
                options.Value["Publish_3D_Models"] = 1;
                options.Value["Launch_Viewer"] = 0;
                options.Value["Password"] = "";
                options.Value["Enable_Large_Assembly_Mode"] = 0;
                options.Value["Enable_Measure"] = 1;
                options.Value["Enable_Printing"] = 1;
                options.Value["Enable_Markups"] = 1;
                options.Value["Enable_Markup_Edits"] = 1;
                options.Value["Output_Path"] = "";
                options.Value["Include_Sheet_Tables"] = 1;
                options.Value["Sheet_Metal_Flat_Pattern"] = 0;
                options.Value["Sheet_Metal_Style_Information"] = 0;
                options.Value["Sheet_Metal_Part"] = 1;
                options.Value["Weldment_Preparation"] = 0;
                options.Value["Weldment_Symbol"] = 0;
                options.Value["BOM_Structured"] = 0;
                options.Value["BOM_Parts_Only"] = 0;
                options.Value["Animations"] = 0;
                options.Value["Instructions"] = 0;
                options.Value["iAssembly_All_Members"] = 0;
                options.Value["iAssembly_3D_Models"] = 0;
                options.Value["iPart_All_Members"] = 0;
                options.Value["iPart_3D_Models"] = 0;
                options.Value["Publish_Component_Props"] = 1;
                options.Value["Publish_Mass_Props"] = 0;
                options.Value["Include_Empty_Properties"] = 0;
                options.Value["Publish_Screenshot"] = 0;
                options.Value["Screenshot_DPI"] = 96;
                options.Value["Override_Sheet_Color"] = 0;
                options.Value["Sheet_Color"] = 0;
                options.Value["Custom_Begin_Sheet"] = 1;
                options.Value["Custom_End_Sheet"] = -1;
                options.Value["All_Color_AS_Black"] = 0;
                options.Value["Remove_Line_Weights"] = 0;
                options.Value["Vector_Resolution"] = 400;
                options.Value["TranscriptAPICall"] = 0;
                options.Value["Publish_Mode"] = DWFPublishModeEnum.kCustomDWFPublish;
                options.Value["Facet_Quality"] = AccuracyEnum.kHigh;
                options.Value["Sheet_Range"] = PrintRangeEnum.kPrintCurrentSheet;
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

        public void SetMaterial(string material)
        {
            foreach (Inventor.Material tmp in m_Document.Materials)
            {
                if (tmp.Name.ToUpper() == material.ToUpper())
                {
                    m_Document.ComponentDefinition.Material = tmp;
                    return;
                }
            }
        }

        public double GetSheetMetalThickness()
        {
            SheetMetalComponentDefinition compDef;
            Parameter parameter;

            compDef = m_Document.ComponentDefinition as SheetMetalComponentDefinition;
            parameter = compDef.Thickness;

            return ((double)parameter.Value) / 2.54;
        }
        public void SetSheetMetalThickness(double thickness)
        {
            SheetMetalComponentDefinition compDef;
            Parameter parameter;

            compDef = m_Document.ComponentDefinition as SheetMetalComponentDefinition;

            compDef.UseSheetMetalStyleThickness = false;

            parameter = compDef.Thickness;
            parameter.Value = thickness * 2.54;
        }
        public void ReduceThicknessPrecision(int precision)
        {
            if (precision <= 0) return;

            double thickness;

            thickness = GetSheetMetalThickness();

            SetSheetMetalThickness(
                Math.Round(Math.Floor(thickness * Math.Pow(10, precision)) / Math.Pow(10, precision), precision));
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DXFLib
{
	public class CDXF
	{
		#region Private Member Data

		private string m_FileLocation = "";
		
		private CHeader m_Header = null;

		private Hashtable m_LayerList = null;		
		private Hashtable m_ApplicationList = null;
		private Hashtable m_DimensionStyleList = null;
		private Hashtable m_LineTypeList = null;
		private Hashtable m_StyleList = null;
		private Hashtable m_UCSList = null;
		private Hashtable m_ViewList = null;
		private ArrayList m_ViewportList = null;		
		private ArrayList m_UnknownTableList = null;
		private ArrayList m_BlockList = null;

		#endregion

		#region Constructors

		public CDXF()
		{
		}
		public CDXF(string file)
		{
			Read(file);
        }
        public CDXF(byte[] buffer)
        {
            Read(buffer);
        }
        
		#endregion

		#region Public Properties
	
		public string FileLocation
		{
			get { return m_FileLocation; }
			set { m_FileLocation = value; }
		}
		
		public CHeader Header
		{
			get { return m_Header; }
			set { m_Header = value; }
		}
		
		public Hashtable LayerList
		{
			get { return m_LayerList; }
			set { m_LayerList = value; }
		}		
		public Hashtable ApplicationList
		{
			get { return m_ApplicationList; }
			set { m_ApplicationList = value; }
		}
		public Hashtable DimensionStyleList
		{
			get { return m_DimensionStyleList; }
			set { m_DimensionStyleList = value; }
		}
		public Hashtable LineTypeList
		{
			get { return m_LineTypeList; }
			set { m_LineTypeList = value; }
		}
		public Hashtable StyleList
		{
			get { return m_StyleList; }
			set { m_StyleList = value; }
		}
		public Hashtable UCSList
		{
			get { return m_UCSList; }
			set { m_UCSList = value; }
		}
		public Hashtable ViewList
		{
			get { return m_ViewList; }
			set { m_ViewList = value; }
		}
		public ArrayList ViewportList
		{
			get { return m_ViewportList; }
			set { m_ViewportList = value; }
		}
		public ArrayList UnknownTableList
		{
			get { return m_UnknownTableList; }
			set { m_UnknownTableList = value; }
		}
		public ArrayList BlockList
		{
			get { return m_BlockList; }
			set { m_BlockList = value; }
		}

		#endregion
	
		#region Reader

		public void Read(string file)
		{			
			try
			{
				m_FileLocation = file;

                if (File.Exists(m_FileLocation))
                {
                    Read(File.ReadAllLines(m_FileLocation.Trim()));
                }
                else
                {
                    throw new FileNotFoundException();
                }
			}
			catch(Exception e)
			{
				throw new Exception("CDXF: Exception Caught: See Inner Exception For Details", e);
			}
			finally
			{
			}
		}
        public void Read(byte[] buffer)
        {
            List<string> list;
            StreamReader reader = null;
            MemoryStream memStream = null;

            try
            {
                memStream = new MemoryStream(buffer);
                reader = new StreamReader(memStream);

                list = new List<string>();
                while (reader.Peek() >= 0)
                {
                    list.Add(reader.ReadLine());
                }

                Read(list.ToArray());
            }
            catch (Exception e)
            {
        	    throw new Exception("CDXF: Exception Caught: See Inner Exception For Details", e);
            }
            finally
            {
                try
                {
                    reader.Close();
                }
                catch (Exception)
                {
                }

                try
                {
                    memStream.Close();
                }
                catch (Exception)
                {
                }
            }
        }
        public void Read(string[] lines)
        {
            ArrayList gcList;
            CGroupCode gc = null;
            bool codeLine;
            bool inTableHeader;
            Hashtable sections;
            string sectionName;

            CHeaderVariable headerVariable;
            CHeaderVariableParameter headerVariableParameter;
            CEntity entity;
            CTable table;
            CBlock block;

            Hashtable entityTypes;
            Hashtable tableTypes;

            bool processingChildren;

            try
            {
                m_Header = new CHeader();

                m_LayerList = new Hashtable();

                m_ApplicationList = new Hashtable();
                m_DimensionStyleList = new Hashtable();
                m_LineTypeList = new Hashtable();
                m_StyleList = new Hashtable();
                m_UCSList = new Hashtable();
                m_ViewList = new Hashtable();
                m_ViewportList = new ArrayList();

                m_UnknownTableList = new ArrayList();

                m_BlockList = new ArrayList();

                #region Setup Entity Type Look Up

                entityTypes = new Hashtable();

                entityTypes.Add("LINE", typeof(CEntityLine));
                entityTypes.Add("POINT", typeof(CEntityPoint));
                entityTypes.Add("CIRCLE", typeof(CEntityCircle));
                entityTypes.Add("ARC", typeof(CEntityArc));
                entityTypes.Add("TRACE", typeof(CEntityTrace));
                entityTypes.Add("SOLID", typeof(CEntitySolid));
                entityTypes.Add("TEXT", typeof(CEntityText));
                entityTypes.Add("SHAPE", typeof(CEntityShape));
                entityTypes.Add("POLYLINE", typeof(CEntityPolyline));
                entityTypes.Add("3DFACE", typeof(CEntity3DFace));

                #endregion

                #region Setup Table Type Look Up

                tableTypes = new Hashtable();

                tableTypes.Add("APPID", typeof(CTableApplicationID));
                tableTypes.Add("DIMSTYLE", typeof(CTableDimensionStyle));
                tableTypes.Add("LTYPE", typeof(CTableLineType));
                tableTypes.Add("LAYER", typeof(CTableLayer));
                tableTypes.Add("STYLE", typeof(CTableTextStyle));
                tableTypes.Add("UCS", typeof(CTableUCS));
                tableTypes.Add("VIEW", typeof(CTableView));
                tableTypes.Add("VPORT", typeof(CTableViewport));
                tableTypes.Add("BLOCK_RECORD", typeof(CTable));

                #endregion

                #region Create Group Code List

                gcList = new ArrayList();

                codeLine = true;
                foreach (string line in lines)
                {
                    if (codeLine)
                    {
                        codeLine = false;

                        gc = new CGroupCode();
                        gc.Code = CGlobals.ConvertToInt(line.Trim());
                    }
                    else
                    {
                        if (gc != null)
                        {
                            gc.Value = line.Trim();

                            gcList.Add(gc);
                        }

                        codeLine = true;
                    }
                }

                #endregion

                #region Build Sections Hashtable

                sections = new Hashtable();

                sectionName = "XX";
                for (int x = 0; x < gcList.Count; x++)
                {
                    gc = (CGroupCode)gcList[x];

                    if (
                        gc.Code == 0 &&
                        gc.Value.ToUpper().Trim() == "SECTION")
                    {
                        sectionName = ((CGroupCode)gcList[x + 1]).Value.Trim().ToUpper();
                        sections.Add(sectionName, new ArrayList());
                    }
                    else if (
                        gc.Code == 0 &&
                        gc.Value.ToUpper().Trim() == "ENDSEC")
                    {
                    }
                    else if (
                        gc.Code == 0 &&
                        gc.Value.ToUpper().Trim() == "EOF")
                    {
                    }
                    else
                    {
                        if (
                            sections[sectionName] != null &&
                            sections[sectionName] is ArrayList)
                        {
                            ((ArrayList)sections[sectionName]).Add(gc);
                        }
                    }
                }

                #endregion

                if (sections.ContainsKey("HEADER"))
                {
                    #region Header Section

                    m_Header.HeaderVariableList = new Hashtable();

                    headerVariable = null;
                    for (int x = 0; x < ((ArrayList)sections["HEADER"]).Count; x++)
                    {
                        gc = (CGroupCode)((ArrayList)sections["HEADER"])[x];

                        if (gc.Code == 2 && gc.Value.Trim().ToUpper() == "HEADER")
                        {
                            // Header Section Tag - ignore
                        }
                        else if (gc.Code == 9)
                        {
                            // Header Variable Start
                            if (headerVariable != null)
                            {
                                m_Header.HeaderVariableList.Add(headerVariable.Name.Trim(), headerVariable);
                            }

                            headerVariable = new CHeaderVariable();
                            headerVariable.Name = gc.Value.Trim();
                            headerVariable.ParameterList = new ArrayList();
                        }
                        else
                        {
                            // Header Variable Parameter
                            headerVariableParameter = new CHeaderVariableParameter();
                            headerVariableParameter.GroupCode = gc.Code;
                            headerVariableParameter.Value = gc.Value;

                            if (headerVariable != null)
                            {
                                headerVariable.ParameterList.Add(headerVariableParameter);
                            }
                        }
                    }

                    if (headerVariable != null)
                    {
                        m_Header.HeaderVariableList.Add(headerVariable.Name, headerVariable);
                    }

                    if (
                        m_Header.HeaderVariableList.ContainsKey("$ACADVER") &&
                        ((CHeaderVariable)m_Header.HeaderVariableList["$ACADVER"]).ParameterList.Count > 0)
                    {
                        m_Header.Version =
                            ((CHeaderVariableParameter)((CHeaderVariable)m_Header.HeaderVariableList["$ACADVER"]).ParameterList[0]).Value.Trim();

                        m_Header.HeaderVariableList.Remove("$ACADVER");
                    }

                    if (
                        m_Header.HeaderVariableList.ContainsKey("$HANDSEED") &&
                        ((CHeaderVariable)m_Header.HeaderVariableList["$HANDSEED"]).ParameterList.Count > 0)
                    {
                        m_Header.HandleSeed = ((CHeaderVariableParameter)((CHeaderVariable)m_Header.HeaderVariableList["$HANDSEED"]).ParameterList[0]).Value.Trim();
                    }

                    m_Header.HeaderVariableList.Remove("$HANDSEED");

                    #endregion
                }

                if (sections.ContainsKey("TABLES"))
                {
                    #region Table Section

                    table = null;
                    inTableHeader = false;
                    for (int x = 0; x < ((ArrayList)sections["TABLES"]).Count; x++)
                    {
                        gc = (CGroupCode)((ArrayList)sections["TABLES"])[x];

                        if (
                            gc.Code == 2 &&
                            (gc.Value.Trim().ToUpper() == "TABLES" || tableTypes.ContainsKey(gc.Value.Trim().ToUpper())))
                        {
                            // Tables Section Tag - ignore
                            inTableHeader = true;
                        }
                        else if (
                            gc.Code == 0 &&
                            (gc.Value.Trim().ToUpper() == "TABLE" || gc.Value.Trim().ToUpper() == "ENDTAB"))
                        {
                            // Header or Footer - ignore
                        }
                        else if (
                            gc.Code == 0 &&
                            tableTypes.ContainsKey(gc.Value.Trim().ToUpper()))
                        {
                            inTableHeader = false;

                            #region Start Table

                            if (table != null)
                            {
                                table.ReadGroupCodes();

                                if (table is CTableApplicationID)
                                {
                                    if (m_ApplicationList.ContainsKey(((CTableApplicationID)table).ApplicationName.Trim().ToUpper()) == false)
                                    {
                                        m_ApplicationList.Add(((CTableApplicationID)table).ApplicationName.Trim().ToUpper(), table);
                                    }
                                }
                                else if (table is CTableDimensionStyle)
                                {
                                    if (m_DimensionStyleList.ContainsKey(((CTableDimensionStyle)table).DimensionStyleName.Trim().ToUpper()) == false)
                                    {
                                        m_DimensionStyleList.Add(((CTableDimensionStyle)table).DimensionStyleName.Trim().ToUpper(), table);
                                    }
                                }
                                else if (table is CTableLineType)
                                {
                                    if (m_LineTypeList.ContainsKey(((CTableLineType)table).LineTypeName.Trim().ToUpper()) == false)
                                    {
                                        m_LineTypeList.Add(((CTableLineType)table).LineTypeName.Trim().ToUpper(), table);
                                    }
                                }
                                else if (table is CTableLayer)
                                {
                                    if (m_LayerList.ContainsKey(((CTableLayer)table).LayerName.Trim().ToUpper()) == false)
                                    {
                                        m_LayerList.Add(((CTableLayer)table).LayerName.Trim().ToUpper(), table);
                                        ((CTableLayer)m_LayerList[((CTableLayer)table).LayerName.Trim().ToUpper()]).EntityList = new ArrayList();
                                    }
                                }
                                else if (table is CTableTextStyle)
                                {
                                    if (m_StyleList.ContainsKey(((CTableTextStyle)table).StyleName.Trim().ToUpper()) == false)
                                    {
                                        m_StyleList.Add(((CTableTextStyle)table).StyleName.Trim().ToUpper(), table);
                                    }
                                }
                                else if (table is CTableUCS)
                                {
                                    if (m_UCSList.ContainsKey(((CTableUCS)table).UCSName.Trim().ToUpper()) == false)
                                    {
                                        m_UCSList.Add(((CTableUCS)table).UCSName.Trim().ToUpper(), table);
                                    }
                                }
                                else if (table is CTableView)
                                {
                                    if (m_ViewList.ContainsKey(((CTableView)table).ViewName.Trim().ToUpper()) == false)
                                    {
                                        m_ViewList.Add(((CTableView)table).ViewName.Trim().ToUpper(), table);
                                    }
                                }
                                else if (table is CTableViewport)
                                {
                                    m_ViewportList.Add(table);
                                }
                                else
                                {
                                    m_UnknownTableList.Add(table);
                                }
                            }

                            if (tableTypes.ContainsKey(gc.Value.Trim().ToUpper()))
                            {
                                table = (CTable)Activator.CreateInstance((Type)tableTypes[gc.Value.Trim().ToUpper()]);
                            }
                            else
                            {
                                table = new CTable();
                            }

                            table.GroupCodeList = new ArrayList();

                            #endregion
                        }
                        else
                        {
                            if (table != null && inTableHeader == false)
                            {
                                table.GroupCodeList.Add(gc);
                            }
                        }
                    }

                    if (table != null)
                    {
                        #region Add Last Table

                        table.ReadGroupCodes();

                        if (table is CTableApplicationID)
                        {
                            if (m_ApplicationList.ContainsKey(((CTableApplicationID)table).ApplicationName.Trim().ToUpper()) == false)
                            {
                                m_ApplicationList.Add(((CTableApplicationID)table).ApplicationName.Trim().ToUpper(), table);
                            }
                        }
                        else if (table is CTableDimensionStyle)
                        {
                            if (m_DimensionStyleList.ContainsKey(((CTableDimensionStyle)table).DimensionStyleName.Trim().ToUpper()) == false)
                            {
                                m_DimensionStyleList.Add(((CTableDimensionStyle)table).DimensionStyleName.Trim().ToUpper(), table);
                            }
                        }
                        else if (table is CTableLineType)
                        {
                            if (m_LineTypeList.ContainsKey(((CTableLineType)table).LineTypeName.Trim().ToUpper()) == false)
                            {
                                m_LineTypeList.Add(((CTableLineType)table).LineTypeName.Trim().ToUpper(), table);
                            }
                        }
                        else if (table is CTableLayer)
                        {
                            if (m_LayerList.ContainsKey(((CTableLayer)table).LayerName.Trim().ToUpper()) == false)
                            {
                                m_LayerList.Add(((CTableLayer)table).LayerName.Trim().ToUpper(), table);
                                ((CTableLayer)m_LayerList[((CTableLayer)table).LayerName.Trim().ToUpper()]).EntityList = new ArrayList();
                            }
                        }
                        else if (table is CTableTextStyle)
                        {
                            if (m_StyleList.ContainsKey(((CTableTextStyle)table).StyleName.Trim().ToUpper()) == false)
                            {
                                m_StyleList.Add(((CTableTextStyle)table).StyleName.Trim().ToUpper(), table);
                            }
                        }
                        else if (table is CTableUCS)
                        {
                            if (m_UCSList.ContainsKey(((CTableUCS)table).UCSName.Trim().ToUpper()) == false)
                            {
                                m_UCSList.Add(((CTableUCS)table).UCSName.Trim().ToUpper(), table);
                            }
                        }
                        else if (table is CTableView)
                        {
                            if (m_ViewList.ContainsKey(((CTableView)table).ViewName.Trim().ToUpper()) == false)
                            {
                                m_ViewList.Add(((CTableView)table).ViewName.Trim().ToUpper(), table);
                            }
                        }
                        else if (table is CTableViewport)
                        {
                            m_ViewportList.Add(table);
                        }
                        else
                        {
                            m_UnknownTableList.Add(table);
                        }

                        #endregion
                    }

                    #endregion
                }

                if (sections.ContainsKey("BLOCKS"))
                {
                    #region Blocks Section

                    block = null;
                    for (int x = 0; x < ((ArrayList)sections["BLOCKS"]).Count; x++)
                    {
                        gc = (CGroupCode)((ArrayList)sections["BLOCKS"])[x];

                        if (
                            gc.Code == 2 &&
                            gc.Value.Trim().ToUpper() == "BLOCKS")
                        {
                            // Tables Section Tag - ignore
                        }
                        else if (
                            gc.Code == 0 &&
                            gc.Value.Trim().ToUpper() == "BLOCK")
                        {
                            if (block != null)
                            {
                                BlockList.Add(block);
                            }

                            block = new CBlock();
                        }
                        else if (
                            gc.Code == 0 &&
                            gc.Value.Trim().ToUpper() == "ENDBLK")
                        {
                            // Ignore End Block Record
                        }
                        else if (
                            gc.Code == 2 &&
                            gc.Value.ToUpper().Trim() != "BLOCKS")
                        {
                            if (block != null) block.BlockName = gc.Value.Trim();
                        }
                        else if (gc.Code == 5)
                        {
                            if (block != null) block.Handle = gc.Value.Trim();
                        }
                        else if (gc.Code == 8)
                        {
                            if (block != null) block.LayerName = gc.Value.Trim();
                        }
                        else
                        {
                            if (block != null) block.GroupCodeList.Add(gc);
                        }
                    }

                    if (block != null)
                    {
                        BlockList.Add(block);
                    }

                    #endregion
                }

                if (sections.ContainsKey("ENTITIES"))
                {
                    #region Entities Section

                    processingChildren = false;
                    entity = null;
                    for (int x = 0; x < ((ArrayList)sections["ENTITIES"]).Count; x++)
                    {
                        gc = (CGroupCode)((ArrayList)sections["ENTITIES"])[x];

                        if (
                            gc.Code == 2 &&
                            gc.Value.Trim().ToUpper() == "ENTITIES")
                        {
                            // Entities Section Tag - ignore
                        }
                        else if (gc.Code == 0)
                        {
                            #region Start Entity

                            if (entity != null)
                            {
                                if (entity.ChildTypeList.Contains(gc.Value.Trim().ToUpper()))
                                {
                                    entity.GroupCodeList.Add(gc);
                                    processingChildren = true;
                                }
                                else
                                {
                                    if (m_LayerList.ContainsKey(entity.LayerName.Trim().ToUpper()) == false)
                                    {
                                        m_LayerList.Add(entity.LayerName.Trim().ToUpper(), new CTableLayer());
                                        ((CTableLayer)m_LayerList[entity.LayerName.Trim().ToUpper()]).LayerName = entity.LayerName.Trim().ToUpper();
                                        ((CTableLayer)m_LayerList[entity.LayerName.Trim().ToUpper()]).EntityList = new ArrayList();
                                    }

                                    entity.ReadGroupCodes();

                                    ((CTableLayer)m_LayerList[entity.LayerName.Trim().ToUpper()]).EntityList.Add(entity);

                                    if (entityTypes.ContainsKey(gc.Value.Trim().ToUpper()))
                                    {
                                        entity = (CEntity)Activator.CreateInstance((Type)entityTypes[gc.Value.Trim().ToUpper()]);
                                    }
                                    else
                                    {
                                        entity = new CEntity();
                                    }
                                    entity.GroupCodeList = new ArrayList();
                                    entity.EntityName = gc.Value.Trim().ToUpper();

                                    processingChildren = false;
                                }
                            }
                            else
                            {
                                if (entityTypes.ContainsKey(gc.Value.Trim().ToUpper()))
                                {
                                    entity = (CEntity)Activator.CreateInstance((Type)entityTypes[gc.Value.Trim().ToUpper()]);
                                }
                                else
                                {
                                    entity = new CEntity();
                                }
                                entity.GroupCodeList = new ArrayList();
                                entity.EntityName = gc.Value.Trim().ToUpper();

                                processingChildren = false;
                            }

                            #endregion
                        }
                        else if (gc.Code == 5)
                        {
                            // Handle

                            if (entity != null && !processingChildren) entity.Handle = gc.Value.Trim();
                        }
                        else if (gc.Code == 6)
                        {
                            // Line Type

                            if (entity != null && !processingChildren) entity.LineType = gc.Value.Trim();
                        }
                        else if (gc.Code == 8)
                        {
                            // Layer Name
                            if (entity != null && !processingChildren) entity.LayerName = gc.Value.Trim();
                        }
                        else
                        {
                            // Group Code List
                            if (entity != null)
                            {
                                entity.GroupCodeList.Add(gc);
                            }
                        }

                    }

                    if (entity != null)
                    {
                        if (m_LayerList.ContainsKey(entity.LayerName.Trim().ToUpper()) == false)
                        {
                            m_LayerList.Add(entity.LayerName.Trim().ToUpper(), new CTableLayer());
                            ((CTableLayer)m_LayerList[entity.LayerName.Trim().ToUpper()]).LayerName = entity.LayerName.Trim().ToUpper();
                            ((CTableLayer)m_LayerList[entity.LayerName.Trim().ToUpper()]).EntityList = new ArrayList();
                        }

                        entity.ReadGroupCodes();

                        ((CTableLayer)m_LayerList[entity.LayerName.Trim().ToUpper()]).EntityList.Add(entity);
                    }

                    #endregion
                }
            }
            catch (Exception e)
            {
                throw new Exception("CDXF: Exception Caught: See Inner Exception For Details", e);
            }
            finally
            {
            }
        }

		#endregion

		#region Writers

		public void Save()
		{
			SaveAs(FileLocation.Trim());
		}
		public void SaveAs(string file)
		{
			try
			{
                File.WriteAllBytes(file, SaveAsBytes());
			}
			catch(Exception e)
			{
                throw new Exception("CDXF: Exception Caught: See Inner Exception For Details", e);
            }
		}

        public byte[] SaveAsBytes()
        {
            StreamWriter writer = null;
            MemoryStream stream = null;
            ArrayList gcList = null;

            try
            {
                RenumberHandles();

                stream = new MemoryStream();
                writer = new StreamWriter(stream);

                #region Header Section

                writer.WriteLine("0");
                writer.WriteLine("SECTION");
                writer.WriteLine("2");
                writer.WriteLine("HEADER");

                if (Header != null && Header.HeaderVariableList != null)
                {
                    Header.Version = "AC1009";

                    writer.WriteLine("9");
                    writer.WriteLine("$ACADVER");

                    writer.WriteLine("1");
                    writer.WriteLine(Header.Version.Trim());

                    writer.WriteLine("9");
                    writer.WriteLine("$HANDSEED");

                    writer.WriteLine("5");
                    writer.WriteLine(Header.HandleSeed.Trim());

                    foreach (CHeaderVariable var in Header.HeaderVariableList.Values)
                    {
                        writer.WriteLine("9");
                        writer.WriteLine(var.Name.Trim());

                        if (var.ParameterList != null)
                        {
                            foreach (CHeaderVariableParameter param in var.ParameterList)
                            {
                                writer.WriteLine(param.GroupCode.ToString().Trim());
                                writer.WriteLine(param.Value.Trim());
                            }
                        }
                    }
                }

                writer.WriteLine("0");
                writer.WriteLine("ENDSEC");

                #endregion

                #region Tables Section

                writer.WriteLine("0");
                writer.WriteLine("SECTION");
                writer.WriteLine("2");
                writer.WriteLine("TABLES");

                #region VPORT

                writer.WriteLine("0");
                writer.WriteLine("TABLE");
                writer.WriteLine("2");
                writer.WriteLine("VPORT");

                writer.WriteLine("70");
                writer.WriteLine(ViewportList.Count.ToString().Trim());

                foreach (CTableViewport table in ViewportList)
                {
                    writer.WriteLine("0");
                    writer.WriteLine("VPORT");

                    table.WriteGroupCodes();

                    foreach (CGroupCode gc in table.GroupCodeList)
                    {
                        writer.WriteLine(gc.Code.ToString().Trim());
                        writer.WriteLine(gc.Value.Trim());
                    }
                }

                writer.WriteLine("0");
                writer.WriteLine("ENDTAB");

                #endregion

                #region LTYPE

                writer.WriteLine("0");
                writer.WriteLine("TABLE");
                writer.WriteLine("2");
                writer.WriteLine("LTYPE");

                writer.WriteLine("70");
                writer.WriteLine(LineTypeList.Count.ToString().Trim());

                foreach (CTableLineType table in LineTypeList.Values)
                {
                    writer.WriteLine("0");
                    writer.WriteLine("LTYPE");

                    table.WriteGroupCodes();

                    foreach (CGroupCode gc in table.GroupCodeList)
                    {
                        writer.WriteLine(gc.Code.ToString().Trim());
                        writer.WriteLine(gc.Value.Trim());
                    }
                }

                writer.WriteLine("0");
                writer.WriteLine("ENDTAB");

                #endregion

                #region LAYER

                writer.WriteLine("0");
                writer.WriteLine("TABLE");
                writer.WriteLine("2");
                writer.WriteLine("LAYER");

                writer.WriteLine("70");
                writer.WriteLine(LayerList.Count.ToString().Trim());

                foreach (CTableLayer table in LayerList.Values)
                {
                    writer.WriteLine("0");
                    writer.WriteLine("LAYER");

                    table.WriteGroupCodes();

                    foreach (CGroupCode gc in table.GroupCodeList)
                    {
                        writer.WriteLine(gc.Code.ToString().Trim());
                        writer.WriteLine(gc.Value.Trim());
                    }
                }

                writer.WriteLine("0");
                writer.WriteLine("ENDTAB");

                #endregion

                #region STYLE

                writer.WriteLine("0");
                writer.WriteLine("TABLE");
                writer.WriteLine("2");
                writer.WriteLine("STYLE");

                writer.WriteLine("70");
                writer.WriteLine(StyleList.Count.ToString().Trim());

                foreach (CTableTextStyle table in StyleList.Values)
                {
                    writer.WriteLine("0");
                    writer.WriteLine("STYLE");

                    table.WriteGroupCodes();

                    foreach (CGroupCode gc in table.GroupCodeList)
                    {
                        writer.WriteLine(gc.Code.ToString().Trim());
                        writer.WriteLine(gc.Value.Trim());
                    }
                }

                writer.WriteLine("0");
                writer.WriteLine("ENDTAB");

                #endregion

                #region VIEW

                writer.WriteLine("0");
                writer.WriteLine("TABLE");
                writer.WriteLine("2");
                writer.WriteLine("VIEW");

                writer.WriteLine("70");
                writer.WriteLine(ViewList.Count.ToString().Trim());

                foreach (CTableView table in ViewList.Values)
                {
                    writer.WriteLine("0");
                    writer.WriteLine("VIEW");

                    table.WriteGroupCodes();

                    foreach (CGroupCode gc in table.GroupCodeList)
                    {
                        writer.WriteLine(gc.Code.ToString().Trim());
                        writer.WriteLine(gc.Value.Trim());
                    }
                }

                writer.WriteLine("0");
                writer.WriteLine("ENDTAB");

                #endregion

                #region DIMSTYLE

                writer.WriteLine("0");
                writer.WriteLine("TABLE");
                writer.WriteLine("2");
                writer.WriteLine("DIMSTYLE");

                writer.WriteLine("70");
                writer.WriteLine(DimensionStyleList.Count.ToString().Trim());

                foreach (CTableDimensionStyle table in DimensionStyleList.Values)
                {
                    writer.WriteLine("0");
                    writer.WriteLine("DIMSTYLE");

                    table.WriteGroupCodes();

                    foreach (CGroupCode gc in table.GroupCodeList)
                    {
                        writer.WriteLine(gc.Code.ToString().Trim());
                        writer.WriteLine(gc.Value.Trim());
                    }
                }

                writer.WriteLine("0");
                writer.WriteLine("ENDTAB");

                #endregion

                #region UCS

                writer.WriteLine("0");
                writer.WriteLine("TABLE");
                writer.WriteLine("2");
                writer.WriteLine("UCS");

                writer.WriteLine("70");
                writer.WriteLine(UCSList.Count.ToString().Trim());

                foreach (CTableUCS table in UCSList.Values)
                {
                    writer.WriteLine("0");
                    writer.WriteLine("UCS");

                    table.WriteGroupCodes();

                    foreach (CGroupCode gc in table.GroupCodeList)
                    {
                        writer.WriteLine(gc.Code.ToString().Trim());
                        writer.WriteLine(gc.Value.Trim());
                    }
                }

                writer.WriteLine("0");
                writer.WriteLine("ENDTAB");

                #endregion

                #region APPID

                writer.WriteLine("0");
                writer.WriteLine("TABLE");
                writer.WriteLine("2");
                writer.WriteLine("APPID");

                writer.WriteLine("70");
                writer.WriteLine(ApplicationList.Count.ToString().Trim());

                foreach (CTableApplicationID table in ApplicationList.Values)
                {
                    writer.WriteLine("0");
                    writer.WriteLine("APPID");

                    table.WriteGroupCodes();

                    foreach (CGroupCode gc in table.GroupCodeList)
                    {
                        writer.WriteLine(gc.Code.ToString().Trim());
                        writer.WriteLine(gc.Value.Trim());
                    }
                }

                writer.WriteLine("0");
                writer.WriteLine("ENDTAB");

                #endregion

                writer.WriteLine("0");
                writer.WriteLine("ENDSEC");

                #endregion

                #region Blocks Section

                writer.WriteLine("0");
                writer.WriteLine("SECTION");
                writer.WriteLine("2");
                writer.WriteLine("BLOCKS");

                foreach (CBlock block in BlockList)
                {
                    writer.WriteLine("0");
                    writer.WriteLine("BLOCK");
                    writer.WriteLine("8");
                    writer.WriteLine(block.LayerName.Trim());
                    writer.WriteLine("2");
                    writer.WriteLine(block.BlockName.Trim());

                    foreach (CGroupCode gc in block.GroupCodeList)
                    {
                        writer.WriteLine(gc.Code.ToString().Trim());
                        writer.WriteLine(gc.Value.Trim());
                    }

                    writer.WriteLine("0");
                    writer.WriteLine("ENDBLK");
                    writer.WriteLine("5");
                    writer.WriteLine(block.Handle.Trim());
                    writer.WriteLine("8");
                    writer.WriteLine(block.LayerName.Trim());
                }

                writer.WriteLine("0");
                writer.WriteLine("ENDSEC");

                #endregion

                #region Entities Section

                writer.WriteLine("0");
                writer.WriteLine("SECTION");
                writer.WriteLine("2");
                writer.WriteLine("ENTITIES");

                foreach (CTableLayer layer in LayerList.Values)
                {
                    foreach (CEntity entity in layer.EntityList)
                    {
                        writer.WriteLine("0");
                        writer.WriteLine(entity.EntityName.Trim());

                        writer.WriteLine("5");
                        writer.WriteLine(entity.Handle.Trim());

                        writer.WriteLine("6");
                        writer.WriteLine(entity.LineType.Trim());

                        writer.WriteLine("8");
                        writer.WriteLine(entity.LayerName.Trim());

                        entity.GroupCodeList.Clear();
                        entity.WriteGroupCodes();

                        gcList = entity.GetGroupCodes();

                        foreach (CGroupCode gc in gcList)
                        {
                            writer.WriteLine(gc.Code.ToString().Trim());
                            writer.WriteLine(gc.Value.Trim());
                        }
                    }
                }

                writer.WriteLine("0");
                writer.WriteLine("ENDSEC");

                #endregion

                #region End Of File

                writer.WriteLine("0");
                writer.WriteLine("EOF");

                #endregion

                writer.Flush();

                return stream.ToArray();
            }
            catch (Exception e)
            {
                throw new Exception("CDXF: Exception Caught: See Inner Exception For Details", e);
            }
            finally
            {
                #region Writer

                if (writer != null)
                {
                    try { writer.Flush(); }
                    catch (Exception) { }

                    try { writer.Close(); }
                    catch (Exception) { }

                    writer = null;
                }

                #endregion

                #region Stream

                if (stream != null)
                {
                    try { stream.Flush(); }
                    catch (Exception) { }

                    try { stream.Close(); }
                    catch (Exception) { }

                    stream = null;
                }

                #endregion
            }
        }

		private void RenumberHandles()
		{
			int x;
	
			x = 1;

			foreach(CBlock block in BlockList)
			{
				block.Handle = CGlobals.IntToHex(x);
				x++;
			}

			foreach(CTableLayer layer in LayerList.Values)
			{
				foreach(CEntity entity in layer.EntityList)
				{
					entity.Handle = CGlobals.IntToHex(x);
					x++;

					x = entity.RenumberHandles(x);
				}
			}

			Header.HandleSeed = CGlobals.IntToHex(x);
		}

		#endregion

		#region New DXF Generator

		public static CDXF NewDXF()
		{
			Assembly assy;
            Stream stream = null;
			CDXF dxf;
			byte[] byteStream;
			
			try
			{						
				assy = typeof(CDXF).Assembly;

				stream = assy.GetManifestResourceStream("DXFLib.blankR12.dxf");
			
				byteStream = new byte[stream.Length];
				stream.Read(byteStream, 0, (int) stream.Length);
				stream.Close();
		
				dxf = new CDXF();
				dxf.Read(byteStream);

				return dxf;
			}
			catch(Exception e)
			{
				throw e;
			}
			finally
			{
                try
                {
                    if (stream != null) stream.Close();
                }
                catch (Exception)
                {
                }
			}
		}

		#endregion

		#region Temp File Utilities

		public static string GetUniqueTempDir()
		{
			FileInfo tempFileInfo;
			bool isUnique;
			string dir;
			string tempDir;

			tempDir = Path.GetTempPath().Trim();

			if(tempDir.EndsWith(@"\") == false) tempDir += @"\";

			isUnique = false;

			dir = "";
			while(!isUnique)
			{
				tempFileInfo = new FileInfo(Path.GetTempFileName().Trim());
				tempFileInfo.Delete();
			
				dir = tempDir + tempFileInfo.Name.Trim().Substring(0, tempFileInfo.Name.Trim().Length - 4) + "\\";
			
				if(Directory.Exists(dir) == false) isUnique = true;
			}

			return dir;
		}
		public static void DeleteDir(string dirLocation)
		{
		
			try
			{
				if(Directory.Exists(dirLocation))
				{
					Directory.Delete(dirLocation, true);
				}
			}
			catch(Exception)
			{
			}

			try
			{
				if(Directory.Exists(dirLocation))
				{
                    System.Threading.Thread.Sleep(1000);
					Directory.Delete(dirLocation, true);
				}
			}
			catch(Exception)
			{
			}
		}

		#endregion

		#region Other Methods

		public void Rotate(double deg)
		{
			foreach(CTableLayer layer in LayerList.Values)
			{
				foreach(CEntity entity in layer.EntityList)
				{
					entity.Rotate(deg);
				}
			}
		}
        public void FixIncorrectZeroPoint(string shapeLayerName)
        {
            if (shapeLayerName == "")
            {
                shapeLayerName = "shape";
            }
            if (ContainsLayer(shapeLayerName))
            {
                shapeLayerName = shapeLayerName.ToUpper();
                double yAdder = 100000000;
                double xAdder = 100000000;
                bool foundZeroPoint = false;
                foreach (CTableLayer layer in this.LayerList.Values)
                {
                    foreach (object obj in layer.EntityList)
                    {
                        if (obj is CEntityLine)
                        {
                            CEntityLine line = obj as CEntityLine;
                            if (line.X0 > line.X1)
                            {
                                double hold = line.X0;
                                line.X0 = line.X1;
                                line.X1 = hold;
                                hold = line.StartAngle;
                                line.StartAngle = line.EndAngle;
                                line.EndAngle = hold;
                            }
                            if (line.Y0 > line.Y1)
                            {
                                double hold = line.Y0;
                                line.Y0 = line.Y1;
                                line.Y1 = hold;
                            }
                        }
                    }
                }
                foreach (CTableLayer layer in this.LayerList.Values)
                {
                    if (layer.LayerName.ToUpper() == shapeLayerName)
                    {
                        foreach (object obj in layer.EntityList)
                        {
                            if (obj is CEntityLine)
                            {
                                CEntityLine line = obj as CEntityLine;
                                if (line != null)
                                {
                                    foundZeroPoint = true;
                                    if (line.Y0 < yAdder)
                                    {
                                        yAdder = line.Y0;
                                    }
                                    if (line.X0 < xAdder)
                                    {
                                        xAdder = line.X0;
                                    }
                                }
                            }
                        }
                    }
                }
                if (!foundZeroPoint)//no lines
                {
                    foreach (CTableLayer layer in this.LayerList.Values)
                    {
                        if (layer.LayerName.ToUpper() == shapeLayerName)
                        {
                            foreach (object obj in layer.EntityList)
                            {
                                if (obj is CEntity)
                                {
                                    CEntity enty = obj as CEntity;
                                    if (enty != null)
                                    {
                                        if (enty.Y0 < yAdder)
                                        {
                                            yAdder = enty.Y0;
                                        }
                                        if (enty.X0 < xAdder)
                                        {
                                            xAdder = enty.X0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (yAdder < 0)
                {
                    yAdder = Math.Abs(yAdder);
                }
                else
                {
                    yAdder = -(yAdder);
                }
                if (xAdder < 0)
                {
                    xAdder = Math.Abs(xAdder);
                }
                else
                {
                    xAdder = -(xAdder);
                }
                foreach (CTableLayer layer in this.LayerList.Values)
                {
                    foreach (object obj in layer.EntityList)
                    {
                        if (obj is CEntity)
                        {
                            CEntity entityToReposition = obj as CEntity;
                            if (entityToReposition != null)
                            {
                                entityToReposition.Y0 += yAdder;
                                entityToReposition.X0 += xAdder;
                                if (obj is CEntityLine)
                                {
                                    entityToReposition.Y1 += yAdder;
                                    entityToReposition.X1 += xAdder;
                                }
                                entityToReposition.Y0 = Math.Round(entityToReposition.Y0, 3);
                                entityToReposition.Y1 = Math.Round(entityToReposition.Y1, 3);
                                entityToReposition.X0 = Math.Round(entityToReposition.X0, 3);
                                entityToReposition.X1 = Math.Round(entityToReposition.X1, 3);
                            }
                        }
                    }
                }
            }
        }
        public bool ContainsLayer(string layerName)
        {
            layerName = layerName.ToUpper();
            foreach (CTableLayer layer in this.LayerList.Values)
            {
                if (layer.LayerName.ToUpper() == layerName)
                {
                    return true;
                }
            }
            return false;
        }
        public double DXF_PartHeight(string shapeLayerName)
        {
            double minY = 10000;
            double maxY = 0;
            foreach (CTableLayer layer in this.LayerList.Values)
            {
                if (layer.LayerName.ToUpper() == shapeLayerName.ToUpper())
                {
                    foreach (object obj in layer.EntityList)
                    {
                        if (obj is CEntityLine)
                        {
                            CEntityLine line = obj as CEntityLine;
                            if (line != null)
                            {
                                if (line.Y0 < minY)
                                {
                                    minY = line.Y0;
                                }
                                if (line.Y1 < minY)
                                {
                                    minY = line.Y1;
                                }
                                if (line.Y0 > maxY)
                                {
                                    maxY = line.Y0;
                                }
                                if (line.Y1 > maxY)
                                {
                                    maxY = line.Y1;
                                }
                            }
                        }
                    }
                }
            }
            return maxY - minY;
        }
        public double DXF_PartWidth(string shapeLayerName)
        {
            double minX = 10000;
            double maxX = 0;
            foreach (CTableLayer layer in this.LayerList.Values)
            {
                if (layer.LayerName.ToUpper() == shapeLayerName.ToUpper())
                {
                    foreach (object obj in layer.EntityList)
                    {
                        if (obj is CEntityLine)
                        {
                            CEntityLine line = obj as CEntityLine;
                            if (line != null)
                            {
                                if (line.X0 < minX)
                                {
                                    minX = line.X0;
                                }
                                if (line.X1 < minX)
                                {
                                    minX = line.X1;
                                }
                                if (line.X0 > maxX)
                                {
                                    maxX = line.X0;
                                }
                                if (line.X1 > maxX)
                                {
                                    maxX = line.X1;
                                }
                            }
                        }
                    }
                }
            }
            return maxX - minX;
        }

		#endregion
	}
}

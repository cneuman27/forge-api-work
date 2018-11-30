using System;
using System.Collections;

namespace DXFLib
{
	public class CBlock 
	{
		#region Private Member Data

		private string m_BlockName = "";
		private string m_Handle = "";
		private string m_LayerName = "";

		private ArrayList m_GroupCodeList = new ArrayList();

		#endregion

		#region Constructors

		public CBlock()
		{
		}

		#endregion

		#region Public Properties

		public string BlockName
		{
			get { return m_BlockName; }
			set { m_BlockName = value; }
		}
		public string Handle
		{
			get { return m_Handle; }
			set { m_Handle = value; }
		}
		public string LayerName
		{
			get { return m_LayerName; }
			set { m_LayerName = value; }
		}
        
		public ArrayList GroupCodeList
		{
			get { return m_GroupCodeList; }
			set { m_GroupCodeList = value; }
		}

		#endregion
	}
}

using System;
using System.Collections;

namespace DXFLib
{
	public class CHeader 
	{
		#region Private Member Data

		private string m_Version = "";
		private string m_HandleSeed = "";
		private Hashtable m_HeaderVariableList = null;

		#endregion

		#region Constructors

		public CHeader()
		{
		}
        
		#endregion

		#region Public Properties
	
		public string Version
		{
			get { return m_Version; }
			set { m_Version = value; }
		}
		public string HandleSeed
		{
			get { return m_HandleSeed; }
			set { m_HandleSeed = value; }
		}
		public Hashtable HeaderVariableList
		{
			get { return m_HeaderVariableList; }
			set { m_HeaderVariableList = value; }
		}

		#endregion
	}
}

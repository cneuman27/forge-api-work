using System;
using System.Collections;

namespace DXFLib
{
	public class CHeaderVariable 
	{
		#region Private Member Data

		private string m_Name = "";
		private ArrayList m_ParameterList = null;

		#endregion

		#region Constructors

		public CHeaderVariable()
		{
		}

		#endregion

		#region Public Properties

		public string Name
		{
			get { return m_Name; }
			set { m_Name = value; }
		}
		public ArrayList ParameterList
		{
			get { return m_ParameterList; }
			set { m_ParameterList = value; }
		}
               
		#endregion
	}
}

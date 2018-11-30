using System;

namespace DXFLib
{
	public class CHeaderVariableParameter
	{
		#region Private Member Data

		private int m_GroupCode;
		private string m_Value;

		#endregion

		#region Constructors

		public CHeaderVariableParameter()
		{
		}
        
		#endregion
	
		#region Public Properties
		
		public int GroupCode
		{
			get { return m_GroupCode; }
			set { m_GroupCode = value; }
		}
        public string Value
		{
			get { return m_Value; }
			set { m_Value = value; }
		}
        
		#endregion
	}
}

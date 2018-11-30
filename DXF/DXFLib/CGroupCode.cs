using System;

namespace DXFLib
{
	public class CGroupCode : ICloneable
	{
		#region Private Member Data

		private int m_Code = -1;
		private string m_Value = "";

		#endregion

		#region Constructors

		public CGroupCode()
		{
		}
        
		#endregion
		
		#region Public Properties

		public int Code
		{
			get { return m_Code; }
			set { m_Code = value; }
		}
		public string Value
		{
			get { return m_Value; }
			set { m_Value = value; }
		}
        
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			CGroupCode gc;

			gc = new CGroupCode();

			gc.Code = Code;
			gc.Value = Value;

			return gc;
		}

		#endregion
	}
}

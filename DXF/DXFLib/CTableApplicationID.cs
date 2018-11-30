using System;
using System.Collections;

namespace DXFLib
{
	public class CTableApplicationID : CTable
	{
		#region Private Member Data

		private string m_ApplicationName = "";

		#endregion

		#region Constructors

		public CTableApplicationID() : base()
		{
		}
        
		#endregion

		#region Public Properties
		
		public string ApplicationName
		{
			get { return m_ApplicationName; }
			set { m_ApplicationName = value; }
		}
        
		#endregion

		#region CTable Overrides

		public override void ReadGroupCodes()
		{
			foreach(CGroupCode gc in GroupCodeList)
			{
				if(gc.Code == 2)
				{
					ApplicationName = gc.Value.Trim();
				}
				else if(gc.Code == 70)
				{
					SetStandardFlags(CGlobals.ConvertToInt(gc.Value.Trim()));
				}
			}
		}
		public override void WriteGroupCodes()
		{
			WriteGroupCodeValue(2, ApplicationName.Trim());
			WriteGroupCodeValue(70, GetStandardFlags().ToString().Trim());
		}
        
		#endregion
	}
}

using System;
using System.Collections;

namespace DXFLib
{
	public class CTableUCS : CTable
	{
		#region Private Member Data

		private string m_UCSName = "";
		
		private double m_OriginX = 0.000;
		private double m_OriginY = 0.000;
		private double m_OriginZ = 0.000;

		private double m_XAxisX = 0.000;
		private double m_XAxisY = 0.000;
		private double m_XAxisZ = 0.000;

		private double m_YAxisX = 0.000;
		private double m_YAxisY = 0.000;
		private double m_YAxisZ = 0.000;

		#endregion

		#region Constructors

		public CTableUCS() : base()
		{
		}
        
		#endregion

		#region Public Properties

		public string UCSName
		{
			get { return m_UCSName; }
			set { m_UCSName = value; }
		}
		
		public double OriginX
		{
			get { return m_OriginX; }
			set { m_OriginX = value; }
		}
		public double OriginY
		{
			get { return m_OriginY; }
			set { m_OriginY = value; }
		}
		public double OriginZ
		{
			get { return m_OriginZ; }
			set { m_OriginZ = value; }
		}

		public double XAxisX
		{
			get { return m_XAxisX; }
			set { m_XAxisX = value; }
		}
		public double XAxisY
		{
			get { return m_XAxisY; }
			set { m_XAxisY = value; }
		}
		public double XAxisZ
		{
			get { return m_XAxisZ; }
			set { m_XAxisZ = value; }
		}

		public double YAxisX
		{
			get { return m_YAxisX; }
			set { m_YAxisX = value; }
		}
		public double YAxisY
		{
			get { return m_YAxisY; }
			set { m_YAxisY = value; }
		}
		public double YAxisZ
		{
			get { return m_YAxisZ; }
			set { m_YAxisZ = value; }
		}

		#endregion

		#region CTable Overrides

		public override void ReadGroupCodes()
		{
			foreach(CGroupCode gc in GroupCodeList)
			{
				if(gc.Code == 2)
				{
					UCSName = gc.Value.Trim();
				}
				else if(gc.Code == 10)
				{
					OriginX = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 20)
				{
					OriginY = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 30)
				{
					OriginZ = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 11)
				{
					XAxisX = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 21)
				{
					XAxisY = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 31)
				{
					XAxisZ = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 12)
				{
					YAxisX = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 22)
				{
					YAxisY = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 32)
				{
					YAxisZ = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 70)
				{
					SetStandardFlags(CGlobals.ConvertToInt(gc.Value.Trim()));
				}
			}
		}
		public override void WriteGroupCodes()
		{
			WriteGroupCodeValue(2, UCSName.Trim());

			WriteGroupCodeValue(10, OriginX.ToString().Trim());
			WriteGroupCodeValue(11, OriginY.ToString().Trim());
			WriteGroupCodeValue(12, OriginZ.ToString().Trim());

			WriteGroupCodeValue(20, XAxisX.ToString().Trim());
			WriteGroupCodeValue(21, XAxisY.ToString().Trim());
			WriteGroupCodeValue(22, XAxisZ.ToString().Trim());

			WriteGroupCodeValue(30, YAxisX.ToString().Trim());
			WriteGroupCodeValue(31, YAxisY.ToString().Trim());
			WriteGroupCodeValue(32, YAxisZ.ToString().Trim());
			
			WriteGroupCodeValue(70, GetStandardFlags().ToString().Trim());
		}
        
		#endregion
	}
}

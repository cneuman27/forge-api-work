using System;
using System.Collections;

namespace DXFLib
{
	public class CTableView : CTable
	{
		#region Private Member Data

		private string m_ViewName = "";

		private double m_ViewHeight = 0.000;
		private double m_ViewWidth = 0.000;

		private double m_ViewCenterX = 0.000;
		private double m_ViewCenterY = 0.000;

		private double m_ViewDirectionX = 0.000;
		private double m_ViewDirectionY = 0.000;
		private double m_ViewDirectionZ = 0.000;

		private double m_ViewTargetX = 0.000;
		private double m_ViewTargetY = 0.000;
		private double m_ViewTargetZ = 0.000;

		private double m_LensLength = 0.000;

		private double m_FrontClippingPlaneOffset = 0.000;
		private double m_BackClippingPlaneOffset = 0.000;

		private double m_TwistAngle = 0.000;

		#endregion

		#region Constructors

		public CTableView() : base()
		{
		}

		#endregion

		#region Public Properties

		public string ViewName
		{
			get { return m_ViewName; }
			set { m_ViewName = value; }
		}

		public double ViewHeight
		{
			get { return m_ViewHeight; }
			set { m_ViewHeight = value; }
		}
		public double ViewWidth
		{
			get { return m_ViewWidth; }
			set { m_ViewWidth = value; }
		}

		public double ViewCenterX
		{
			get { return m_ViewCenterX; }
			set { m_ViewCenterX = value; }
		}
		public double ViewCenterY
		{
			get { return m_ViewCenterY; }
			set { m_ViewCenterY = value; }
		}

		public double ViewDirectionX
		{
			get { return m_ViewDirectionX; }
			set { m_ViewDirectionX = value; }
		}
		public double ViewDirectionY
		{
			get { return m_ViewDirectionY; }
			set { m_ViewDirectionY = value; }
		}
		public double ViewDirectionZ
		{
			get { return m_ViewDirectionZ; }
			set { m_ViewDirectionZ = value; }
		}
	
		public double ViewTargetX
		{
			get { return m_ViewTargetX; }
			set { m_ViewTargetX = value; }
		}
		public double ViewTargetY
		{
			get { return m_ViewTargetY; }
			set { m_ViewTargetY = value; }
		}
		public double ViewTargetZ
		{
			get { return m_ViewTargetZ; }
			set { m_ViewTargetZ = value; }
		}

		public double LensLength
		{
			get { return m_LensLength; }
			set { m_LensLength = value; }
		}

		public double FrontClippingPlaneOffset
		{
			get { return m_FrontClippingPlaneOffset; }
			set { m_FrontClippingPlaneOffset = value; }
		}
		public double BackClippingPlaneOffset
		{
			get { return m_BackClippingPlaneOffset; }
			set { m_BackClippingPlaneOffset = value; }
		}

		public double TwistAngle
		{
			get { return m_TwistAngle; }
			set { m_TwistAngle = value; }
		}

		#endregion

		#region CTable Overrides

		public override void ReadGroupCodes()
		{
			foreach(CGroupCode gc in GroupCodeList)
			{
				if(gc.Code == 2)
				{
					ViewName = gc.Value.Trim();
				}
				else if(gc.Code == 40)
				{
					ViewHeight = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 41)
				{
					ViewWidth = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 10)
				{
					ViewCenterX = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 20)
				{
					ViewCenterY = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 11)
				{
					ViewDirectionX = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 21)
				{
					ViewDirectionY = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 31)
				{
					ViewDirectionZ = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 12)
				{
					ViewTargetX = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 22)
				{
					ViewTargetY = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 32)
				{
					ViewTargetZ = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 42)
				{
					LensLength = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 43)
				{
					FrontClippingPlaneOffset = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 44)
				{
					BackClippingPlaneOffset = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 50)
				{
					TwistAngle = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 70)
				{
					SetStandardFlags(CGlobals.ConvertToInt(gc.Value.Trim()));
				}
			}
		}
		public override void WriteGroupCodes()
		{
			WriteGroupCodeValue(2, ViewName.Trim());
			
			WriteGroupCodeValue(40, ViewHeight.ToString().Trim());
			WriteGroupCodeValue(41, ViewWidth.ToString().Trim());

			WriteGroupCodeValue(10, ViewCenterX.ToString().Trim());
			WriteGroupCodeValue(20, ViewCenterY.ToString().Trim());

			WriteGroupCodeValue(11, ViewDirectionX.ToString().Trim());
			WriteGroupCodeValue(21, ViewDirectionY.ToString().Trim());
			WriteGroupCodeValue(31, ViewDirectionZ.ToString().Trim());

			WriteGroupCodeValue(12, ViewTargetX.ToString().Trim());
			WriteGroupCodeValue(22, ViewTargetY.ToString().Trim());
			WriteGroupCodeValue(32, ViewTargetZ.ToString().Trim());

			WriteGroupCodeValue(42, LensLength.ToString().Trim());

			WriteGroupCodeValue(43, FrontClippingPlaneOffset.ToString().Trim());
			WriteGroupCodeValue(44, BackClippingPlaneOffset.ToString().Trim());

			WriteGroupCodeValue(50, TwistAngle.ToString().Trim());

			WriteGroupCodeValue(70, GetStandardFlags().ToString().Trim());
		}

		#endregion
	}
}

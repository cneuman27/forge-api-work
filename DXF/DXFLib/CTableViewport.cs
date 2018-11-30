using System;
using System.Collections;

namespace DXFLib
{
	public class CTableViewport : CTable
	{
		#region Private Member Data

		private string m_ViewportName = "";

		private double m_LowerLeftX = 0.000;
		private double m_LowerLeftY = 0.000;
		
		private double m_UpperRightX = 0.000;
		private double m_UpperRightY = 0.000;

		private double m_CenterX = 0.000;
		private double m_CenterY = 0.000;

		private double m_SnapBaseX = 0.000;
		private double m_SnapBaseY = 0.000;

		private double m_SnapSpacingX = 0.000;
		private double m_SnapSpacingY = 0.000;

		private double m_GridSpacingX = 0.000;
		private double m_GridSpacingY = 0.000;

		private double m_ViewDirectionX = 0.000;
		private double m_ViewDirectionY = 0.000;
		private double m_ViewDirectionZ = 0.000;
		
		private double m_ViewTargetX = 0.000;
		private double m_ViewTargetY = 0.000;
		private double m_ViewTargetZ = 0.000;

		private double m_ViewHeight = 0.000;		
		private double m_ViewportAspectRatio = 0.000;
		private double m_LensLength = 0.000;

		private double m_FrontClippingPlaneOffset = 0.000;
		private double m_BackClippingPlaneOffset = 0.000;

		private double m_SnapRotationAngle = 0.000;
		private double m_TwistAngle = 0.000;

		#endregion

		#region Constructors

		public CTableViewport() : base()
		{
		}
        
		#endregion

		#region Public Properties

		public string ViewportName
		{
			get { return m_ViewportName; }
			set { m_ViewportName = value; }
		}

		public double LowerLeftX
		{
			get { return m_LowerLeftX; }
			set { m_LowerLeftX = value; }
		}
		public double LowerLeftY
		{
			get { return m_LowerLeftY; }
			set { m_LowerLeftY = value; }
		}

		public double UpperRightX
		{
			get { return m_UpperRightX; }
			set { m_UpperRightX = value; }
		}
		public double UpperRightY
		{
			get { return m_UpperRightY; }
			set { m_UpperRightY = value; }
		}

		public double CenterX
		{
			get { return m_CenterX; }
			set { m_CenterX  = value; } 
		}
		public double CenterY
		{
			get { return m_CenterY; }
			set { m_CenterY = value; }
		}

		public double SnapBaseX
		{
			get { return m_SnapBaseX; }
			set { m_SnapBaseX = value; }
		}
		public double SnapBaseY
		{
			get { return m_SnapBaseY; }
			set { m_SnapBaseY = value; }
		}
		
		public double SnapSpacingX
		{ 
			get { return m_SnapSpacingX; }
			set { m_SnapSpacingX = value; }
		}
		public double SnapSpacingY
		{
			get { return m_SnapSpacingY; }
			set { m_SnapSpacingY = value; }
		}
		
		public double GridSpacingX
		{
			get { return m_GridSpacingX; }
			set { m_GridSpacingX = value; }
		}
		public double GridSpacingY
		{
			get { return m_GridSpacingY; }
			set { m_GridSpacingY = value; }
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

		public double ViewHeight
		{
			get { return m_ViewHeight; }
			set { m_ViewHeight = value; }
		}
		public double ViewportAspectRatio
		{
			get { return m_ViewportAspectRatio; }
			set { m_ViewportAspectRatio = value; }
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

		public double SnapRotationAngle
		{
			get { return m_SnapRotationAngle; }
			set { m_SnapRotationAngle = value; }
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
					ViewportName = gc.Value.Trim();
				}
				else if(gc.Code == 70)
				{
					SetStandardFlags(CGlobals.ConvertToInt(gc.Value.Trim()));
				}
				else if(gc.Code == 10)
				{
					LowerLeftX = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 20)
				{
					LowerLeftY = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 11)
				{
					UpperRightX = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 21)
				{
					UpperRightY = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 12)
				{
					CenterX = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 22)
				{
					CenterY = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 13)
				{
					SnapBaseX = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 23)
				{
					SnapBaseY = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 14)
				{
					SnapSpacingX = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 24)
				{
					SnapSpacingY = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 15)
				{
					GridSpacingX = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 25)
				{
					GridSpacingY = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 16)
				{
					ViewDirectionX = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 26)
				{
					ViewDirectionY = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 36)
				{
					ViewDirectionZ = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 17)
				{
					ViewTargetX = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 27)
				{
					ViewTargetY = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 37)
				{
					ViewTargetZ = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 40)
				{
					ViewHeight = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 41)
				{
					ViewportAspectRatio = CGlobals.ConvertToDouble(gc.Value.Trim());
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
					SnapRotationAngle = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 51)
				{
					TwistAngle = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
			}
		}
		public override void WriteGroupCodes()
		{
			WriteGroupCodeValue(2, ViewportName.Trim());

			WriteGroupCodeValue(10, LowerLeftX.ToString().Trim());
			WriteGroupCodeValue(20, LowerLeftY.ToString().Trim());

			WriteGroupCodeValue(11, UpperRightX.ToString().Trim());
			WriteGroupCodeValue(21, UpperRightY.ToString().Trim());

			WriteGroupCodeValue(12, CenterX.ToString().Trim());
			WriteGroupCodeValue(22, CenterY.ToString().Trim());

			WriteGroupCodeValue(13, SnapBaseX.ToString().Trim());
			WriteGroupCodeValue(23, SnapBaseY.ToString().Trim());

			WriteGroupCodeValue(14, SnapSpacingX.ToString().Trim());
			WriteGroupCodeValue(24, SnapSpacingY.ToString().Trim());

			WriteGroupCodeValue(15, GridSpacingX.ToString().Trim());
			WriteGroupCodeValue(25, GridSpacingY.ToString().Trim());

			WriteGroupCodeValue(16, ViewDirectionX.ToString().Trim());
			WriteGroupCodeValue(26, ViewDirectionY.ToString().Trim());
			WriteGroupCodeValue(36, ViewDirectionZ.ToString().Trim());

			WriteGroupCodeValue(17, ViewTargetX.ToString().Trim());
			WriteGroupCodeValue(27, ViewTargetY.ToString().Trim());
			WriteGroupCodeValue(37, ViewTargetZ.ToString().Trim());

			WriteGroupCodeValue(40, ViewHeight.ToString().Trim());

			WriteGroupCodeValue(41, ViewportAspectRatio.ToString().Trim());

			WriteGroupCodeValue(42, LensLength.ToString().Trim());

			WriteGroupCodeValue(43, FrontClippingPlaneOffset.ToString().Trim());
			WriteGroupCodeValue(44, BackClippingPlaneOffset.ToString().Trim());

			WriteGroupCodeValue(50, SnapRotationAngle.ToString().Trim());

			WriteGroupCodeValue(51, TwistAngle.ToString().Trim());

			WriteGroupCodeValue(70, GetStandardFlags().ToString().Trim());
		}
        
		#endregion
	}
}

using System;
using System.Collections;

namespace DXFLib
{
	public class CEntityShape : CEntity, ICloneable
	{
		#region Private Member Data

		private double m_Size = 0.000;		
		private string m_ShapeName = "";
		private double m_RotationAngle = 0.000;
		private double m_ObliqueAngle = 0.000;
		private double m_XScale = 1.000;

		#endregion

		#region Constructors

		public CEntityShape() : base()
		{
		}
        
		#endregion

		#region Public Properties

		public double Size
		{
			get { return m_Size; }
			set { m_Size = value; }
		}
		public string ShapeName
		{
			get { return m_ShapeName; }
			set { m_ShapeName = value; }
		}
		public double RotationAngle
		{
			get { return m_RotationAngle; }
			set { m_RotationAngle = value; }
		}
		public double ObliqueAngle
		{
			get { return m_ObliqueAngle; }
			set { m_ObliqueAngle = value; }
		}
		public double XScale
		{
			get { return m_XScale; }
			set { m_XScale = value; }
		}

		#endregion

		#region CEntity Overrides

		public override void ReadGroupCodes()
		{
			foreach(CGroupCode gc in GroupCodeList)
			{
				if(gc.Code == 10)
				{
					X0 = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 20)
				{
					Y0 = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 30)
				{
					Z0 = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 40)
				{
					Size = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 2)
				{
					ShapeName = gc.Value;
				}
				else if(gc.Code == 50)
				{
					RotationAngle = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 41)
				{
					XScale = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 51)
				{
					ObliqueAngle = CGlobals.ConvertToDouble(gc.Value);
				}
			}
		}
		public override void WriteGroupCodes()
		{
			WriteGroupCodeValue(10, X0.ToString().Trim());
			WriteGroupCodeValue(20, Y0.ToString().Trim());
			WriteGroupCodeValue(30, Z0.ToString().Trim());

			WriteGroupCodeValue(40, Size.ToString().Trim());
			
			WriteGroupCodeValue(2, ShapeName.Trim());

			WriteGroupCodeValue(50, RotationAngle.ToString().Trim());
			WriteGroupCodeValue(51, ObliqueAngle.ToString().Trim());

			WriteGroupCodeValue(41, XScale.ToString().Trim());
		}
		public override ArrayList GetGroupCodes()
		{
			return GroupCodeList;
		}
		public override void Rotate(double angle)
		{
			double oldX;
			double oldY;

			oldX = X0;
			oldY = Y0;

			X0 = (oldX * Math.Cos(CGlobals.DTR(angle))) - (oldY * Math.Sin(CGlobals.DTR(angle)));
			Y0 = (oldX * Math.Sin(CGlobals.DTR(angle))) + (oldY * Math.Cos(CGlobals.DTR(angle)));
		}

		#endregion
        
		#region ICloneable Members

		public new object Clone()
		{
			CEntityShape entity;

			entity = new CEntityShape();

			CloneEntity(entity);

			entity.Size = Size;

			entity.ShapeName = ShapeName;

			entity.RotationAngle = RotationAngle;
			entity.ObliqueAngle = ObliqueAngle;

			entity.XScale = XScale;

			return entity;
		}
	
		#endregion
	}
}

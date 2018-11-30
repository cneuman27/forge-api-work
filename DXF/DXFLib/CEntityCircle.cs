using System;
using System.Collections;

namespace DXFLib
{
	public class CEntityCircle : CEntity, ICloneable
	{
		#region Private Member Data

		#endregion

		#region Constructors

		public CEntityCircle() : base()
		{
		}

		#endregion

		#region Public Properties

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
					Radius = CGlobals.ConvertToDouble(gc.Value);
				}
			}
		}
		public override void WriteGroupCodes()
		{
			WriteGroupCodeValue(10, X0.ToString().Trim());
			WriteGroupCodeValue(20, Y0.ToString().Trim());
			WriteGroupCodeValue(30, Z0.ToString().Trim());

			WriteGroupCodeValue(40, Radius.ToString().Trim());
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
			CEntityCircle entity;

			entity = new CEntityCircle();

			CloneEntity(entity);

			return entity;
		}

		#endregion
	}
}

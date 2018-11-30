using System;
using System.Collections;

namespace DXFLib
{
	public class CEntitySolid : CEntity, ICloneable
	{
		#region Constructors

		public CEntitySolid() : base()
		{
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
				else if(gc.Code == 11)
				{
					X1 = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 21)
				{
					Y1 = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 31)
				{
					Z1 = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 12)
				{
					X2 = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 22)
				{
					Y2 = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 32)
				{
					Z2 = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 13)
				{
					X3 = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 23)
				{
					Y3 = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 33)
				{
					Z3 = CGlobals.ConvertToDouble(gc.Value);
				}
			}
		}
		public override void WriteGroupCodes()
		{
			WriteGroupCodeValue(10, X0.ToString().Trim());
			WriteGroupCodeValue(20, Y0.ToString().Trim());
			WriteGroupCodeValue(30, Z0.ToString().Trim());

			WriteGroupCodeValue(11, X1.ToString().Trim());
			WriteGroupCodeValue(21, Y1.ToString().Trim());
			WriteGroupCodeValue(31, Z1.ToString().Trim());
		
			WriteGroupCodeValue(12, X2.ToString().Trim());
			WriteGroupCodeValue(22, Y2.ToString().Trim());
			WriteGroupCodeValue(32, Z2.ToString().Trim());
		
			WriteGroupCodeValue(13, X3.ToString().Trim());
			WriteGroupCodeValue(23, Y3.ToString().Trim());
			WriteGroupCodeValue(33, Z3.ToString().Trim());
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

			oldX = X1;
			oldY = Y1;

			X1 = (oldX * Math.Cos(CGlobals.DTR(angle))) - (oldY * Math.Sin(CGlobals.DTR(angle)));
			Y1 = (oldX * Math.Sin(CGlobals.DTR(angle))) + (oldY * Math.Cos(CGlobals.DTR(angle)));
		
			oldX = X2;
			oldY = Y2;

			X2 = (oldX * Math.Cos(CGlobals.DTR(angle))) - (oldY * Math.Sin(CGlobals.DTR(angle)));
			Y2 = (oldX * Math.Sin(CGlobals.DTR(angle))) + (oldY * Math.Cos(CGlobals.DTR(angle)));
		
			oldX = X3;
			oldY = Y3;

			X3 = (oldX * Math.Cos(CGlobals.DTR(angle))) - (oldY * Math.Sin(CGlobals.DTR(angle)));
			Y3 = (oldX * Math.Sin(CGlobals.DTR(angle))) + (oldY * Math.Cos(CGlobals.DTR(angle)));
		}

		#endregion

		#region ICloneable Members

		public new object Clone()
		{
			CEntitySolid entity;

			entity = new CEntitySolid();

			CloneEntity(entity);
			
			return entity;
		}

		#endregion
	}
}

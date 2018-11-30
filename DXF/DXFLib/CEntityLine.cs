using System;
using System.Collections;

namespace DXFLib
{	
	public class CEntityLine : CEntity , ICloneable
	{
        public static int PRECISION = 3;

		#region Constructors

		public CEntityLine() : base()
		{
		}

		#endregion

		#region Public Properties

        public double Length
        {
            get
            {
                return Math.Sqrt( (((X1 - X0) * (X1 - X0)) + ((Y1 - Y0) * (Y1 - Y0))));
            }
        }
		
        #endregion

		#region CEntity Overrides

		public override void ReadGroupCodes()
		{
			foreach(CGroupCode gc in GroupCodeList)
			{
				if(gc.Code == 10)
				{
					X0 = Math.Round(CGlobals.ConvertToDouble(gc.Value.Trim()), PRECISION);
				}
				else if(gc.Code == 20)
				{
					Y0 = Math.Round(CGlobals.ConvertToDouble(gc.Value.Trim()), PRECISION);
				}
				else if(gc.Code == 30)
				{
					Z0 = Math.Round(CGlobals.ConvertToDouble(gc.Value.Trim()), PRECISION);
				}
				else if(gc.Code == 11)
				{
					X1 = Math.Round(CGlobals.ConvertToDouble(gc.Value.Trim()), PRECISION);
				}
				else if(gc.Code == 21)
				{
					Y1 = Math.Round(CGlobals.ConvertToDouble(gc.Value.Trim()), PRECISION);
				}
				else if(gc.Code == 31)
				{
					Z1 = Math.Round(CGlobals.ConvertToDouble(gc.Value.Trim()), PRECISION);
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

			X0 = Math.Round((oldX * Math.Cos(CGlobals.DTR(angle))) - (oldY * Math.Sin(CGlobals.DTR(angle))), PRECISION);
			Y0 = Math.Round((oldX * Math.Sin(CGlobals.DTR(angle))) + (oldY * Math.Cos(CGlobals.DTR(angle))), PRECISION);

			oldX = X1;
			oldY = Y1;

			X1 = Math.Round((oldX * Math.Cos(CGlobals.DTR(angle))) - (oldY * Math.Sin(CGlobals.DTR(angle))), PRECISION);
			Y1 = Math.Round((oldX * Math.Sin(CGlobals.DTR(angle))) + (oldY * Math.Cos(CGlobals.DTR(angle))), PRECISION);
		}

		#endregion

		#region ICloneable Members

		public new object Clone()
		{
			CEntityLine entity;

			entity = new CEntityLine();

			CloneEntity(entity);

			return entity;
		}

		#endregion
	}
}

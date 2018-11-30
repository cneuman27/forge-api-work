using System;
using System.Collections;

namespace DXFLib
{
	public class CEntityArc : CEntity, ICloneable 
	{
		#region Private Member Data

        private double m_ArcStartPointX = 0.000;
        private double m_ArcStartPointY = 0.000;
        
        private double m_ArcEndPointX = 0.000;
        private double m_ArcEndPointY = 0.000;

		#endregion

		#region Constructors

		public CEntityArc() : base()
		{
		}

		#endregion

		#region Public Properties

        public double ArcStartPointX
        {
            get { return m_ArcStartPointX; }
        }
        public double ArcStartPointY
        {
            get { return m_ArcStartPointY; }
        }

        public double ArcEndPointX
        {
            get { return m_ArcEndPointX; }
        }
        public double ArcEndPointY
        {
            get { return m_ArcEndPointY; }
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
					Radius = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 50)
				{
                    // In Degrees?

					StartAngle = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 51)
				{
                    // In Degrees?

					EndAngle = CGlobals.ConvertToDouble(gc.Value);
				}
			}

            m_ArcStartPointX = X0 + (Radius * Math.Sin(CGlobals.DTR(StartAngle)));
            m_ArcStartPointY = Y0 + (Radius * Math.Cos(CGlobals.DTR(StartAngle)));

            m_ArcEndPointX = X0 + (Radius * Math.Sin(CGlobals.DTR(EndAngle)));
            m_ArcEndPointY = Y0 + (Radius * Math.Cos(CGlobals.DTR(EndAngle)));
		}
		public override void WriteGroupCodes()
		{
			WriteGroupCodeValue(10, X0.ToString().Trim());
			WriteGroupCodeValue(20, Y0.ToString().Trim());
			WriteGroupCodeValue(30, Z0.ToString().Trim());

			WriteGroupCodeValue(40, Radius.ToString().Trim());
			WriteGroupCodeValue(50, StartAngle.ToString().Trim());
			WriteGroupCodeValue(51, EndAngle.ToString().Trim());
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

			StartAngle += angle;
			EndAngle += angle;

            StartAngle = Math.Round(StartAngle, 2);
            EndAngle = Math.Round(EndAngle, 2);
		}

		#endregion

		#region ICloneable Members

		public new object Clone()
		{
			CEntityArc entity;

			entity = new CEntityArc();

			CloneEntity(entity);

			return entity;
		}
        
		#endregion
	}
}

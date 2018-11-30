using System;
using System.Collections;

namespace DXFLib
{
	public class CEntity3DFace : CEntity, ICloneable
	{
		#region Private Member Data

		private bool m_Edge1Invisible = false;
		private bool m_Edge2Invisible = false;
		private bool m_Edge3Invisible = false;
		private bool m_Edge4Invisible = false;

		#endregion

		#region Constructors

		public CEntity3DFace() : base()
		{
		}

		#endregion

		#region Public Properties

		public bool Edge1Invisible
		{
			get { return m_Edge1Invisible; }
			set { m_Edge1Invisible = value; } 
		}
		public bool Edge2Invisible
		{
			get { return m_Edge2Invisible; }
			set { m_Edge2Invisible = value; }
		}
		public bool Edge3Invisible
		{
			get { return m_Edge3Invisible; }
			set { m_Edge3Invisible = value; }
		}
		public bool Edge4Invisible
		{
			get { return m_Edge4Invisible; }
			set { m_Edge4Invisible = value; }
		}
        
		#endregion

		#region CEntity Overrides

		public override void ReadGroupCodes()
		{
			BitArray flags;
			BitArray comparer;

			foreach(CGroupCode gc in GroupCodeList)
			{
				if(gc.Code == 10)
				{
					X0 = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 20)
				{
					Y0 = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 30)
				{
					Z0 = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 11)
				{
					X1 = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 21)
				{
					Y1 = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 31)
				{
					Z1 = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 12)
				{
					X2 = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 22)
				{
					Y2 = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 32)
				{
					Z2 = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 13)
				{
					X3 = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 23)
				{
					Y3 = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 33)
				{
					Z3 = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 70)
				{
					flags = new BitArray(new int[] { CGlobals.ConvertToInt(gc.Value.Trim()) });
			
					comparer = new BitArray(new int[] { 1 });
					Edge1Invisible = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 2 });
					Edge2Invisible = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 4 });
					Edge3Invisible = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 8 });
					Edge4Invisible = flags.And(comparer)[0];
				}
			}
		}
		public override void WriteGroupCodes()
		{
			int flags;

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

			flags = 0;

			if(Edge1Invisible) flags += 1;
			if(Edge2Invisible) flags += 2;
			if(Edge3Invisible) flags += 4;
			if(Edge4Invisible) flags += 8;

			WriteGroupCodeValue(70, flags.ToString().Trim());
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
			CEntity3DFace entity;

			entity = new CEntity3DFace();

			CloneEntity(entity);

			entity.Edge1Invisible = Edge1Invisible;
			entity.Edge2Invisible = Edge2Invisible;
			entity.Edge3Invisible = Edge3Invisible;
			entity.Edge4Invisible = Edge4Invisible;

			return entity;
		}

		#endregion
	}
}

using System;
using System.Collections;

namespace DXFLib
{
	public class CTableTextStyle : CTable
	{
		#region Private Member Data

		private string m_StyleName = "";
		
		private double m_FixedHeight = 0;
		private double m_WidthFactor = 0;
		private double m_ObliqueAngle = 0;
		private double m_LastHeightUsed = 0;

		private string m_PrimaryFontFileName = "";
		private string m_BigFontFileName = "";

		private bool m_Backwards = false;
		private bool m_UpsideDown = false;
		private bool m_IsVerticallyOriented = false;

		#endregion

		#region Constructors

		public CTableTextStyle() : base()
		{
		}
        
		#endregion

		#region Public Properties

		public string StyleName
		{
			get { return m_StyleName; }
			set { m_StyleName = value; }
		}

		public double FixedHeight
		{
			get { return m_FixedHeight; }
			set { m_FixedHeight = value; }
		}
		public double WidthFactor
		{
			get { return m_WidthFactor; }
			set { m_WidthFactor = value; }
		}
		public double ObliqueAngle
		{
			get { return m_ObliqueAngle; }
			set { m_ObliqueAngle = value; }
		}
		public double LastHeightUsed
		{
			get { return m_LastHeightUsed; }
			set { m_LastHeightUsed = value; }
		}
		
		public string PrimaryFontFileName
		{
			get { return m_PrimaryFontFileName; }
			set { m_PrimaryFontFileName = value; }
		}
		public string BigFontFileName
		{
			get { return m_BigFontFileName; }
			set { m_BigFontFileName = value; }
		}

		public bool Backwards
		{
			get { return m_Backwards; }
			set { m_Backwards = value; }
		}
		public bool UpsideDown
		{
			get { return m_UpsideDown; }
			set { m_UpsideDown = value; }
		}
		public bool IsVerticallyOriented
		{
			get { return m_IsVerticallyOriented; }
			set { m_IsVerticallyOriented = value; }
		}

		#endregion

		#region CTable Overrides

		public override void ReadGroupCodes()
		{
			BitArray flags;
			BitArray comparer;

			foreach(CGroupCode gc in GroupCodeList)
			{
				if(gc.Code == 2)
				{
					StyleName = gc.Value.Trim();
				}
				else if(gc.Code == 40)
				{
					FixedHeight = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 41)
				{
					WidthFactor = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 50)
				{
					ObliqueAngle = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 71)
				{
					flags = new BitArray(new int[] { CGlobals.ConvertToInt(gc.Value.Trim()) });
					
					comparer = new BitArray(new int[] { 2 });
					Backwards = flags.And(comparer)[0];
	
					comparer = new BitArray(new int[] { 4 });
					UpsideDown = flags.And(comparer)[0];
				}
				else if(gc.Code == 42)
				{
					LastHeightUsed = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 3)
				{
					PrimaryFontFileName = gc.Value.Trim();
				}
				else if(gc.Code == 4)
				{
					BigFontFileName = gc.Value.Trim();
				}
				else if(gc.Code == 70)
				{
					SetStandardFlags(CGlobals.ConvertToInt(gc.Value.Trim()));
				
					flags = new BitArray(new int [] { CGlobals.ConvertToInt(gc.Value.Trim()) });
					
					comparer = new BitArray(new int[] { 4 });
					IsVerticallyOriented = flags.And(comparer)[0];
				}
			}
		}
		public override void WriteGroupCodes()
		{
			int flags;

			WriteGroupCodeValue(2, StyleName.Trim());
			
			WriteGroupCodeValue(40, FixedHeight.ToString().Trim());
			WriteGroupCodeValue(41, WidthFactor.ToString().Trim());
			WriteGroupCodeValue(50, ObliqueAngle.ToString().Trim());
			WriteGroupCodeValue(42, LastHeightUsed.ToString().Trim());

			WriteGroupCodeValue(3, PrimaryFontFileName.Trim());
			WriteGroupCodeValue(4, BigFontFileName.Trim());

			flags = GetStandardFlags();

			if(IsVerticallyOriented) flags += 4;

			WriteGroupCodeValue(70, flags.ToString().Trim());

			flags = 0;

			if(Backwards) flags += 2;
			if(UpsideDown) flags += 4;

			WriteGroupCodeValue(71, flags.ToString().Trim());
		}
        
		#endregion
	}
}

using System;
using System.Collections;

namespace DXFLib
{
	public class CTableDimensionStyle : CTable
	{
		#region Private Member Data

		private string m_DimensionStyleName = "";

		private string m_DIMPOST = "";
		private string m_DIMAPOST = "";
		private string m_DIMBLK = "";
		private string m_DIMBLK1 = "";
		private string m_DIMBLK2 = "";
		private double m_DIMSCALE = 0.000;
		private double m_DIMASZ = 0.000;
		private double m_DIMEXO = 0.000;
		private double m_DIMDLI = 0.000;
		private double m_DIMEXE = 0.000;
		private double m_DIMRND = 0.000;
		private double m_DIMDLE = 0.000;
		private double m_DIMTP = 0.000;
		private double m_DIMTM = 0.000;
		private double m_DIMTXT = 0.000;
		private double m_DIMCEN = 0.000;
		private double m_DIMTSZ = 0.000;
		private double m_DIMALTF = 0.000;
		private double m_DIMLFAC = 0.000;
		private double m_DIMTVP = 0.000;
		private double m_DIMTFAC = 0.000;
		private double m_DIMGAP = 0.000;
		private double m_DIMTOL = 0.000;
		private double m_DIMLIM = 0.000;
		private double m_DIMTIH = 0.000;
		private double m_DIMTOH = 0.000;
		private double m_DIMSE1 = 0.000;
		private double m_DIMSE2 = 0.000;
		private double m_DIMTAD = 0.000;
		private double m_DIMZIN = 0.000;
		private double m_DIMALT = 0.000;
		private double m_DIMALTD = 0.000;
		private double m_DIMTOFL = 0.000;
		private double m_DIMSAH = 0.000;
		private double m_DIMTIX = 0.000;
		private double m_DIMSOXD = 0.000;
		private double m_DIMCLRD = 0.000;
		private double m_DIMCLRE = 0.000;
		private double m_DIMCLRT  = 0.000;

		#endregion

		#region Constructors

		public CTableDimensionStyle() : base()
		{
		}
        
		#endregion

		#region Public Properties

		public string DimensionStyleName
		{
			get { return m_DimensionStyleName; }
			set { m_DimensionStyleName = value; }
		}

		public string DIMPOST
		{
			get { return m_DIMPOST; }
			set { m_DIMPOST = value; }
		}
		public string DIMAPOST
		{
			get { return m_DIMAPOST; }
			set { m_DIMAPOST = value; }
		}
		public string DIMBLK
		{
			get { return m_DIMBLK; }
			set { m_DIMBLK = value; }
		}
		public string DIMBLK1
		{
			get { return m_DIMBLK1; }
			set { m_DIMBLK1 = value; }
		}
		public string DIMBLK2
		{
			get { return m_DIMBLK2; }
			set { m_DIMBLK2 = value; }
		}
		public double DIMSCALE
		{
			get { return m_DIMSCALE; }
			set { m_DIMSCALE = value; }
		}
		public double DIMASZ
		{
			get { return m_DIMASZ; }
			set { m_DIMASZ = value; }
		}
		public double DIMEXO
		{
			get { return m_DIMEXO; }
			set { m_DIMEXO = value; }
		}
		public double DIMDLI
		{
			get { return m_DIMDLI; }
			set { m_DIMDLI = value; }
		}
		public double DIMEXE
		{
			get { return m_DIMEXE; }
			set { m_DIMEXE = value; }
		}
		public double DIMRND
		{
			get { return m_DIMRND; }
			set { m_DIMRND = value; }
		}
		public double DIMDLE
		{
			get { return m_DIMDLE; }
			set { m_DIMDLE = value; }
		}
		public double DIMTP
		{
			get { return m_DIMTP; }
			set { m_DIMTP = value; }
		}
		public double DIMTM
		{
			get { return m_DIMTM; }
			set { m_DIMTM = value; }
		}
		public double DIMTXT
		{
			get { return m_DIMTXT; }
			set { m_DIMTXT = value; }
		}
		public double DIMCEN
		{
			get { return m_DIMCEN; }
			set { m_DIMCEN = value; }
		}
		public double DIMTSZ
		{
			get { return m_DIMTSZ; }
			set { m_DIMTSZ = value; }
		}
		public double DIMALTF
		{
			get { return m_DIMALTF; }
			set { m_DIMALTF = value; }
		}
		public double DIMLFAC
		{
			get { return m_DIMLFAC; }
			set { m_DIMLFAC = value; }
		}
		public double DIMTVP
		{
			get { return m_DIMTVP; }
			set { m_DIMTVP = value; }
		}
		public double DIMTFAC
		{
			get { return m_DIMTFAC; }
			set { m_DIMTFAC = value; }
		}
		public double DIMGAP
		{
			get { return m_DIMGAP; }
			set { m_DIMGAP = value; }
		}
		public double DIMTOL
		{
			get { return m_DIMTOL; }
			set { m_DIMTOL = value; }
		}
		public double DIMLIM
		{
			get { return m_DIMLIM; }
			set { m_DIMLIM = value; }
		}
		public double DIMTIH
		{
			get { return m_DIMTIH; }
			set { m_DIMTIH = value; }
		}
		public double DIMTOH
		{
			get { return m_DIMTOH; }
			set { m_DIMTOH = value; }
		}
		public double DIMSE1
		{
			get { return m_DIMSE1; }
			set { m_DIMSE1 = value; }
		}
		public double DIMSE2
		{
			get { return m_DIMSE2; }
			set { m_DIMSE2 = value; }
		}
		public double DIMTAD
		{
			get { return m_DIMTAD; }
			set { m_DIMTAD = value; }
		}
		public double DIMZIN
		{
			get { return m_DIMZIN; }
			set { m_DIMZIN = value; }
		}
		public double DIMALT
		{
			get { return m_DIMALT; }
			set { m_DIMALT = value; }
		}
		public double DIMALTD
		{
			get { return m_DIMALTD; }
			set { m_DIMALTD = value; }
		}
		public double DIMTOFL
		{
			get { return m_DIMTOFL; }
			set { m_DIMTOFL = value; }
		}
		public double DIMSAH
		{
			get { return m_DIMSAH; }
			set { m_DIMSAH = value; }
		}
		public double DIMTIX
		{
			get { return m_DIMTIX; }
			set { m_DIMTIX = value; }
		}
		public double DIMSOXD
		{
			get { return m_DIMSOXD; }
			set { m_DIMSOXD = value; }
		}
		public double DIMCLRD
		{
			get { return m_DIMCLRD; }
			set { m_DIMCLRD = value; }
		}
		public double DIMCLRE
		{
			get { return m_DIMCLRE; }
			set { m_DIMCLRE = value; }
		}
		public double DIMCLRT
		{
			get { return m_DIMCLRT; }
			set { m_DIMCLRT = value; }
		}

		#endregion

		#region CTable Overrides

		public override void ReadGroupCodes()
		{
			foreach(CGroupCode gc in GroupCodeList)
			{
				if(gc.Code == 2)
				{
					DimensionStyleName = gc.Value.Trim();
				}
				else if(gc.Code == 3)
				{
					DIMPOST = gc.Value.Trim();
				}
				else if(gc.Code == 4)
				{
					DIMAPOST = gc.Value.Trim();
				}
				else if(gc.Code == 5)
				{
					DIMBLK = gc.Value.Trim();
				}
				else if(gc.Code == 6)
				{
					DIMBLK1 = gc.Value.Trim();
				}
				else if(gc.Code == 7)
				{
					DIMBLK2 = gc.Value.Trim();
				}
				else if(gc.Code == 40)
				{
					DIMSCALE = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 41)
				{
					DIMASZ = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 42)
				{
					DIMEXO = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 43)
				{
					DIMDLI = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 44)
				{
					DIMEXE = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 45)
				{
					DIMRND = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 46)
				{
					DIMDLE = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 47)
				{
					DIMTP = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 48)
				{
					DIMTM = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 140)
				{
					DIMTXT = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 141)
				{
					DIMCEN = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 142)
				{
					DIMTSZ = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 143)
				{
					DIMALTF = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 144)
				{
					DIMLFAC = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 145)
				{
					DIMTVP = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 146)
				{
					DIMTFAC = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 147)
				{
					DIMGAP = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 71)
				{
					DIMTOL = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 72)
				{
					DIMLIM = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 73)
				{
					DIMTIH = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 74)
				{
					DIMTOH = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 75)
				{
					DIMSE1 = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 76)
				{
					DIMSE2 = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 77)
				{
					DIMTAD = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 78)
				{
					DIMZIN = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 170)
				{
					DIMALT = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 171)
				{
					DIMALTD = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 172)
				{
					DIMTOFL = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 173)
				{
					DIMSAH = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 174)
				{
					DIMTIX = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 175)
				{
					DIMSOXD = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 176)
				{
					DIMCLRD = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 177)
				{
					DIMCLRE = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 178)
				{
					DIMCLRT = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 70)
				{
					SetStandardFlags(CGlobals.ConvertToInt(gc.Value.Trim()));
				}
			}
		}
		public override void WriteGroupCodes()
		{
			WriteGroupCodeValue(2, DimensionStyleName.Trim());
			
			WriteGroupCodeValue(3, DIMPOST.Trim());
			WriteGroupCodeValue(4, DIMAPOST.Trim());
			WriteGroupCodeValue(5, DIMBLK.Trim());
			WriteGroupCodeValue(6, DIMBLK1.Trim());
			WriteGroupCodeValue(7, DIMBLK2.Trim());

			WriteGroupCodeValue(40, DIMSCALE.ToString().Trim());
			WriteGroupCodeValue(41, DIMASZ.ToString().Trim());
			WriteGroupCodeValue(42, DIMEXO.ToString().Trim());
			WriteGroupCodeValue(43, DIMDLI.ToString().Trim());
			WriteGroupCodeValue(44, DIMEXE.ToString().Trim());
			WriteGroupCodeValue(45, DIMRND.ToString().Trim());
			WriteGroupCodeValue(46, DIMDLE.ToString().Trim());
			WriteGroupCodeValue(47, DIMTP.ToString().Trim());
			WriteGroupCodeValue(48, DIMTM.ToString().Trim());
			WriteGroupCodeValue(140, DIMTXT.ToString().Trim());
			WriteGroupCodeValue(141, DIMCEN.ToString().Trim());
			WriteGroupCodeValue(142, DIMTSZ.ToString().Trim());
			WriteGroupCodeValue(143, DIMALTF.ToString().Trim());
			WriteGroupCodeValue(144, DIMLFAC.ToString().Trim());
			WriteGroupCodeValue(145, DIMTVP.ToString().Trim());
			WriteGroupCodeValue(146, DIMTFAC.ToString().Trim());
			WriteGroupCodeValue(147, DIMGAP.ToString().Trim());
			WriteGroupCodeValue(71, DIMTOL.ToString().Trim());
			WriteGroupCodeValue(72, DIMLIM.ToString().Trim());
			WriteGroupCodeValue(73, DIMTIH.ToString().Trim());
			WriteGroupCodeValue(74, DIMTOH.ToString().Trim());
			WriteGroupCodeValue(75, DIMSE1.ToString().Trim());
			WriteGroupCodeValue(76, DIMSE2.ToString().Trim());
			WriteGroupCodeValue(77, DIMTAD.ToString().Trim());
			WriteGroupCodeValue(78, DIMZIN.ToString().Trim());
			WriteGroupCodeValue(170, DIMALT.ToString().Trim());
			WriteGroupCodeValue(171, DIMALTD.ToString().Trim());
			WriteGroupCodeValue(172, DIMTOFL.ToString().Trim());
			WriteGroupCodeValue(173, DIMSAH.ToString().Trim());
			WriteGroupCodeValue(174, DIMTIX.ToString().Trim());
			WriteGroupCodeValue(175, DIMSOXD.ToString().Trim());
			WriteGroupCodeValue(176, DIMCLRD.ToString().Trim());
			WriteGroupCodeValue(177, DIMCLRE.ToString().Trim());
			WriteGroupCodeValue(178, DIMCLRT.ToString().Trim());

			WriteGroupCodeValue(70, GetStandardFlags().ToString().Trim());
		}
        
		#endregion
	}
}

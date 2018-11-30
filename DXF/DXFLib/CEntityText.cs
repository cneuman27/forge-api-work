using System;
using System.Collections;

namespace DXFLib
{
	public class CEntityText : CEntity, ICloneable
	{
		#region Private Members

		private double m_AlignmentX = 0.000;
		private double m_AlignmentY = 0.000;
		private double m_AlignmentZ = 0.000;

		private double m_Height = 0.000;
		
		private string m_Text = "";

		private double m_RotationAngle = 0.000;
		private double m_ObliqueAngle = 0.000;

		private double m_XScale = 1.000;

		private string m_TextStyle = "STANDARD";

		private bool m_Backwards = false;
		private bool m_UpsideDown = false;

		private CGlobals.TextVerticalAlignment m_VerticalAlignment = CGlobals.TextVerticalAlignment.Baseline;
		private CGlobals.TextHorizontalAlignment m_HorizontalAlignment = CGlobals.TextHorizontalAlignment.Left;

		#endregion

		#region Constructors

		public CEntityText() : base()
		{
		}
        
		#endregion

		#region Public Properties
		
		public double AlignmentX
		{
			get { return m_AlignmentX; }
			set { m_AlignmentX = value; }
		}
		public double AlignmentY 
		{
			get { return m_AlignmentY; }
			set { m_AlignmentY = value; }
		}
		public double AlignmentZ
		{
			get { return m_AlignmentZ; }
			set { m_AlignmentZ = value; }
		}

		public double Height
		{
			get { return m_Height; }
			set { m_Height = value; }
		}
        
		public string Text
		{
			get { return m_Text; }
			set { m_Text = value; }
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

		public string TextStyle
		{
			get { return m_TextStyle; }
			set { m_TextStyle = value; }
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

		public CGlobals.TextVerticalAlignment VerticalAlignment
		{
			get { return m_VerticalAlignment; }
			set { m_VerticalAlignment = value; }
		}
		public CGlobals.TextHorizontalAlignment HorizontalAlignment
		{
			get { return m_HorizontalAlignment; }
			set { m_HorizontalAlignment = value; }
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
					Height = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 1)
				{
					Text = gc.Value;
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
				else if(gc.Code == 7)
				{
					TextStyle = gc.Value.Trim();
				}
				else if(gc.Code == 71)
				{
					if(CGlobals.ConvertToDouble(gc.Value) == 2)
					{
						Backwards = true;
						UpsideDown = false;
					}
					else if(CGlobals.ConvertToDouble(gc.Value) == 4)
					{
						Backwards = false;
						UpsideDown = true;
					}
					else if(CGlobals.ConvertToDouble(gc.Value) == 6)
					{
						Backwards = true;
						UpsideDown = true;
					}
					else
					{
						Backwards = false;
						UpsideDown = false;
					}
				}
				else if(gc.Code == 72)
				{
					if(CGlobals.ConvertToDouble(gc.Value) == 0)
					{
						HorizontalAlignment = CGlobals.TextHorizontalAlignment.Left;
					}
					else if(CGlobals.ConvertToDouble(gc.Value) == 1)
					{
						HorizontalAlignment = CGlobals.TextHorizontalAlignment.Center;
					}
					else if(CGlobals.ConvertToDouble(gc.Value) == 2)
					{
						HorizontalAlignment = CGlobals.TextHorizontalAlignment.Right;
					}
					else if(CGlobals.ConvertToDouble(gc.Value) == 3)
					{
						HorizontalAlignment = CGlobals.TextHorizontalAlignment.Aligned;
					}
					else if(CGlobals.ConvertToDouble(gc.Value) == 4)
					{
						HorizontalAlignment = CGlobals.TextHorizontalAlignment.Middle;
					}
					else if(CGlobals.ConvertToDouble(gc.Value) == 5)
					{
						HorizontalAlignment = CGlobals.TextHorizontalAlignment.Fit;
					}
				}
				else if(gc.Code == 73)
				{
					if(CGlobals.ConvertToDouble(gc.Value) == 0)
					{
						VerticalAlignment = CGlobals.TextVerticalAlignment.Baseline;
					}
					else if(CGlobals.ConvertToDouble(gc.Value) == 1)
					{
						VerticalAlignment = CGlobals.TextVerticalAlignment.Bottom;
					}
					else if(CGlobals.ConvertToDouble(gc.Value) == 2)
					{
						VerticalAlignment = CGlobals.TextVerticalAlignment.Middle;
					}
					else if(CGlobals.ConvertToDouble(gc.Value) == 3)
					{
						VerticalAlignment = CGlobals.TextVerticalAlignment.Top;
					}
				}
				else if(gc.Code == 11)
				{
					AlignmentX = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 21)
				{
					AlignmentY = CGlobals.ConvertToDouble(gc.Value);
				}
				else if(gc.Code == 31)
				{
					AlignmentZ = CGlobals.ConvertToDouble(gc.Value);
				}
			}
		}
		public override void WriteGroupCodes()
		{
			int flags;

			WriteGroupCodeValue(10, X0.ToString().Trim());
			WriteGroupCodeValue(20, Y0.ToString().Trim());
			WriteGroupCodeValue(30, Z0.ToString().Trim());

			WriteGroupCodeValue(11, AlignmentX.ToString().Trim());
			WriteGroupCodeValue(21, AlignmentY.ToString().Trim());
			WriteGroupCodeValue(31, AlignmentZ.ToString().Trim());

			WriteGroupCodeValue(40, Height.ToString().Trim());

			WriteGroupCodeValue(1, Text.Trim());

			WriteGroupCodeValue(50, RotationAngle.ToString().Trim());
			WriteGroupCodeValue(51, ObliqueAngle.ToString().Trim());

			WriteGroupCodeValue(41, XScale.ToString().Trim());

			WriteGroupCodeValue(7, TextStyle.Trim());

			flags = 0;

			if(Backwards) flags += 2;
			if(UpsideDown) flags += 4;

			WriteGroupCodeValue(71, flags.ToString().Trim());

			if(HorizontalAlignment == CGlobals.TextHorizontalAlignment.Left)
			{
				WriteGroupCodeValue(72, "0");
			}
			else if(HorizontalAlignment == CGlobals.TextHorizontalAlignment.Center) 
			{
				WriteGroupCodeValue(72, "1");
			}
			else if(HorizontalAlignment == CGlobals.TextHorizontalAlignment.Right)
			{
				WriteGroupCodeValue(72, "2");
			}
			else if(HorizontalAlignment == CGlobals.TextHorizontalAlignment.Aligned)
			{
				WriteGroupCodeValue(72, "3");
			}
			else if(HorizontalAlignment == CGlobals.TextHorizontalAlignment.Middle)
			{
				WriteGroupCodeValue(72, "4");
			}
			else if(HorizontalAlignment == CGlobals.TextHorizontalAlignment.Fit)
			{
				WriteGroupCodeValue(72, "5");
			}
			else
			{
				WriteGroupCodeValue(72, "0");
			}

			if(VerticalAlignment == CGlobals.TextVerticalAlignment.Baseline)
			{
				WriteGroupCodeValue(73, "0");
			}
			else if(VerticalAlignment == CGlobals.TextVerticalAlignment.Bottom)
			{
				WriteGroupCodeValue(73, "1");
			}
			else if(VerticalAlignment == CGlobals.TextVerticalAlignment.Middle)
			{
				WriteGroupCodeValue(73, "2");
			}
			else if(VerticalAlignment == CGlobals.TextVerticalAlignment.Top)
			{
				WriteGroupCodeValue(73, "4");
			}
			else
			{
				WriteGroupCodeValue(73, "0");
			}
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
			CEntityText entity;

			entity = new CEntityText();

			CloneEntity(entity);
		
			entity.AlignmentX = AlignmentX;
			entity.AlignmentY = AlignmentY;
			entity.AlignmentZ = AlignmentZ;

			entity.Height = Height;
			
			entity.Text = Text;

			entity.RotationAngle = RotationAngle;
			entity.ObliqueAngle = ObliqueAngle;

			entity.XScale = XScale;

			entity.TextStyle = TextStyle;

			entity.Backwards = Backwards;
			entity.UpsideDown = UpsideDown;

			entity.VerticalAlignment = VerticalAlignment;
			entity.HorizontalAlignment = HorizontalAlignment;

			return entity;
		}

		#endregion
	}
}

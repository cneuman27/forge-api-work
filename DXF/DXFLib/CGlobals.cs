using System;

namespace DXFLib
{
	public class CGlobals
	{
		public CGlobals()
		{
		}

		#region Safer Type Converters

		public static int ConvertToInt(string str)
		{
			try
			{
				return Convert.ToInt32(str);
			}
			catch(Exception)
			{
				return 0;
			}
		}
		public static double ConvertToDouble(string str)
		{
			try
			{
				return Convert.ToDouble(str);
			}
			catch(Exception)
			{
				return 0.000;
			}
		}


		#endregion

		#region Hex Conversions

		public static string IntToHex(int val)
		{
			int remainder;
			char[] hexList = new char[]
			{
				'0',
				'1',
				'2',
				'3',
				'4',
				'5',
				'6',
				'7',
				'8',
				'9',
				'A',
				'B',
				'C',
				'D',
				'E',
				'F'
			};

			string res  = "";

			while(val > 0)
			{
				remainder = val % 16;

				res = hexList[remainder] + res;

				val = val / 16;
			}

			
			return res;
		}


		#endregion

		public enum TextVerticalAlignment
		{
			Undefined,
			Top,
			Middle,
			Bottom,
			Baseline
		}

		public enum TextHorizontalAlignment
		{
			Undefined,
			Left,
			Center,
			Right,
			Aligned,
			Middle,
			Fit
		}

		public static double RTD(double radians)
		{
			return 180 * (radians / Math.PI);
		}
		
		public static double DTR(double degrees)
		{
			return Math.PI * (degrees / 180);
		}
	}
}

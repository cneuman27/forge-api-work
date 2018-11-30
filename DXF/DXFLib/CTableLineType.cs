using System;
using System.Collections;

namespace DXFLib
{
	public class CTableLineType : CTable
	{
		#region Private Member Data

		private string m_LineTypeName = "";
		private string m_Description = "";

		private double m_AlignmentCode = 65;
		private double m_DashLengthItemCount = 0;
		private double m_TotalPatternLength = 0;

		private ArrayList m_DashLengthList = new ArrayList();

		#endregion

		#region Constructors

		public CTableLineType() : base()
		{
		}
        
		#endregion

		#region Public Properties

		public string LineTypeName
		{
			get { return m_LineTypeName; }
			set { m_LineTypeName = value; }
		}
		public string Description
		{
			get { return m_Description; }
			set { m_Description = value; }
		}

		public double AlignmentCode
		{
			get { return m_AlignmentCode; }
			set { m_AlignmentCode = value; }
		}
		public double DashLengthItemCount
		{
			get { return m_DashLengthItemCount; }
			set { m_DashLengthItemCount = value; }
		}
		public double TotalPatternLength
		{
			get { return m_TotalPatternLength; }
			set { m_TotalPatternLength = value; }
		}

		public ArrayList DashLengthList
		{
			get { return m_DashLengthList; }
			set { m_DashLengthList = value; }
		}

		#endregion

		#region CTable Overrides

		public override void ReadGroupCodes()
		{
			DashLengthList = new ArrayList();

			foreach(CGroupCode gc in GroupCodeList)
			{
				if(gc.Code == 2)
				{
					LineTypeName = gc.Value.Trim();
				}
				else if(gc.Code == 3)
				{
					Description = gc.Value.Trim();
				}
				else if(gc.Code == 72)
				{
					AlignmentCode = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 73)
				{
					DashLengthItemCount = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 40)
				{
					TotalPatternLength = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 49)
				{
					DashLengthList.Add(CGlobals.ConvertToInt(gc.Value.Trim()));
				}
				else if(gc.Code == 70)
				{
					SetStandardFlags(CGlobals.ConvertToInt(gc.Value.Trim()));
				}
			}
		}
		public override void WriteGroupCodes()
		{
			CGroupCode gc;
			int flags;

			GroupCodeList.Clear();

			WriteGroupCodeValue(2, LineTypeName.Trim());
			WriteGroupCodeValue(3, Description.Trim());
			WriteGroupCodeValue(72, AlignmentCode.ToString().Trim());
			WriteGroupCodeValue(73, DashLengthItemCount.ToString().Trim());
			WriteGroupCodeValue(40, TotalPatternLength.ToString().Trim());

			foreach(int x in DashLengthList)
			{
				gc = new CGroupCode();
				gc.Code = 49;
				gc.Value = x.ToString().Trim();

				GroupCodeList.Add(gc);
			}

			flags = GetStandardFlags();

			WriteGroupCodeValue(70, flags.ToString().Trim());
		}

		#endregion
	}
}

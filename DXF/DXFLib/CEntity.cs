using System;
using System.Collections;

namespace DXFLib
{
	public class CEntity : ICloneable
	{
		#region Private Member Data

		private string m_EntityName = "";
		private string m_LayerName = "";
		private string m_Handle = "";
		private string m_LineType = "BYLAYER";

		private ArrayList m_GroupCodeList = new ArrayList();
		private ArrayList m_ChildTypeList = new ArrayList();

		private double m_X0 = 0.000;
		private double m_X1 = 0.000;
		private double m_X2 = 0.000;
		private double m_X3 = 0.000;
		private double m_X4 = 0.000;

		private double m_Y0 = 0.000;
		private double m_Y1 = 0.000;
		private double m_Y2 = 0.000;
		private double m_Y3 = 0.000;
		private double m_Y4 = 0.000;

		private double m_Z0 = 0.000;
		private double m_Z1 = 0.000;
		private double m_Z2 = 0.000;
		private double m_Z3 = 0.000;
		private double m_Z4 = 0.000;

		private double m_Radius = 0.000;
		private double m_StartAngle = 0.000;
		private double m_EndAngle = 0.000;

        private bool m_PartOfComplexShape = false;
        private bool m_InScope = false;
        private bool m_Ignore = false;

		#endregion

		#region Constructors

		public CEntity()
		{
		}
        
		#endregion

		#region Public Properties

        public string EntityName
		{
			get { return m_EntityName; }
			set { m_EntityName = value; }
		}
        public string LayerName
		{
			get { return m_LayerName; }
			set { m_LayerName = value; }
		}
        public string Handle
		{
			get { return m_Handle; }
			set { m_Handle = value; }
		}
        public string LineType
		{
			get { return m_LineType; }
			set { m_LineType = value; }
		}
				
        public ArrayList GroupCodeList
		{
			get { return m_GroupCodeList; }
			set { m_GroupCodeList = value; }
		}
        public ArrayList ChildTypeList
		{
			get { return m_ChildTypeList; }
			set { m_ChildTypeList = value; }
		}
        
        public double X0
		{
			get { return m_X0; }
			set { m_X0 = value; }
		}
        public double X1
		{ 
			get { return m_X1; }
			set { m_X1 = value; }
		}
        public double X2
		{
			get { return m_X2; }
			set { m_X2 = value; }
		}
        public double X3
		{
			get { return m_X3; }
			set { m_X3 = value; }
		}
        public double X4
		{
			get { return m_X4; }
			set { m_X4 = value; }
		}

        public double Y0
		{
			get { return m_Y0; }
			set { m_Y0 = value; }
		}
        public double Y1
		{ 
			get { return m_Y1; }
			set { m_Y1 = value; }
		}
        public double Y2
		{
			get { return m_Y2; }
			set { m_Y2 = value; }
		}
        public double Y3
		{
			get { return m_Y3; }
			set { m_Y3 = value; }
		}
        public double Y4
		{
			get { return m_Y4; }
			set { m_Y4 = value; }
		}

        public double Z0
		{
			get { return m_Z0; }
			set { m_Z0 = value; }
		}
        public double Z1
		{ 
			get { return m_Z1; }
			set { m_Z1 = value; }
		}
        public double Z2
		{
			get { return m_Z2; }
			set { m_Z2 = value; }
		}
        public double Z3
		{
			get { return m_Z3; }
			set { m_Z3 = value; }
		}
        public double Z4
		{
			get { return m_Z4; }
			set { m_Z4 = value; }
		}

        public double Radius
		{
			get { return m_Radius; }
			set { m_Radius = value; }
		}
        public double StartAngle
		{
			get { return m_StartAngle; }
			set { m_StartAngle = value; }
		}
        public double EndAngle
		{
			get { return m_EndAngle; }
			set { m_EndAngle = value; }
		}

        public bool PartOfComplexShape
        {
            get { return m_PartOfComplexShape; }
            set { m_PartOfComplexShape = value; }
        }
        public bool InScope
        {
            get { return m_InScope; }
            set { m_InScope = value; }
        }
        public bool Ignore
        {
            get { return m_Ignore; }
            set { m_Ignore = value; }
        }
        
        #endregion

        #region Virtual Interface

        public virtual void ReadGroupCodes()
		{
		}
		public virtual void WriteGroupCodes()
		{
		}

		public virtual ArrayList GetGroupCodes()
		{
			return GroupCodeList;
		}
		public virtual int RenumberHandles(int next)
		{
			return next;
		}

        public virtual void Rotate(double angle)
        {
        }

		#endregion

		#region Utilities

		protected void WriteGroupCodeValue(int code, string val)
		{
			bool foundOne;
			CGroupCode gc;

			if(GroupCodeList == null) GroupCodeList = new ArrayList();

			foundOne = false;
			foreach(CGroupCode tmp in GroupCodeList)
			{
				if(tmp.Code == code)
				{
					tmp.Value = val;
					foundOne = true;
					break;
				}
			}

			if(!foundOne)
			{
				gc = new CGroupCode();
				gc.Code = code;
				gc.Value = val;
			
				GroupCodeList.Add(gc);
			}

			return;
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			CEntity entity;

			entity = new CEntity();
			
			CloneEntity(entity);

			return entity;			
		}
		public void CloneEntity(CEntity entity)
		{
			CGroupCode gc;

			entity.EntityName = EntityName;
			entity.LayerName = LayerName;
			entity.Handle = Handle;
			entity.LineType = LineType;

			entity.X0 = X0;
			entity.Y0 = Y0;
			entity.Z0 = Z0;

			entity.X1 = X1;
			entity.Y1 = Y1;
			entity.Z1 = Z1;

			entity.X2 = X2;
			entity.Y2 = Y2;
			entity.Z2 = Z2;

			entity.X3 = X3;
			entity.Y3 = Y3;
			entity.Z3 = Z3;

			entity.X4 = X4;
			entity.Y4 = Y4;
			entity.Z4 = Z4;

			entity.Radius = Radius;
			entity.StartAngle = StartAngle;
			entity.EndAngle = EndAngle;

			entity.GroupCodeList = new ArrayList();

			if(GroupCodeList != null)
			{
				foreach(CGroupCode tmp in GroupCodeList)
				{
					gc = (CGroupCode) tmp.Clone();
					entity.GroupCodeList.Add(gc);
				}
			}

			entity.ChildTypeList = new ArrayList();

			if(ChildTypeList != null)
			{
				foreach(string tmp in ChildTypeList)
				{
					entity.ChildTypeList.Add(tmp);
				}
			}
		}
        
		#endregion
	}
}

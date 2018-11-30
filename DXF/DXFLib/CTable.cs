using System;
using System.Collections;

namespace DXFLib
{
	public class CTable
	{
		#region Private Member Data

		private bool m_HasExternalXRef = false;
		private bool m_ExternalXRefResolved = false;
		private bool m_TableIsReferenced = false;

		private ArrayList m_GroupCodeList = new ArrayList();

		#endregion

		#region Constructors

		public CTable()
		{
		}

		#endregion

		#region Public Properties

		public bool HasExternalXRef
		{
			get { return m_HasExternalXRef; }
			set { m_HasExternalXRef = value; }
		}
		public bool ExternalXRefResolved
		{
			get { return m_ExternalXRefResolved; }
			set { m_ExternalXRefResolved = value; }
		}
		public bool TableIsReferenced
		{
			get { return m_TableIsReferenced; }
			set { m_TableIsReferenced = value; }
		}

		public ArrayList GroupCodeList
		{
			get { return m_GroupCodeList; }
			set { m_GroupCodeList = value; }
		}
	
		#endregion

		#region Virtual Interface

		public virtual void ReadGroupCodes()
		{
		}
		public virtual void WriteGroupCodes()
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
		protected int GetStandardFlags()
		{
			int flags;
			
			flags = 0;

			if(HasExternalXRef) flags += 16;
			if(ExternalXRefResolved) flags += 32;
			if(TableIsReferenced) flags += 64;

			return flags;
		}
		protected void SetStandardFlags(int flgs)
		{
			BitArray flags;
			BitArray comparer;

			flags = new BitArray(new int[] { flgs });

			comparer = new BitArray( new int[] { 16 });
			HasExternalXRef = flags.And(comparer)[0];

			comparer = new BitArray( new int[] { 32 });
			ExternalXRefResolved = flags.And(comparer)[0];

			comparer = new BitArray( new int[] { 64 });
			TableIsReferenced = flags.And(comparer)[0];
		}
        
		#endregion
	}
}

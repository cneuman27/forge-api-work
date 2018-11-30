using System;
using System.Collections;

namespace DXFLib
{
	public class CEntityVertex : CEntity, ICloneable
	{
		#region Private Member Data

		private double m_StartingWidth = 0.000;
		private double m_EndingWidth = 0.000;
		private double m_Bulge = 0.000;
		private double m_CurveFitTangentDirection;
		private bool m_ExtraVertexCreatedByCurveFitting = false;
		private bool m_HasCurveFitTangent = false;
		private bool m_SplineVertexCreatedBySplineFitting = false;
		private bool m_SplineFrameControlPoint = false;
		private bool m_Is3DPolylineVertex = false;
		private bool m_Is3DPolygonMeshVertex = false;
		private bool m_IsPolyFaceMeshVertex = false;

		#endregion

		#region Constructors

		public CEntityVertex() : base()
		{
		}
        
		#endregion

		#region Public Properties

		public double StartingWidth
		{
			get { return m_StartingWidth; }
			set { m_StartingWidth = value; }
		}
		public double EndingWidth
		{
			get { return m_EndingWidth; }
			set { m_EndingWidth = value; }
		}
		public double Bulge
		{
			get { return m_Bulge; }
			set { m_Bulge = value; }
		}
		public double CurveFitTangentDirection
		{
			get { return m_CurveFitTangentDirection; }
			set { m_CurveFitTangentDirection = value; }
		}
		public bool ExtraVertexCreatedByCurveFitting
		{
			get { return m_ExtraVertexCreatedByCurveFitting; }
			set { m_ExtraVertexCreatedByCurveFitting = value; }
		}
		public bool HasCurveFitTangent
		{
			get { return m_HasCurveFitTangent; }
			set { m_HasCurveFitTangent = value; }
		}
		public bool SplineVertexCreatedBySplineFitting
		{
			get { return m_SplineVertexCreatedBySplineFitting; }
			set { m_SplineVertexCreatedBySplineFitting = value; }
		}
		public bool SplineFrameControlPoint
		{
			get { return m_SplineFrameControlPoint; }
			set { m_SplineFrameControlPoint = value; }
		}		
		public bool Is3DPolylineVertex
		{
			get { return m_Is3DPolylineVertex; }
			set { m_Is3DPolylineVertex = value; }
		}
		public bool Is3DPolygonMeshVertex
		{
			get { return m_Is3DPolygonMeshVertex; }
			set { m_Is3DPolygonMeshVertex = value; }
		}
		public bool IsPolyFaceMeshVertex
		{
			get { return m_IsPolyFaceMeshVertex; }
			set { m_IsPolyFaceMeshVertex = value; }
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
				else if(gc.Code == 40)
				{
					StartingWidth = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 41)
				{
					EndingWidth = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 42)
				{
					Bulge = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 70)
				{
					flags = new BitArray(new int[] { CGlobals.ConvertToInt(gc.Value.Trim()) });
					
					comparer = new BitArray(new int[] { 1 });
					ExtraVertexCreatedByCurveFitting = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 2 });
					HasCurveFitTangent = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 8 });
					SplineVertexCreatedBySplineFitting = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 16 });
					SplineFrameControlPoint = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 32 });
					Is3DPolylineVertex = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 64 });
					Is3DPolygonMeshVertex = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 128 });
					IsPolyFaceMeshVertex = flags.And(comparer)[0];
				}
				else if(gc.Code == 50)
				{
					CurveFitTangentDirection = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
			}
		}
		public override void WriteGroupCodes()
		{
			int flags;

			WriteGroupCodeValue(10, X0.ToString().Trim());
			WriteGroupCodeValue(20, Y0.ToString().Trim());
			WriteGroupCodeValue(30, Z0.ToString().Trim());

			WriteGroupCodeValue(40, StartingWidth.ToString().Trim());
			WriteGroupCodeValue(41, EndingWidth.ToString().Trim());
			WriteGroupCodeValue(42, Bulge.ToString().Trim());
			WriteGroupCodeValue(50, CurveFitTangentDirection.ToString().Trim());
			
			flags = 0;

			if(ExtraVertexCreatedByCurveFitting) flags += 1;
			if(HasCurveFitTangent) flags += 2;
			if(SplineVertexCreatedBySplineFitting) flags += 8;
			if(SplineFrameControlPoint) flags += 16;
			if(Is3DPolylineVertex) flags += 32;
			if(Is3DPolygonMeshVertex) flags += 64;
			if(IsPolyFaceMeshVertex) flags += 128;

			WriteGroupCodeValue(70, flags.ToString().Trim());

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
			CEntityVertex entity;

			entity = new CEntityVertex();

			CloneEntity(entity);

			entity.StartingWidth = StartingWidth;
			entity.EndingWidth = EndingWidth;
			entity.Bulge = Bulge;
			entity.CurveFitTangentDirection = CurveFitTangentDirection;

			entity.ExtraVertexCreatedByCurveFitting = ExtraVertexCreatedByCurveFitting;
			entity.HasCurveFitTangent = HasCurveFitTangent;
			entity.SplineVertexCreatedBySplineFitting = SplineVertexCreatedBySplineFitting;
			entity.SplineFrameControlPoint = SplineFrameControlPoint;
			entity.Is3DPolylineVertex = Is3DPolylineVertex;
			entity.Is3DPolygonMeshVertex = Is3DPolygonMeshVertex;
			entity.IsPolyFaceMeshVertex = IsPolyFaceMeshVertex;
			
			return entity;
		}

		#endregion
	}
}

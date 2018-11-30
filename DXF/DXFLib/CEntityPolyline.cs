using System;
using System.Collections;

namespace DXFLib
{
	public class CEntityPolyline : CEntity, ICloneable
	{
		#region Private Member Data

		private double m_Elevation = 0.000;
		private double m_DefaultStartingWidth = 0.000;
		private double m_DefaultEndingWidth = 0.000;
		private double m_PolygonMeshMVertexCount = 0;
		private double m_PolygonMeshNVertexCount = 0;
		private double m_SmoothSurfaceMDensity = 0;
		private double m_SmoothSurfaceNDensity = 0;

		private bool m_IsClosedPolyline = false;
		private bool m_HasCurveFitVertices = false;
		private bool m_HasSplineFitVertices = false;
		private bool m_Is3DPolyline = false;
		
		private bool m_Is3DMesh = false;
		private bool m_NoSmoothSurfaceFitted = true;
		private bool m_QuadraticBSplineSurface = false;
		private bool m_CubicBSplineSurface;
		private bool m_BezierSurface = false;

		private bool m_MeshClosedInTheNDirection = false;
		private bool m_IsPolyfaceMesh = false;
		private bool m_ContinuousLineType = false;

		private ArrayList m_VertexList = new ArrayList();

		private string m_SeqEndHandle = "";

		#endregion

		#region Constructors

		public CEntityPolyline() : base()
		{
			ChildTypeList = new ArrayList();
			ChildTypeList.Add("VERTEX");
			ChildTypeList.Add("SEQEND");
		}
        
		#endregion

		#region Public Properties

		public double Elevation
		{
			get { return m_Elevation; }
			set { m_Elevation = value; }
		}
		public double DefaultStartingWidth
		{
			get { return m_DefaultStartingWidth; }
			set { m_DefaultStartingWidth = value; }
		}
		public double DefaultEndingWidth
		{
			get { return m_DefaultEndingWidth; }
			set { m_DefaultEndingWidth = value; }
		}
		public double PolygonMeshMVertexCount
		{
			get { return m_PolygonMeshMVertexCount; }
			set { m_PolygonMeshMVertexCount = value; }
		}
		public double PolygonMeshNVertexCount
		{
			get { return m_PolygonMeshNVertexCount; }
			set { m_PolygonMeshNVertexCount = value; }
		}
		public double SmoothSurfaceMDensity
		{
			get { return m_SmoothSurfaceMDensity; }
			set { m_SmoothSurfaceMDensity = value; }
		}
		public double SmoothSurfaceNDensity
		{
			get { return m_SmoothSurfaceNDensity; }
			set { m_SmoothSurfaceNDensity = value; }
		}

		public bool IsClosedPolyline
		{
			get { return m_IsClosedPolyline; }
			set { m_IsClosedPolyline = value; }
		}
		public bool HasCurveFitVertices
		{
			get { return m_HasCurveFitVertices; }
			set { m_HasCurveFitVertices = value; }
		}
		public bool HasSplineFitVertices
		{
			get { return m_HasSplineFitVertices; }
			set { m_HasSplineFitVertices = value; }
		}
		public bool Is3DPolyline
		{
			get { return m_Is3DPolyline; }
			set { m_Is3DPolyline = value; }
		}

		public bool Is3DMesh
		{
			get { return m_Is3DMesh; }
			set { m_Is3DMesh = value; }
		}
		public bool NoSmoothSurfaceFitted
		{
			get { return m_NoSmoothSurfaceFitted; }
			set { m_NoSmoothSurfaceFitted = value; }
		}
		public bool QuadraticBSplineSurface
		{
			get { return m_QuadraticBSplineSurface; }
			set { m_QuadraticBSplineSurface = value; }
		}
		public bool CubicBSplineSurface
		{
			get { return m_CubicBSplineSurface; }
			set { m_CubicBSplineSurface = value; }
		}
		public bool BezierSurface
		{
			get { return m_BezierSurface; }
			set { m_BezierSurface = value; }
		}

		public bool MeshClosedInTheNDirection
		{
			get { return m_MeshClosedInTheNDirection; }
			set { m_MeshClosedInTheNDirection = value; }
		}
		public bool IsPolyfaceMesh
		{
			get { return m_IsPolyfaceMesh; }
			set { m_IsPolyfaceMesh = value; }
		}
		public bool ContinuousLineType
		{
			get { return m_ContinuousLineType; }
			set { m_ContinuousLineType = value; }
		}
		
		public ArrayList VertexList
		{
			get { return m_VertexList; }
			set { m_VertexList = value; }
		}

		public string SeqEndHandle
		{
			get { return m_SeqEndHandle; }
			set { m_SeqEndHandle = value; }
		}

		#endregion

		#region CEntity Overrides

		public override void ReadGroupCodes()
		{
			BitArray flags;
			BitArray comparer;
			bool processingVertices;
			CEntityVertex vertex;

			VertexList = new ArrayList();
			processingVertices = false;
			vertex = null;

		
			foreach(CGroupCode gc in GroupCodeList)
			{
				if(gc.Code == 0)
				{
					if(gc.Value.Trim().ToUpper() == "VERTEX")
					{
						processingVertices = true;

						if(vertex != null)
						{
							vertex.ReadGroupCodes();
							VertexList.Add(vertex);
						}

						vertex = new CEntityVertex();
						vertex.EntityName = "VERTEX";
						vertex.LayerName = LayerName;
					}
					else if(gc.Value.Trim().ToUpper() == "SEQEND")
					{
						processingVertices = false;
						if(vertex != null)
						{
							vertex.ReadGroupCodes();
							VertexList.Add(vertex);
						}
					}
				}
				else if(gc.Code == 30 && processingVertices == false)
				{
					Elevation = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 40 && processingVertices == false)
				{
					DefaultStartingWidth = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 41 && processingVertices == false)
				{
					DefaultEndingWidth = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 70 && processingVertices == false)
				{
					flags = new BitArray(new int[] {CGlobals.ConvertToInt(gc.Value.Trim()) });
	
					comparer = new BitArray(new int[] { 1 });
					IsClosedPolyline = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 2 });
					HasCurveFitVertices = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 4 });
					HasSplineFitVertices = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 8 });
					Is3DPolyline = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 16 });
					Is3DMesh = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 32 });
					MeshClosedInTheNDirection = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 64 });
					IsPolyfaceMesh = flags.And(comparer)[0];

					comparer = new BitArray(new int[] { 128 });
					ContinuousLineType = flags.And(comparer)[0];
				}
				else if(gc.Code == 71 && processingVertices == false)
				{
					PolygonMeshMVertexCount = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 72 && processingVertices == false)
				{
					PolygonMeshNVertexCount = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 73 && processingVertices == false)
				{
					SmoothSurfaceMDensity = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 74 && processingVertices == false)
				{
					SmoothSurfaceNDensity = CGlobals.ConvertToDouble(gc.Value.Trim());
				}
				else if(gc.Code == 75 && processingVertices == false)
				{
					if(CGlobals.ConvertToInt(gc.Value.Trim()) == 0)
					{
						NoSmoothSurfaceFitted = true;
					}
					else if(CGlobals.ConvertToInt(gc.Value.Trim()) == 5)
					{
						QuadraticBSplineSurface = true;
					}
					else if(CGlobals.ConvertToInt(gc.Value.Trim()) == 6)
					{
						CubicBSplineSurface = true;
					}
					else if(CGlobals.ConvertToInt(gc.Value.Trim()) == 8)
					{
						BezierSurface = true;
					}
				}
				else
				{
					if(processingVertices && vertex != null)
					{
						if(gc.Code == 5)
						{
							// Handle
								
							vertex.Handle = gc.Value.Trim();
						}
						else if(gc.Code == 6)
						{
							// Line Type

							vertex.LineType = gc.Value.Trim();
						}
						else if(gc.Code == 8)
						{
							// Layer Name

							vertex.LayerName = gc.Value.Trim();
						}
						else
						{
							vertex.GroupCodeList.Add(gc);
						}
					}
				}
			}
		}
		public override void WriteGroupCodes()
		{
			int flags;

			WriteGroupCodeValue(66, "1");

			WriteGroupCodeValue(10, "0");
			WriteGroupCodeValue(20, "0");
			WriteGroupCodeValue(30, Elevation.ToString().Trim());

			WriteGroupCodeValue(40, DefaultStartingWidth.ToString().Trim());
			WriteGroupCodeValue(41, DefaultEndingWidth.ToString().Trim());

			WriteGroupCodeValue(71, PolygonMeshMVertexCount.ToString().Trim());
			WriteGroupCodeValue(72, PolygonMeshNVertexCount.ToString().Trim());
			WriteGroupCodeValue(73, SmoothSurfaceMDensity.ToString().Trim());
			WriteGroupCodeValue(74, SmoothSurfaceNDensity.ToString().Trim());

			flags = 0;

			if(IsClosedPolyline) flags += 1;
			if(HasCurveFitVertices) flags += 2;
			if(HasSplineFitVertices) flags += 4;
			if(Is3DPolyline) flags += 8;
			if(Is3DMesh) flags += 16;
			if(MeshClosedInTheNDirection) flags += 32;
			if(IsPolyfaceMesh) flags += 64;
			if(ContinuousLineType) flags += 128;

			WriteGroupCodeValue(70, flags.ToString().Trim());

			if(NoSmoothSurfaceFitted)
			{
				WriteGroupCodeValue(75, "0");
			}
			else if(QuadraticBSplineSurface)
			{
				WriteGroupCodeValue(75, "5");
			}
			else if(CubicBSplineSurface)
			{
				WriteGroupCodeValue(75, "6");
			}
			else if(BezierSurface)
			{
				WriteGroupCodeValue(75, "8");
			}

			if(VertexList != null)
			{
				foreach(CEntityVertex vertex in VertexList)
				{
					vertex.WriteGroupCodes();
				}
			}	

		}
		public override ArrayList GetGroupCodes()
		{
			ArrayList list;
			CGroupCode gc;

			list = new ArrayList();

			foreach(CGroupCode tmp in GroupCodeList)
			{
				list.Add(tmp);
			}

			foreach(CEntityVertex vertex in VertexList)
			{
				gc = new CGroupCode();
				gc.Code = 0;
				gc.Value = "VERTEX";
				list.Add(gc);
		
				gc = new CGroupCode();
				gc.Code = 5;
				gc.Value = vertex.Handle.Trim();
				list.Add(gc);

				gc = new CGroupCode();
				gc.Code = 6;
				gc.Value = vertex.LineType.Trim();
				list.Add(gc);
		
				gc = new CGroupCode();
				gc.Code = 8;
				gc.Value = vertex.LayerName.Trim();
				list.Add(gc);

				foreach(CGroupCode tmp in vertex.GroupCodeList)
				{
					list.Add(tmp);
				}
			}

			#region SEQEND

			gc = new CGroupCode();
			gc.Code = 0;
			gc.Value = "SEQEND";
			list.Add(gc);

			gc = new CGroupCode();
			gc.Code = 8;
			gc.Value = LayerName.Trim();
			list.Add(gc);

			gc = new CGroupCode();
			gc.Code = 5;
			gc.Value = SeqEndHandle.Trim();
			list.Add(gc);

			#endregion

			return list;
		}
		public override void Rotate(double angle)
		{
			foreach(CEntityVertex vertex in VertexList)
			{
				vertex.Rotate(angle);
			}
		}
		public override int RenumberHandles(int next)
		{
			foreach(CEntityVertex vertex in VertexList)
			{
				vertex.Handle = CGlobals.IntToHex(next);
				next++;
			}

			SeqEndHandle = CGlobals.IntToHex(next);

			next++;

			return next;
		}

		#endregion

		#region ICloneable Members

		public new object Clone()
		{
			CEntityPolyline entity;
			CEntityVertex vertex;

			entity = new CEntityPolyline();

			CloneEntity(entity);

			entity.Elevation = Elevation;
			entity.DefaultStartingWidth = DefaultStartingWidth;
			entity.DefaultEndingWidth = DefaultEndingWidth;
			entity.PolygonMeshMVertexCount = PolygonMeshMVertexCount;
			entity.PolygonMeshNVertexCount = PolygonMeshNVertexCount;
			entity.SmoothSurfaceMDensity = SmoothSurfaceMDensity;
			entity.SmoothSurfaceNDensity = SmoothSurfaceNDensity;

			entity.IsClosedPolyline = IsClosedPolyline;
			entity.HasCurveFitVertices = HasCurveFitVertices;
			entity.HasSplineFitVertices = HasSplineFitVertices;
			entity.Is3DPolyline = Is3DPolyline;

			entity.Is3DMesh = Is3DMesh;
			entity.NoSmoothSurfaceFitted = NoSmoothSurfaceFitted;
			entity.QuadraticBSplineSurface = QuadraticBSplineSurface;
			entity.CubicBSplineSurface = CubicBSplineSurface;
			entity.BezierSurface = BezierSurface;
			
			entity.MeshClosedInTheNDirection = MeshClosedInTheNDirection;
			entity.IsPolyfaceMesh = IsPolyfaceMesh;
			entity.ContinuousLineType = ContinuousLineType;

			entity.VertexList = new ArrayList();

			if(VertexList != null)
			{
				foreach(CEntityVertex tmp in VertexList)
				{
					vertex = (CEntityVertex) tmp.Clone();
				
					entity.VertexList.Add(vertex);
				}
			}

			return entity;
		}

		#endregion
	}
}

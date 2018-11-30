using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace DXFLib
{
    public class CEntityComplexShape : CEntity, ICloneable
    {
        #region Constructors

        public CEntityComplexShape()
            : base()
		{
		}

        public CEntityComplexShape(
            CTableLayer layer, 
            Guid ToolPk)
            : base()
        {
            PK = ToolPk;

            foreach (CEntityLine line in layer.LineList)
            {
                LineList.Add(line);
            }

            foreach (CEntityCircle circle in layer.CircleList)
            {
                CircleList.Add(circle);
            }

            foreach (CEntityArc arc in layer.ArcList)
            {
                ArcList.Add(arc);
            }
        }
		
        #endregion

        private object m_BasePointObject = null;
        private List<CEntityLine> m_LineList = null;
        private List<CEntityCircle> m_CircleList = null;
        private List<CEntityArc> m_ArcList = null;
        private double m_XStart = 0;
        private bool m_XStartBuilt = false;
        private double m_YStart = 0;
        private bool m_YStartBuilt = false;
        private double m_XEnd = -10000;
        private double m_YEnd = -10000;

        public Guid PK = Guid.Empty;
        public List<CEntityLine> LineList
        {
            get
            {
                if (m_LineList == null)
                {
                    m_LineList = new List<CEntityLine>();
                }
                return m_LineList;
            }
        }
        public List<CEntityCircle> CircleList
        {
            get
            {
                if (m_CircleList == null)
                {
                    m_CircleList = new List<CEntityCircle>();
                }
                return m_CircleList;
            }
        }
        public List<CEntityArc> ArcList
        {
            get
            {
                if (m_ArcList == null)
                {
                    m_ArcList = new List<CEntityArc>();
                }
                return m_ArcList;
            }
        }
        public List<object> EntityList
        {
            get
            {
                List<object> returnList = new List<object>();
                foreach (CEntityLine line in LineList)
                {
                    returnList.Add(line);
                }
                foreach (CEntityCircle circle in CircleList)
                {
                    returnList.Add(circle);
                }
                foreach (CEntityArc arc in ArcList)
                {
                    returnList.Add(arc);
                }
                return returnList;
            }
        }

        public object BasePointObject
        {
            get
            {
                if (m_BasePointObject == null)
                {
                    foreach (CEntityLine line in LineList)
                    {
                        if (line.X0 == XStart && line.Y0 == YStart)
                        {
                            m_BasePointObject = line;
                            return m_BasePointObject;
                        }
                    }
                    foreach (CEntityCircle circle in CircleList)
                    {
                        if (circle.X0 == XStart && circle.Y0 == YStart)
                        {
                            m_BasePointObject = circle;
                            return m_BasePointObject;
                        }
                    }
                    foreach (CEntityArc arc in ArcList)
                    {
                        if (arc.X0 == XStart && arc.Y0 == YStart)
                        {
                            m_BasePointObject = arc;
                            return m_BasePointObject;
                        }
                    }
                    if (m_BasePointObject == null)//nothing actually touches the lower left corner. Start doing some math to find lowest left object.
                    {
                        double distance = (((XStart - 100000) * (XStart - 100000)) + ((YStart - 100000) * (YStart - 100000)));
                        foreach (CEntityLine line in LineList)
                        {
                            double eDistance = (((XStart - line.X0) * (XStart - line.X0)) + ((XStart - line.Y0) * (XStart - line.Y0)));
                            if (eDistance < distance)
                            {
                                distance = eDistance;
                                m_BasePointObject = line;
                            }
                        }
                        foreach (CEntityCircle circle in CircleList)
                        {
                            double eDistance = (((XStart - circle.X0) * (XStart - circle.X0)) + ((XStart - circle.Y0) * (XStart - circle.Y0)));
                            if (eDistance < distance)
                            {
                                distance = eDistance;
                                m_BasePointObject = circle;
                            }
                        }
                        foreach (CEntityArc arc in ArcList)
                        {
                            double eDistance = (((XStart - arc.X0) * (XStart - arc.X0)) + ((XStart - arc.Y0) * (XStart - arc.Y0)));
                            if (eDistance < distance)
                            {
                                distance = eDistance;
                                m_BasePointObject = arc;
                            }
                        }
                    }
                    
                }
                return m_BasePointObject;
            }
        }
        public CEntity BasePointEntity
        {
            get
            {
                if (BasePointObject is CEntity)
                {
                    return (CEntity)BasePointObject;
                }
                return null;
            }
        }

        public double XStart
        {
            get
            {
                if (!m_XStartBuilt)
                {
                    m_XStart = 10000000;
                    foreach (CEntityLine line in LineList)
                    {
                        if (line.X0 < m_XStart)
                        {
                            m_XStart = line.X0;
                        }
                    }
                    foreach (CEntityCircle circle in CircleList)
                    {
                        if (circle.X0 < m_XStart)
                        {
                            m_XStart = circle.X0;
                        }
                    }
                    foreach (CEntityArc arc in ArcList)
                    {
                        if (arc.X0 < m_XStart)
                        {
                            m_XStart = arc.X0;
                        }
                    }
                    m_XStartBuilt = true;
                }
                return m_XStart;
            }
        }
        public double YStart
        {
            get
            {
                if (!m_YStartBuilt)
                {
                    m_YStart = 10000000;
                    foreach (CEntityLine line in LineList)
                    {
                        if (line.Y0 < m_YStart)
                        {
                            m_YStart = line.Y0;
                        }
                    }
                    foreach (CEntityCircle circle in CircleList)
                    {
                        if (circle.Y0 < m_YStart)
                        {
                            m_YStart = circle.Y0;
                        }
                    }
                    foreach (CEntityArc arc in ArcList)
                    {
                        if (arc.Y0 < m_YStart)
                        {
                            m_YStart = arc.Y0;
                        }
                    }
                    m_YStartBuilt = true;
                }
                return m_YStart;
            }
        }
        public double XEnd
        {
            get
            {
                if (m_XEnd < 0)
                {
                    foreach (CEntityLine line in LineList)
                    {
                        if (line.X1 > m_XEnd)
                        {
                            m_XEnd = line.X1;
                        }
                    }
                    foreach (CEntityCircle circle in CircleList)
                    {
                        if (circle.X0 > m_XEnd)
                        {
                            m_XEnd = circle.X0;
                        }
                    }
                    foreach (CEntityArc arc in ArcList)
                    {
                        if (arc.X0 > m_XEnd)
                        {
                            m_XEnd = arc.X0;
                        }
                    }
                }
                return m_XEnd;
            }
        }
        public double YEnd
        {
            get
            {
                if (m_YEnd < 0)
                {
                    foreach (CEntityLine line in LineList)
                    {
                        if (line.Y1 > m_YEnd)
                        {
                            m_YEnd = line.Y1;
                        }
                    }
                    foreach (CEntityCircle circle in CircleList)
                    {
                        if (circle.Y0 > m_YEnd)
                        {
                            m_YEnd = circle.Y0;
                        }
                    }
                    foreach (CEntityArc arc in ArcList)
                    {
                        if (arc.Y0 > m_YEnd)
                        {
                            m_YEnd = arc.Y0;
                        }
                    }
                }
                return m_YEnd;
            }
        }
        public double Width
        {
            get
            {
                return XEnd - XStart;
            }
        }
        public double Height
        {
            get
            {
                return YEnd - YStart;
            }
        }
        public double CenterPoint_Horizontal
        {
            get
            {
                return XStart + ((XEnd - XStart) / 2);
            }
        }
        public double CenterPoint_Vertical
        {
            get
            {
                return YStart + ((YEnd - YStart) / 2);
            }
        }
    }
}

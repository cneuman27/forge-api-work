using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace DXFLib
{
    public class CTableLayer : CTable
    {
        #region Private Member Data

        private string m_LayerName = "";

        private double m_ColorNumber = 7;
        private string m_LineTypeName = "CONTINUOUS";

        private bool m_IsFrozen = false;
        private bool m_IsFrozenInNewViewports = false;
        private bool m_IsLocked = false;

        private ArrayList m_EntityList = new ArrayList();
        private ArrayList m_SquaresList = new ArrayList();
        private ArrayList m_RectanglesList = new ArrayList();
        private List<CEntityComplexShape> m_ComplexShapeList;

        private bool m_SquareListBuilt = false;

        #endregion

        #region Constructors

        public CTableLayer()
            : base()
        {
        }

        #endregion

        #region Public Properties

        public string LayerName
        {
            get
            {
                return m_LayerName;
            }
            set
            {
                m_LayerName = value;
            }
        }

        public double ColorNumber
        {
            get
            {
                return m_ColorNumber;
            }
            set
            {
                m_ColorNumber = value;
            }
        }
        public string LineTypeName
        {
            get
            {
                return m_LineTypeName;
            }
            set
            {
                m_LineTypeName = value;
            }
        }

        public bool IsFrozen
        {
            get
            {
                return m_IsFrozen;
            }
            set
            {
                m_IsFrozen = value;
            }
        }
        public bool IsFrozenInNewViewports
        {
            get
            {
                return m_IsFrozenInNewViewports;
            }
            set
            {
                m_IsFrozenInNewViewports = value;
            }
        }
        public bool IsLocked
        {
            get
            {
                return m_IsLocked;
            }
            set
            {
                m_IsLocked = value;
            }
        }

        public ArrayList EntityList
        {
            get
            {
                return m_EntityList;
            }
            set
            {
                m_EntityList = value;
            }
        }

        public ArrayList SquaresList
        {
            get
            {
                if (!m_SquareListBuilt)
                {
                    BuildSquareList();
                }
                return m_SquaresList;
            }
        }
        public ArrayList RectanglesList
        {
            get
            {
                if (!m_SquareListBuilt)
                {
                    BuildSquareList();
                }
                return m_RectanglesList;
            }
        }
        public List<CEntityLine> LineList
        {
            get
            {
                List<CEntityLine> returnList = new List<CEntityLine>();
                foreach (object obj in EntityList)
                {
                    if (obj is CEntityLine)
                    {
                        returnList.Add((CEntityLine)obj);
                    }
                }
                return returnList;
            }
        }
        public List<CEntityCircle> CircleList
        {
            get
            {
                List<CEntityCircle> returnList = new List<CEntityCircle>();
                foreach (object obj in EntityList)
                {
                    if (obj is CEntityCircle)
                    {
                        returnList.Add((CEntityCircle)obj);
                    }
                }
                return returnList;
            }
        }
        public List<CEntityArc> ArcList
        {
            get
            {
                List<CEntityArc> returnList = new List<CEntityArc>();
                foreach (object obj in EntityList)
                {
                    if (obj is CEntityArc)
                    {
                        returnList.Add((CEntityArc)obj);
                    }
                }
                return returnList;
            }
        }
        public List<CEntityComplexShape> ComplexShapeList
        {
            get
            {
                if (m_ComplexShapeList == null)
                {
                    m_ComplexShapeList = new List<CEntityComplexShape>();
                }
                return m_ComplexShapeList;
            }
        }

        #endregion

        #region CTable Overrides

        public override void ReadGroupCodes()
        {
            BitArray flags;
            BitArray comparer;

            foreach (CGroupCode gc in GroupCodeList)
            {
                if (gc.Code == 2)
                {
                    LayerName = gc.Value.Trim();
                }
                else if (gc.Code == 62)
                {
                    ColorNumber = CGlobals.ConvertToDouble(gc.Value.Trim());
                }
                else if (gc.Code == 6)
                {
                    LineTypeName = gc.Value.Trim();
                }
                else if (gc.Code == 70)
                {
                    SetStandardFlags(CGlobals.ConvertToInt(gc.Value.Trim()));

                    flags = new BitArray(new int[] { CGlobals.ConvertToInt(gc.Value.Trim()) });

                    comparer = new BitArray(new int[] { 1 });
                    IsFrozen = flags.And(comparer)[0];

                    comparer = new BitArray(new int[] { 2 });
                    IsFrozenInNewViewports = flags.And(comparer)[0];

                    comparer = new BitArray(new int[] { 4 });
                    IsLocked = flags.And(comparer)[0];
                }
            }
        }
        public override void WriteGroupCodes()
        {
            int flags;

            WriteGroupCodeValue(2, LayerName.Trim());
            WriteGroupCodeValue(62, ColorNumber.ToString());
            WriteGroupCodeValue(6, LineTypeName.Trim());

            flags = GetStandardFlags();

            if (IsFrozen) flags += 1;
            if (IsFrozenInNewViewports) flags += 2;
            if (IsLocked) flags += 4;

            WriteGroupCodeValue(70, flags.ToString().Trim());
        }
        
        #endregion

        #region Builders

        private void BuildSquareList()
        {
            List<CEntityLine> lineList = new List<CEntityLine>();
            foreach (object obj in EntityList)
            {
                if (obj is CEntityLine)
                {
                    CEntityLine line = obj as CEntityLine;
                    if (line != null)
                    {
                        lineList.Add(line);
                    }
                }
            }

            foreach (CEntityLine line1 in lineList)
            {
                foreach (CEntityLine line2 in lineList)
                {
                    if (line2.Y0 == line1.Y1 && line2.X0 == line1.X1)
                    {
                        foreach (CEntityLine line3 in lineList)
                        {
                            if (line3.Y0 == line2.Y1 && line3.X0 == line2.X1)
                            {
                                foreach (CEntityLine line4 in lineList)
                                {
                                    if (line4.Y0 == line3.Y1 && line4.X0 == line3.X1)
                                    {
                                        //this would mark a parallagram or trapezoid as a rectangle. Needs modified for angles.
                                        //if (line1.Length == line2.Length && line1.Length == line3.Length && line1.Length == line4.Length)
                                        //{
                                            double smallestX0 = line1.X0;
                                            if (line2.X0 < smallestX0)
                                            {
                                                smallestX0 = line2.X0;
                                            }
                                            if (line3.X0 < smallestX0)
                                            {
                                                smallestX0 = line3.X0;
                                            }
                                            if (line4.X0 < smallestX0)
                                            {
                                                smallestX0 = line4.X0;
                                            }
                                            double smallestY0 = line1.Y0;
                                            if (line2.Y0 < smallestY0)
                                            {
                                                smallestY0 = line2.Y0;
                                            }
                                            if (line3.Y0 < smallestY0)
                                            {
                                                smallestY0 = line3.Y0;
                                            }
                                            if (line4.Y0 < smallestY0)
                                            {
                                                smallestY0 = line4.Y0;
                                            }
                                            double largestX1 = line1.X1;
                                            if (line2.X1 > largestX1)
                                            {
                                                largestX1 = line2.X1;
                                            }
                                            if (line3.X1 > largestX1)
                                            {
                                                largestX1 = line3.X1;
                                            }
                                            if (line4.X1 > largestX1)
                                            {
                                                largestX1 = line4.X1;
                                            }
                                            double largestY1 = line1.Y1;
                                            if (line2.Y1 > largestY1)
                                            {
                                                largestY1 = line2.Y1;
                                            }
                                            if (line3.Y1 > largestY1)
                                            {
                                                largestY1 = line3.Y1;
                                            }
                                            if (line4.Y1 > largestY1)
                                            {
                                                largestY1 = line4.Y1;
                                            }
                                            if (!line1.PartOfComplexShape && !line2.PartOfComplexShape && !line3.PartOfComplexShape && !line4.PartOfComplexShape)
                                            {
                                                CEntityRectangle square = new CEntityRectangle();
                                                square.Width = Math.Round(largestX1 - smallestX0, 3);
                                                square.Height = Math.Round(largestY1 - smallestY0, 3);
                                                square.CenterPoint_Horizontal = Math.Round(smallestX0 + (square.Width / 2), 3);
                                                square.CenterPoint_Vertical = Math.Round(smallestY0 + (square.Height / 2), 3);
                                                bool alreadyHave = false;
                                                foreach (object obj in m_SquaresList)
                                                {
                                                    CEntityRectangle rect = obj as CEntityRectangle;
                                                    if (rect.CenterPoint_Horizontal == square.CenterPoint_Horizontal &&
                                                        rect.CenterPoint_Vertical == square.CenterPoint_Vertical &&
                                                        rect.Width == square.Width)
                                                    {
                                                        alreadyHave = true;
                                                        break;
                                                    }
                                                }
                                                if (!alreadyHave)
                                                {
                                                    if (square.Width == square.Height)
                                                    {
                                                        m_SquaresList.Add(square);
                                                    }
                                                    else
                                                    {
                                                        m_RectanglesList.Add(square);
                                                    }
                                                }
                                            }
                                        //}
                                    }
                                }
                            }
                        }
                    }
                }
            }
            m_SquareListBuilt = true;
        }

        #endregion

        #region Other Methods

        public void FindComplexShapes(
            CEntityComplexShape fShape, 
            double yOffset, 
            double tolerance)
        {
            try
            {
                if (fShape.BasePointObject == null || fShape.BasePointEntity == null)
                {
                    return;
                }
                
                do
                {
                    CEntity basePointEntity = null;
                    #region find the base point entity
                    foreach (object obj in EntityList)
                    {
                        if (obj.GetType() == fShape.BasePointObject.GetType())
                        {
                            if (obj is CEntity)
                            {
                                CEntity entity = obj as CEntity;
                                if (!entity.PartOfComplexShape && !entity.Ignore)
                                {
                                    if (entity != null && fShape.BasePointEntity != null)
                                    {
                                        if (obj is CEntityLine)
                                        {
                                            if (entity.StartAngle == fShape.BasePointEntity.StartAngle &&
                                                entity.EndAngle == fShape.BasePointEntity.EndAngle &&
                                                Math.Round(entity.X1 - entity.X0, 3) == Math.Round(fShape.BasePointEntity.X1 - fShape.BasePointEntity.X0, 3) &&
                                                Math.Round(entity.Y1 - entity.Y0, 3) == Math.Round(fShape.BasePointEntity.Y1 - fShape.BasePointEntity.Y0, 3) &&
                                                Math.Round(entity.Radius, 3) + tolerance >= Math.Round(fShape.BasePointEntity.Radius, 3) &&
                                                Math.Round(entity.Radius, 3) - tolerance <= Math.Round(fShape.BasePointEntity.Radius, 3))
                                            {
                                                basePointEntity = entity;
                                                break;
                                            }
                                        }
                                        else if (obj is CEntityCircle || obj is CEntityArc)
                                        {
                                            if (entity.StartAngle == fShape.BasePointEntity.StartAngle &&
                                                entity.EndAngle == fShape.BasePointEntity.EndAngle &&
                                                Math.Round(entity.Radius, 3) + tolerance >= Math.Round(fShape.BasePointEntity.Radius, 3) &&
                                                Math.Round(entity.Radius, 3) - tolerance <= Math.Round(fShape.BasePointEntity.Radius, 3))
                                            {
                                                basePointEntity = entity;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (basePointEntity == null)
                    {
                        break;
                    }
                    #endregion

                    #region find the shapes
                    bool foundShape = true;
                    foreach (object fObj in fShape.EntityList)
                    {
                        if (fObj is CEntity)
                        {
                            CEntity fEntity = fObj as CEntity;
                            bool foundLine = false;
                            PointF fBegin = new PointF((float)Math.Round(fEntity.X0 - fShape.BasePointEntity.X0, 3), (float)Math.Round(fEntity.Y0 - fShape.BasePointEntity.Y0, 3));
                            PointF fEnd = new PointF((float)Math.Round(fEntity.X1 - fShape.BasePointEntity.X1, 3), (float)Math.Round(fEntity.Y1 - fShape.BasePointEntity.Y1, 3));
                            foreach (object obj in EntityList)
                            {
                                if (obj.GetType() == fObj.GetType())
                                {
                                    CEntity entity = obj as CEntity;
                                    if (!entity.PartOfComplexShape)
                                    {
                                        PointF eBegin = new PointF((float)Math.Round(entity.X0 - basePointEntity.X0, 3), (float)Math.Round(entity.Y0 - basePointEntity.Y0, 3));
                                        PointF eEnd = new PointF((float)Math.Round(entity.X1 - basePointEntity.X1, 3), (float)Math.Round(entity.Y1 - basePointEntity.Y1, 3));
                                        if (ArePointsEquivalent(fBegin, eBegin, (float)tolerance) && ArePointsEquivalent(fEnd, eEnd, (float)tolerance))
                                        {
                                            foundLine = true;
                                            entity.InScope = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!foundLine)
                            {
                                foundShape = false;
                                basePointEntity.Ignore = true;
                                foreach (object obj in EntityList)
                                {
                                    if (obj is CEntity)
                                    {
                                        ((CEntity)obj).InScope = false;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    #endregion

                    #region shape is found
                    if (foundShape)
                    {
                        CEntityComplexShape shapeAdd = new CEntityComplexShape();
                        shapeAdd.PK = fShape.PK;
                        foreach (object obj in EntityList)
                        {
                            if (obj is CEntity)
                            {
                                CEntity entity = obj as CEntity;
                                if (!entity.PartOfComplexShape && entity.InScope)
                                {
                                    entity.PartOfComplexShape = true;
                                    if (obj is CEntityLine)
                                    {
                                        shapeAdd.LineList.Add((CEntityLine)obj);
                                    }
                                    else if (obj is CEntityCircle)
                                    {
                                        shapeAdd.CircleList.Add((CEntityCircle)obj);
                                    }
                                    else if (obj is CEntityArc)
                                    {
                                        shapeAdd.ArcList.Add((CEntityArc)obj);
                                    }
                                }
                            }
                        }
                        if (yOffset + tolerance >= shapeAdd.CenterPoint_Vertical &&
                            yOffset - tolerance <= shapeAdd.CenterPoint_Vertical)
                        {
                            ComplexShapeList.Add(shapeAdd);
                        }
                        else
                        {
                            basePointEntity.Ignore = true;
                            foreach (CEntityLine line in shapeAdd.LineList)
                            {
                                line.PartOfComplexShape = false;
                                //line.Ignore = true;
                                line.InScope = false;
                            }
                            foreach (CEntityCircle circle in shapeAdd.CircleList)
                            {
                                circle.PartOfComplexShape = false;
                                //circle.Ignore = true;
                                circle.InScope = false;
                            }
                            foreach (CEntityArc arc in shapeAdd.ArcList)
                            {
                                arc.PartOfComplexShape = false;
                                //arc.Ignore = true;
                                arc.InScope = false;
                            }
                        }
                    }
                    #endregion

                } while (true);
                foreach (object obj in EntityList)
                {
                    if (obj is CEntity)
                    {
                        ((CEntity)obj).InScope = false;
                        ((CEntity)obj).Ignore = false;
                    }
                }
            }
            catch
            {
            }
        }

        private bool ArePointsEquivalent(
            PointF pointA, 
            PointF pointB, 
            float tolerance)
        {
            if (pointA.X + tolerance >= pointB.X)
            {
                if (pointA.X - tolerance <= pointB.X)
                {
                    if (pointA.Y + tolerance >= pointB.Y)
                    {
                        if (pointA.Y - tolerance <= pointB.Y)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #endregion
    }
}

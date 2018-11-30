using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DXFLib;

namespace BoundaryDXFNative
{
    public class CBoundaryDXF
    {
        #region Locker

        public static object Locker = new object();

        #endregion

        #region Private Members

        private CDXF m_DXF = null;

        #endregion

        #region Constructors and Destructors

        public CBoundaryDXF()
        {
            m_DXF = CDXF.NewDXF();
        }
        public CBoundaryDXF(string fileName)
        {
            lock (CBoundaryDXF.Locker)
            {
                m_DXF = new CDXF(fileName);
            }
        }
        public CBoundaryDXF(byte[] buffer)
        {
            lock (CBoundaryDXF.Locker)
            {
                m_DXF = new CDXF(buffer);
            }
        }

        public void CleanUp()
        {
        }

        #endregion

        #region Public Properties

        public Hashtable Layers
        {
            get
            {
                Hashtable layers;

                layers = new Hashtable();
                foreach (CTableLayer layer in m_DXF.LayerList.Values)
                {
                    layers.Add(layer.LayerName.ToUpper().Trim(), new CBoundaryDXFLayer(this, layer));
                }

                return layers;
            }
        }

        internal CDXF DXF
        {
            get { return m_DXF; }
        }

        #endregion

        #region DXF Functions

        public void CopyDXFEntity(CBoundaryDXFEntity src, CBoundaryDXFEntity dest)
        {
            if (src.Entity.EntityName.Trim().ToUpper() == "LINE")
            {
                dest.Entity = (CEntityLine)((CEntityLine)src.Entity).Clone();
            }
            else if (src.Entity.EntityName.Trim().ToUpper() == "POINT")
            {
                dest.Entity = (CEntityPoint)((CEntityPoint)src.Entity).Clone();
            }
            else if (src.Entity.EntityName.Trim().ToUpper() == "CIRCLE")
            {
                dest.Entity = (CEntityCircle)((CEntityCircle)src.Entity).Clone();
            }
            else if (src.Entity.EntityName.Trim().ToUpper() == "ARC")
            {
                dest.Entity = (CEntityArc)((CEntityArc)src.Entity).Clone();
            }
            else if (src.Entity.EntityName.Trim().ToUpper() == "TRACE")
            {
                dest.Entity = (CEntityTrace)((CEntityTrace)src.Entity).Clone();
            }
            else if (src.Entity.EntityName.Trim().ToUpper() == "SOLID")
            {
                dest.Entity = (CEntitySolid)((CEntitySolid)src.Entity).Clone();
            }
            else if (src.Entity.EntityName.Trim().ToUpper() == "TEXT")
            {
                dest.Entity = (CEntityText)((CEntityText)src.Entity).Clone();
            }
            else if (src.Entity.EntityName.Trim().ToUpper() == "SHAPE")
            {
                dest.Entity = (CEntityShape)((CEntityShape)src.Entity).Clone();
            }
            else if (src.Entity.EntityName.Trim().ToUpper() == "POLYLINE")
            {
                dest.Entity = (CEntityPolyline)((CEntityPolyline)src.Entity).Clone();
            }
            else if (src.Entity.EntityName.Trim().ToUpper() == "3DFACE")
            {
                dest.Entity = (CEntity3DFace)((CEntity3DFace)src.Entity).Clone();
            }
            else
            {
                dest.Entity = (CEntity)src.Entity.Clone();
            }
        }

        public void CopyLayerTo(string srcLayer, string destLayer, ref CBoundaryDXF destDXF)
        {
            CBoundaryDXFLayer dxfLayer;
            CBoundaryDXFEntity newEntity;

            if (destDXF.Layers.ContainsKey(destLayer.Trim().ToUpper()) == false)
            {
                destDXF.CreateLayer(destLayer.Trim().ToUpper());
            }

            if (Layers.ContainsKey(srcLayer.Trim().ToUpper()))
            {
                dxfLayer = (CBoundaryDXFLayer)Layers[srcLayer.Trim().ToUpper()];

                foreach (CBoundaryDXFEntity entity in dxfLayer.Entities)
                {
                    newEntity = new CBoundaryDXFEntity(this, null);
                    CopyDXFEntity(entity, newEntity);
                    newEntity.LayerName = destLayer.Trim().ToUpper();

                    ((CBoundaryDXFLayer)destDXF.Layers[destLayer.Trim().ToUpper()]).AddEntity(newEntity);
                }
            }
        }

        public void SaveAs(string filename)
        {
            lock (CBoundaryDXF.Locker)
            {
                //m_DXF.SaveAs(filename);
                SaveInternal(filename);
            }
        }
        public void SaveAs(string filename, short precision)
        {
            lock (CBoundaryDXF.Locker)
            {
                //m_DXF.SaveAs(filename);
                SaveInternal(filename);
            }
        }
        public byte[] SaveAsBytes()
        {
            lock (CBoundaryDXF.Locker)
            {
                CBoundaryDXF newDXF;

                newDXF = new CBoundaryDXF();

                foreach (CBoundaryDXFLayer layer in Layers.Values)
                {
                    newDXF.CreateLayer(layer.LayerName.Trim().ToUpper());

                    foreach (CBoundaryDXFEntity entity in layer.Entities)
                    {
                        ((CBoundaryDXFLayer)newDXF.Layers[layer.LayerName.Trim().ToUpper()]).AddEntity(entity);
                    }
                }

                return newDXF.DXF.SaveAsBytes();
            }
        }

        private void SaveInternal(string fileName)
        {
            CBoundaryDXF newDXF;

            newDXF = new CBoundaryDXF();

            foreach (CBoundaryDXFLayer layer in Layers.Values)
            {
                newDXF.CreateLayer(layer.LayerName.Trim().ToUpper());

                foreach (CBoundaryDXFEntity entity in layer.Entities)
                {
                    ((CBoundaryDXFLayer)newDXF.Layers[layer.LayerName.Trim().ToUpper()]).AddEntity(entity);
                }
            }

            newDXF.DXF.SaveAs(fileName);
        }
               
        public void DrawText(double originX, double originY, string text, string layerName)
        {
            DrawText(originX, originY, text, layerName, 0.5);
        }
        public void DrawText(double originX, double originY, string text, string layerName, double textHeight)
        {
            CEntityText textEntity;
            CBoundaryDXFEntity entity;

            textEntity = new CEntityText();
            textEntity.EntityName = "TEXT";
            textEntity.X0 = originX;
            textEntity.Y0 = originY;
            textEntity.Text = text;
            textEntity.LayerName = layerName;
            textEntity.Height = textHeight;

            entity = new CBoundaryDXFEntity(this, textEntity);

            if (Layers.ContainsKey(layerName.Trim().ToUpper()))
            {
                ((CBoundaryDXFLayer)Layers[layerName.Trim().ToUpper()]).AddEntity(entity);
            }
        }

        public void ReplaceText(ref CBoundaryDXFEntity entity, string textToSearchFor, string newText)
        {
            CEntityText textEntity = (CEntityText)entity.Entity;
            if (textEntity.Text.Contains(textToSearchFor))
            {
                textEntity.Text = textToSearchFor + " " + newText;
            }
            entity.Entity = textEntity;
        }

        public void DrawCircle(double originX, double originY, double diameter, string layerName)
        {
            CEntityCircle circleEntity;
            CBoundaryDXFEntity entity;

            circleEntity = new CEntityCircle();
            circleEntity.EntityName = "CIRCLE";
            circleEntity.X0 = originX;
            circleEntity.Y0 = originY;
            circleEntity.LayerName = layerName;
            circleEntity.Radius = diameter / 2;

            entity = new CBoundaryDXFEntity(this, circleEntity);

            if (Layers.ContainsKey(layerName.Trim().ToUpper()))
            {
                ((CBoundaryDXFLayer)Layers[layerName.Trim().ToUpper()]).AddEntity(entity);
            }
        }

        public void DrawLine(double startX, double startY, double endX, double endY, string layerName)
        {
            CEntityLine lineEntity;
            CBoundaryDXFEntity entity;

            lineEntity = new CEntityLine();
            lineEntity.EntityName = "LINE";
            lineEntity.X0 = startX;
            lineEntity.Y0 = startY;
            lineEntity.X1 = endX;
            lineEntity.Y1 = endY;
            lineEntity.LayerName = layerName;

            entity = new CBoundaryDXFEntity(this, lineEntity);

            if (Layers.ContainsKey(layerName.Trim().ToUpper()))
            {
                ((CBoundaryDXFLayer)Layers[layerName.Trim().ToUpper()]).AddEntity(entity);
            }
        }

        public void CreateLayer(string layerName)
        {
            CTableLayer layer;

            if (m_DXF.LayerList.ContainsKey(layerName.Trim().ToUpper()) == false)
            {
                layer = new CTableLayer();
                layer.LayerName = layerName.Trim().ToUpper();
                m_DXF.LayerList.Add(layerName.Trim().ToUpper(), layer);
            }
        }
        public bool DoesLayerExist(string layerName)
        {
            bool layerExists = false;
            foreach (CTableLayer layer in m_DXF.LayerList.Values)
            {
                if (layer.LayerName.Trim().ToUpper() == layerName.Trim().ToUpper())
                {
                    layerExists = true;
                    break;
                }
            }
            return layerExists;
        }

        public void DeleteLayer(string layerName)
        {
            if (m_DXF.LayerList.ContainsKey(layerName.Trim().ToUpper()))
            {
                m_DXF.LayerList.Remove(layerName.Trim().ToUpper().Trim());
            }
        }

        public double GetWidth()
        {
            double maxX;
            double minX;

            maxX = GetMaxX();
            minX = GetMinX();

            if (maxX != -9999.9999 && minX != 9999.9999)
            {
                return (GetMaxX() - GetMinX());
            }
            else
            {
                return 0.0000;
            }
        }
        public double GetWidth(string layerName)
        {
            double maxX;
            double minX;

            maxX = GetMaxX(layerName);
            minX = GetMinX(layerName);

            if (maxX != -9999.9999 && minX != 9999.9999)
            {
                return (GetMaxX() - GetMinX());
            }
            else
            {
                return 0.0000;
            }
        }
        public double GetHeight()
        {
            double maxY;
            double minY;

            maxY = GetMaxY();
            minY = GetMinY();

            if (maxY != -9999.9999 && minY != 9999.9999)
            {
                return (GetMaxY() - GetMinY());
            }
            else
            {
                return 0.0000;
            }
        }
        public double GetHeight(string layerName)
        {
            double maxY;
            double minY;

            maxY = GetMaxY(layerName);
            minY = GetMinY(layerName);

            if (maxY != -9999.9999 && minY != 9999.9999)
            {
                return (GetMaxY() - GetMinY());
            }
            else
            {
                return 0.0000;
            }
        }

        public double GetMaxX()
        {
            double ret = -9999.9999;

            foreach (CBoundaryDXFLayer layer in Layers.Values)
            {
                foreach (CBoundaryDXFEntity entity in layer.Entities)
                {
                    if (entity.EntityType.Trim().ToUpper() == "LINE")
                    {
                        if (entity.X0 > ret) { ret = entity.X0; }
                        if (entity.X1 > ret) { ret = entity.X1; }
                    }
                }
            }

            return ret;
        }
        public double GetMaxY()
        {
            double ret = -9999.9999;

            foreach (CBoundaryDXFLayer layer in Layers.Values)
            {
                foreach (CBoundaryDXFEntity entity in layer.Entities)
                {
                    if (entity.EntityType.Trim().ToUpper() == "LINE")
                    {
                        if (entity.Y0 > ret) { ret = entity.Y0; }
                        if (entity.Y1 > ret) { ret = entity.Y1; }
                    }
                }
            }

            return ret;
        }
        public double GetMinX()
        {
            double ret = 9999.9999;

            foreach (CBoundaryDXFLayer layer in Layers.Values)
            {
                foreach (CBoundaryDXFEntity entity in layer.Entities)
                {
                    if (entity.EntityType.Trim().ToUpper() == "LINE")
                    {
                        if (entity.X0 < ret) { ret = entity.X0; }
                        if (entity.X1 < ret) { ret = entity.X1; }
                    }
                }
            }

            return ret;
        }
        public double GetMinY()
        {
            double ret = 9999.9999;

            foreach (CBoundaryDXFLayer layer in Layers.Values)
            {
                foreach (CBoundaryDXFEntity entity in layer.Entities)
                {
                    if (entity.EntityType.Trim().ToUpper() == "LINE")
                    {
                        if (entity.Y0 < ret) { ret = entity.Y0; }
                        if (entity.Y1 < ret) { ret = entity.Y1; }
                    }
                }
            }

            return ret;
        }

        public double GetMaxX(string layerName)
        {
            double ret = -9999.9999;

            foreach (CBoundaryDXFLayer layer in Layers.Values)
            {
                if (layer.LayerName.ToUpper().Trim() == layerName.Trim().ToUpper())
                {
                    foreach (CBoundaryDXFEntity entity in layer.Entities)
                    {
                        if (entity.EntityType.Trim().ToUpper() == "LINE")
                        {
                            if (entity.X0 > ret) { ret = entity.X0; }
                            if (entity.X1 > ret) { ret = entity.X1; }
                        }
                    }
                }
            }

            return ret;
        }
        public double GetMaxY(string layerName)
        {
            double ret = -9999.9999;

            foreach (CBoundaryDXFLayer layer in Layers.Values)
            {
                if (layer.LayerName.Trim().ToUpper() == layerName.Trim().ToUpper())
                {
                    foreach (CBoundaryDXFEntity entity in layer.Entities)
                    {
                        if (entity.EntityType.Trim().ToUpper() == "LINE")
                        {
                            if (entity.Y0 > ret) { ret = entity.Y0; }
                            if (entity.Y1 > ret) { ret = entity.Y1; }
                        }
                    }
                }
            }

            return ret;
        }
        public double GetMinX(string layerName)
        {
            double ret = 9999.9999;

            foreach (CBoundaryDXFLayer layer in Layers.Values)
            {
                if (layer.LayerName.Trim().ToUpper() == layerName.Trim().ToUpper())
                {
                    foreach (CBoundaryDXFEntity entity in layer.Entities)
                    {
                        if (entity.EntityType.Trim().ToUpper() == "LINE")
                        {
                            if (entity.X0 < ret) { ret = entity.X0; }
                            if (entity.X1 < ret) { ret = entity.X1; }
                        }
                    }
                }
            }

            return ret;
        }
        public double GetMinY(string layerName)
        {
            double ret = 9999.9999;

            foreach (CBoundaryDXFLayer layer in Layers.Values)
            {
                if (layer.LayerName.Trim().ToUpper() == layerName.Trim().ToUpper())
                {
                    foreach (CBoundaryDXFEntity entity in layer.Entities)
                    {
                        if (entity.EntityType.Trim().ToUpper() == "LINE")
                        {
                            if (entity.Y0 < ret) { ret = entity.Y0; }
                            if (entity.Y1 < ret) { ret = entity.Y1; }
                        }
                    }
                }
            }

            return ret;
        }

        public void Rotate(double degrees)
        {
            m_DXF.Rotate(degrees);
        }

        public int HoleCount()
        {
            int ret;

            ret = 0;
            if (Layers.ContainsKey("TOOL"))
            {
                foreach (CBoundaryDXFEntity entity in ((CBoundaryDXFLayer)Layers["TOOL"]).Entities)
                {
                    if (
                        (entity.EntityType == "CIRCLE" ||
                         entity.EntityType == "ARC") &&
                        entity.Radius <= 1.25)
                    {
                        ret++;
                    }
                }
            }

            return ret;
        }

        public List<CBoundaryDXFEntity> GetHoles()
        {
            const double MAX_RADIUS_OF_HOLE = 1.25;

            if (this.Layers.ContainsKey("TOOL") == false) return new List<CBoundaryDXFEntity>();

            CBoundaryDXFLayer layer = (CBoundaryDXFLayer)this.Layers["TOOL"];

            List<CBoundaryDXFEntity> outputList = new List<CBoundaryDXFEntity>();

            foreach (CBoundaryDXFEntity entity in layer.Entities)
            {
                if ((entity.EntityType == "CIRCLE" || entity.EntityType == "ARC") &&
                    entity.Radius <= MAX_RADIUS_OF_HOLE)
                {
                    outputList.Add(entity);
                }
            }

            return outputList;
        }

        public int BendCount()
        {
            int ret;

            ret = 0;
            if (Layers.ContainsKey("BEND"))
            {
                foreach (CBoundaryDXFEntity entity in ((CBoundaryDXFLayer)Layers["BEND"]).Entities)
                {
                    if (entity.EntityType == "LINE")
                    {
                        ret++;
                    }
                }
            }
            if (Layers.ContainsKey("JOGGLE"))
            {
                foreach (CBoundaryDXFEntity entity in ((CBoundaryDXFLayer)Layers["JOGGLE"]).Entities)
                {
                    if (entity.EntityType == "LINE")
                    {
                        ret++;
                    }
                }
            }

            return ret;
        }
        public double LinearNibbling()
        {
            double ret;
            double dx;
            double dy;

            ret = 0;
            if (Layers.ContainsKey("TOOL"))
            {
                foreach (CBoundaryDXFEntity entity in ((CBoundaryDXFLayer)Layers["TOOL"]).Entities)
                {
                    if (entity.EntityType == "LINE")
                    {
                        dx = Math.Abs(entity.X0 - entity.X1);
                        dy = Math.Abs(entity.Y0 - entity.Y1);
                        ret += Math.Sqrt((dx * dx) + (dy * dy));
                    }
                }
            }

            //			if(Layers.ContainsKey("SHAPE"))
            //			{
            //				foreach(CBoundaryDXFEntity entity in ((CBoundaryDXFLayer) Layers["SHAPE"]).Entities)
            //				{
            //					if(entity.EntityType == "LINE")
            //					{
            //						dx = Math.Abs(entity.X0 - entity.X1);
            //						dy = Math.Abs(entity.Y0 - entity.Y1);
            //						ret += Math.Sqrt((dx * dx) + (dy * dy));
            //					}
            //				}
            //			}

            return ret;
        }
        public double RadialNibbling()
        {
            double ret;
            double nibin;
            double len;
            double pc;

            ret = 0.000;

            if (Layers.ContainsKey("TOOL"))
            {
                foreach (CBoundaryDXFEntity entity in ((CBoundaryDXFLayer)Layers["TOOL"]).Entities)
                {
                    if (entity.EntityType == "CIRCLE" && entity.Radius > 1.25)
                    {
                        nibin = (2 * entity.Radius * Math.PI);

                        ret += nibin;
                    }

                    if (entity.EntityType == "ARC" && entity.Radius > 1.25)
                    {
                        nibin = (2 * entity.Radius * Math.PI);
                        len = Math.Abs(RTD(entity.StartAngle) - RTD(entity.EndAngle));
                        pc = (len / 360);
                        nibin = nibin * pc;

                        ret += nibin;
                    }
                }
            }

            return ret;
        }
        public int NotchCount()
        {
            int ret = 0;
            if (Layers.ContainsKey("SHAPE"))
            {
                foreach (CBoundaryDXFEntity entity in ((CBoundaryDXFLayer)Layers["SHAPE"]).Entities)
                {
                    if (entity.EntityType == "LINE")
                    {
                        if (
                            entity.X0 == entity.X1 ||
                            entity.Y0 == entity.Y1)
                        {
                            ret++;
                        }
                    }
                }
            }

            if (ret < 4)
            {
                ret = 0;
            }
            else
            {
                ret = (ret - 4) / 2;
            }

            return ret;
        }
        public int HoleTypeCount()
        {
            Hashtable holes;
            int ret;

            holes = new Hashtable();
            ret = 0;
            if (Layers.ContainsKey("TOOL"))
            {
                foreach (CBoundaryDXFEntity entity in ((CBoundaryDXFLayer)Layers["TOOL"]).Entities)
                {
                    if (
                        (entity.EntityType == "CIRCLE" ||
                         entity.EntityType == "ARC") &&
                        entity.Radius < 1.25)
                    {
                        if (holes.ContainsValue(entity.Radius) == false)
                        {
                            ret++;
                            holes.Add("X" + ret.ToString().Trim(), entity.Radius);
                        }
                    }
                }
            }

            return ret;
        }

        #region Linear Measurements

        public double CircularCuts()
        {
            double ret;
            double cuts;

            ret = 0.0;

            if (Layers.ContainsKey("TOOL"))
            {
                foreach (CBoundaryDXFEntity entity in ((CBoundaryDXFLayer)Layers["TOOL"]).Entities)
                {
                    if (entity.EntityType == "CIRCLE")
                    {
                        cuts = (2 * entity.Radius * Math.PI);

                        ret += cuts;
                    }
                }
            }

            return ret;
        }
        public double ArchedCuts()
        {
            double ret;
            double cuts;
            double circ;
            double len;
            double pc;

            ret = 0.0;

            if (Layers.ContainsKey("TOOL"))
            {
                foreach (CBoundaryDXFEntity entity in ((CBoundaryDXFLayer)Layers["TOOL"]).Entities)
                {
                    if (entity.EntityType == "ARC")
                    {
                        circ = (2 * entity.Radius * Math.PI);
                        len = Math.Abs((entity.StartAngle) - (entity.EndAngle));
                        pc = (len / 360);
                        cuts = circ * pc;

                        ret += cuts;
                    }
                }
            }

            return ret;
        }
        public double LinearCuts()
        {
            double ret;
            double dx;
            double dy;

            ret = 0.0;

            if (Layers.ContainsKey("TOOL"))
            {
                foreach (CBoundaryDXFEntity entity in ((CBoundaryDXFLayer)Layers["TOOL"]).Entities)
                {
                    if (entity.EntityType == "LINE")
                    {
                        dx = Math.Abs(entity.X0 - entity.X1);
                        dy = Math.Abs(entity.Y0 - entity.Y1);
                        ret += Math.Sqrt((dx * dx) + (dy * dy));
                    }
                }
            }

            if (Layers.ContainsKey("SHAPE"))
            {
                foreach (CBoundaryDXFEntity entity in ((CBoundaryDXFLayer)Layers["SHAPE"]).Entities)
                {
                    if (entity.EntityType == "LINE")
                    {
                        dx = Math.Abs(entity.X0 - entity.X1);
                        dy = Math.Abs(entity.Y0 - entity.Y1);
                        ret += Math.Sqrt((dx * dx) + (dy * dy));
                    }
                }
            }

            return ret;
        }
        public double TotalCuts()
        {
            double ret;

            ret =
                CircularCuts() +
                ArchedCuts() +
                LinearCuts();

            return ret;
        }

        #endregion

        public double GetTextStartX()
        {
            double ret;

            ret = 9999.999;

            foreach (CBoundaryDXFLayer layer in Layers)
            {
                foreach (CBoundaryDXFEntity entity in layer.Entities)
                {
                    if (entity.EntityType.Trim().ToUpper() == "TEXT")
                    {
                        ret = entity.X0;
                        break;
                    }
                }
            }

            if (ret == 9999.99)
            {
                ret = GetMaxX() + 0.500;
            }

            return ret;
        }
        public void RenameLayer(string currentLayerName, string newLayerName)
        {
            foreach (CBoundaryDXFLayer layer in Layers.Values)
            {
                if (layer.LayerName.Trim().ToUpper() == currentLayerName.Trim().ToUpper())
                {
                    foreach (CBoundaryDXFEntity entity in layer.Entities)
                    {
                        entity.LayerName = newLayerName.Trim().ToUpper();
                    }
                    layer.LayerName = newLayerName.Trim().ToUpper();
                    break;
                }
            }

            m_DXF.LayerList.Add(newLayerName, m_DXF.LayerList[currentLayerName]);
            m_DXF.LayerList.Remove(currentLayerName);
        }

        #endregion

        #region Static Methods

        public static double RTD(double radians)
        {
            return 180 * (radians / Math.PI);
        }
        public static double DTR(double degrees)
        {
            return Math.PI * (degrees / 180);
        }

        public static bool InThreshold(double compare1, double compare2, double threshold)
        {
            if (Math.Abs(Math.Round(compare1, 5) - Math.Round(compare2, 5)) <= threshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool InThreshold(double compare1, double compare2)
        {
            return InThreshold(compare1, compare2, 0.0005);

            //			if(Math.Round(compare1,4) == Math.Round(compare2,4))
            //			{
            //				return true;
            //			}
            //			else
            //			{
            //				return false;
            //			}
        }

        #endregion
    }
    public class CBoundaryDXFLayer
    {
        #region Private Member Data

        private CBoundaryDXF m_DXF;
        private CTableLayer m_Layer;

        #endregion

        #region Constructors

        public CBoundaryDXFLayer(
            CBoundaryDXF dxf, 
            CTableLayer layer)
        {
            m_DXF = dxf;
            m_Layer = layer;
        }
        
        #endregion

        #region Public Properties

        public ArrayList Entities
        {
            get
            {
                ArrayList entities;

                entities = new ArrayList();

                foreach (CEntity entity in m_Layer.EntityList)
                {
                    entities.Add(new CBoundaryDXFEntity(m_DXF, entity));
                }

                return entities;
            }
        }
        public string LayerName
        {
            get { return m_Layer.LayerName; }
            set
            {
                m_Layer.LayerName = value;
            }
        }
        
        #endregion

        #region Other Methods

        public void AddEntity(CBoundaryDXFEntity srcEntity)
        {
            object entity;

            if (srcEntity.Entity.EntityName.Trim().ToUpper() == "LINE")
            {
                entity = ((CEntityLine)srcEntity.Entity).Clone();
                ((CEntity)entity).LayerName = m_Layer.LayerName;
            }
            else if (srcEntity.Entity.EntityName.Trim().ToUpper() == "POINT")
            {
                entity = ((CEntityPoint)srcEntity.Entity).Clone();
                ((CEntity)entity).LayerName = m_Layer.LayerName;
            }
            else if (srcEntity.Entity.EntityName.Trim().ToUpper() == "CIRCLE")
            {
                entity = ((CEntityCircle)srcEntity.Entity).Clone();
                ((CEntity)entity).LayerName = m_Layer.LayerName;
            }
            else if (srcEntity.Entity.EntityName.Trim().ToUpper() == "ARC")
            {
                entity = ((CEntityArc)srcEntity.Entity).Clone();
                ((CEntity)entity).LayerName = m_Layer.LayerName;
            }
            else if (srcEntity.Entity.EntityName.Trim().ToUpper() == "TRACE")
            {
                entity = ((CEntityTrace)srcEntity.Entity).Clone();
                ((CEntity)entity).LayerName = m_Layer.LayerName;
            }
            else if (srcEntity.Entity.EntityName.Trim().ToUpper() == "SOLID")
            {
                entity = ((CEntitySolid)srcEntity.Entity).Clone();
                ((CEntity)entity).LayerName = m_Layer.LayerName;
            }
            else if (srcEntity.Entity.EntityName.Trim().ToUpper() == "TEXT")
            {
                entity = ((CEntityText)srcEntity.Entity).Clone();
                ((CEntity)entity).LayerName = m_Layer.LayerName;
            }
            else if (srcEntity.Entity.EntityName.Trim().ToUpper() == "SHAPE")
            {
                entity = ((CEntityShape)srcEntity.Entity).Clone();
                ((CEntity)entity).LayerName = m_Layer.LayerName;
            }
            else if (srcEntity.Entity.EntityName.Trim().ToUpper() == "POLYLINE")
            {
                entity = ((CEntityPolyline)srcEntity.Entity).Clone();
                ((CEntity)entity).LayerName = m_Layer.LayerName;
            }
            else if (srcEntity.Entity.EntityName.Trim().ToUpper() == "3DFACE")
            {
                entity = ((CEntity3DFace)srcEntity.Entity).Clone();
                ((CEntity)entity).LayerName = m_Layer.LayerName;
            }
            else
            {
                entity = srcEntity.Entity.Clone();
                ((CEntity)entity).LayerName = m_Layer.LayerName;
            }

            ((CTableLayer)m_DXF.DXF.LayerList[m_Layer.LayerName.Trim().ToUpper()]).EntityList.Add(entity);
        }
        public bool ContainsEntity(CBoundaryDXFEntity srcEntity)
        {
            bool isFound = false;

            foreach (CBoundaryDXFEntity entity in Entities)
            {
                //if (String.Compare(entity.LayerName.Trim().ToUpper(), "LINE") == 0)
                //{
                //    if(entity.Orientation == srcEntity.Orientation &&
                //        entity.X0 == srcEntity.X0 && entity.X1 == srcEntity.X1 && entity.X2 == srcEntity.X2 && entity.X3 == srcEntity.X3 &&
                //        entity.X4 == srcEntity.X4 && entity.Y0 == srcEntity.Y0 && entity.Y1 == srcEntity.Y1 && entity.Y2 == srcEntity.Y2 &&
                //        entity.Y3 == srcEntity.Y3 && entity.Y4 == srcEntity.Y4 && entity.Z0 == srcEntity.Z0 && entity.Z1 == srcEntity.Z1 &&
                //        entity.Z2 == srcEntity.Z2 && entity.Z3 == srcEntity.Z3 && entity.Z4 == srcEntity.Z4)
                //    {
                //        isFound = true;
                //        break;
                //    }
                //}

                // Same Layer
                if (String.Compare(entity.LayerName.Trim().ToUpper(), srcEntity.LayerName.Trim().ToUpper()) == 0)
                {
                    // Same Handle
                    if (String.Compare(entity.Handle, srcEntity.Handle) == 0)
                    {
                        isFound = true;
                        break;
                    }
                }
            }

            return isFound;
        }
        
        #endregion
    }
    public class CBoundaryDXFEntity : IComparable
    {
        #region Private Member Data

        private CBoundaryDXF m_DXF;
        private CEntity m_Entity;
        private int m_Index;

        #endregion

        #region Constructors

        public CBoundaryDXFEntity(CBoundaryDXF dxf, CEntity entity)
        {
            m_DXF = dxf;
            m_Entity = entity;
        }
        
        #endregion

        #region Public Properties

        public int Index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }
        internal CEntity Entity
        {
            get { return m_Entity; }
            set { m_Entity = value; }
        }
        public string LayerName
        {
            get { return m_Entity.LayerName.ToUpper().Trim(); }
            set { m_Entity.LayerName = value; }
        }
        public string EntityType
        {
            get { return m_Entity.EntityName.ToUpper().Trim(); }
            set { m_Entity.EntityName = value; }
        }
        public string Handle
        {
            get { return m_Entity.Handle; }
        }

        public CBoundaryDXF DXF
        {
            get { return m_DXF; }
        }
        
        public double X0
        {
            get { return Math.Round(m_Entity.X0, 5); }
            set { m_Entity.X0 = Math.Round(value, 5); }
        }
        public double X1
        {
            get { return Math.Round(m_Entity.X1, 5); }
            set { m_Entity.X1 = Math.Round(value, 5); }
        }
        public double X2
        {
            get { return Math.Round(m_Entity.X2, 5); }
            set { m_Entity.X2 = Math.Round(value, 5); }
        }
        public double X3
        {
            get { return Math.Round(m_Entity.X3, 5); }
            set { m_Entity.X3 = Math.Round(value, 5); }
        }
        public double X4
        {
            get { return Math.Round(m_Entity.X4, 5); }
            set { m_Entity.X4 = Math.Round(value, 5); }
        }

        public double Y0
        {
            get { return Math.Round(m_Entity.Y0, 5); }
            set { m_Entity.Y0 = Math.Round(value, 5); }
        }
        public double Y1
        {
            get { return Math.Round(m_Entity.Y1, 5); }
            set { m_Entity.Y1 = Math.Round(value, 5); }
        }
        public double Y2
        {
            get { return Math.Round(m_Entity.Y2, 5); }
            set { m_Entity.Y2 = Math.Round(value, 5); }
        }
        public double Y3
        {
            get { return Math.Round(m_Entity.Y3, 5); }
            set { m_Entity.Y3 = Math.Round(value, 5); }
        }
        public double Y4
        {
            get { return Math.Round(m_Entity.Y4, 5); }
            set { m_Entity.Y4 = Math.Round(value, 5); }
        }

        public double Z0
        {
            get { return Math.Round(m_Entity.Z0, 5); }
            set { m_Entity.Z0 = Math.Round(value, 5); }
        }
        public double Z1
        {
            get { return Math.Round(m_Entity.Z1, 5); }
            set { m_Entity.Z1 = Math.Round(value, 5); }
        }
        public double Z2
        {
            get { return Math.Round(m_Entity.Z2, 5); }
            set { m_Entity.Z2 = Math.Round(value, 5); }
        }
        public double Z3
        {
            get { return Math.Round(m_Entity.Z3, 5); }
            set { m_Entity.Z3 = Math.Round(value, 5); }
        }
        public double Z4
        {
            get { return Math.Round(m_Entity.Z4, 5); }
            set { m_Entity.Z4 = Math.Round(value, 5); }
        }

        public double Radius
        {
            get { return Math.Round(m_Entity.Radius, 5); }
            set { m_Entity.Radius = Math.Round(value, 5); }
        }
        public double StartAngle
        {
            get { return m_Entity.StartAngle; }
            set { m_Entity.StartAngle = value; }
        }
        public double EndAngle
        {
            get { return m_Entity.EndAngle; }
            set { m_Entity.EndAngle = value; }
        }

        public string Orientation
        {
            get
            {
                if (EntityType.ToUpper().Trim() == "LINE")
                {
                    if (X0 == X1)
                    {
                        return "V";
                    }
                    else if (Y0 == Y1)
                    {
                        return "H";
                    }
                    else
                    {
                        return "D";
                    }
                }
                else
                {
                    return "X";
                }
            }
        }
        public double LineLength()
        {
            return Math.Sqrt(((X1 - X0) * (X1 - X0)) + ((Y1 - Y0) * (Y1 - Y0)));
        }

        #endregion

        #region IComparable Implementation

        public int CompareTo(Object rhs)
        {
            double thisx;
            double comparex;
            double thisy;
            double comparey;

            CBoundaryDXFEntity r = (CBoundaryDXFEntity)rhs;

            if (r.Orientation == this.Orientation)
            {
                if (Orientation == "H")
                {
                    if (X0 < X1)
                    {
                        thisx = X0;
                    }
                    else
                    {
                        thisx = X1;
                    }

                    if (r.X0 < r.X1)
                    {
                        comparex = r.X0;
                    }
                    else
                    {
                        comparex = r.X1;
                    }

                    return thisx.CompareTo(comparex);
                }
                else if (Orientation == "V")
                {
                    if (Y0 < Y1)
                    {
                        thisy = Y0;
                    }
                    else
                    {
                        thisy = Y1;
                    }

                    if (r.Y0 < r.Y1)
                    {
                        comparey = r.Y0;
                    }
                    else
                    {
                        comparey = r.Y1;
                    }

                    return thisy.CompareTo(comparey);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        #endregion

        public bool OverLaps(CBoundaryDXFEntity entity)
        {
            double boundsTop;
            double boundsBottom;
            double boundsLeft;
            double boundsRight;
            CBoundaryDXFEntity shortEntity;
            CBoundaryDXFEntity longEntity;

            if (entity.EntityType != this.EntityType)
            {
                return false;
            }
            else
            {
                if (entity.EntityType == "LINE")
                {
                    if (entity.Orientation == "D" && this.Orientation == "D")
                    {
                        if (entity.X0 == this.X0 && entity.Y0 == this.Y0 && entity.X1 == this.X1 && entity.Y1 == this.Y1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (entity.Orientation != "D" && entity.Orientation == this.Orientation)
                    {
                        if (this.Orientation == "H")
                        {
                            if (this.Y0 == entity.Y0)
                            {
                                if (Math.Abs(this.X0 - this.X1) > Math.Abs(entity.X0 - entity.X1))
                                {
                                    if (this.X0 > this.X1)
                                    {
                                        boundsRight = this.X0;
                                        boundsLeft = this.X1;
                                    }
                                    else
                                    {
                                        boundsRight = this.X1;
                                        boundsLeft = this.X0;
                                    }
                                    shortEntity = entity;
                                    longEntity = this;
                                }
                                else
                                {
                                    if (entity.X0 > entity.X1)
                                    {
                                        boundsRight = entity.X0;
                                        boundsLeft = entity.X1;
                                    }
                                    else
                                    {
                                        boundsRight = entity.X1;
                                        boundsLeft = entity.X0;
                                    }
                                    shortEntity = this;
                                    longEntity = entity;
                                }

                                if (longEntity.X0 == shortEntity.X0 || longEntity.X1 == shortEntity.X0)
                                {
                                    if (shortEntity.X1 <= boundsRight && shortEntity.X1 >= boundsLeft)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else if (longEntity.X0 == shortEntity.X1 || longEntity.X1 == shortEntity.X1)
                                {
                                    if (shortEntity.X0 <= boundsRight && shortEntity.X0 >= boundsLeft)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else
                                {

                                    if ((shortEntity.X0 <= boundsRight && shortEntity.X0 >= boundsLeft) ||
                                        (shortEntity.X1 <= boundsRight && shortEntity.X1 >= boundsLeft))
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (this.Orientation == "V")
                        {
                            if (this.X0 == entity.X0)
                            {
                                if (Math.Abs(this.Y0 - this.Y1) > Math.Abs(entity.Y0 - entity.Y1))
                                {
                                    if (this.Y0 > this.Y1)
                                    {
                                        boundsTop = this.Y0;
                                        boundsBottom = this.Y1;
                                    }
                                    else
                                    {
                                        boundsTop = this.Y1;
                                        boundsBottom = this.Y0;
                                    }
                                    shortEntity = entity;
                                    longEntity = this;
                                }
                                else
                                {
                                    if (entity.Y0 > entity.Y1)
                                    {
                                        boundsTop = entity.Y0;
                                        boundsBottom = entity.Y1;
                                    }
                                    else
                                    {
                                        boundsTop = entity.Y1;
                                        boundsBottom = entity.Y0;
                                    }
                                    shortEntity = this;
                                    longEntity = entity;
                                }

                                if (longEntity.Y0 == shortEntity.Y0 || longEntity.Y1 == shortEntity.Y0)
                                {
                                    if (shortEntity.Y1 <= boundsTop && shortEntity.Y1 >= boundsBottom)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else if (longEntity.Y0 == shortEntity.Y1 || longEntity.Y1 == shortEntity.Y1)
                                {
                                    if (shortEntity.Y0 <= boundsTop && shortEntity.Y0 >= boundsBottom)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else
                                {

                                    if ((shortEntity.Y0 <= boundsTop && shortEntity.Y0 >= boundsBottom) ||
                                        (shortEntity.Y1 <= boundsTop && shortEntity.Y1 >= boundsBottom))
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (entity.EntityType == "ARC")
                {
                    if (entity.X0 == this.X0 && entity.Y0 == this.Y0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (entity.EntityType == "SPLINE")
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    public class CBoundaryDXFControlPoint
    {
        #region Private Member Data

        private CEntityPoint m_ControlPoint;

        #endregion

        #region Constructors

        public CBoundaryDXFControlPoint(CEntityPoint controlPoint)
        {
            m_ControlPoint = controlPoint;
        }
        
        #endregion

        #region Public Properties

        public double X
        {
            get { return (double)Math.Round(m_ControlPoint.X0, 4); }
            set { m_ControlPoint.X0 = (float)Math.Round(value, 4); }
        }
        public double Y
        {
            get { return (double)Math.Round(m_ControlPoint.Y0, 4); }
            set { m_ControlPoint.Y0 = (float)Math.Round(value, 4); }
        }
        public double Z
        {
            get { return (double)Math.Round(m_ControlPoint.Z0, 4); }
            set { m_ControlPoint.Z0 = (float)Math.Round(value, 4); }
        }

        #endregion
    }
}

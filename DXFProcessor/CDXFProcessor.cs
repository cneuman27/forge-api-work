using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.IO;

using BoundaryDXFNative;

namespace DXFProcessor
{
    public class CDXFProcessor
    {
        private bool m_HasNonContinuousShape = false;
        private bool m_ShapeHealingDisabled = false;

        private double m_ShearTime;
        private double m_PunchTime;
        private double m_BrakeTime;
        private double m_RunTime;
        private double m_SquareFeet;
        private double m_Width;
        private double m_Height;
        private double m_LinearDistance;

        public void ProcessDXFFile(
            string src,
            string dest,
            string partNumber,
            string dxfMaterialCode,
            bool skipHeal,
            bool specialBeltGuardHandling,
            bool doNotProcessFeatureProfile,
            bool createJoggleLayer)
        {
            CBoundaryDXF dxfSource = null;
            CBoundaryDXF dxfTemp = null;
            CBoundaryDXF dxfTemp2 = null;
            CBoundaryDXF dxfDest = null;

            double maxX = 0.000;
            double maxY = 0.000;
            double minX = 0.000;

            double width = 0.000;
            double height = 0.000;

            double currentY = 0.000;

            string version;

            bool hasNonContinuousShape;
            bool hasRipRelief;

            dxfSource = new CBoundaryDXF(src);
            dxfTemp = new CBoundaryDXF();

            dxfTemp.CreateLayer("SHAPE");
            dxfTemp.CreateLayer("TOOL");
            dxfTemp.CreateLayer("BEND");

            dxfSource.CopyLayerTo("OUTER_PROFILE", "SHAPE", ref dxfTemp);
            dxfSource.CopyLayerTo("IV_OUTER_PROFILE", "SHAPE", ref dxfTemp);
            dxfSource.CopyLayerTo("INTERIOR_PROFILE", "TOOL", ref dxfTemp);
            dxfSource.CopyLayerTo("INTERIOR_PROFILES", "TOOL", ref dxfTemp);
            dxfSource.CopyLayerTo("IV_INTERIOR_PROFILES", "TOOL", ref dxfTemp);
            if (doNotProcessFeatureProfile == false) dxfSource.CopyLayerTo("FEATURE_PROFILE", "TOOL", ref dxfTemp);
            dxfSource.CopyLayerTo("TANGENT", "BEND", ref dxfTemp);
            dxfSource.CopyLayerTo("TANGENT_LINES", "BEND", ref dxfTemp);
            dxfSource.CopyLayerTo("IV_BEND", "BEND", ref dxfTemp);
            dxfSource.CopyLayerTo("IV_TANGENT", "BEND", ref dxfTemp);

            dxfTemp = RemoveVeryShortLines(dxfTemp);
            dxfTemp = RemoveDuplicateBendLines(dxfTemp);
            dxfTemp = RemoveInvalidToolEntities(dxfTemp);
            dxfTemp = RemoveToolEntitiesOverBendEntities(dxfTemp);
            dxfTemp = RemoveShapeEntitiesOverToolEntities(dxfTemp);
            dxfTemp = RemoveShapeEntitiesOverBendEntities(dxfTemp);

            dxfTemp.CreateLayer("RIP");

            // Copy all circles to tool layer - This is to get around certain situations where inventor puts holes on
            // the OUTER_PROFILE LAYER

            foreach (CBoundaryDXFEntity entity in ((CBoundaryDXFLayer)dxfTemp.Layers["SHAPE"]).Entities)
            {
                if (entity.EntityType.Trim().ToUpper() == "CIRCLE")
                {
                    entity.LayerName = "TOOL";
                }
            }

            hasNonContinuousShape = false;
            if (skipHeal == false)
            {
                dxfTemp2 = HealShape(dxfTemp);
                hasNonContinuousShape = m_HasNonContinuousShape;

                if (hasNonContinuousShape)
                {
                    dxfTemp2 = dxfTemp;
                    hasNonContinuousShape = false;
                }

                dxfTemp2 = MoveToolLinesToShape(dxfTemp2);
                dxfDest = RemoveEntitiesOutsideShape(dxfTemp2);
                //dxfDest = dxfTemp2;
            }
            else
            {
                dxfDest = dxfTemp;
            }

            // Rerun some dup checks to trap any healed shape lines.

            dxfDest = RemoveShapeEntitiesOverBendEntities(dxfDest);

            if (specialBeltGuardHandling) dxfDest = DoSpecialBeltGuardHandling(dxfDest);

            height = dxfDest.GetHeight();
            width = dxfDest.GetWidth();
            // If orientation is vertical - rotate 270 degrees
            if (height > width)
            {
                // Removed as per Kevin Stein
                // 07.30.2003
                // Readded as per Kevin Stein
                // 08.06.2003
                dxfDest.Rotate(270);
            }

            maxX = dxfDest.GetMaxX();
            maxY = dxfDest.GetMaxY();
            minX = dxfDest.GetMinX();

            dxfDest.DrawText(maxX + 0.5, maxY - 0.5, "RW: " + dxfMaterialCode, "TOOL");
            dxfDest.DrawText(maxX + 0.5, maxY - 1.5, "REV: -", "TOOL");
            dxfDest.DrawText(maxX + 0.5, maxY - 2.5, "PN: " + partNumber, "TOOL");

            hasRipRelief = FindRipRelief(dxfDest);

            currentY = maxY + 6.5;

            if (hasRipRelief || skipHeal || hasNonContinuousShape || (m_ShapeHealingDisabled && !skipHeal))
            {
                if (dxfDest.Layers.ContainsKey("ERRTEXT") == false)
                {
                    dxfDest.CreateLayer("ERRTEXT");
                }
            }

            dxfDest.CreateLayer("VERSION");

            try
            {
                version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Trim();
            }
            catch (Exception)
            {
                version = "UNKNOWN";
            }
            
            dxfDest.DrawText(minX, currentY, "RUN DATE: " + DateTime.Now.ToString().Trim(), "VERSION");
            currentY -= 1.0;

            dxfDest.DrawText(minX, currentY, "INVENTOR DXF PROCESSOR VERSION: " + version.Trim(), "VERSION");
            currentY -= 1.0;

            if (hasRipRelief)
            {
                dxfDest.DrawText(minX, currentY, "*** WARNING - HAS POSSIBLE RIP RELIEF ***", "ERRTEXT");
                currentY -= 1.0;
            }

            if (hasNonContinuousShape)
            {
                dxfDest.DrawText(minX, currentY, "*** WARNING - HAS POSSIBLE NON-CONTINUOUS SHAPE ***", "ERRTEXT");
                currentY -= 1.0;
            }

            if (skipHeal)
            {
                dxfDest.DrawText(minX, currentY, "*** WARNING - SHAPE HEALING DISABLED BY USER ***", "ERRTEXT");
                currentY -= 1.0;
            }

            if (m_ShapeHealingDisabled && !skipHeal)
            {
                dxfDest.DrawText(minX, currentY, "*** WARNING - SHAPE HEALING DISABLED - POSSIBLE NON-CONTINUOUS SHAPE DETECTED ***", "ERRTEXT");
                currentY -= 1.0;
            }

            if (createJoggleLayer)
            {
                dxfDest.RenameLayer("BEND", "JOGGLE");
            }

            dxfDest.SaveAs(dest);

            dxfDest.CleanUp();
            dxfDest = null;

            dxfSource.CleanUp();
            dxfSource = null;

            if (dxfTemp != null) dxfTemp.CleanUp();
            dxfTemp = null;

            if (dxfTemp2 != null) dxfTemp2.CleanUp();
            dxfTemp2 = null;
        }

        public void CalculatePartLaborAndRouting(
            string srcFile, 
            string destFile)
        {
            File.WriteAllBytes(
                destFile,
                CalculatePartLaborAndRouting(File.ReadAllBytes(srcFile)));
        }

        private byte[] CalculatePartLaborAndRouting(byte[] srcDXF)
        {
            CBoundaryDXF rtDXF = null;

            int holeCount;
            int holeTypeCount;
            int notchCount;
            double nibbleRadial;
            double nibbleLinear;
            int bendCount;
            double linearDistance;

            double startX;
            double startY;

            double writeWidth;
            double writeHeight;
            double writeSQFT;
            double writeBrakeTime;
            double writeShearTime;
            double writePunchTime;
            double writeRunTime;
            double writeLinearDistance;

            rtDXF = new CBoundaryDXF(srcDXF);

            m_Width = rtDXF.GetWidth();
            m_Height = rtDXF.GetHeight();

            holeCount = rtDXF.HoleCount();
            holeTypeCount = rtDXF.HoleTypeCount();
            notchCount = rtDXF.NotchCount();
            nibbleRadial = rtDXF.RadialNibbling();
            nibbleLinear = rtDXF.LinearNibbling();
            bendCount = rtDXF.BendCount();
            linearDistance = rtDXF.TotalCuts();

            m_LinearDistance = linearDistance;

            // 
            // Note
            // ----
            // All Runtimes are stored in seconds
            // Presentation is responsible for all conversions to appropriate units.
            //

            #region Shear Time

            if ((m_Width * m_Height) < 600)
            {
                m_ShearTime = 13;
            }
            else
            {
                if (m_Height < 10)
                {
                    m_ShearTime = 14;
                }
                else if (m_Height < 20)
                {
                    m_ShearTime = 20;
                }
                else if (m_Height < 40)
                {
                    m_ShearTime = 70;
                }
                else if (m_Height < 50)
                {
                    m_ShearTime = 90;
                }
                else
                {
                    m_ShearTime = 120;
                }
            }

            m_ShearTime += 50;

            #endregion

            #region Punch Time

            if (
                holeCount > 0 ||
                notchCount > 0 ||
                nibbleRadial > 0 ||
                nibbleLinear > 0)
            {
                if (m_Height < 30)
                {
                    m_PunchTime =
                        (m_Width * m_Height / 100) +
                        (holeTypeCount * 3) +
                        (holeCount * 1.4) +
                        (notchCount * 2.4) +
                        (nibbleRadial * 2.4) +
                        (nibbleLinear / 2.75 * 1.2);
                }
                else
                {
                    m_PunchTime =
                        (m_Width * m_Height / 100) +
                        (holeTypeCount * 3) +
                        (holeCount * 0.8) +
                        (notchCount * 2.4) +
                        (nibbleRadial * 2.4) +
                        (nibbleLinear / 2.75 * 1.2);
                }

                if (m_Width > 60)
                {
                    m_PunchTime += (holeTypeCount * 1.2) + 10.2;
                }

                m_PunchTime += 50;
            }
            else
            {
                m_PunchTime = 0;
            }

            #endregion

            #region Brake Time

            if (bendCount > 0)
            {
                m_BrakeTime =
                    (bendCount * 4) +
                    (((m_Width * m_Height) / 500) + 6) +
                    ((((m_Width * m_Height) / 650) + 2) * bendCount);

                if (m_Width * m_Height > 2500)
                {
                    m_BrakeTime *= 2;
                }
            }
            else
            {
                m_BrakeTime = 0;
            }

            #endregion

            m_RunTime = m_ShearTime + m_PunchTime + m_BrakeTime;
            m_SquareFeet = (m_Width * m_Height) / 144;

            #region Write Out Values to DXF

            startX = rtDXF.GetMaxX() + 0.5;
            startY = rtDXF.GetMaxY() - 3.5;

            writeWidth = Math.Round(m_Width, 3);
            writeHeight = Math.Round(m_Height, 3);
            writeSQFT = Math.Round(m_SquareFeet, 3);
            writeBrakeTime = Math.Round(m_BrakeTime / 60, 3);
            writeShearTime = Math.Round(m_ShearTime / 60, 3);
            writePunchTime = Math.Round(m_PunchTime / 60, 3);
            writeRunTime = Math.Round(m_RunTime / 3600, 3);
            writeLinearDistance = Math.Round(m_LinearDistance, 3);

            rtDXF.CreateLayer("RUNTIMES");

            rtDXF.DrawText(
                startX,
                startY,
                "WIDTH: " + writeWidth.ToString("#.000"),
                "RUNTIMES"
                );

            rtDXF.DrawText(
                startX,
                startY - 1,
                "HEIGHT: " + writeHeight.ToString("#.000"),
                "RUNTIMES"
                );

            rtDXF.DrawText(
                startX,
                startY - 2,
                "SQ FT: " + writeSQFT.ToString("#.000"),
                "RUNTIMES"
                );

            rtDXF.DrawText(
                startX,
                startY - 3,
                "SHEAR TIME: " + writeShearTime.ToString("#.000"),
                "RUNTIMES"
                );

            rtDXF.DrawText(
                startX,
                startY - 4,
                "PUNCH TIME: " + writePunchTime.ToString("#.000"),
                "RUNTIMES"
                );

            rtDXF.DrawText(
                startX,
                startY - 5,
                "BRAKE TIME: " + writeBrakeTime.ToString("#.000"),
                "RUNTIMES"
                );

            rtDXF.DrawText(
                startX,
                startY - 6,
                "RUN TIME (HRS): " + writeRunTime.ToString("#.000"),
                "RUNTIMES"
                );

            rtDXF.DrawText(
                startX,
                startY - 7,
                "LINEAR DISTANCE OF CUTS: " + writeLinearDistance.ToString("#.000"),
                "RUNTIMES"
                );

            #endregion

            return rtDXF.SaveAsBytes();
        }

        private CBoundaryDXF RemoveVeryShortLines(CBoundaryDXF dxf)
        {
            CBoundaryDXF newDXF;
            CBoundaryDXFLayer srcLayer;
            CBoundaryDXFLayer destLayer;

            newDXF = new CBoundaryDXF();

            foreach (string layerName in dxf.Layers.Keys)
            {
                newDXF.CreateLayer(layerName);

                srcLayer = dxf.Layers[layerName] as CBoundaryDXFLayer;
                destLayer = newDXF.Layers[layerName] as CBoundaryDXFLayer;

                foreach (CBoundaryDXFEntity entity in srcLayer.Entities)
                {
                    if (entity.EntityType == "LINE" &&
                        entity.LineLength() < 0.005) continue;

                    destLayer.AddEntity(entity);
                }
            }

            return newDXF;
        }
        private CBoundaryDXF RemoveDuplicateBendLines(CBoundaryDXF dxf)
        {
            CBoundaryDXF newDXF;
            CBoundaryDXFLayer srcLayer;
            CBoundaryDXFLayer destLayer;
            ArrayList srcEntities;
            ArrayList destEntities;
            bool foundOne;

            newDXF = new CBoundaryDXF();

            dxf.CopyLayerTo("TOOL", "TOOL", ref newDXF);
            dxf.CopyLayerTo("SHAPE", "SHAPE", ref newDXF);

            newDXF.CreateLayer("BEND");

            srcLayer = (CBoundaryDXFLayer)dxf.Layers["BEND"];
            destLayer = (CBoundaryDXFLayer)newDXF.Layers["BEND"];

            srcEntities = srcLayer.Entities;

            if (srcEntities.Count == 0) return newDXF;

            foreach (CBoundaryDXFEntity entity in srcEntities)
            {
                if (entity.EntityType.ToUpper().Trim() == "LINE")
                {
                    destEntities = destLayer.Entities;

                    foundOne = false;
                    foreach (CBoundaryDXFEntity tempEntity in destEntities)
                    {
                        if (
                            (CBoundaryDXF.InThreshold(tempEntity.X0, entity.X0, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.X1, entity.X1, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y0, entity.Y0, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y1, entity.Y1, 0.005)) ||
                            (CBoundaryDXF.InThreshold(tempEntity.X0, entity.X1, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.X1, entity.X0, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y0, entity.Y1, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y1, entity.Y0, 0.005)))
                        {
                            foundOne = true;
                            break;
                        }
                    }

                    if (!foundOne)
                    {
                        destLayer.AddEntity(entity);
                    }
                }
            }

            return newDXF;
        }
        private CBoundaryDXF RemoveInvalidToolEntities(CBoundaryDXF dxf)
        {
            CBoundaryDXF newDXF;
            CBoundaryDXF newDXF2;

            CBoundaryDXFLayer srcLayer;
            CBoundaryDXFLayer destLayer;
            ArrayList srcEntities;

            double xDist;
            double yDist;
            double len;
            ArrayList badToolLines;

            bool foundOne;

            badToolLines = new ArrayList();

            newDXF = new CBoundaryDXF();

            dxf.CopyLayerTo("BEND", "BEND", ref newDXF);
            dxf.CopyLayerTo("SHAPE", "SHAPE", ref newDXF);

            newDXF.CreateLayer("TOOL");

            srcLayer = (CBoundaryDXFLayer)dxf.Layers["TOOL"];
            destLayer = (CBoundaryDXFLayer)newDXF.Layers["TOOL"];

            srcEntities = srcLayer.Entities;

            if (srcEntities.Count == 0) return newDXF;

            foreach (CBoundaryDXFEntity entity in srcEntities)
            {
                if (entity.EntityType.ToUpper().Trim() == "LINE")
                {
                    xDist = Math.Abs(entity.X0 - entity.X1);
                    yDist = Math.Abs(entity.Y0 - entity.Y1);

                    len = Math.Sqrt((xDist * xDist) + (yDist * yDist));

                    if (len > 0.0500)
                    {
                        destLayer.AddEntity(entity);
                    }
                    else
                    {
                        badToolLines.Add(entity);
                    }
                }
                else if (entity.EntityType.ToUpper().Trim() == "CIRCLE")
                {
                    if (entity.Radius > 0.0100)
                    {
                        destLayer.AddEntity(entity);
                    }
                }
                else if (entity.EntityType.ToUpper().Trim() == "ARC")
                {
                    if (entity.Radius > 0.0100)
                    {
                        destLayer.AddEntity(entity);
                    }
                }
                else
                {
                    destLayer.AddEntity(entity);
                }
            }

            if (badToolLines.Count > 0)
            {
                /*
				 * CAN
				 * 05.02.03
				 * 
				 * OK... The small (< 0.05") lines on the tool layer that we were trying to cleanup here...
				 * Well... that's half the story - these lines makeup a small box - half the lines on TOOL/half the lines on SHAPE...
				 * We've got the TOOL lines - now let's clobber the SHAPE lines.... * sigh *
				 * 
				 */

                newDXF2 = new CBoundaryDXF();

                newDXF.CopyLayerTo("BEND", "BEND", ref newDXF2);
                newDXF.CopyLayerTo("TOOL", "TOOL", ref newDXF2);

                srcLayer = (CBoundaryDXFLayer)newDXF.Layers["SHAPE"];
                srcEntities = srcLayer.Entities;

                newDXF2.CreateLayer("SHAPE");

                destLayer = (CBoundaryDXFLayer)newDXF2.Layers["SHAPE"];

                foreach (CBoundaryDXFEntity entity in srcEntities)
                {
                    if (entity.EntityType.ToUpper().Trim() == "LINE")
                    {
                        foundOne = false;
                        foreach (CBoundaryDXFEntity tempEntity in badToolLines)
                        {
                            if (ShareEndPoint(tempEntity, entity))
                            {
                                foundOne = true;
                                break;
                            }
                        }

                        if (!foundOne)
                        {
                            destLayer.AddEntity(entity);
                        }
                    }
                    else
                    {
                        destLayer.AddEntity(entity);
                    }
                }

                return newDXF2;
            }
            else
            {
                return newDXF;
            }
        }
        private CBoundaryDXF RemoveShapeEntitiesOverBendEntities(CBoundaryDXF dxf)
        {
            CBoundaryDXF newDXF;
            CBoundaryDXFLayer srcLayer;
            CBoundaryDXFLayer destLayer;
            CBoundaryDXFLayer bendLayer;
            ArrayList srcEntities;
            ArrayList destEntities;
            ArrayList bendEntities;

            bool foundOne;

            newDXF = new CBoundaryDXF();

            dxf.CopyLayerTo("TOOL", "TOOL", ref newDXF);
            dxf.CopyLayerTo("BEND", "BEND", ref newDXF);

            newDXF.CreateLayer("SHAPE");

            srcLayer = (CBoundaryDXFLayer)dxf.Layers["SHAPE"];
            destLayer = (CBoundaryDXFLayer)newDXF.Layers["SHAPE"];
            bendLayer = (CBoundaryDXFLayer)dxf.Layers["BEND"];

            srcEntities = srcLayer.Entities;
            bendEntities = bendLayer.Entities;
            destEntities = destLayer.Entities;

            if (srcEntities.Count == 0) return newDXF;

            foreach (CBoundaryDXFEntity entity in srcEntities)
            {
                if (entity.EntityType.ToUpper().Trim() == "LINE")
                {
                    foundOne = false;
                    foreach (CBoundaryDXFEntity tempEntity in bendEntities)
                    {
                        if (
                            (CBoundaryDXF.InThreshold(tempEntity.X0, entity.X0, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.X1, entity.X1, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y0, entity.Y0, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y1, entity.Y1, 0.005)) ||
                            (CBoundaryDXF.InThreshold(tempEntity.X0, entity.X1, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.X1, entity.X0, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y0, entity.Y1, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y1, entity.Y0, 0.005)))
                        {
                            foundOne = true;
                            break;
                        }
                    }

                    if (!foundOne)
                    {
                        destLayer.AddEntity(entity);
                    }
                }
                else
                {
                    destLayer.AddEntity(entity);
                }
            }

            return newDXF;
        }
        private CBoundaryDXF RemoveShapeEntitiesOverToolEntities(CBoundaryDXF dxf)
        {
            CBoundaryDXF newDXF;
            CBoundaryDXFLayer srcLayer;
            CBoundaryDXFLayer destLayer;
            CBoundaryDXFLayer toolLayer;
            ArrayList srcEntities;
            ArrayList destEntities;
            ArrayList toolEntities;

            bool foundOne;

            newDXF = new CBoundaryDXF();

            dxf.CopyLayerTo("TOOL", "TOOL", ref newDXF);
            dxf.CopyLayerTo("BEND", "BEND", ref newDXF);

            newDXF.CreateLayer("SHAPE");

            srcLayer = (CBoundaryDXFLayer)dxf.Layers["SHAPE"];
            destLayer = (CBoundaryDXFLayer)newDXF.Layers["SHAPE"];
            toolLayer = (CBoundaryDXFLayer)dxf.Layers["TOOL"];

            srcEntities = srcLayer.Entities;
            toolEntities = toolLayer.Entities;
            destEntities = destLayer.Entities;

            if (srcEntities.Count == 0) return newDXF;

            foreach (CBoundaryDXFEntity entity in srcEntities)
            {
                if (entity.EntityType.ToUpper().Trim() == "LINE")
                {
                    foundOne = false;
                    foreach (CBoundaryDXFEntity tempEntity in toolEntities)
                    {
                        if (
                            (CBoundaryDXF.InThreshold(tempEntity.X0, entity.X0, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.X1, entity.X1, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y0, entity.Y0, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y1, entity.Y1, 0.005)) ||
                            (CBoundaryDXF.InThreshold(tempEntity.X0, entity.X1, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.X1, entity.X0, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y0, entity.Y1, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y1, entity.Y0, 0.005)))
                        {
                            foundOne = true;
                            break;
                        }
                    }

                    if (!foundOne)
                    {
                        destLayer.AddEntity(entity);
                    }
                }
                else
                {
                    destLayer.AddEntity(entity);
                }
            }

            return newDXF;
        }
        private CBoundaryDXF RemoveToolEntitiesOverBendEntities(CBoundaryDXF dxf)
        {
            CBoundaryDXF newDXF;
            CBoundaryDXFLayer srcLayer;
            CBoundaryDXFLayer destLayer;
            CBoundaryDXFLayer bendLayer;
            ArrayList srcEntities;
            ArrayList destEntities;
            ArrayList bendEntities;

            bool foundOne;

            newDXF = new CBoundaryDXF();

            dxf.CopyLayerTo("SHAPE", "SHAPE", ref newDXF);
            dxf.CopyLayerTo("BEND", "BEND", ref newDXF);

            newDXF.CreateLayer("TOOL");

            srcLayer = (CBoundaryDXFLayer)dxf.Layers["TOOL"];
            destLayer = (CBoundaryDXFLayer)newDXF.Layers["TOOL"];
            bendLayer = (CBoundaryDXFLayer)dxf.Layers["BEND"];

            srcEntities = srcLayer.Entities;
            bendEntities = bendLayer.Entities;
            destEntities = destLayer.Entities;

            if (srcEntities.Count == 0) return newDXF;

            foreach (CBoundaryDXFEntity entity in srcEntities)
            {
                if (entity.EntityType.ToUpper().Trim() == "LINE")
                {
                    foundOne = false;
                    foreach (CBoundaryDXFEntity tempEntity in bendEntities)
                    {
                        if (
                            (CBoundaryDXF.InThreshold(tempEntity.X0, entity.X0, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.X1, entity.X1, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y0, entity.Y0, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y1, entity.Y1, 0.005)) ||
                            (CBoundaryDXF.InThreshold(tempEntity.X0, entity.X1, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.X1, entity.X0, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y0, entity.Y1, 0.005) &&
                             CBoundaryDXF.InThreshold(tempEntity.Y1, entity.Y0, 0.005)))
                        {
                            foundOne = true;
                            break;
                        }
                    }

                    if (!foundOne)
                    {
                        destLayer.AddEntity(entity);
                    }
                }
                else
                {
                    destLayer.AddEntity(entity);
                }
            }

            return newDXF;
        }
        private CBoundaryDXF MoveToolLinesToShape(CBoundaryDXF dxf)
        {
            CBoundaryDXF newDXF;

            CBoundaryDXFLayer srcLayer;
            CBoundaryDXFLayer destLayer;
            ArrayList srcEntities;
            CBoundaryDXFLayer shapeLayer;
            ArrayList shapeEntities;

            ArrayList badToolLines;

            bool foundOne;

            badToolLines = new ArrayList();

            newDXF = new CBoundaryDXF();

            dxf.CopyLayerTo("BEND", "BEND", ref newDXF);
            dxf.CopyLayerTo("SHAPE", "SHAPE", ref newDXF);

            newDXF.CreateLayer("TOOL");

            srcLayer = (CBoundaryDXFLayer)dxf.Layers["TOOL"];
            destLayer = (CBoundaryDXFLayer)newDXF.Layers["TOOL"];

            if (srcLayer == null || destLayer == null)
            {
                return newDXF;
            }

            shapeLayer = (CBoundaryDXFLayer)newDXF.Layers["SHAPE"];
            shapeEntities = ((CBoundaryDXFLayer)dxf.Layers["SHAPE"]).Entities;


            srcEntities = srcLayer.Entities;

            if (srcEntities.Count == 0) return newDXF;

            foreach (CBoundaryDXFEntity entity in srcEntities)
            {
                if (entity.EntityType.ToUpper().Trim() == "LINE")
                {
                    foundOne = false;
                    foreach (CBoundaryDXFEntity tempEntity in shapeEntities)
                    {
                        if (ShareEndPoint(entity, tempEntity))
                        {
                            foundOne = true;
                            break;
                        }
                    }

                    if (foundOne)
                    {
                        shapeLayer.AddEntity(entity);
                    }
                    else
                    {
                        destLayer.AddEntity(entity);
                    }

                }
                else
                {
                    destLayer.AddEntity(entity);
                }
            }

            return newDXF;
        }
        private CBoundaryDXF RemoveEntitiesOutsideShape(CBoundaryDXF dxf)
        {
            double maxX;
            double maxY;
            double minX;
            double minY;

            CBoundaryDXF newDXF;

            newDXF = new CBoundaryDXF();

            maxX = dxf.GetMaxX("SHAPE");
            maxY = dxf.GetMaxY("SHAPE");
            minX = dxf.GetMinX("SHAPE");
            minY = dxf.GetMinY("SHAPE");

            dxf.CopyLayerTo("SHAPE", "SHAPE", ref newDXF);

            newDXF.CreateLayer("TOOL");
            newDXF.CreateLayer("BEND");

            if (dxf.Layers["TOOL"] == null)
            {
                dxf.CreateLayer("TOOL");
            }

            if (dxf.Layers["BEND"] == null)
            {
                dxf.CreateLayer("BEND");
            }

            foreach (CBoundaryDXFEntity entity in ((CBoundaryDXFLayer)dxf.Layers["TOOL"]).Entities)
            {
                if (entity.EntityType != "POINT")
                {
                    if (entity.EntityType == "LINE")
                    {
                        if (entity.X0 <= maxX && entity.X1 <= maxX &&
                            entity.X0 >= minX && entity.X1 >= minX &&
                            entity.Y0 <= maxY && entity.Y1 <= maxY &&
                            entity.Y0 >= minY && entity.Y1 >= minY)
                        {
                            ((CBoundaryDXFLayer)newDXF.Layers["TOOL"]).AddEntity(entity);
                        }
                    }
                    else
                    {
                        // For Now Just Add It - Eventually there should be processing for other entity types

                        ((CBoundaryDXFLayer)newDXF.Layers["TOOL"]).AddEntity(entity);
                    }
                }
            }

            foreach (CBoundaryDXFEntity entity in ((CBoundaryDXFLayer)dxf.Layers["BEND"]).Entities)
            {
                if (entity.EntityType != "POINT")
                {
                    if (entity.EntityType == "LINE")
                    {
                        if (entity.X0 <= (maxX + 0.5) && entity.X1 <= (maxX + 0.5) &&
                            entity.X0 >= (minX - 0.5) && entity.X1 >= (minX - 0.5) &&
                            entity.Y0 <= (maxY + 0.5) && entity.Y1 <= (maxY + 0.5) &&
                            entity.Y0 >= (minY - 0.5) && entity.Y1 >= (minY - 0.5))
                        {
                            ((CBoundaryDXFLayer)newDXF.Layers["BEND"]).AddEntity(entity);
                        }
                    }
                    else
                    {
                        // For Now Just Add It - Eventually there should be processing for other entity types

                        ((CBoundaryDXFLayer)newDXF.Layers["BEND"]).AddEntity(entity);
                    }
                }
            }

            return newDXF;
        }
        private CBoundaryDXF DoSpecialBeltGuardHandling(CBoundaryDXF dxf)
        {
            CBoundaryDXF newDXF;

            CBoundaryDXFLayer srcLayer;
            CBoundaryDXFLayer destLayer;
            ArrayList srcEntities;

            newDXF = new CBoundaryDXF();

            dxf.CopyLayerTo("BEND", "BEND", ref newDXF);
            dxf.CopyLayerTo("SHAPE", "SHAPE", ref newDXF);

            newDXF.CreateLayer("TOOL");

            srcLayer = (CBoundaryDXFLayer)dxf.Layers["TOOL"];
            destLayer = (CBoundaryDXFLayer)newDXF.Layers["TOOL"];

            if (srcLayer == null || destLayer == null)
            {
                return newDXF;
            }

            srcEntities = srcLayer.Entities;

            if (srcEntities.Count == 0) return newDXF;

            foreach (CBoundaryDXFEntity entity in srcEntities)
            {
                if (entity.EntityType.ToUpper().Trim() == "CIRCLE")
                {
                    destLayer.AddEntity(entity);
                }
            }

            return newDXF;
        }

        private bool FindRipRelief(CBoundaryDXF dxf)
        {
            bool foundOne;

            double len;

            if (dxf.Layers["SHAPE"] == null)
            {
                dxf.CreateLayer("SHAPE");
            }

            foundOne = false;
            foreach (CBoundaryDXFEntity entity in ((CBoundaryDXFLayer)dxf.Layers["SHAPE"]).Entities)
            {
                if (entity.EntityType == "LINE")
                {
                    if (entity.Y0 == entity.Y1)
                    {
                        if (Math.Abs(entity.X0 - entity.X1) <= 0.002)
                        {
                            if (dxf.Layers.ContainsKey("ERROR_RIP_RELIEF") == false)
                            {
                                dxf.CreateLayer("ERROR_RIP_RELIEF");
                            }

							dxf.DrawCircle(entity.X0 + ((entity.X1 - entity.X0) / 2), entity.Y0, 4.000, "ERROR_RIP_RELIEF");

                            foundOne = true;
                        }
                    }
                    else if (entity.X0 == entity.X1)
                    {
                        if (Math.Abs(entity.Y0 - entity.Y1) <= 0.002)
                        {
                            if (dxf.Layers.ContainsKey("ERROR_RIP_RELIEF") == false)
                            {
                                dxf.CreateLayer("ERROR_RIP_RELIEF");
                            }

							dxf.DrawCircle(entity.X0, entity.Y0 + ((entity.Y1 - entity.Y0) / 2), 4.000, "ERROR_RIP_RELIEF");

                            foundOne = true;
                        }
                    }
                    else
                    {
                        len = Math.Sqrt((Math.Abs(entity.Y0 - entity.Y1) * Math.Abs(entity.Y0 - entity.Y1)) + (Math.Abs(entity.X0 - entity.X1) * Math.Abs(entity.X0 - entity.X1)));
                        if (len <= 0.002)
                        {
                            if (dxf.Layers.ContainsKey("ERROR_RIP_RELIEF") == false)
                            {
                                dxf.CreateLayer("ERROR_RIP_RELIEF");
                            }

                            dxf.DrawCircle(entity.X0 + ((entity.X1 - entity.X0) / 2), entity.Y0 + ((entity.Y1 - entity.Y0) / 2), 4.000, "ERROR_RIP_RELIEF");

                            foundOne = true;
                        }
                    }
                }
            }

            return foundOne;
        }
        private CBoundaryDXF HealShape(CBoundaryDXF dxf)
        {
            CBoundaryDXF newDXF;
            CBoundaryDXFLayer srcLayer;
            CBoundaryDXFLayer destLayer;
            ArrayList srcEntities;
            CBoundaryDXFEntity startEntity;
            CBoundaryDXFEntity entity;
            CBoundaryDXFEntity nextEntity;
            PointF endPoint;
            PointF[] points;
            bool finished;
            PointF finalPoint;
            int iteration;
            int maxIteration;
            CBoundaryDXFEntity tempEntity;

            newDXF = new CBoundaryDXF();

            newDXF.CreateLayer("SHAPE");

            srcLayer = (CBoundaryDXFLayer)dxf.Layers["SHAPE"];
            destLayer = (CBoundaryDXFLayer)newDXF.Layers["SHAPE"];
            srcEntities = srcLayer.Entities;

            if (srcEntities.Count == 0) return newDXF;

            // Find Line Furtherest To Left

            tempEntity = null;
            foreach (CBoundaryDXFEntity myEntity in srcEntities)
            {
                if (myEntity.EntityType == "LINE")
                {
                    if (tempEntity == null || myEntity.X0 < tempEntity.X0)
                    {
                        tempEntity = myEntity;
                    }
                }
            }

            if (tempEntity == null)
            {
                throw new Exception("Invalid DXF Layer - No LINE EntityTypes Found!");
            }

            startEntity = GetLineSource(tempEntity);
            points = GetEndPoints(startEntity);

            if (startEntity.Orientation == "V")
            {
                endPoint = GetTopPoint(points);
                finalPoint = GetBottomPoint(points);
            }
            else
            {
                endPoint = GetRightPoint(points);
                finalPoint = GetLeftPoint(points);
            }

            finished = false;
            entity = startEntity;
            iteration = 0;
            maxIteration = ((CBoundaryDXFLayer)dxf.Layers["SHAPE"]).Entities.Count * 2;
            while (!finished)
            {
                iteration++;
                nextEntity = GetEntityAtPoint(endPoint.X, endPoint.Y, entity, dxf, "SHAPE");

                if (nextEntity == null)
                {
                    // Look For It On TOOL
                    // Inventor Sometimes Puts OUTER_PROFILE Entities on the INTERIOR_PROFILE Layer

                    nextEntity = GetEntityAtPoint(endPoint.X, endPoint.Y, entity, dxf, "TOOL");
                }

                if (nextEntity == null || iteration > maxIteration)
                {
                    m_HasNonContinuousShape = true;
                    finished = true;
                    break;
                }

                if (entity.EntityType == "LINE" && nextEntity.Orientation == entity.Orientation && entity.Orientation != "D")
                {
                    points = GetEndPoints(nextEntity);

                    if (CBoundaryDXF.InThreshold(entity.X0, endPoint.X) && CBoundaryDXF.InThreshold(entity.Y0, endPoint.Y))
                    {
                        endPoint = GetOtherEndPoint(points, endPoint);
                        entity.X0 = endPoint.X;
                        entity.Y0 = endPoint.Y;
                    }
                    else if (CBoundaryDXF.InThreshold(entity.X1, endPoint.X) && CBoundaryDXF.InThreshold(entity.Y1, endPoint.Y))
                    {
                        endPoint = GetOtherEndPoint(points, endPoint);
                        entity.X1 = endPoint.X;
                        entity.Y1 = endPoint.Y;
                    }
                    else
                    {
                        m_HasNonContinuousShape = true;
                        finished = true;
                        break;
                    }
                }
                else
                {
                    if (((CBoundaryDXFLayer)newDXF.Layers["SHAPE"]).ContainsEntity(entity))
                    {
                        // New shape layer already has this line entity.  We must be back-tracking...
                        m_HasNonContinuousShape = true;
                        finished = true;
                        break;
                    }
                    else
                    {
                        ((CBoundaryDXFLayer)newDXF.Layers["SHAPE"]).AddEntity(entity);
                    }

                    entity = nextEntity;
                    points = GetEndPoints(entity);

                    endPoint = GetOtherEndPoint(points, endPoint);
                    if (endPoint == PointF.Empty)
                    {
                        endPoint = GetOtherEndPoint(points, endPoint, 0.002);
                        if (endPoint == PointF.Empty)
                        {
                            m_HasNonContinuousShape = true;
                            finished = true;
                            break;
                        }
                    }

                    if (endPoint == finalPoint)
                    {
                        if (((CBoundaryDXFLayer)newDXF.Layers["SHAPE"]).ContainsEntity(entity))
                        {
                            // New shape layer already has this line entity.  We must be back-tracking...
                            m_HasNonContinuousShape = true;
                        }
                        else
                        {
                            ((CBoundaryDXFLayer)newDXF.Layers["SHAPE"]).AddEntity(entity);
                        }

                        finished = true;
                        break;
                    }
                }
            }
            
            // 2012.10.23 
            // CAN
            // ----------
            // Changed what DXF processor sends back if a "Non-Continuous" shape is found.
            // Before it would send back the DXF that was in process (may contain real gaps).
            // Will now send back original shape layer - hope to limit real gaps...
            //
            
            if (m_HasNonContinuousShape)
            {
                newDXF = new CBoundaryDXF();

                dxf.CopyLayerTo("SHAPE", "SHAPE", ref newDXF);
                dxf.CopyLayerTo("TOOL", "TOOL", ref newDXF);
                dxf.CopyLayerTo("BEND", "BEND", ref newDXF);
            }
            else
            {
                dxf.CopyLayerTo("TOOL", "TOOL", ref newDXF);
                dxf.CopyLayerTo("BEND", "BEND", ref newDXF);
            }

            return newDXF;
        }
        
        private bool ShareEndPoint(
            CBoundaryDXFEntity e1, 
            CBoundaryDXFEntity e2)
        {
            if ((e1.X0 == e2.X0 &&
                e1.Y0 == e2.Y0) ||
                (e1.X1 == e2.X1 &&
                e1.Y1 == e2.Y1) ||
                (e1.X1 == e2.X0 &&
                e1.Y1 == e2.Y0) ||
                (e1.X0 == e2.X1 &&
                e1.Y0 == e2.Y1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private CBoundaryDXFEntity GetLineSource(CBoundaryDXFEntity line)
        {
            double x;
            double y;
            CBoundaryDXFEntity newLine;
            bool finished = false;

            if (line.EntityType != "LINE") return line;

            if (line.Orientation == "H")
            {
                while (finished == false)
                {
                    y = line.Y0;
                    if (line.X0 < line.X1)
                    {
                        x = line.X0;
                    }
                    else
                    {
                        x = line.X1;
                    }
                    newLine = GetEntityAtPoint(x, y, line, line.DXF, line.LayerName);

                    if (newLine == null)
                    {
                        finished = true;
                    }
                    else
                    {
                        if (newLine.EntityType != "LINE" || newLine.Orientation != "H")
                        {
                            finished = true;
                        }
                        else
                        {
                            line = newLine;
                        }
                    }
                }

                return line;
            }
            else if (line.Orientation == "V")
            {
                while (finished == false)
                {
                    x = line.X0;
                    if (line.Y0 < line.Y1)
                    {
                        y = line.Y0;
                    }
                    else
                    {
                        y = line.Y1;
                    }

                    newLine = GetEntityAtPoint(x, y, line, line.DXF, line.LayerName);

                    if (newLine == null)
                    {
                        finished = true;
                    }
                    else
                    {
                        if (newLine.EntityType != "LINE" || newLine.Orientation != "V")
                        {
                            finished = true;
                        }
                        else
                        {
                            line = newLine;
                        }
                    }
                }

                return line;
            }
            else
            {
                return line;
            }
        }

        private CBoundaryDXFEntity GetEntityAtPoint(
            double x, 
            double y, 
            CBoundaryDXFEntity ignoreEntity, 
            CBoundaryDXF dxf, 
            string layerName)
        {
            CBoundaryDXFLayer layer;
            double x0;
            double x1;
            double y0;
            double y1;

            x = Math.Round(x, 4);
            y = Math.Round(y, 4);

            layer = (CBoundaryDXFLayer)dxf.Layers[layerName];

            foreach (CBoundaryDXFEntity entity in layer.Entities)
            {
                if (entity.EntityType == "LINE")
                {
                    if (ignoreEntity.EntityType == "SPLINE")
                    {
                        if (((CBoundaryDXF.InThreshold(entity.X0, x, 0.002) && CBoundaryDXF.InThreshold(entity.Y0, y, 0.002)) ||
                            (CBoundaryDXF.InThreshold(entity.X1, x, 0.002) && CBoundaryDXF.InThreshold(entity.Y1, y, 0.002))))
                        {
                            if (!(CBoundaryDXF.InThreshold(entity.X0, entity.X1) && CBoundaryDXF.InThreshold(entity.Y0, entity.Y1)))
                            {
                                if (entity.OverLaps(ignoreEntity) == false) return entity;
                            }
                        }
                    }
                    else if (ignoreEntity.EntityType == "LINE" && ignoreEntity.Orientation == "D" && entity.Orientation == "D")
                    {
                        if (((CBoundaryDXF.InThreshold(entity.X0, x, 0.002) && CBoundaryDXF.InThreshold(entity.Y0, y, 0.002)) ||
                            (CBoundaryDXF.InThreshold(entity.X1, x, 0.002) && CBoundaryDXF.InThreshold(entity.Y1, y, 0.002))))
                        {
                            if (!(CBoundaryDXF.InThreshold(entity.X0, entity.X1, 0.0002) && CBoundaryDXF.InThreshold(entity.Y0, entity.Y1)))
                            {
                                if (entity.OverLaps(ignoreEntity) == false) return entity;
                            }
                        }
                    }
                    else
                    {
                        if ((entity.X0 == x && entity.Y0 == y) ||
                            (entity.X1 == x && entity.Y1 == y))
                        {
                            if (!(entity.X0 == entity.X1 && entity.Y0 == entity.Y1))
                            {
                                if (entity.OverLaps(ignoreEntity) == false) return entity;
                            }
                        }

                    }
                }
                else if (entity.EntityType == "ARC")
                {
                    x0 = (Math.Cos(CBoundaryDXF.DTR(entity.StartAngle)) * entity.Radius) + entity.X0;
                    y0 = (Math.Sin(CBoundaryDXF.DTR(entity.StartAngle)) * entity.Radius) + entity.Y0;
                    x1 = (Math.Cos(CBoundaryDXF.DTR(entity.EndAngle)) * entity.Radius) + entity.X0;
                    y1 = (Math.Sin(CBoundaryDXF.DTR(entity.EndAngle)) * entity.Radius) + entity.Y0;

                    x0 = Math.Round(x0, 4);
                    y0 = Math.Round(y0, 4);
                    x1 = Math.Round(x1, 4);
                    y1 = Math.Round(y1, 4);

                    if (((CBoundaryDXF.InThreshold(x0, x) && CBoundaryDXF.InThreshold(y0, y)) ||
                        (CBoundaryDXF.InThreshold(x1, x) && CBoundaryDXF.InThreshold(y1, y))))
                    {
                        if (entity.OverLaps(ignoreEntity) == false) return entity;
                    }
                }
            }

            return null;
        }

        private PointF GetOtherEndPoint(
            PointF[] points, 
            PointF endPoint)
        {
            if (points[0].X == endPoint.X && points[0].Y == endPoint.Y)
            {
                return points[1];
            }
            else if (points[1].X == endPoint.X && points[1].Y == endPoint.Y)
            {
                return points[0];
            }
            else
            {
                return PointF.Empty;
            }
        }

        private PointF GetOtherEndPoint(
            PointF[] points, 
            PointF endPoint, 
            double tolerance)
        {
            if (CBoundaryDXF.InThreshold(points[0].X, endPoint.X, tolerance) &&
                CBoundaryDXF.InThreshold(points[0].Y, endPoint.Y, tolerance))
            {
                return points[1];
            }
            else if (
                CBoundaryDXF.InThreshold(points[1].X, endPoint.X, tolerance) && 
                CBoundaryDXF.InThreshold(points[1].Y, endPoint.Y, tolerance))
            {
                return points[0];
            }
            else
            {
                return PointF.Empty;
            }
        }

        private PointF[] GetEndPoints(CBoundaryDXFEntity entity)
        {
            PointF[] points;

            points = new PointF[2];

            if (entity.EntityType == "LINE")
            {
                points[0].X = (float)Math.Round((float)entity.X0, 4);
                points[0].Y = (float)Math.Round((float)entity.Y0, 4);
                points[1].X = (float)Math.Round((float)entity.X1, 4);
                points[1].Y = (float)Math.Round((float)entity.Y1, 4);
            }
            else if (entity.EntityType == "ARC")
            {
                points[0].X = (float)Math.Round((float)((Math.Cos(CBoundaryDXF.DTR(entity.StartAngle)) * entity.Radius) + entity.X0), 4);
                points[0].Y = (float)Math.Round((float)((Math.Sin(CBoundaryDXF.DTR(entity.StartAngle)) * entity.Radius) + entity.Y0), 4);
                points[1].X = (float)Math.Round((float)((Math.Cos(CBoundaryDXF.DTR(entity.EndAngle)) * entity.Radius) + entity.X0), 4);
                points[1].Y = (float)Math.Round((float)((Math.Sin(CBoundaryDXF.DTR(entity.EndAngle)) * entity.Radius) + entity.Y0), 4);
            }
            else
            {
                points = null;
            }

            return points;
        }

        private PointF GetBottomPoint(PointF[] points)
        {
            if (points[0].Y < points[1].Y)
            {
                return points[0];
            }
            else
            {
                return points[1];
            }
        }
        private PointF GetTopPoint(PointF[] points)
        {
            if (points[0].Y > points[1].Y)
            {
                return points[0];
            }
            else
            {
                return points[1];
            }
        }
        private PointF GetLeftPoint(PointF[] points)
        {
            if (points[0].X < points[1].X)
            {
                return points[0];
            }
            else
            {
                return points[1];
            }
        }
        private PointF GetRightPoint(PointF[] points)
        {
            if (points[0].X > points[1].X)
            {
                return points[0];
            }
            else
            {
                return points[1];
            }
        }
    }
}

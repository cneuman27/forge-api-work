import * as Poly2Tri from "poly2tri";
import * as _ from "lodash";
import * as ClipperLib from "@jci-ahu/ui.shared.clipper";

import { C3DGeometry } from "./Types/C3DGeometry";
import { C3DOpening } from "./Types/C3DOpening";
import { COpeningEdgeProfile } from "./Types/COpeningEdgeProfile";
import { CRect3D } from "./Types/CRect3D";
import { CRect2D } from "./Types/CRect2D";
import { CGeometryCollection } from "./Types/CGeometryCollection";

import { CConstants } from "./CConstants";
import * as Enums from "./Enums";

declare const THREE: any;

export class CUtility
{
    public static get2DRectForFace(
        rect3d: CRect3D,
        face: Enums.E_Location): CRect2D
    {
        let rect: CRect2D;

        rect = new CRect2D();

        if (face === Enums.E_Location.Front ||
            face === Enums.E_Location.Rear)
        {
            rect.x = rect3d.z;
            rect.y = rect3d.y;

            rect.xlength = rect3d.zlength;
            rect.ylength = rect3d.ylength;
        }
        else if (
            face === Enums.E_Location.Bottom ||
            face === Enums.E_Location.Top)
        {
            rect.x = rect3d.x;
            rect.y = rect3d.z;

            rect.xlength = rect3d.xlength;
            rect.ylength = rect3d.zlength;
        }
        else if (
            face === Enums.E_Location.Left ||
            face === Enums.E_Location.Right)
        {
            rect.x = rect3d.x;
            rect.y = rect3d.y;

            rect.xlength = rect3d.xlength;
            rect.ylength = rect3d.ylength;
        }

        return rect;
    }

    public static executeClipper(
        subjectPolygons: ClipperLib.IntPoint[][],
        clippingPolygons: ClipperLib.IntPoint[][]): ClipperLib.PolyTree
    {
        let clipper: ClipperLib.Clipper;
        let solution: ClipperLib.PolyTree

        ClipperLib.JS.ScaleUpPaths(subjectPolygons, CConstants.CLIPPER_SCALE);
        ClipperLib.JS.ScaleUpPaths(clippingPolygons, CConstants.CLIPPER_SCALE);

        clipper = new ClipperLib.Clipper();

        clipper.AddPaths(subjectPolygons, ClipperLib.PolyType.ptSubject, true);
        clipper.AddPaths(clippingPolygons, ClipperLib.PolyType.ptClip, true);

        solution = new ClipperLib.PolyTree();

        clipper.Execute(
            ClipperLib.ClipType.ctDifference,
            solution,
            ClipperLib.PolyFillType.pftEvenOdd,
            ClipperLib.PolyFillType.pftNonZero);

        CUtility.scaleDownPolyNode(solution, CConstants.CLIPPER_SCALE);

        return solution;
    }

    public static scaleDownPolyNode(
        node: ClipperLib.PolyNode,
        scale: number)
    {
        let contour: ClipperLib.Path;
        let children: ClipperLib.PolyNode[];

        contour = node.Contour();

        if (contour)
        {
            ClipperLib.JS.ScaleDownPath(contour, scale);
        }

        children = node.Childs();

        if (children)
        {
            for (let i of children)
            {
                CUtility.scaleDownPolyNode(i, scale);
            }
        }
    }

    public static isEqual(
        val1: number,
        val2: number,
        precision: number = CConstants.GEOMETRY_PRECISION): boolean
    {
        let ret: boolean;
        let rounder: number;

        rounder = Math.pow(10, precision);
        ret = ((Math.round(val1 * rounder) / rounder) === (Math.round(val2 * rounder) / rounder));

        return ret;
    }

    public static getBorderPolygon(
        perimeter: Poly2Tri.Point[],
        borderDim: number): Poly2Tri.Point[]
    {
        let border: Poly2Tri.Point[] = [];
        let pt: Poly2Tri.Point;
        let last_pt: Poly2Tri.Point;
        let next_pt: Poly2Tri.Point;
        let v1;
        let v2;
        let v3;
        let angle: number;
        let dist: number;
        let border_pt: Poly2Tri.Point;

        for (let x = 0; x < perimeter.length; x++)
        {
            // #region Get Points

            pt = perimeter[x];

            if (x === 0)
            {
                last_pt = perimeter[perimeter.length - 1];
            }
            else
            {
                last_pt = perimeter[x - 1];
            }

            if (x === perimeter.length - 1)
            {
                next_pt = perimeter[0];
            }
            else
            {
                next_pt = perimeter[x + 1];
            }

            // #endregion

            v1 = new THREE.Vector2(last_pt.x - pt.x, last_pt.y - pt.y);
            v2 = new THREE.Vector2(next_pt.x - pt.x, next_pt.y - pt.y);

            v1.normalize();
            v2.normalize();

            angle = CUtility.angleBetween(v1, v2);
            dist = borderDim / Math.sin(angle / 2);

            v3 = new THREE.Vector2((v1.x + v2.x) / 2, (v1.y + v2.y) / 2);
            v3.normalize();
            v3.multiplyScalar(dist);

            border_pt = new Poly2Tri.Point(pt.x + v3.x, pt.y + v3.y);

            if (CUtility.pointInPolygon(perimeter, border_pt) === false)
            {
                v3.negate();
                border_pt = new Poly2Tri.Point(pt.x + v3.x, pt.y + v3.y);
            }

            if (CUtility.pointInPolygon(perimeter, border_pt) === true)
            {
                border.push(border_pt);
            }
        }

        return border;
    }  

    public static angleBetween(
        v1,
        v2): number
    {
        return Math.acos(v1.dot(v2) / (v1.length() * v2.length()));
    }

    public static pointInPolygon(
        polygon: Poly2Tri.Point[],
        pt: Poly2Tri.Point): boolean
    {
        // http://alienryderflex.com/polygon/

        let res: boolean = false;
        let j: number;

        j = polygon.length - 1;

        for (let i = 0; i < polygon.length; i++)
        {
            if (polygon[i].y < pt.y && polygon[j].y >= pt.y ||
                polygon[j].y < pt.y && polygon[i].y >= pt.y)
            {
                if (polygon[i].x + (pt.y - polygon[i].y) / (polygon[j].y - polygon[i].y) * (polygon[j].x - polygon[i].x) < pt.x)
                {
                    res = !res;
                }
            }

            j = i
        }

        return res;
    }

    public static generateGeometryFromTriangles(
        triangles: Poly2Tri.Triangle[],
        face: Enums.E_Location,
        objectGeometry: C3DGeometry,
        offset: number,
        reverseFaces: boolean = false)
    {
        let geometry;
        let count: number;

        geometry = new THREE.Geometry();

        count = 0;
        for (let triangle of triangles)
        {
            // #region Generate Geometry Based on Face Location

            if (face === Enums.E_Location.Front)
            {
                // #region Front

                geometry.vertices.push(new THREE.Vector3(
                    objectGeometry.x + offset,
                    triangle.getPoint(0).y,
                    triangle.getPoint(0).x));

                geometry.vertices.push(new THREE.Vector3(
                    objectGeometry.x + offset,
                    triangle.getPoint(1).y,
                    triangle.getPoint(1).x));

                geometry.vertices.push(new THREE.Vector3(
                    objectGeometry.x + offset,
                    triangle.getPoint(2).y,
                    triangle.getPoint(2).x));

                if (reverseFaces)
                {
                    geometry.faces.push(new THREE.Face3(
                        count * 3 + 2,
                        count * 3 + 1,
                        count * 3));
                }
                else
                {
                    geometry.faces.push(new THREE.Face3(
                        count * 3,
                        count * 3 + 1,
                        count * 3 + 2));
                }

                // #endregion
            }
            else if (face === Enums.E_Location.Rear)
            {
                // #region Rear

                geometry.vertices.push(new THREE.Vector3(
                    objectGeometry.xend - offset,
                    triangle.getPoint(0).y,
                    triangle.getPoint(0).x));

                geometry.vertices.push(new THREE.Vector3(
                    objectGeometry.xend - offset,
                    triangle.getPoint(1).y,
                    triangle.getPoint(1).x));

                geometry.vertices.push(new THREE.Vector3(
                    objectGeometry.xend - offset,
                    triangle.getPoint(2).y,
                    triangle.getPoint(2).x));

                if (reverseFaces)
                {
                    geometry.faces.push(new THREE.Face3(
                        count * 3,
                        count * 3 + 1,
                        count * 3 + 2));
                }
                else
                {
                    geometry.faces.push(new THREE.Face3(
                        count * 3 + 2,
                        count * 3 + 1,
                        count * 3));
                }

                // #endregion
            }
            else if (face === Enums.E_Location.Bottom)
            {
                // #region Bottom

                geometry.vertices.push(new THREE.Vector3(
                    triangle.getPoint(0).x,
                    objectGeometry.y + offset,
                    triangle.getPoint(0).y));

                geometry.vertices.push(new THREE.Vector3(
                    triangle.getPoint(1).x,
                    objectGeometry.y + offset,
                    triangle.getPoint(1).y));

                geometry.vertices.push(new THREE.Vector3(
                    triangle.getPoint(2).x,
                    objectGeometry.y + offset,
                    triangle.getPoint(2).y));

                if (reverseFaces)
                {
                    geometry.faces.push(new THREE.Face3(
                        count * 3 + 2,
                        count * 3 + 1,
                        count * 3));
                }
                else
                {
                    geometry.faces.push(new THREE.Face3(
                        count * 3,
                        count * 3 + 1,
                        count * 3 + 2));
                }

                // #endregion
            }
            else if (face === Enums.E_Location.Top)
            {
                // #region Top

                geometry.vertices.push(new THREE.Vector3(
                    triangle.getPoint(0).x,
                    objectGeometry.yend - offset, 
                    triangle.getPoint(0).y));

                geometry.vertices.push(new THREE.Vector3(
                    triangle.getPoint(1).x,
                    objectGeometry.yend - offset, 
                    triangle.getPoint(1).y));

                geometry.vertices.push(new THREE.Vector3(
                    triangle.getPoint(2).x,
                    objectGeometry.yend - offset, 
                    triangle.getPoint(2).y));

                if (reverseFaces)
                {
                    geometry.faces.push(new THREE.Face3(
                        count * 3,
                        count * 3 + 1,
                        count * 3 + 2));
                }
                else
                {
                    geometry.faces.push(new THREE.Face3(
                        count * 3 + 2,
                        count * 3 + 1,
                        count * 3));
                }

                // #endregion
            }
            else if (face === Enums.E_Location.Left)
            {
                // #region Left

                geometry.vertices.push(new THREE.Vector3(
                    triangle.getPoint(0).x,
                    triangle.getPoint(0).y,
                    objectGeometry.z + offset));

                geometry.vertices.push(new THREE.Vector3(
                    triangle.getPoint(1).x,
                    triangle.getPoint(1).y,
                    objectGeometry.z + offset));

                geometry.vertices.push(new THREE.Vector3(
                    triangle.getPoint(2).x,
                    triangle.getPoint(2).y,
                    objectGeometry.z + offset));

                if (reverseFaces)
                {
                    geometry.faces.push(new THREE.Face3(
                        count * 3,
                        count * 3 + 1,
                        count * 3 + 2));
                }
                else
                {
                    geometry.faces.push(new THREE.Face3(
                        count * 3 + 2,
                        count * 3 + 1,
                        count * 3));
                }

                // #endregion
            }
            else if (face === Enums.E_Location.Right)
            {
                // #region Right

                geometry.vertices.push(new THREE.Vector3(
                    triangle.getPoint(0).x,
                    triangle.getPoint(0).y,
                    objectGeometry.zend - offset));

                geometry.vertices.push(new THREE.Vector3(
                    triangle.getPoint(1).x,
                    triangle.getPoint(1).y,
                    objectGeometry.zend - offset));

                geometry.vertices.push(new THREE.Vector3(
                    triangle.getPoint(2).x,
                    triangle.getPoint(2).y,
                    objectGeometry.zend - offset));

                if (reverseFaces)
                {
                    geometry.faces.push(new THREE.Face3(
                        count * 3 + 2,
                        count * 3 + 1,
                        count * 3));
                }
                else
                {
                    geometry.faces.push(new THREE.Face3(
                        count * 3,
                        count * 3 + 1,
                        count * 3 + 2));
                }

                // #endregion
            }

            // #endregion

            count++;
        }

        return geometry;
    }

    public static combineGeometries(geometryList)
    {
        let geometry;
        let lastPosCount: number;

        geometry = new THREE.Geometry();

        for (let oldGeometry of geometryList)
        {
            lastPosCount = geometry.vertices.length;

            for (let pt of oldGeometry.vertices)
            {
                geometry.vertices.push(new THREE.Vector3(pt.x, pt.y, pt.z));
            }

            for (let face of oldGeometry.faces)
            {
                geometry.faces.push(new THREE.Face3(face.a + lastPosCount, face.b + lastPosCount, face.c + lastPosCount));
            }
        }

        return geometry;
    }

    public static getFlatOpeningPolygon(opening: C3DOpening): ClipperLib.IntPoint[]
    {
        let polygon: ClipperLib.IntPoint[];
        let rect: CRect2D;

        rect = CUtility.get2DRectForFace(
            opening.geometry.toRect3D(),
            opening.face);

        if (rect.xlength <= 0 ||
            rect.ylength <= 0)
        {
            return null;
        }

        polygon = [];

        if (opening.shape === Enums.E_OpeningShape.Rectangle)
        {
            // #region Rectangle

            polygon.push(new ClipperLib.IntPoint(rect.x, rect.y));
            polygon.push(new ClipperLib.IntPoint(rect.xend, rect.y));
            polygon.push(new ClipperLib.IntPoint(rect.xend, rect.yend));
            polygon.push(new ClipperLib.IntPoint(rect.x, rect.yend));

            // #endregion
        }
        else if (
            opening.shape === Enums.E_OpeningShape.Round ||
            (opening.shape == Enums.E_OpeningShape.Oblong &&
                rect.xlength == rect.ylength))
        {
            // #region Round

            let centerX: number;
            let centerY: number;

            centerX = rect.x + (rect.xlength / 2);
            centerY = rect.y + (rect.ylength / 2);

            for (let x = 0; x < CConstants.OVAL_SLICE_COUNT; x++)
            {
                polygon.push(new ClipperLib.IntPoint(
                    centerX + (Math.cos(CConstants.OVAL_THETA * x) * (rect.xlength / 2)),
                    centerY + (Math.sin(CConstants.OVAL_THETA * x) * (rect.ylength / 2))));
            }

            // #endregion
        }
        else if (opening.shape === Enums.E_OpeningShape.Oblong)
        {
            // #region Oblong

            let centerX: number;
            let centerY: number;

            if (rect.xlength > rect.ylength)
            {
                // #region Horizontal Orientation

                // #region Bottom Line

                polygon.push(new ClipperLib.IntPoint(
                    rect.x + (rect.ylength / 2),
                    rect.y));

                polygon.push(new ClipperLib.IntPoint(
                    rect.x + (rect.ylength / 2) + (rect.xlength - rect.ylength),
                    rect.y));

                // #endregion

                // #region Right Arc

                centerX = rect.x + (rect.xlength / 2) + (rect.xlength - rect.ylength);
                centerY = rect.y + (rect.ylength / 2);

                for (let x = ((3 * Math.PI / 2) + CConstants.OVAL_THETA); x < (5 * Math.PI / 2); x += CConstants.OVAL_THETA)
                {
                    polygon.push(new ClipperLib.IntPoint(
                        centerX + (Math.cos(x) * (rect.ylength / 2)),
                        centerY + (Math.sin(x) * (rect.ylength / 2))));
                }

                // #endregion

                // #region Top Line

                polygon.push(new ClipperLib.IntPoint(
                    rect.x + (rect.ylength / 2) + (rect.xlength - rect.ylength),
                    rect.yend));

                polygon.push(new ClipperLib.IntPoint(
                    rect.x + (rect.ylength / 2),
                    rect.yend));

                // #endregion

                // #region Left Arc

                centerX = rect.x + (rect.ylength / 2);
                centerY = rect.y + (rect.ylength / 2);

                for (let x = ((Math.PI / 2) + CConstants.OVAL_THETA); x < (3 * Math.PI / 2); x += CConstants.OVAL_THETA)
                {
                    polygon.push(new ClipperLib.IntPoint(
                        centerX + (Math.cos(x) * (rect.ylength / 2)),
                        centerY + (Math.sin(x) * (rect.ylength / 2))));
                }

                // #endregion

                // #endregion
            }
            else 
            {
                // #region Vertical Orientation

                // #region Left Line

                polygon.push(new ClipperLib.IntPoint(
                    rect.x,
                    rect.y + (rect.xlength / 2) + (rect.ylength - rect.xlength)));

                polygon.push(new ClipperLib.IntPoint(
                    rect.x,
                    rect.y + (rect.xlength / 2)));

                // #endregion

                // #region Bottom Arc

                centerX = rect.x + (rect.xlength / 2);
                centerY = rect.y + (rect.xlength / 2);

                for (let x = (Math.PI + CConstants.OVAL_THETA); x < (2 * Math.PI); x += CConstants.OVAL_THETA)
                {
                    polygon.push(new ClipperLib.IntPoint(
                        centerX + (Math.cos(x) * (rect.xlength / 2)),
                        centerY + (Math.sin(x) * (rect.xlength / 2))));
                }

                // #endregion

                // #region Right Line

                polygon.push(new ClipperLib.IntPoint(
                    rect.xend,
                    rect.y + (rect.xlength / 2)));

                polygon.push(new ClipperLib.IntPoint(
                    rect.xend,
                    rect.y + (rect.xlength / 2) + (rect.ylength - rect.xlength)));

                // #endregion

                // #region Top Arc

                centerX = rect.x + (rect.xlength / 2);
                centerY = rect.y + (rect.xlength / 2) + (rect.ylength - rect.xlength);

                for (let x = CConstants.OVAL_THETA; x < Math.PI; x += CConstants.OVAL_THETA)
                {
                    polygon.push(new ClipperLib.IntPoint(
                        centerX + (Math.cos(x) * (rect.xlength / 2)),
                        centerY + (Math.sin(x) * (rect.xlength / 2))));
                }

                // #endregion

                // #endregion
            }

            // #endregion
        }

        return polygon;
    }

    public static addToGeometryCollection(
        collection: CGeometryCollection,
        geometry,
        faceType: Enums.E_FaceType)
    {
        if (faceType === Enums.E_FaceType.Edge)
        {
            collection.edges.push(geometry);
        }
        else if (faceType === Enums.E_FaceType.Exterior)
        {
            collection.exteriors.push(geometry);
        }
        else if (faceType === Enums.E_FaceType.Interior)
        {
            collection.interiors.push(geometry);
        }
    }
    //#region
    public static getLabelMaterial(
        label: string,
        backColor: string,
        foreColor: string,
        width: number,
        height: number) 
    {
        let canvas: HTMLCanvasElement = document.createElement("canvas") as HTMLCanvasElement;
        let context: CanvasRenderingContext2D;
        let texture;
        let textSize: TextMetrics;
        let x: number;
        let y: number;
        let material;

        canvas.height = 512;
        canvas.width = 512 * (width / height);

        context = canvas.getContext("2d");
        context.font = "bold 32px Arial";
        
        context.fillStyle = backColor;
        context.fillRect(0, 0, canvas.width, canvas.height);

        context.fillStyle = foreColor;
        context.textAlign = "center";
        context.textBaseline = "middle";
        context.fillText(label, canvas.width / 2, canvas.height / 2);

        texture = new THREE.CanvasTexture(canvas);
        texture.magFilter = THREE.LinearFilter;
        texture.minFilter = THREE.LinearFilter;
        texture.wrapS = THREE.ClampToEdgeWrapping;
        texture.wrapT = THREE.ClampToEdgeWrapping;
        texture.generateMipmaps = false;
        texture.needsUpdate = true;

        material = new THREE.MeshBasicMaterial(
            {
                map: texture
            });

        return material;
    }
    //#endregion

    public static generatePlanarTextureCoordinates(
        geometry,
        face: Enums.E_Location)
    {
        let bounds: CRect3D;
        let offset;
        let range;

        let _this = this;

        geometry.faceVertexUvs[0] = [];
        geometry.computeBoundingBox();
        
        bounds = CRect3D.fromBox3(geometry.boundingBox);

        if (face === Enums.E_Location.Front)
        {
            // #region Front Face

            _.forEach(
                geometry.faces,
                function (i)
                {
                    let v1;
                    let v2;
                    let v3;

                    v1 = geometry.vertices[i.a];
                    v2 = geometry.vertices[i.b];
                    v3 = geometry.vertices[i.c];

                    geometry.faceVertexUvs[0].push(
                        [
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(v1.z, bounds.z, bounds.zlength),
                                _this.getPlanarCoordinate(v1.y, bounds.y, bounds.ylength)),
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(v2.z, bounds.z, bounds.zlength),
                                _this.getPlanarCoordinate(v2.y, bounds.y, bounds.ylength)),
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(v3.z, bounds.z, bounds.zlength),
                                _this.getPlanarCoordinate(v3.y, bounds.y, bounds.ylength))
                        ]);
                });

            geometry.uvsNeedUpdate = true;

            // #endregion
        }
        else if (face === Enums.E_Location.Rear)
        {
            // #region Rear Face
            
            _.forEach(
                geometry.faces,
                function (i)
                {
                    let v1;
                    let v2;
                    let v3;

                    v1 = geometry.vertices[i.a];
                    v2 = geometry.vertices[i.b];
                    v3 = geometry.vertices[i.c];

                    geometry.faceVertexUvs[0].push(
                        [
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(bounds.zend - v1.z, 0, bounds.zlength),
                                _this.getPlanarCoordinate(v1.y, bounds.y, bounds.ylength)),
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(bounds.zend - v2.z, 0, bounds.zlength),
                                _this.getPlanarCoordinate(v2.y, bounds.y, bounds.ylength)),
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(bounds.zend - v3.z, 0, bounds.zlength),
                                _this.getPlanarCoordinate(v3.y, bounds.y, bounds.ylength))
                        ]);
                });

            geometry.uvsNeedUpdate = true;

            // #endregion
        }
        else if (face === Enums.E_Location.Bottom)
        {
            // #region Bottom Face

            _.forEach(
                geometry.faces,
                function (i)
                {
                    let v1;
                    let v2;
                    let v3;

                    v1 = geometry.vertices[i.a];
                    v2 = geometry.vertices[i.b];
                    v3 = geometry.vertices[i.c];

                    geometry.faceVertexUvs[0].push(
                        [
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(v1.x, bounds.x, bounds.xlength),
                                _this.getPlanarCoordinate(v1.z, bounds.z, bounds.zlength)),
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(v2.x, bounds.x, bounds.xlength),
                                _this.getPlanarCoordinate(v2.z, bounds.z, bounds.zlength)),
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(v3.x, bounds.x, bounds.xlength),
                                _this.getPlanarCoordinate(v3.z, bounds.z, bounds.zlength))
                        ]);
                });

            geometry.uvsNeedUpdate = true;

            // #endregion
        }
        else if (face === Enums.E_Location.Top)
        {
            // #region Top Face

            _.forEach(
                geometry.faces,
                function (i)
                {
                    let v1;
                    let v2;
                    let v3;

                    v1 = geometry.vertices[i.a];
                    v2 = geometry.vertices[i.b];
                    v3 = geometry.vertices[i.c];

                    geometry.faceVertexUvs[0].push(
                        [
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(bounds.xend - v1.x, 0, bounds.xlength),
                                _this.getPlanarCoordinate(v1.z, bounds.z, bounds.zlength)),
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(bounds.xend - v2.x, 0, bounds.xlength),
                                _this.getPlanarCoordinate(v2.z, bounds.z, bounds.zlength)),
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(bounds.xend - v3.x, 0, bounds.xlength),
                                _this.getPlanarCoordinate(v3.z, bounds.z, bounds.zlength))
                        ]);
                });

            geometry.uvsNeedUpdate = true;

            // #endregion
        }
        else if (face === Enums.E_Location.Left)
        {
            // #region Left Face

            _.forEach(
                geometry.faces,
                function (i)
                {
                    let v1;
                    let v2;
                    let v3;

                    v1 = geometry.vertices[i.a];
                    v2 = geometry.vertices[i.b];
                    v3 = geometry.vertices[i.c];

                    geometry.faceVertexUvs[0].push(
                        [
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(bounds.xend - v1.x, 0, bounds.xlength),
                                _this.getPlanarCoordinate(v1.y, bounds.y, bounds.ylength)),
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(bounds.xend - v2.x, 0, bounds.xlength),
                                _this.getPlanarCoordinate(v2.y, bounds.y, bounds.ylength)),
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(bounds.xend - v3.x, 0, bounds.xlength),
                                _this.getPlanarCoordinate(v3.y, bounds.y, bounds.ylength))
                        ]);
                });

            geometry.uvsNeedUpdate = true;

            // #endregion
        }
        else if (face === Enums.E_Location.Right)
        {
            // #region Right Face

            _.forEach(
                geometry.faces,
                function (i)
                {
                    let v1;
                    let v2;
                    let v3;

                    v1 = geometry.vertices[i.a];
                    v2 = geometry.vertices[i.b];
                    v3 = geometry.vertices[i.c];

                    geometry.faceVertexUvs[0].push(
                        [
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(v1.x, bounds.x, bounds.xlength),
                                _this.getPlanarCoordinate(v1.y, bounds.y, bounds.ylength)),
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(v2.x, bounds.x, bounds.xlength),
                                _this.getPlanarCoordinate(v2.y, bounds.y, bounds.ylength)),
                            new THREE.Vector2(
                                _this.getPlanarCoordinate(v3.x, bounds.x, bounds.xlength),
                                _this.getPlanarCoordinate(v3.y, bounds.y, bounds.ylength))
                        ]);
                });

            geometry.uvsNeedUpdate = true;

            // #endregion
        }
        
    }

    private static getPlanarCoordinate(
        end: number,
        start: number,
        width: number): number
    {
        return (end - start) / width;
    }

    private static getNearestPOW2(n: number): number
    {
        return Math.pow(2, Math.round(Math.log(n) / Math.log(2)));
    }

    //#region 
    public static getLabelArrowMaterial(
        label: string,
        backColor: string,
        foreColor: string,
        width: number,
        height: number,
        direction: string,
        airPathColor: string,
        displayLabel: boolean,
        displayAirFlow: boolean
    ) {
        let canvas: HTMLCanvasElement = document.createElement("canvas") as HTMLCanvasElement;
        let context: CanvasRenderingContext2D;
        let texture;
        let textSize: TextMetrics;
        let x: number;
        let y: number;
        let material;

        canvas.height = 512;
        canvas.width = 512 * (width / height);
        let radius = 70; 

        context = canvas.getContext("2d");
        context.font = "bold 32px Arial";


        context.fillStyle = backColor;
        context.fillRect(0, 0, canvas.width, canvas.height);

        context.fillStyle = foreColor;
        context.textAlign = "center";
        context.textBaseline = "middle";
        if (displayLabel) {
            context.fillText(label, canvas.width / 2, canvas.height / 2);
        }

        if (displayAirFlow) {
            if (direction === 'left') {
                context.beginPath();
                context.moveTo(0, (canvas.height / 2) - 50);
                context.lineTo(canvas.width - 50, (canvas.height / 2) - 50)
                context.lineWidth = 15;
                context.strokeStyle = airPathColor;
                context.stroke();
                context.beginPath();
                context.moveTo(canvas.width, (canvas.height / 2) - 50);
                context.lineTo(canvas.width - 50, ((canvas.height / 2) - 50) + 20);
                context.lineTo(canvas.width - 50, ((canvas.height / 2) - 50) - 20);
                context.fillStyle = airPathColor;
                context.fill();
            } else {
                context.beginPath();
                context.moveTo(50, (canvas.height / 2) - 50);
                context.lineTo(canvas.width, (canvas.height / 2) - 50)
                context.lineWidth = 15;
                context.strokeStyle = airPathColor;
                context.stroke();

                context.beginPath();
                context.moveTo(0, (canvas.height / 2) - 50);
                context.lineTo(50, ((canvas.height / 2) - 50) + 20);
                context.lineTo(50, ((canvas.height / 2) - 50) - 20);
                //context.strokeStyle = airPathColor;
                context.fillStyle = airPathColor;
                context.fill();
                //context.stroke();
            }
        }

        texture = new THREE.CanvasTexture(canvas);
        texture.magFilter = THREE.LinearFilter;
        texture.minFilter = THREE.LinearFilter;
        texture.wrapS = THREE.ClampToEdgeWrapping;
        texture.wrapT = THREE.ClampToEdgeWrapping;
        texture.generateMipmaps = false;
        texture.needsUpdate = true;

        material = new THREE.MeshBasicMaterial(
            {
                map: texture
            });

        return material;
    }
    //#endregion

}
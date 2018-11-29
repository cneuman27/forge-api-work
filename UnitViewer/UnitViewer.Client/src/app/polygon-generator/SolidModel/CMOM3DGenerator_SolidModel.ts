import * as _ from "lodash";
import * as Poly2Tri from "poly2tri";
import * as ClipperLib from "@jci-ahu/ui.shared.clipper";

import { C3DRenderOptions_SolidModel } from "./C3DRenderOptions_SolidModel";
import { C3DModel } from "../Types/C3DModel";
import { C3DOpening } from "../Types/C3DOpening";

import { CRect2D } from "../Types/CRect2D";
import { CRect3D } from "../Types/CRect3D";
import { CFacePolygonData } from "../Types/CFacePolygonData";
import { CPolygonData } from "../Types/CPolygonData";
import { CEdgeLine } from "../Types/CEdgeLine";
import { CGeometryCollection } from "../Types/CGeometryCollection";
import { COpeningEdgeProfile } from "../Types/COpeningEdgeProfile";

import { CConstants } from "../CConstants";
import { CUtility } from "../CUtility";
import * as Enums from "../Enums";

declare const THREE: any;

export class CMOM3DGenerator_SolidModel
{
    private _options: C3DRenderOptions_SolidModel = null;
    private _facePolygonDataList: CFacePolygonData[] = [];

    public generateModel(options: C3DRenderOptions_SolidModel): C3DModel
    {
        this._options = options;
        if (this._options === null) return null;
        if (this._options.geometry === null) return null;
        if (this._options.geometry.xlength <= 0) return null;
        if (this._options.geometry.ylength <= 0) return null;
        if (this._options.geometry.zlength <= 0) return null; 

        let model: C3DModel = null;

        this.validateOpenings();

        if (this._options.solidType === Enums.E_SolidType.Cuboid)
        {
            model = this.renderCuboid();
        }
        else if (this._options.solidType === Enums.E_SolidType.Cylinder)
        {
            model = this.renderCylinder();
        }
        else if (this._options.solidType === Enums.E_SolidType.OblongSolid)
        {
            model = this.renderOblongSolid();
        }        
        
        return model;
    }

    private validateOpenings()
    {
        _.forEach(
            this._options.openingList,
            function (i: C3DOpening)
            {
                i.isValid = true;
            });
    }

    private renderCuboid(): C3DModel
    {
        let model: C3DModel = new C3DModel();
        let reevaluatedFacePolygonDataList: CFacePolygonData[];
        let geometryCollection: CGeometryCollection;
        let geometry;
        let mesh;

        let exteriors = [];
        let interiors = [];
        let edges = [];

        // #region Setup Polygon Data

        this._facePolygonDataList = [];
        for (let i of CConstants.FACE_VECTOR_DATA)
        {
            this._facePolygonDataList.push(this.getFacePolygonData(i.face));
        }

        reevaluatedFacePolygonDataList = [];
        for (let i of this._facePolygonDataList)
        {
            reevaluatedFacePolygonDataList.push(this.reevaluateFacePolygonBasedOnSpannedOpenings(i));
        }

        this._facePolygonDataList = reevaluatedFacePolygonDataList;

        // #endregion

        for (let vectorData of CConstants.FACE_VECTOR_DATA)
        {
            if (this._options.getFaceType(vectorData.face) === Enums.E_FaceType.NoRender) continue;

            geometryCollection = this.renderCuboidFace(vectorData.face);

            exteriors = _.concat(exteriors, geometryCollection.exteriors);
            interiors = _.concat(interiors, geometryCollection.interiors);
            edges = _.concat(edges, geometryCollection.edges);
        }

        // #region Combine Geometries and add To Model

        if (exteriors.length > 0)
        {
            geometry = CUtility.combineGeometries(exteriors);
            mesh = new THREE.Mesh(geometry, this._options.material_Exterior);
            model.exteriorModelList_All.push(mesh);
        }

        if (interiors.length > 0)
        {
            geometry = CUtility.combineGeometries(interiors);
            mesh = new THREE.Mesh(geometry, this._options.material_Interior);
            model.interiorModelList_All.push(mesh);
        } 

        if (edges.length > 0)
        {
            geometry = CUtility.combineGeometries(edges);
            mesh = new THREE.Mesh(geometry, this._options.material_Edge);
            model.edgeModelList_All.push(mesh);
        }

        // #endregion

        return model;
    }
    private renderCylinder(): C3DModel
    {
        return null;
    }
    private renderOblongSolid(): C3DModel
    {
        return null;
    }

    private renderCuboidFace(face: Enums.E_Location): CGeometryCollection
    {
        let collection: CGeometryCollection;
        let facePolygonData: CFacePolygonData;
        let faceType: Enums.E_FaceType;
        let renderBorder: boolean;
        let context: Poly2Tri.SweepContext;
        let triangles: Poly2Tri.Triangle[];
        let perimeter_main: Poly2Tri.Point[];
        let perimeter_border: Poly2Tri.Point[];
        let geometry;
        let geometryList;
        let openingProfileList: COpeningEdgeProfile[];

        collection = new CGeometryCollection();

        facePolygonData = _.find(
            this._facePolygonDataList,
            function (i: CFacePolygonData): boolean
            {
                return i.face === face;
            });

        if (facePolygonData === undefined) return collection;

        faceType = this._options.getFaceType(face);
        if (faceType === Enums.E_FaceType.NoRender) return collection;

        // #region renderBorder flag

        renderBorder = false;

        if (faceType !== Enums.E_FaceType.Edge &&
            this._options.renderBorder === true)
        {
            if (face === Enums.E_Location.Front ||
                face === Enums.E_Location.Rear)
            {
                renderBorder =
                    (this._options.geometry.ylength > (2 & CConstants.BORDER_DIM) &&
                        this._options.geometry.zlength > (2 & CConstants.BORDER_DIM));
            }

            if (face === Enums.E_Location.Bottom ||
                face === Enums.E_Location.Top)
            {
                renderBorder =
                    (this._options.geometry.xlength > (2 & CConstants.BORDER_DIM) &&
                        this._options.geometry.zlength > (2 & CConstants.BORDER_DIM));
            }

            if (face === Enums.E_Location.Left ||
                face === Enums.E_Location.Right)
            {
                renderBorder =
                    (this._options.geometry.xlength > (2 & CConstants.BORDER_DIM) &&
                        this._options.geometry.ylength > (2 & CConstants.BORDER_DIM));
            }
        }

        // #endregion

        for (let polygon of facePolygonData.exteriorPolygonList)
        {
            perimeter_main = [];

            _.forEach(
                polygon.polygon_Poly2Tri,
                function (i: Poly2Tri.Point): void 
                {
                    perimeter_main.push(i.clone());
                });

            if (renderBorder)
            {
                // #region Render Border

                perimeter_border = CUtility.getBorderPolygon(perimeter_main, this._options.borderDim);

                context = new Poly2Tri.SweepContext(perimeter_main);
                context.addHole(perimeter_border);
                context.triangulate();
                triangles = context.getTriangles();

                geometry = CUtility.generateGeometryFromTriangles(
                    triangles,
                    face,
                    this._options.geometry,
                    0);

                collection.edges.push(geometry);

                perimeter_main = perimeter_border;

                // #endregion
            }

            if (face === this._options.faceLocation &&
                renderBorder)
            {
                // #region Render Edges For Cutouts

                let lastPoint: Poly2Tri.Point;
                let nextPoint: Poly2Tri.Point;
                let geometryList;
                let onEdge: boolean;

                geometryList = [];

                for (let x = 0; x < polygon.polygon_Poly2Tri.length; x++)
                {
                    if (x === 0)
                    {
                        lastPoint = polygon.polygon_Poly2Tri[polygon.polygon_Poly2Tri.length - 1];
                    }

                    nextPoint = polygon.polygon_Poly2Tri[x];

                    onEdge = false;

                    // #region Determine if This Edge Lies on Edge of the Main Face

                    if (lastPoint.x === nextPoint.x)
                    {
                        // #region Vertical Orientation

                        if (CUtility.isEqual(lastPoint.x, facePolygonData.exteriorRect.x) ||
                            CUtility.isEqual(lastPoint.x, facePolygonData.exteriorRect.xend))
                        {
                            onEdge = true;
                        }

                        // #endregion
                    }
                    else if (lastPoint.y === nextPoint.y)
                    {
                        // #region Horizontal Orientation

                        if (CUtility.isEqual(lastPoint.y, facePolygonData.exteriorRect.y) ||
                            CUtility.isEqual(lastPoint.y, facePolygonData.exteriorRect.yend))
                        {
                            onEdge = true;
                        }

                        // #endregion
                    }

                    // #endregion

                    if (onEdge)
                    {
                        lastPoint = nextPoint;
                        continue;
                    }

                    geometry = new THREE.Geometry();

                    // #region Generate Geometry based on Face Location

                    if (face === Enums.E_Location.Front)
                    {
                        // #region Front

                        geometry.vertices.push(new THREE.Vector3(
                            this._options.geometry.xend,
                            lastPoint.y,
                            lastPoint.x));

                        geometry.vertices.push(new THREE.Vector3(
                            this._options.geometry.xend,
                            nextPoint.y,
                            nextPoint.x));

                        geometry.vertices.push(new THREE.Vector3(
                            this._options.geometry.x,
                            nextPoint.y,
                            nextPoint.x));

                        geometry.vertices.push(new THREE.Vector3(
                            this._options.geometry.x,
                            lastPoint.y,
                            lastPoint.x));

                        geometry.faces.push(new THREE.Face3(1, 2, 0));
                        geometry.faces.push(new THREE.Face3(2, 3, 0));

                        // #endregion
                    }
                    else if (face === Enums.E_Location.Rear)
                    {
                        // #region Rear

                        geometry.vertices.push(new THREE.Vector3(
                            this._options.geometry.x,
                            lastPoint.y,
                            lastPoint.x));

                        geometry.vertices.push(new THREE.Vector3(
                            this._options.geometry.x,
                            nextPoint.y,
                            nextPoint.x));

                        geometry.vertices.push(new THREE.Vector3(
                            this._options.geometry.xend,
                            nextPoint.y,
                            nextPoint.x));

                        geometry.vertices.push(new THREE.Vector3(
                            this._options.geometry.xend,
                            lastPoint.y,
                            lastPoint.x));

                        geometry.faces.push(new THREE.Face3(0, 2, 1));
                        geometry.faces.push(new THREE.Face3(0, 3, 2));

                        // #endregion
                    }
                    else if (face === Enums.E_Location.Bottom)
                    {
                        // #region Bottom

                        geometry.vertices.push(new THREE.Vector3(
                            lastPoint.x,
                            this._options.geometry.yend,
                            lastPoint.y));

                        geometry.vertices.push(new THREE.Vector3(
                            nextPoint.x,
                            this._options.geometry.yend,
                            nextPoint.y));

                        geometry.vertices.push(new THREE.Vector3(
                            nextPoint.x,
                            this._options.geometry.y,
                            nextPoint.y));

                        geometry.vertices.push(new THREE.Vector3(
                            lastPoint.x,
                            this._options.geometry.y,
                            lastPoint.y));

                        geometry.faces.push(new THREE.Face3(1, 2, 0));
                        geometry.faces.push(new THREE.Face3(2, 3, 0));

                        // #endregion
                    }
                    else if (face === Enums.E_Location.Top)
                    {
                        // #region Top

                        geometry.vertices.push(new THREE.Vector3(
                            lastPoint.x,
                            this._options.geometry.y,
                            lastPoint.y));

                        geometry.vertices.push(new THREE.Vector3(
                            nextPoint.x,
                            this._options.geometry.y,
                            nextPoint.y));

                        geometry.vertices.push(new THREE.Vector3(
                            nextPoint.x,
                            this._options.geometry.yend,
                            nextPoint.y));

                        geometry.vertices.push(new THREE.Vector3(
                            lastPoint.x,
                            this._options.geometry.yend,
                            lastPoint.y));

                        geometry.faces.push(new THREE.Face3(0, 2, 1));
                        geometry.faces.push(new THREE.Face3(0, 3, 2));

                        // #endregion
                    }
                    else if (face === Enums.E_Location.Left)
                    {
                        // #region Left

                        geometry.vertices.push(new THREE.Vector3(
                            lastPoint.x,
                            lastPoint.y,
                            this._options.geometry.zend));

                        geometry.vertices.push(new THREE.Vector3(
                            nextPoint.x,
                            nextPoint.y,
                            this._options.geometry.zend));

                        geometry.vertices.push(new THREE.Vector3(
                            nextPoint.x,
                            nextPoint.y,
                            this._options.geometry.z));

                        geometry.vertices.push(new THREE.Vector3(
                            lastPoint.x,
                            lastPoint.y,
                            this._options.geometry.z));

                        geometry.faces.push(new THREE.Face3(0, 2, 1));
                        geometry.faces.push(new THREE.Face3(0, 3, 2));

                        // #endregion
                    }
                    else if (face === Enums.E_Location.Right)
                    {
                        // #region Right

                        geometry.vertices.push(new THREE.Vector3(
                            lastPoint.x,
                            lastPoint.y,
                            this._options.geometry.z));

                        geometry.vertices.push(new THREE.Vector3(
                            nextPoint.x,
                            nextPoint.y,
                            this._options.geometry.z));

                        geometry.vertices.push(new THREE.Vector3(
                            nextPoint.x,
                            nextPoint.y,
                            this._options.geometry.zend));

                        geometry.vertices.push(new THREE.Vector3(
                            lastPoint.x,
                            lastPoint.y,
                            this._options.geometry.zend));

                        geometry.faces.push(new THREE.Face3(1, 2, 0));
                        geometry.faces.push(new THREE.Face3(2, 3, 0));

                        // #endregion
                    }

                    // #endregion

                    geometryList.push(geometry);

                    lastPoint = nextPoint;
                }

                _.forEach(
                    geometryList,
                    function (i): void
                    {
                        collection.edges.push(i);
                    });

                // #endregion
            }

            // #region Render Main Face and Get Opening Profiles

            context = new Poly2Tri.SweepContext(perimeter_main);

            openingProfileList = [];

            for (let o of polygon.openingPolygonList)
            {
                if (renderBorder)
                {
                    // #region Enlarge Opening To Account For Border

                    let pt: Poly2Tri.Point;
                    let last_pt: Poly2Tri.Point;
                    let next_pt: Poly2Tri.Point;
                    let new_pt: Poly2Tri.Point;
                    let o_polygon: Poly2Tri.Point[];
                    let v1;
                    let v2;
                    let v3;
                    let angle: number;
                    let dist: number;
                    let openingProfile: COpeningEdgeProfile;

                    o_polygon = [];

                    openingProfile = new COpeningEdgeProfile();
                    openingProfile.vertexCount = 0;
                    openingProfile.innerPoints = [];
                    openingProfile.outerPoints = [];

                    for (let x = 0; x < o.polygon_Poly2Tri.length; x++)
                    {
                        // #region Get Points

                        pt = o.polygon_Poly2Tri[x];

                        if (x === 0)
                        {
                            last_pt = o.polygon_Poly2Tri[o.polygon_Poly2Tri.length - 1];
                        }
                        else
                        {
                            last_pt = o.polygon_Poly2Tri[x - 1];
                        }

                        if (x === o.polygon_Poly2Tri.length - 1)
                        {
                            next_pt = o.polygon_Poly2Tri[0];
                        }
                        else
                        {
                            next_pt = o.polygon_Poly2Tri[x + 1];
                        }

                        // #endregion

                        v1 = new THREE.Vector2(last_pt.x - pt.x, last_pt.y - pt.y);
                        v2 = new THREE.Vector2(next_pt.x - pt.x, next_pt.y - pt.y);

                        v1.normalize();
                        v2.normalize();

                        angle = CUtility.angleBetween(v1, v2);
                        dist = this._options.borderDim / Math.sin(angle / 2);

                        v3 = new THREE.Vector2((v1.x + v2.x) / 2, (v1.y + v2.y) / 2);
                        v3.normalize();
                        v3.multiplyScalar(dist);

                        new_pt = new Poly2Tri.Point(pt.x + v3.x, pt.y + v3.y);

                        if (CUtility.pointInPolygon(o.polygon_Poly2Tri, new_pt) === true)
                        {
                            v3.negate();
                            new_pt = new Poly2Tri.Point(pt.x + v3.x, pt.y + v3.y);
                        }

                        if (CUtility.pointInPolygon(o.polygon_Poly2Tri, new_pt) === false)
                        {
                            o_polygon.push(new_pt);

                            openingProfile.vertexCount++;
                            openingProfile.outerPoints.push(new_pt);
                            openingProfile.innerPoints.push(pt);
                        }
                    }

                    openingProfileList.push(openingProfile);
                    context.addHole(o_polygon);

                    // #endregion
                }
                else
                {
                    context.addHole(o.polygon_Poly2Tri);
                }
            }

            context.triangulate();
            triangles = context.getTriangles();

            geometry = CUtility.generateGeometryFromTriangles(
                triangles,
                face,
                this._options.geometry,
                0);

            CUtility.addToGeometryCollection(
                collection,
                geometry,
                faceType);

            // #endregion 

            if (renderBorder)
            {
                // #region Render Opening Profiles

                for (let i of openingProfileList)
                {
                    geometryList = this.generateGeometryFromOpeningEdgeProfile(
                        i,
                        face);

                    _.forEach(
                        geometryList,
                        function (i): void
                        {
                            CUtility.addToGeometryCollection(
                                collection,
                                i,
                                Enums.E_FaceType.Edge);
                        });
                }

                // #endregion
            }
        }

        return collection;
    }

    private getFacePolygonData(
        face: Enums.E_Location,
        spannedOpenings: C3DOpening[] = null): CFacePolygonData
    {
        let data: CFacePolygonData;
        let openingList: C3DOpening[];
        let container3DRect: CRect3D;
        let facePathList: ClipperLib.IntPoint[][];
        let facePath: ClipperLib.IntPoint[];;
        let openingPathList: ClipperLib.IntPoint[][];
        let openingPath: ClipperLib.IntPoint[];
        let solution: ClipperLib.PolyTree;
        let polygonData_face: CPolygonData;
        let polygonData_opening: CPolygonData;

        data = new CFacePolygonData();
        data.face = face;
        
        // #region Get Openings

        openingList = _.filter(
            this._options.openingList,
            function (i: C3DOpening)
            {
                return (i.face === face ||
                        i.face === CConstants.OPPOSITE_FACE_LOOKUP.get(face)) &&
                        i.isValid === true;
            });
            
        if (spannedOpenings !== null)
        {
            _.forEach(
                spannedOpenings,
                function (i: C3DOpening)
                {
                    openingList.push(i);
                });
        }

        // #endregion

        // #region Exterior Polygons

        container3DRect = this._options.geometry.toRect3D();

        data.exteriorRect = CUtility.get2DRectForFace(
            container3DRect,
            face);

        facePathList = [];
        facePath = [];

        facePath.push(new ClipperLib.IntPoint(data.exteriorRect.x, data.exteriorRect.y));
        facePath.push(new ClipperLib.IntPoint(data.exteriorRect.xend, data.exteriorRect.y));
        facePath.push(new ClipperLib.IntPoint(data.exteriorRect.xend, data.exteriorRect.yend));
        facePath.push(new ClipperLib.IntPoint(data.exteriorRect.x, data.exteriorRect.yend));

        facePathList.push(facePath);

        openingPathList = [];

        for (let tmp of openingList)
        {
            openingPath = CUtility.getFlatOpeningPolygon(tmp);
            if (openingPath !== null &&
                openingPath.length > 0)
            {
                openingPathList.push(openingPath);
            }
        }       

        solution = CUtility.executeClipper(
            facePathList,
            openingPathList);

        for (let f of solution.Childs())
        {
            polygonData_face = new CPolygonData();
            polygonData_face.polygon_Clipper = f.Contour();

            // #region Assign Openings

            for (let o of f.Childs())
            {
                polygonData_opening = new CPolygonData();
                polygonData_opening.polygon_Clipper = o.Contour();

                polygonData_face.openingPolygonList.push(polygonData_opening);
            }

            // #endregion

            // #region Get Edge Lines

            let lastPoint: ClipperLib.IntPoint;
            let nextPoint: ClipperLib.IntPoint;
            let line: CEdgeLine;
            let tmpLine: CEdgeLine;

            lastPoint = new ClipperLib.IntPoint(-1, -1);
            nextPoint = new ClipperLib.IntPoint(-1, -1);

            for (let x = 0; x < polygonData_face.polygon_Clipper.length; x++)
            {
                if (x === 0)
                {
                    lastPoint = polygonData_face.polygon_Clipper[polygonData_face.polygon_Clipper.length - 1];
                }

                nextPoint = polygonData_face.polygon_Clipper[x];

                if (CUtility.isEqual(lastPoint.Y, data.exteriorRect.yend) &&
                    CUtility.isEqual(nextPoint.Y, data.exteriorRect.yend))
                {
                    // #region Top

                    line = new CEdgeLine();
                    line.startPoint = new THREE.Vector2(lastPoint.X, lastPoint.Y);
                    line.endPoint = new THREE.Vector2(nextPoint.X, nextPoint.Y);

                    tmpLine = _.find(
                        data.edgeList_Top,
                        function (i: CEdgeLine): boolean
                        {
                            return i.isEqual(line);
                        });

                    if (tmpLine === undefined)
                    {
                        data.edgeList_Top.push(line);
                    }

                    // #endregion
                }
                else if (
                    CUtility.isEqual(lastPoint.Y, data.exteriorRect.y) &&
                    CUtility.isEqual(nextPoint.Y, data.exteriorRect.y))
                {
                    // #region Bottom

                    line = new CEdgeLine();
                    line.startPoint = new THREE.Vector2(lastPoint.X, lastPoint.Y);
                    line.endPoint = new THREE.Vector2(nextPoint.X, nextPoint.Y);

                    tmpLine = _.find(
                        data.edgeList_Bottom,
                        function (i: CEdgeLine): boolean
                        {
                            return i.isEqual(line);
                        });

                    if (tmpLine === undefined)
                    {
                        data.edgeList_Bottom.push(line);
                    }
                    
                    // #endregion
                }
                else if (
                    CUtility.isEqual(lastPoint.X, data.exteriorRect.x) &&
                    CUtility.isEqual(nextPoint.X, data.exteriorRect.x))
                {
                    // #region Left

                    line = new CEdgeLine();
                    line.startPoint = new THREE.Vector2(lastPoint.X, lastPoint.Y);
                    line.endPoint = new THREE.Vector2(nextPoint.X, nextPoint.Y);

                    if (data.face === Enums.E_Location.Top ||
                        data.face === Enums.E_Location.Bottom ||
                        data.face === Enums.E_Location.Left)
                    {
                        // Left and Right Inversed on Top, Bottom, and Left Faces

                        tmpLine = _.find(
                            data.edgeList_Right,
                            function (i: CEdgeLine): boolean
                            {
                                return i.isEqual(line);
                            });

                        if (tmpLine === undefined)
                        {
                            data.edgeList_Right.push(line);
                        }
                    }
                    else
                    {
                        tmpLine = _.find(
                            data.edgeList_Left,
                            function (i: CEdgeLine): boolean
                            {
                                return i.isEqual(line);
                            });

                        if (tmpLine === undefined)
                        {
                            data.edgeList_Left.push(line);
                        }
                    }

                    // #endregion
                }
                else if (
                    CUtility.isEqual(lastPoint.X, data.exteriorRect.xend) &&
                    CUtility.isEqual(nextPoint.X, data.exteriorRect.xend))
                {
                    // #region Right

                    line = new CEdgeLine();
                    line.startPoint = new THREE.Vector2(lastPoint.X, lastPoint.Y);
                    line.endPoint = new THREE.Vector2(nextPoint.X, nextPoint.Y);

                    if (data.face === Enums.E_Location.Top ||
                        data.face === Enums.E_Location.Bottom ||
                        data.face === Enums.E_Location.Left)
                    {
                        // Left and Right Inversed on Top, Bottom, and Left Faces

                        tmpLine = _.find(
                            data.edgeList_Left,
                            function (i: CEdgeLine): boolean
                            {
                                return i.isEqual(line);
                            });

                        if (tmpLine === undefined)
                        {
                            data.edgeList_Left.push(line);
                        }
                    }
                    else
                    {
                        tmpLine = _.find(
                            data.edgeList_Right,
                            function (i: CEdgeLine): boolean
                            {
                                return i.isEqual(line);
                            });

                        if (tmpLine === undefined)
                        {
                            data.edgeList_Right.push(line);
                        }
                    }
                    
                    // #endregion
                }

                lastPoint = nextPoint;
            }

            // #endregion

            data.exteriorPolygonList.push(polygonData_face);
        }

        // #endregion

        data.normalizeEdges();

        return data;
    }
    
    private reevaluateFacePolygonBasedOnSpannedOpenings(data: CFacePolygonData): CFacePolygonData
    {
        let newData: CFacePolygonData;
        let spannedOpenings: C3DOpening[];

        spannedOpenings = this.getExteriorSpannedOpenings(data);
        if (spannedOpenings.length === 0) return data;

        newData = this.getFacePolygonData(data.face, spannedOpenings);

        return newData;

    }
    private getExteriorSpannedOpenings(data: CFacePolygonData): C3DOpening[]
    {
        let openingList: C3DOpening[];
        let sideData: CFacePolygonData;
        let lastEnd: number;
        let opening: C3DOpening;

        openingList = [];

        if (data.face === Enums.E_Location.Front)
        {
            // #region Front Face

            // #region Top Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Top;
                });

            lastEnd = sideData.exteriorRect.y;

            if (sideData.edgeList_Right.length > 0)
            {
                for (let line of sideData.edgeList_Right)
                {
                    if (line.startPoint.y != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Front;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = this._options.geometry.x;
                        opening.geometry.xlength = 0;
                        opening.geometry.y = data.exteriorRect.y;
                        opening.geometry.ylength = data.exteriorRect.ylength;
                        opening.geometry.z = lastEnd;
                        opening.geometry.zlength = line.startPoint.y - lastEnd;

                        if (opening.geometry.ylength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.y;
                }

                if (lastEnd != sideData.exteriorRect.yend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Front;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = this._options.geometry.x;
                    opening.geometry.xlength = 0;
                    opening.geometry.y = data.exteriorRect.y;
                    opening.geometry.ylength = data.exteriorRect.ylength;
                    opening.geometry.z = lastEnd;
                    opening.geometry.zlength = sideData.exteriorRect.yend - lastEnd;

                    if (opening.geometry.ylength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Bottom Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Bottom;
                });

            lastEnd = sideData.exteriorRect.y;

            if (sideData.edgeList_Right.length > 0)
            {
                for (let line of sideData.edgeList_Right)
                {
                    if (line.startPoint.y != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Front;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = this._options.geometry.x;
                        opening.geometry.xlength = 0;
                        opening.geometry.y = data.exteriorRect.y;
                        opening.geometry.ylength = data.exteriorRect.ylength;
                        opening.geometry.z = lastEnd;
                        opening.geometry.zlength = line.startPoint.y - lastEnd;

                        if (opening.geometry.ylength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.y;
                }

                if (lastEnd != sideData.exteriorRect.yend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Front;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = this._options.geometry.x;
                    opening.geometry.xlength = 0;
                    opening.geometry.y = data.exteriorRect.y;
                    opening.geometry.ylength = data.exteriorRect.ylength;
                    opening.geometry.z = lastEnd;
                    opening.geometry.zlength = sideData.exteriorRect.yend - lastEnd;

                    if (opening.geometry.ylength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Left Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Left;
                });

            lastEnd = sideData.exteriorRect.y;

            if (sideData.edgeList_Right.length > 0)
            {
                for (let line of sideData.edgeList_Right)
                {
                    if (line.startPoint.y != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Front;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = this._options.geometry.x;
                        opening.geometry.xlength = 0;
                        opening.geometry.y = lastEnd;
                        opening.geometry.ylength = line.startPoint.y - lastEnd;
                        opening.geometry.z = data.exteriorRect.x;
                        opening.geometry.zlength = data.exteriorRect.xlength;

                        if (opening.geometry.ylength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.y;
                }

                if (lastEnd != sideData.exteriorRect.yend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Front;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = this._options.geometry.x;
                    opening.geometry.xlength = 0;
                    opening.geometry.y = lastEnd;
                    opening.geometry.ylength = sideData.exteriorRect.yend - lastEnd;
                    opening.geometry.z = data.exteriorRect.x;
                    opening.geometry.zlength = data.exteriorRect.xlength;

                    if (opening.geometry.ylength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Right Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Right;
                });

            lastEnd = sideData.exteriorRect.y;

            if (sideData.edgeList_Left.length > 0)
            {
                for (let line of sideData.edgeList_Left)
                {
                    if (line.startPoint.y != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Front;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = this._options.geometry.x;
                        opening.geometry.xlength = 0;
                        opening.geometry.y = lastEnd;
                        opening.geometry.ylength = line.startPoint.y - lastEnd;
                        opening.geometry.z = data.exteriorRect.x;
                        opening.geometry.zlength = data.exteriorRect.xlength;

                        if (opening.geometry.ylength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.y;
                }

                if (lastEnd != sideData.exteriorRect.yend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Front;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = this._options.geometry.x;
                    opening.geometry.xlength = 0;
                    opening.geometry.y = lastEnd;
                    opening.geometry.ylength = sideData.exteriorRect.yend - lastEnd;
                    opening.geometry.z = data.exteriorRect.x;
                    opening.geometry.zlength = data.exteriorRect.xlength;

                    if (opening.geometry.ylength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #endregion
        }
        else if (data.face === Enums.E_Location.Rear)
        {
            // #region Rear Face

            // #region Top Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Top;
                });

            lastEnd = sideData.exteriorRect.y;

            if (sideData.edgeList_Left.length > 0)
            {
                for (let line of sideData.edgeList_Left)
                {
                    if (line.startPoint.y != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Rear;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = this._options.geometry.xend;
                        opening.geometry.xlength = 0;
                        opening.geometry.y = data.exteriorRect.y;
                        opening.geometry.ylength = data.exteriorRect.ylength;
                        opening.geometry.z = lastEnd;
                        opening.geometry.zlength = line.startPoint.y - lastEnd;

                        if (opening.geometry.ylength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.y;
                }

                if (lastEnd != sideData.exteriorRect.yend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Rear;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = this._options.geometry.xend;
                    opening.geometry.xlength = 0;
                    opening.geometry.y = data.exteriorRect.y;
                    opening.geometry.ylength = data.exteriorRect.ylength;
                    opening.geometry.z = lastEnd;
                    opening.geometry.zlength = sideData.exteriorRect.yend - lastEnd;

                    if (opening.geometry.ylength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Bottom Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Bottom;
                });

            lastEnd = sideData.exteriorRect.y;

            if (sideData.edgeList_Left.length > 0)
            {
                for (let line of sideData.edgeList_Left)
                {
                    if (line.startPoint.y != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Rear;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = this._options.geometry.xend;
                        opening.geometry.xlength = 0;
                        opening.geometry.y = data.exteriorRect.y;
                        opening.geometry.ylength = data.exteriorRect.ylength;
                        opening.geometry.z = lastEnd;
                        opening.geometry.zlength = line.startPoint.y - lastEnd;

                        if (opening.geometry.ylength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.y;
                }

                if (lastEnd != sideData.exteriorRect.yend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Rear;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = this._options.geometry.xend;
                    opening.geometry.xlength = 0;
                    opening.geometry.y = data.exteriorRect.y;
                    opening.geometry.ylength = data.exteriorRect.ylength;
                    opening.geometry.z = lastEnd;
                    opening.geometry.zlength = sideData.exteriorRect.yend - lastEnd;

                    if (opening.geometry.ylength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Left Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Left;
                });

            lastEnd = sideData.exteriorRect.y;

            if (sideData.edgeList_Left.length > 0)
            {
                for (let line of sideData.edgeList_Left)
                {
                    if (line.startPoint.y != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Rear;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = this._options.geometry.xend;
                        opening.geometry.xlength = 0;
                        opening.geometry.y = lastEnd;
                        opening.geometry.ylength = line.startPoint.y - lastEnd;
                        opening.geometry.z = data.exteriorRect.x;
                        opening.geometry.zlength = data.exteriorRect.xlength;

                        if (opening.geometry.ylength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.y;
                }

                if (lastEnd != sideData.exteriorRect.yend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Rear;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = this._options.geometry.xend;
                    opening.geometry.xlength = 0;
                    opening.geometry.y = lastEnd;
                    opening.geometry.ylength = sideData.exteriorRect.yend - lastEnd;
                    opening.geometry.z = data.exteriorRect.x;
                    opening.geometry.zlength = data.exteriorRect.xlength;

                    if (opening.geometry.ylength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Right Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Right;
                });

            lastEnd = sideData.exteriorRect.y;

            if (sideData.edgeList_Right.length > 0)
            {
                for (let line of sideData.edgeList_Right)
                {
                    if (line.startPoint.y != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Rear;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = this._options.geometry.xend;
                        opening.geometry.xlength = 0;
                        opening.geometry.y = lastEnd;
                        opening.geometry.ylength = line.startPoint.y - lastEnd;
                        opening.geometry.z = data.exteriorRect.x;
                        opening.geometry.zlength = data.exteriorRect.xlength;

                        if (opening.geometry.ylength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.y;
                }

                if (lastEnd != sideData.exteriorRect.yend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Rear;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = this._options.geometry.xend;
                    opening.geometry.xlength = 0;
                    opening.geometry.y = lastEnd;
                    opening.geometry.ylength = sideData.exteriorRect.yend - lastEnd;
                    opening.geometry.z = data.exteriorRect.x;
                    opening.geometry.zlength = data.exteriorRect.xlength;

                    if (opening.geometry.ylength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #endregion
        }
        else if (data.face === Enums.E_Location.Top)
        {
            // #region Top Face

            // #region Front Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Front;
                });

            lastEnd = sideData.exteriorRect.x;

            if (sideData.edgeList_Top.length > 0)
            {
                for (let line of sideData.edgeList_Top)
                {
                    if (line.startPoint.x != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Top;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = data.exteriorRect.x;
                        opening.geometry.xlength = data.exteriorRect.xlength;
                        opening.geometry.y = this._options.geometry.yend;
                        opening.geometry.ylength = 0;
                        opening.geometry.z = lastEnd;
                        opening.geometry.zlength = line.startPoint.x - lastEnd;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.x;
                }

                if (lastEnd != sideData.exteriorRect.xend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Top;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = data.exteriorRect.x;
                    opening.geometry.xlength = data.exteriorRect.xlength;
                    opening.geometry.y = this._options.geometry.yend;
                    opening.geometry.ylength = 0;
                    opening.geometry.z = lastEnd;
                    opening.geometry.zlength = sideData.exteriorRect.xend - lastEnd;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Rear Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Rear;
                });

            lastEnd = sideData.exteriorRect.x;

            if (sideData.edgeList_Top.length > 0)
            {
                for (let line of sideData.edgeList_Top)
                {
                    if (line.startPoint.x != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Top;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = data.exteriorRect.x;
                        opening.geometry.xlength = data.exteriorRect.xlength;
                        opening.geometry.y = this._options.geometry.yend;
                        opening.geometry.ylength = 0;
                        opening.geometry.z = lastEnd;
                        opening.geometry.zlength = line.startPoint.x - lastEnd;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.x;
                }

                if (lastEnd != sideData.exteriorRect.xend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Top;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = data.exteriorRect.x;
                    opening.geometry.xlength = data.exteriorRect.xlength;
                    opening.geometry.y = this._options.geometry.yend;
                    opening.geometry.ylength = 0;
                    opening.geometry.z = lastEnd;
                    opening.geometry.zlength = sideData.exteriorRect.xend - lastEnd;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Left Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Left;
                });

            lastEnd = sideData.exteriorRect.x;

            if (sideData.edgeList_Top.length > 0)
            {
                for (let line of sideData.edgeList_Top)
                {
                    if (line.startPoint.x != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Top;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = lastEnd;
                        opening.geometry.xlength = line.startPoint.x - lastEnd;
                        opening.geometry.y = this._options.geometry.yend;
                        opening.geometry.ylength = 0;
                        opening.geometry.z = data.exteriorRect.y;
                        opening.geometry.zlength = data.exteriorRect.ylength;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.x;
                }

                if (lastEnd != sideData.exteriorRect.xend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Top;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = lastEnd;
                    opening.geometry.xlength = sideData.exteriorRect.xend - lastEnd;
                    opening.geometry.y = this._options.geometry.yend;
                    opening.geometry.ylength = 0;
                    opening.geometry.z = data.exteriorRect.y;
                    opening.geometry.zlength = data.exteriorRect.ylength;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Right Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Right;
                });

            lastEnd = sideData.exteriorRect.x;

            if (sideData.edgeList_Top.length > 0)
            {
                for (let line of sideData.edgeList_Top)
                {
                    if (line.startPoint.x != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Top;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = lastEnd;
                        opening.geometry.xlength = line.startPoint.x - lastEnd;
                        opening.geometry.y = this._options.geometry.yend;
                        opening.geometry.ylength = 0;
                        opening.geometry.z = data.exteriorRect.y;
                        opening.geometry.zlength = data.exteriorRect.ylength;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.x;
                }

                if (lastEnd != sideData.exteriorRect.xend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Top;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = lastEnd;
                    opening.geometry.xlength = sideData.exteriorRect.xend - lastEnd;
                    opening.geometry.y = this._options.geometry.yend;
                    opening.geometry.ylength = 0;
                    opening.geometry.z = data.exteriorRect.y;
                    opening.geometry.zlength = data.exteriorRect.ylength;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #endregion
        }
        else if (data.face === Enums.E_Location.Bottom)
        {
            // #region Bottom Face

            // #region Front Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Front;
                });

            lastEnd = sideData.exteriorRect.x;

            if (sideData.edgeList_Bottom.length > 0)
            {
                for (let line of sideData.edgeList_Bottom)
                {
                    if (line.startPoint.x != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Bottom;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = data.exteriorRect.x;
                        opening.geometry.xlength = data.exteriorRect.xlength;
                        opening.geometry.y = this._options.geometry.y;
                        opening.geometry.ylength = 0;
                        opening.geometry.z = lastEnd;
                        opening.geometry.zlength = line.startPoint.x - lastEnd;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.x;
                }

                if (lastEnd != sideData.exteriorRect.xend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Bottom;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = data.exteriorRect.x;
                    opening.geometry.xlength = data.exteriorRect.xlength;
                    opening.geometry.y = this._options.geometry.y;
                    opening.geometry.ylength = 0;
                    opening.geometry.z = lastEnd;
                    opening.geometry.zlength = sideData.exteriorRect.xend - lastEnd;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Rear Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Rear;
                });

            lastEnd = sideData.exteriorRect.x;

            if (sideData.edgeList_Bottom.length > 0)
            {
                for (let line of sideData.edgeList_Bottom)
                {
                    if (line.startPoint.x != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Bottom;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = data.exteriorRect.x;
                        opening.geometry.xlength = data.exteriorRect.xlength;
                        opening.geometry.y = this._options.geometry.y;
                        opening.geometry.ylength = 0;
                        opening.geometry.z = lastEnd;
                        opening.geometry.zlength = line.startPoint.x - lastEnd;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.x;
                }

                if (lastEnd != sideData.exteriorRect.xend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Bottom;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = data.exteriorRect.x;
                    opening.geometry.xlength = data.exteriorRect.xlength;
                    opening.geometry.y = this._options.geometry.y;
                    opening.geometry.ylength = 0;
                    opening.geometry.z = lastEnd;
                    opening.geometry.zlength = sideData.exteriorRect.xend - lastEnd;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Left Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Left;
                });

            lastEnd = sideData.exteriorRect.x;

            if (sideData.edgeList_Bottom.length > 0)
            {
                for (let line of sideData.edgeList_Bottom)
                {
                    if (line.startPoint.x != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Bottom;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = lastEnd;
                        opening.geometry.xlength = line.startPoint.x - lastEnd;
                        opening.geometry.y = this._options.geometry.y;
                        opening.geometry.ylength = 0;
                        opening.geometry.z = data.exteriorRect.y;
                        opening.geometry.zlength = data.exteriorRect.ylength;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.x;
                }

                if (lastEnd != sideData.exteriorRect.xend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Bottom;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = lastEnd;
                    opening.geometry.xlength = sideData.exteriorRect.xend - lastEnd;
                    opening.geometry.y = this._options.geometry.y;
                    opening.geometry.ylength = 0;
                    opening.geometry.z = data.exteriorRect.y;
                    opening.geometry.zlength = data.exteriorRect.ylength;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Right Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Right;
                });

            lastEnd = sideData.exteriorRect.x;

            if (sideData.edgeList_Bottom.length > 0)
            {
                for (let line of sideData.edgeList_Bottom)
                {
                    if (line.startPoint.x != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Bottom;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = lastEnd;
                        opening.geometry.xlength = line.startPoint.x - lastEnd;
                        opening.geometry.y = this._options.geometry.y;
                        opening.geometry.ylength = 0;
                        opening.geometry.z = data.exteriorRect.y;
                        opening.geometry.zlength = data.exteriorRect.ylength;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.zlength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.x;
                }

                if (lastEnd != sideData.exteriorRect.xend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Bottom;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = lastEnd;
                    opening.geometry.xlength = sideData.exteriorRect.xend - lastEnd;
                    opening.geometry.y = this._options.geometry.y;
                    opening.geometry.ylength = 0;
                    opening.geometry.z = data.exteriorRect.y;
                    opening.geometry.zlength = data.exteriorRect.ylength;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.zlength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #endregion
        }
        else if (data.face === Enums.E_Location.Left)
        {
            // #region Left Face

            // #region Front Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Front;
                });

            lastEnd = sideData.exteriorRect.y;

            if (sideData.edgeList_Left.length > 0)
            {
                for (let line of sideData.edgeList_Left)
                {
                    if (line.startPoint.y != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Left;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = data.exteriorRect.x;
                        opening.geometry.xlength = data.exteriorRect.xlength;
                        opening.geometry.y = lastEnd;
                        opening.geometry.ylength = line.startPoint.y - lastEnd;
                        opening.geometry.z = this._options.geometry.z;
                        opening.geometry.zlength = 0;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.ylength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.y;
                }

                if (lastEnd != sideData.exteriorRect.yend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Left;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = data.exteriorRect.x;
                    opening.geometry.xlength = data.exteriorRect.xlength;
                    opening.geometry.y = lastEnd;
                    opening.geometry.ylength = sideData.exteriorRect.yend - lastEnd;
                    opening.geometry.z = this._options.geometry.z;
                    opening.geometry.zlength = 0;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.ylength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Rear Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Rear;
                });

            lastEnd = sideData.exteriorRect.y;

            if (sideData.edgeList_Left.length > 0)
            {
                for (let line of sideData.edgeList_Left)
                {
                    if (line.startPoint.y != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Left;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = data.exteriorRect.x;
                        opening.geometry.xlength = data.exteriorRect.xlength;
                        opening.geometry.y = lastEnd;
                        opening.geometry.ylength = line.startPoint.y - lastEnd;
                        opening.geometry.z = this._options.geometry.z;
                        opening.geometry.zlength = 0;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.ylength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.y;
                }

                if (lastEnd != sideData.exteriorRect.yend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Left;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = data.exteriorRect.x;
                    opening.geometry.xlength = data.exteriorRect.xlength;
                    opening.geometry.y = lastEnd;
                    opening.geometry.ylength = sideData.exteriorRect.yend - lastEnd;
                    opening.geometry.z = this._options.geometry.z;
                    opening.geometry.zlength = 0;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.ylength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Top Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Top;
                });

            lastEnd = sideData.exteriorRect.x;

            if (sideData.edgeList_Bottom.length > 0)
            {
                for (let line of sideData.edgeList_Bottom)
                {
                    if (line.startPoint.x != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Left;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = lastEnd;
                        opening.geometry.xlength = line.startPoint.x - lastEnd;
                        opening.geometry.y = data.exteriorRect.y;
                        opening.geometry.ylength = data.exteriorRect.ylength;
                        opening.geometry.z = this._options.geometry.z;
                        opening.geometry.zlength = 0;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.ylength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.x;
                }

                if (lastEnd != sideData.exteriorRect.xend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Left;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = lastEnd;
                    opening.geometry.xlength = sideData.exteriorRect.xend - lastEnd;
                    opening.geometry.y = data.exteriorRect.y;
                    opening.geometry.ylength = data.exteriorRect.ylength;
                    opening.geometry.z = this._options.geometry.z;
                    opening.geometry.zlength = 0;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.ylength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Bottom Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Bottom;
                });

            lastEnd = sideData.exteriorRect.x;

            if (sideData.edgeList_Bottom.length > 0)
            {
                for (let line of sideData.edgeList_Bottom)
                {
                    if (line.startPoint.x != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Left;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = lastEnd;
                        opening.geometry.xlength = line.startPoint.x - lastEnd;
                        opening.geometry.y = data.exteriorRect.y;
                        opening.geometry.ylength = data.exteriorRect.ylength;
                        opening.geometry.z = this._options.geometry.z;
                        opening.geometry.zlength = 0;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.ylength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.x;
                }

                if (lastEnd != sideData.exteriorRect.xend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Left;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = lastEnd;
                    opening.geometry.xlength = sideData.exteriorRect.xend - lastEnd;
                    opening.geometry.y = data.exteriorRect.y;
                    opening.geometry.ylength = data.exteriorRect.ylength;
                    opening.geometry.z = this._options.geometry.z;
                    opening.geometry.zlength = 0;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.ylength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #endregion
        }
        else if (data.face === Enums.E_Location.Right)
        {
            // #region Right Face

            // #region Front Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Front;
                });

            lastEnd = sideData.exteriorRect.y;

            if (sideData.edgeList_Right.length > 0)
            {
                for (let line of sideData.edgeList_Right)
                {
                    if (line.startPoint.y != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Right;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = data.exteriorRect.x;
                        opening.geometry.xlength = data.exteriorRect.xlength;
                        opening.geometry.y = lastEnd;
                        opening.geometry.ylength = line.startPoint.y - lastEnd;
                        opening.geometry.z = this._options.geometry.zend;
                        opening.geometry.zlength = 0;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.ylength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.y;
                }

                if (lastEnd != sideData.exteriorRect.yend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Right;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = data.exteriorRect.x;
                    opening.geometry.xlength = data.exteriorRect.xlength;
                    opening.geometry.y = lastEnd;
                    opening.geometry.ylength = sideData.exteriorRect.yend - lastEnd;
                    opening.geometry.z = this._options.geometry.zend;
                    opening.geometry.zlength = 0;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.ylength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Rear Side Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Rear;
                });

            lastEnd = sideData.exteriorRect.y;

            if (sideData.edgeList_Right.length > 0)
            {
                for (let line of sideData.edgeList_Right)
                {
                    if (line.startPoint.y != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Right;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = data.exteriorRect.x;
                        opening.geometry.xlength = data.exteriorRect.xlength;
                        opening.geometry.y = lastEnd;
                        opening.geometry.ylength = line.startPoint.y - lastEnd;
                        opening.geometry.z = this._options.geometry.zend;
                        opening.geometry.zlength = 0;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.ylength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.y;
                }

                if (lastEnd != sideData.exteriorRect.yend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Right;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = data.exteriorRect.x;
                    opening.geometry.xlength = data.exteriorRect.xlength;
                    opening.geometry.y = lastEnd;
                    opening.geometry.ylength = sideData.exteriorRect.yend - lastEnd;
                    opening.geometry.z = this._options.geometry.zend;
                    opening.geometry.zlength = 0;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.ylength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Top Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Top;
                });

            lastEnd = sideData.exteriorRect.x;

            if (sideData.edgeList_Top.length > 0)
            {
                for (let line of sideData.edgeList_Top)
                {
                    if (line.startPoint.x != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Right;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = lastEnd;
                        opening.geometry.xlength = line.startPoint.x - lastEnd;
                        opening.geometry.y = data.exteriorRect.y;
                        opening.geometry.ylength = data.exteriorRect.ylength;
                        opening.geometry.z = this._options.geometry.zend;
                        opening.geometry.zlength = 0;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.ylength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.x;
                }

                if (lastEnd != sideData.exteriorRect.xend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Right;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = lastEnd;
                    opening.geometry.xlength = sideData.exteriorRect.xend - lastEnd;
                    opening.geometry.y = data.exteriorRect.y;
                    opening.geometry.ylength = data.exteriorRect.ylength;
                    opening.geometry.z = this._options.geometry.zend;
                    opening.geometry.zlength = 0;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.ylength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #region Bottom Spanned Openings

            sideData = _.find(
                this._facePolygonDataList,
                function (i: CFacePolygonData): boolean
                {
                    return i.face === Enums.E_Location.Bottom;
                });

            lastEnd = sideData.exteriorRect.x;

            if (sideData.edgeList_Top.length > 0)
            {
                for (let line of sideData.edgeList_Top)
                {
                    if (line.startPoint.x != lastEnd)
                    {
                        opening = new C3DOpening();

                        opening.face = Enums.E_Location.Right;
                        opening.isValid = true;
                        opening.shape = Enums.E_OpeningShape.Rectangle;

                        opening.geometry.x = lastEnd;
                        opening.geometry.xlength = line.startPoint.x - lastEnd;
                        opening.geometry.y = data.exteriorRect.y;
                        opening.geometry.ylength = data.exteriorRect.ylength;
                        opening.geometry.z = this._options.geometry.zend;
                        opening.geometry.zlength = 0;

                        if (opening.geometry.xlength > CConstants.EPSILON &&
                            opening.geometry.ylength > CConstants.EPSILON)
                        {
                            openingList.push(opening);
                        }
                    }

                    lastEnd = line.endPoint.x;
                }

                if (lastEnd != sideData.exteriorRect.xend)
                {
                    opening = new C3DOpening();

                    opening.face = Enums.E_Location.Right;
                    opening.isValid = true;
                    opening.shape = Enums.E_OpeningShape.Rectangle;

                    opening.geometry.x = lastEnd;
                    opening.geometry.xlength = sideData.exteriorRect.xend - lastEnd;
                    opening.geometry.y = data.exteriorRect.y;
                    opening.geometry.ylength = data.exteriorRect.ylength;
                    opening.geometry.z = this._options.geometry.zend;
                    opening.geometry.zlength = 0;

                    if (opening.geometry.xlength > CConstants.EPSILON &&
                        opening.geometry.ylength > CConstants.EPSILON)
                    {
                        openingList.push(opening);
                    }
                }
            }

            // #endregion

            // #endregion
        }
        
        return openingList;
    }
    
    private generateGeometryFromOpeningEdgeProfile(
        profile: COpeningEdgeProfile,
        face: Enums.E_Location)
    {
        let geometryList = [];
        let geometry;
        let nextPointOuter: Poly2Tri.Point;
        let nextPointInner: Poly2Tri.Point;
        let lastPointOuter: Poly2Tri.Point;
        let lastPointInner: Poly2Tri.Point;

        for (let x = 0; x < profile.vertexCount; x++)
        {
            if (x == 0)
            {
                lastPointInner = profile.innerPoints[profile.vertexCount - 1];
                lastPointOuter = profile.outerPoints[profile.vertexCount - 1];
            }

            nextPointInner = profile.innerPoints[x];
            nextPointOuter = profile.outerPoints[x];

            // #region Render Exterior Profile (on Face) Based On Location

            geometry = new THREE.Geometry();

            if (face === Enums.E_Location.Front)
            {
                // #region Front

                geometry.vertices.push(new THREE.Vector3(
                    this._options.geometry.x,
                    lastPointOuter.y,
                    lastPointOuter.x));

                geometry.vertices.push(new THREE.Vector3(
                    this._options.geometry.x,
                    nextPointOuter.y,
                    nextPointOuter.x));

                geometry.vertices.push(new THREE.Vector3(
                    this._options.geometry.x,
                    nextPointInner.y,
                    nextPointInner.x));

                geometry.vertices.push(new THREE.Vector3(
                    this._options.geometry.x,
                    lastPointInner.y,
                    lastPointInner.x));

                geometry.faces.push(new THREE.Face3(0, 2, 1));
                geometry.faces.push(new THREE.Face3(0, 3, 2));

                // #endregion
            }
            else if (face === Enums.E_Location.Rear)
            {
                // #region Rear

                geometry.vertices.push(new THREE.Vector3(
                    this._options.geometry.xend,
                    lastPointOuter.y,
                    lastPointOuter.x));

                geometry.vertices.push(new THREE.Vector3(
                    this._options.geometry.xend,
                    nextPointOuter.y,
                    nextPointOuter.x));

                geometry.vertices.push(new THREE.Vector3(
                    this._options.geometry.xend,
                    nextPointInner.y,
                    nextPointInner.x));

                geometry.vertices.push(new THREE.Vector3(
                    this._options.geometry.xend,
                    lastPointInner.y,
                    lastPointInner.x));

                geometry.faces.push(new THREE.Face3(1, 2, 0));
                geometry.faces.push(new THREE.Face3(2, 3, 0));

                // #endregion
            }
            else if (face === Enums.E_Location.Bottom)
            {
                // #region Bottom

                geometry.vertices.push(new THREE.Vector3(
                    lastPointOuter.x,
                    this._options.geometry.y,
                    lastPointOuter.y));

                geometry.vertices.push(new THREE.Vector3(
                    nextPointOuter.x,
                    this._options.geometry.y,
                    nextPointOuter.y));

                geometry.vertices.push(new THREE.Vector3(
                    nextPointInner.x,
                    this._options.geometry.y,
                    nextPointInner.y));

                geometry.vertices.push(new THREE.Vector3(
                    lastPointInner.x,
                    this._options.geometry.y,
                    lastPointInner.y));

                geometry.faces.push(new THREE.Face3(0, 2, 1));
                geometry.faces.push(new THREE.Face3(0, 3, 2));

                // #endregion
            }
            else if (face === Enums.E_Location.Top)
            {
                // #region Top

                geometry.vertices.push(new THREE.Vector3(
                    lastPointOuter.x,
                    this._options.geometry.yend,
                    lastPointOuter.y));

                geometry.vertices.push(new THREE.Vector3(
                    nextPointOuter.x,
                    this._options.geometry.yend,
                    nextPointOuter.y));

                geometry.vertices.push(new THREE.Vector3(
                    nextPointInner.x,
                    this._options.geometry.yend,
                    nextPointInner.y));

                geometry.vertices.push(new THREE.Vector3(
                    lastPointInner.x,
                    this._options.geometry.yend,
                    lastPointInner.y));

                geometry.faces.push(new THREE.Face3(1, 2, 0));
                geometry.faces.push(new THREE.Face3(2, 3, 0));

                // #endregion
            }
            else if (face === Enums.E_Location.Left)
            {
                // #region Left

                geometry.vertices.push(new THREE.Vector3(
                    lastPointOuter.x,
                    lastPointOuter.y,
                    this._options.geometry.z));

                geometry.vertices.push(new THREE.Vector3(
                    nextPointOuter.x,
                    nextPointOuter.y,
                    this._options.geometry.z));

                geometry.vertices.push(new THREE.Vector3(
                    nextPointInner.x,
                    nextPointInner.y,
                    this._options.geometry.z));

                geometry.vertices.push(new THREE.Vector3(
                    lastPointInner.x,
                    lastPointInner.y,
                    this._options.geometry.z));

                geometry.faces.push(new THREE.Face3(1, 2, 0));
                geometry.faces.push(new THREE.Face3(2, 3, 0));

                // #endregion
            }
            else if (face === Enums.E_Location.Right)
            {
                // #region Right

                geometry.vertices.push(new THREE.Vector3(
                    lastPointOuter.x,
                    lastPointOuter.y,
                    this._options.geometry.zend));

                geometry.vertices.push(new THREE.Vector3(
                    nextPointOuter.x,
                    nextPointOuter.y,
                    this._options.geometry.zend));

                geometry.vertices.push(new THREE.Vector3(
                    nextPointInner.x,
                    nextPointInner.y,
                    this._options.geometry.zend));

                geometry.vertices.push(new THREE.Vector3(
                    lastPointInner.x,
                    lastPointInner.y,
                    this._options.geometry.zend));

                geometry.faces.push(new THREE.Face3(0, 2, 1));
                geometry.faces.push(new THREE.Face3(0, 3, 2));

                // #endregion
            }

            geometryList.push(geometry);

            // #endregion

            if (face === this._options.faceLocation)
            {
                // #region Render Interior Profile (inside opening) Based on Location

                geometry = new THREE.Geometry();

                if (face === Enums.E_Location.Front)
                {
                    // #region Front

                    geometry.vertices.push(new THREE.Vector3(
                        this._options.geometry.xend,
                        lastPointInner.y,
                        lastPointInner.x));

                    geometry.vertices.push(new THREE.Vector3(
                        this._options.geometry.xend,
                        nextPointInner.y,
                        nextPointInner.x));

                    geometry.vertices.push(new THREE.Vector3(
                        this._options.geometry.x,
                        nextPointInner.y,
                        nextPointInner.x));

                    geometry.vertices.push(new THREE.Vector3(
                        this._options.geometry.x,
                        lastPointInner.y,
                        lastPointInner.x));

                    geometry.faces.push(new THREE.Face3(1, 2, 0));
                    geometry.faces.push(new THREE.Face3(2, 3, 0));

                    // #endregion
                }
                else if (face === Enums.E_Location.Rear)
                {
                    // #region Rear

                    geometry.vertices.push(new THREE.Vector3(
                        this._options.geometry.x,
                        lastPointInner.y,
                        lastPointInner.x));

                    geometry.vertices.push(new THREE.Vector3(
                        this._options.geometry.x,
                        nextPointInner.y,
                        nextPointInner.x));

                    geometry.vertices.push(new THREE.Vector3(
                        this._options.geometry.xend,
                        nextPointInner.y,
                        nextPointInner.x));

                    geometry.vertices.push(new THREE.Vector3(
                        this._options.geometry.xend,
                        lastPointInner.y,
                        lastPointInner.x));

                    geometry.faces.push(new THREE.Face3(0, 2, 1));
                    geometry.faces.push(new THREE.Face3(0, 3, 2));

                    // #endregion
                }
                else if (face === Enums.E_Location.Bottom)
                {
                    // #region Bottom

                    geometry.vertices.push(new THREE.Vector3(
                        lastPointInner.x,
                        this._options.geometry.yend,
                        lastPointInner.y));

                    geometry.vertices.push(new THREE.Vector3(
                        nextPointInner.x,
                        this._options.geometry.yend,
                        nextPointInner.y));

                    geometry.vertices.push(new THREE.Vector3(
                        nextPointInner.x,
                        this._options.geometry.y,
                        nextPointInner.y));

                    geometry.vertices.push(new THREE.Vector3(
                        lastPointInner.x,
                        this._options.geometry.y,
                        lastPointInner.y));

                    geometry.faces.push(new THREE.Face3(1, 2, 0));
                    geometry.faces.push(new THREE.Face3(2, 3, 0));

                    // #endregion
                }
                else if (face === Enums.E_Location.Top)
                {
                    // #region Top

                    geometry.vertices.push(new THREE.Vector3(
                        lastPointInner.x,
                        this._options.geometry.y,
                        lastPointInner.y));

                    geometry.vertices.push(new THREE.Vector3(
                        nextPointInner.x,
                        this._options.geometry.y,
                        nextPointInner.y));

                    geometry.vertices.push(new THREE.Vector3(
                        nextPointInner.x,
                        this._options.geometry.yend,
                        nextPointInner.y));

                    geometry.vertices.push(new THREE.Vector3(
                        lastPointInner.x,
                        this._options.geometry.yend,
                        lastPointInner.y));

                    geometry.faces.push(new THREE.Face3(0, 2, 1));
                    geometry.faces.push(new THREE.Face3(0, 3, 2));

                    // #endregion
                }
                else if (face === Enums.E_Location.Left)
                {
                    // #region Left

                    geometry.vertices.push(new THREE.Vector3(
                        lastPointInner.x,
                        lastPointInner.y,
                        this._options.geometry.zend));

                    geometry.vertices.push(new THREE.Vector3(
                        nextPointInner.x,
                        nextPointInner.y,
                        this._options.geometry.zend));

                    geometry.vertices.push(new THREE.Vector3(
                        nextPointInner.x,
                        nextPointInner.y,
                        this._options.geometry.z));

                    geometry.vertices.push(new THREE.Vector3(
                        lastPointInner.x,
                        lastPointInner.y,
                        this._options.geometry.z));

                    geometry.faces.push(new THREE.Face3(0, 2, 1));
                    geometry.faces.push(new THREE.Face3(0, 3, 2));

                    // #endregion
                }
                else if (face === Enums.E_Location.Right)
                {
                    // #region Right

                    geometry.vertices.push(new THREE.Vector3(
                        lastPointInner.x,
                        lastPointInner.y,
                        this._options.geometry.z));

                    geometry.vertices.push(new THREE.Vector3(
                        nextPointInner.x,
                        nextPointInner.y,
                        this._options.geometry.z));

                    geometry.vertices.push(new THREE.Vector3(
                        nextPointInner.x,
                        nextPointInner.y,
                        this._options.geometry.zend));

                    geometry.vertices.push(new THREE.Vector3(
                        lastPointInner.x,
                        lastPointInner.y,
                        this._options.geometry.zend));

                    geometry.faces.push(new THREE.Face3(1, 2, 0));
                    geometry.faces.push(new THREE.Face3(2, 3, 0));

                    // #endregion
                }

                geometryList.push(geometry);

                // #endregion
            }

            lastPointInner = nextPointInner;
            lastPointOuter = nextPointOuter;
        }

        return geometryList;
    }
}
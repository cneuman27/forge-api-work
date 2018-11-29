import * as Enums from "./Enums"; 
import { CFaceVectorData } from "./Types/CFaceVectorData";

declare const THREE: any;

export class CConstants
{
    public static BORDER_DIM: number = 0.150;

    public static OVAL_SLICE_COUNT: number = 32;
    public static OVAL_THETA: number = ((2 * Math.PI) / CConstants.OVAL_SLICE_COUNT);

    public static GEOMETRY_PRECISION: number = 4;
    public static CLIPPER_SCALE: number = Math.pow(10, CConstants.GEOMETRY_PRECISION);
    public static EPSILON: number = Math.pow(10, -CConstants.GEOMETRY_PRECISION);

    public static DEFAULT_MATERIAL_EXTERIOR = new THREE.MeshBasicMaterial({ color: 0x0000ff });
    public static DEFAULT_MATERIAL_INTERIOR = new THREE.MeshBasicMaterial({ color: 0xc0c0c0 });
    public static DEFAULT_MATERIAL_EDGE = new THREE.MeshBasicMaterial({ color: 0x000000 });

    public static FACE_VECTOR_DATA: CFaceVectorData[] =
    [
        new CFaceVectorData(
            {
                face: Enums.E_Location.Front,
                normal: new THREE.Vector3(-1, 0, 0),
                topDirection: new THREE.Vector3(0, 1, 0)
            }),
        new CFaceVectorData(
            {
                face: Enums.E_Location.Rear,
                normal: new THREE.Vector3(1, 0, 0),
                topDirection: new THREE.Vector3(0, 1, 0)
            }),
        new CFaceVectorData(
            {
                face: Enums.E_Location.Bottom,
                normal: new THREE.Vector3(0, -1, 0),
                topDirection: new THREE.Vector3(0, 0, 1)
            }),
        new CFaceVectorData(
            {
                face: Enums.E_Location.Top,
                normal: new THREE.Vector3(0, 1, 0),
                topDirection: new THREE.Vector3(0, 0, -1)
            }),
        new CFaceVectorData(
            {
                face: Enums.E_Location.Left,
                normal: new THREE.Vector3(0, 0, -1),
                topDirection: new THREE.Vector3(0, 1, 0)
            }),
        new CFaceVectorData(
            {
                face: Enums.E_Location.Right,
                normal: new THREE.Vector3(0, 0, 1),
                topDirection: new THREE.Vector3(0, 1, 0)
            })
    ];
    public static OPPOSITE_FACE_LOOKUP: Map<Enums.E_Location, Enums.E_Location> = null;

    public static X_AXIS = new THREE.Vector3(1, 0, 0);
    public static Y_AXIS = new THREE.Vector3(0, 1, 0);
    public static Z_AXIS = new THREE.Vector3(0, 0, 1);

    public static initialize()
    {
        if (CConstants.OPPOSITE_FACE_LOOKUP === null)
        {
            CConstants.OPPOSITE_FACE_LOOKUP = new Map<Enums.E_Location, Enums.E_Location>();

            CConstants.OPPOSITE_FACE_LOOKUP.set(Enums.E_Location.Front, Enums.E_Location.Rear);
            CConstants.OPPOSITE_FACE_LOOKUP.set(Enums.E_Location.Rear, Enums.E_Location.Front);
            CConstants.OPPOSITE_FACE_LOOKUP.set(Enums.E_Location.Bottom, Enums.E_Location.Top);
            CConstants.OPPOSITE_FACE_LOOKUP.set(Enums.E_Location.Top, Enums.E_Location.Bottom);
            CConstants.OPPOSITE_FACE_LOOKUP.set(Enums.E_Location.Left, Enums.E_Location.Right);
            CConstants.OPPOSITE_FACE_LOOKUP.set(Enums.E_Location.Right, Enums.E_Location.Left);
        }
    }
}
CConstants.initialize();
import { C3DGeometry } from "../Types/C3DGeometry";
import { C3DOpening } from "../Types/C3DOpening";
import { CConstants } from "../CConstants";

import * as Enums from "../Enums";

declare const THREE: any;

export class C3DRenderOptions_SolidModel
{
    private _geometry: C3DGeometry = new C3DGeometry();

    private _solidType: Enums.E_SolidType = Enums.E_SolidType.Cuboid;
    private _faceLocation: Enums.E_Location = Enums.E_Location.Undefined;

    private _faceType_Front: Enums.E_FaceType = Enums.E_FaceType.Exterior;
    private _faceType_Rear: Enums.E_FaceType = Enums.E_FaceType.Exterior;
    private _faceType_Bottom: Enums.E_FaceType = Enums.E_FaceType.Exterior;
    private _faceType_Top: Enums.E_FaceType = Enums.E_FaceType.Exterior;
    private _faceType_Left: Enums.E_FaceType = Enums.E_FaceType.Exterior;
    private _faceType_Right: Enums.E_FaceType = Enums.E_FaceType.Exterior;

    private _renderBorder: boolean = true;
    private _borderDim: number = CConstants.BORDER_DIM;
    
    private _material_Exterior = CConstants.DEFAULT_MATERIAL_EXTERIOR;
    private _material_Interior = CConstants.DEFAULT_MATERIAL_INTERIOR;
    private _material_Edge = CConstants.DEFAULT_MATERIAL_EDGE;

    private _openingList: C3DOpening[] = [];

    public get geometry(): C3DGeometry
    {
        return this._geometry;
    }
    public set geometry(value: C3DGeometry)
    {
        this._geometry = value;
    }

    public get solidType(): Enums.E_SolidType
    {
        return this._solidType
    }
    public set solidType(value: Enums.E_SolidType)
    {
        this._solidType = value;
    }

    public get faceLocation(): Enums.E_Location
    {
        return this._faceLocation;
    }
    public set faceLocation(value: Enums.E_Location)
    {
        this._faceLocation = value;
    }

    public get faceType_Front(): Enums.E_FaceType
    {
        return this._faceType_Front;
    }
    public set faceType_Front(value: Enums.E_FaceType)
    {
        this._faceType_Front = value;
    }

    public get faceType_Rear(): Enums.E_FaceType
    {
        return this._faceType_Rear;
    }
    public set faceType_Rear(value: Enums.E_FaceType)
    {
        this._faceType_Rear = value;
    }

    public get faceType_Bottom(): Enums.E_FaceType
    {
        return this._faceType_Bottom;
    }
    public set faceType_Bottom(value: Enums.E_FaceType)
    {
        this._faceType_Bottom = value;
    }

    public get faceType_Top(): Enums.E_FaceType
    {
        return this._faceType_Top;
    }
    public set faceType_Top(value: Enums.E_FaceType)
    {
        this._faceType_Top = value;
    }

    public get faceType_Left(): Enums.E_FaceType
    {
        return this._faceType_Left;
    }
    public set faceType_Left(value: Enums.E_FaceType)
    {
        this._faceType_Left = value;
    }

    public get faceType_Right(): Enums.E_FaceType
    {
        return this._faceType_Right;
    }
    public set faceType_Right(value: Enums.E_FaceType)
    {
        this._faceType_Right = value;
    }

    public get renderBorder(): boolean
    {
        return this._renderBorder;
    }
    public set renderBorder(value: boolean)
    {
        this._renderBorder = value;
    }

    public get borderDim(): number
    {
        return this._borderDim; 
    }
    public set borderDim(value: number)
    {
        this._borderDim = value;
    }

    public get material_Exterior()
    {
        return this._material_Exterior;
    }
    public set material_Exterior(value)
    {
        this._material_Exterior = value;
    }

    public get material_Interior()
    {
        return this._material_Interior;
    }
    public set material_Interior(value)
    {
        this._material_Interior = value;
    }

    public get material_Edge()
    {
        return this._material_Edge;
    }
    public set material_Edge(value)
    {
        this._material_Edge = value;
    }

    public get openingList(): C3DOpening[]
    {
        return this._openingList;
    }
    public set openingList(value: C3DOpening[])
    {
        this._openingList = value;
    }
    
    public getFaceType(face: Enums.E_Location): Enums.E_FaceType
    {
        if (face === Enums.E_Location.Front)
        {
            return this.faceType_Front;
        }

        if (face === Enums.E_Location.Rear)
        {
            return this.faceType_Rear;
        }

        if (face === Enums.E_Location.Bottom)
        {
            return this.faceType_Bottom;
        }

        if (face === Enums.E_Location.Top)
        {
            return this.faceType_Top;
        }

        if (face === Enums.E_Location.Left)
        {
            return this.faceType_Left;
        }

        if (face === Enums.E_Location.Right)
        {
            return this.faceType_Right;
        }

        return Enums.E_FaceType.NoRender;
    }
}
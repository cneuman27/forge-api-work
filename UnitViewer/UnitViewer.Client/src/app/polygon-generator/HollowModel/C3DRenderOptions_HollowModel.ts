import { C3DGeometry } from "../Types/C3DGeometry";
import { C3DOpening } from "../Types/C3DOpening";
import { CConstants } from "../CConstants";

import * as Enums from "../Enums";

declare const THREE: any;

export class C3DRenderOptions_HollowModel
{
    private _geometry: C3DGeometry = new C3DGeometry();

    private _renderFacesSeparately = false;

    private _wallThickness_Front: number = 0;
    private _wallThickness_Rear: number = 0;
    private _wallThickness_Bottom: number = 0;
    private _wallThickness_Top: number = 0;
    private _wallThickness_Left: number = 0;
    private _wallThickness_Right: number = 0;

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

    public get renderFacesSeparately(): boolean
    {
        return this._renderFacesSeparately;
    }
    public set renderFacesSeparately(value: boolean)
    {
        this._renderFacesSeparately = value;
    }

    public get wallThickness_Front(): number
    {
        return this._wallThickness_Front;
    }
    public set wallThickness_Front(value: number)
    {
        this._wallThickness_Front = value;
    }

    public get wallThickness_Rear(): number
    {
        return this._wallThickness_Rear;
    }
    public set wallThickness_Rear(value: number)
    {
        this._wallThickness_Rear = value;
    }

    public get wallThickness_Bottom(): number
    {
        return this._wallThickness_Bottom;
    }
    public set wallThickness_Bottom(value: number)
    {
        this._wallThickness_Bottom = value;
    }

    public get wallThickness_Top(): number
    {
        return this._wallThickness_Top;
    }
    public set wallThickness_Top(value: number)
    {
        this._wallThickness_Top = value;
    }

    public get wallThickness_Left(): number
    {
        return this._wallThickness_Left;
    }
    public set wallThickness_Left(value: number)
    {
        this._wallThickness_Left = value;
    }

    public get wallThickness_Right(): number
    {
        return this._wallThickness_Right;
    }
    public set wallThickness_Right(value: number)
    {
        this._wallThickness_Right = value;
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
    
    public getWallThickness(face: Enums.E_Location): number
    {
        if (face === Enums.E_Location.Front)
        {
            return this.wallThickness_Front;
        }

        if (face === Enums.E_Location.Rear)
        {
            return this.wallThickness_Rear;
        }

        if (face === Enums.E_Location.Bottom)
        {
            return this.wallThickness_Bottom;
        }

        if (face === Enums.E_Location.Top)
        {
            return this.wallThickness_Top;
        }

        if (face === Enums.E_Location.Left)
        {
            return this.wallThickness_Left;
        }

        if (face === Enums.E_Location.Right)
        {
            return this.wallThickness_Right;
        }

        return 0;
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
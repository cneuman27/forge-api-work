import { C3DGeometry } from "./C3DGeometry";
import * as Enums from "../Enums";

export class C3DOpening
{
    private _geometry: C3DGeometry = new C3DGeometry();
    private _face: Enums.E_Location = Enums.E_Location.Undefined;
    private _shape: Enums.E_OpeningShape = Enums.E_OpeningShape.Rectangle;

    private _isValid: boolean = true;

    public get geometry(): C3DGeometry
    {
        return this._geometry;
    }
    public set geometry(value: C3DGeometry)
    {
        this._geometry = value;
    }

    public get face(): Enums.E_Location
    {
        return this._face; 
    }
    public set face(value: Enums.E_Location)
    {
        this._face = value;
    }

    public get shape(): Enums.E_OpeningShape
    {
        return this._shape;
    }
    public set shape(value: Enums.E_OpeningShape)
    {
        this._shape = value;
    }

    public get isValid(): boolean
    {
        return this._isValid;
    }
    public set isValid(value: boolean)
    {
        this._isValid = value;
    }
}
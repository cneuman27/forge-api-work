import { CRect3D } from "./CRect3D";

export class C3DGeometry
{
    private _x: number = 0;
    private _y: number = 0;
    private _z: number = 0;

    private _xlength: number = 0;
    private _ylength: number = 0;
    private _zlength: number = 0;

    constructor(
        x: number = 0,
        y: number = 0,
        z: number = 0,
        xlen: number = 0,
        ylen: number = 0,
        zlen: number = 0)
    {
        this._x = x;
        this._y = y;
        this._z = z;

        this._xlength = xlen;
        this._ylength = ylen;
        this._zlength = zlen;
    }

    public get x(): number
    {
        return this._x;
    }
    public set x(value: number)
    {
        this._x = value;
    }

    public get y(): number
    {
        return this._y;
    }
    public set y(value: number)
    {
        this._y = value;
    }

    public get z(): number
    {
        return this._z;
    }
    public set z(value: number)
    {
        this._z = value;
    }

    public get xlength(): number
    {
        return this._xlength;
    }
    public set xlength(value: number)
    {
        this._xlength = value;
    }

    public get ylength(): number
    {
        return this._ylength;
    }
    public set ylength(value: number)
    {
        this._ylength = value;
    }

    public get zlength(): number
    {
        return this._zlength;
    }
    public set zlength(value: number)
    {
        this._zlength = value;
    }

    public get xend(): number
    {
        return this.x + this.xlength;
    }
    public get yend(): number
    {
        return this.y + this.ylength;
    }
    public get zend(): number
    {
        return this.z + this.zlength;
    }

    public toRect3D(): CRect3D
    {
        let rect: CRect3D;

        rect = new CRect3D();

        rect.x = this.x;
        rect.y = this.y;
        rect.z = this.z;

        rect.xlength = this.xlength;
        rect.ylength = this.ylength;
        rect.zlength = this.zlength;

        return rect;
    }
}

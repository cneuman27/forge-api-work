import * as Enums from "../Enums";

declare const THREE: any;

export class CFaceVectorData
{
    constructor(params: any = { })
    {
        if (params["face"] !== undefined)
        {
            this._face = params["face"];
        }

        if (params["normal"] !== undefined)
        {
            this._normal = params["normal"];
        }

        if (params["topDirection"] !== undefined)
        {
            this._topDirection = params["topDirection"];
        }
    }

    private _face: Enums.E_Location = Enums.E_Location.Undefined;
    private _normal = new THREE.Vector3();
    private _topDirection = new THREE.Vector3();

    public get face(): Enums.E_Location
    {
        return this._face;
    }
    public set face(value: Enums.E_Location)
    {
        this._face = value;
    }

    public get normal()
    {
        return this._normal;
    }
    public set normal(value)
    {
        this._normal = value;
    }

    public get topDirection()
    {
        return this._topDirection; 
    }
    public set topDirection(value)
    {
        this._topDirection = value;
    }
}
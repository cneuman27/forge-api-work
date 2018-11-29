declare const THREE: any;

export class CGeometryCollection
{
    private _exteriors = [];
    private _interiors = [];
    private _edges = [];

    public get exteriors()
    {
        return this._exteriors;
    }
    public set exteriors(value)
    {
        this._exteriors = value;
    }

    public get interiors()
    {
        return this._interiors;
    }
    public set interiors(value)
    {
        this._interiors = value;
    }

    public get edges()
    {
        return this._edges;
    }
    public set edges(value)
    {
        this._edges = value;
    }
}
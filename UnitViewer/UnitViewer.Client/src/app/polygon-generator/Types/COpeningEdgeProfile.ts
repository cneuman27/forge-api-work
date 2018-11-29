import * as Poly2Tri from "poly2tri";

export class COpeningEdgeProfile
{
    private _vertexCount: number = 0;

    private _outerPoints: Poly2Tri.Point[] = [];
    private _innerPoints: Poly2Tri.Point[] = [];

    public get vertexCount(): number
    {
        return this._vertexCount;
    }
    public set vertexCount(value: number)
    {
        this._vertexCount = value;
    }

    public get outerPoints(): Poly2Tri.Point[]
    {
        return this._outerPoints;
    }
    public set outerPoints(value: Poly2Tri.Point[])
    {
        this._outerPoints = value;
    }

    public get innerPoints(): Poly2Tri.Point[]
    {
        return this._innerPoints;
    }
    public set innerPoints(value: Poly2Tri.Point[])
    {
        this._innerPoints = value;
    }
}
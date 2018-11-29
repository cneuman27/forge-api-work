declare const THREE: any;

export class CEdgeLine
{
    private _startPoint;
    private _endPoint;

    public get startPoint()
    {
        return this._startPoint;
    }
    public set startPoint(value)
    {
        this._startPoint = value;
    }

    public get endPoint()
    {
        return this._endPoint;
    }
    public set endPoint(value)
    {
        this._endPoint = value;
    }

    public isEqual(line: CEdgeLine): boolean
    {
        if (this.startPoint.x === line.startPoint.x &&
            this.startPoint.y === line.startPoint.y &&
            this.endPoint.x === line.endPoint.x &&
            this.endPoint.y === line.endPoint.y)
        {
            return true;
        }

        if (this.startPoint.x === line.endPoint.x &&
            this.startPoint.y === line.endPoint.y &&
            this.endPoint.x === line.startPoint.x &&
            this.endPoint.y === line.startPoint.y)
        {
            return true;
        }

        return false;
    }
}

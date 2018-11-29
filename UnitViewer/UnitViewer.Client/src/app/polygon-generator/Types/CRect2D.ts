export class CRect2D
{
    constructor(
        x: number = +Infinity,
        y: number = +Infinity,
        xlength: number = -Infinity,
        ylength: number = -Infinity)
    {
        this._x = x;
        this._y = y;

        this._xlength = xlength;
        this._ylength = ylength;
    }
    
    private _x: number = +Infinity;
    private _y: number = +Infinity;

    private _xlength: number = -Infinity;
    private _ylength: number = -Infinity;

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

    public get xend(): number
    {
        return this.x + this.xlength;
    }
    public get yend(): number
    {
        return this.y + this.ylength;
    }    

    public isEmpty(): boolean
    {
        return this.xend < this.x ||
               this.yend < this.y;
    }

    public intersectsWith(rect: CRect2D): boolean
    {
        let x1: number;
        let x2: number;
        let y1: number;
        let y2: number;

        x1 = Math.max(this.x, rect.x);
        x2 = Math.min(this.xend, rect.xend);
        y1 = Math.max(this.y, rect.y);
        y2 = Math.min(this.yend, rect.yend);

        if (x2 >= x1 &&
            y2 >= y1)
        {
            return true;
        }

        return false;
    }
}
import * as _ from "lodash";

declare const THREE: any;

export class CRect3D
{
    constructor(
        x: number = +Infinity,
        y: number = +Infinity,
        z: number = +Infinity,
        xlength: number = -Infinity,
        ylength: number = -Infinity,
        zlength: number = -Infinity)
    {
        this._x = x;
        this._y = y;
        this._z = z;

        this._xlength = xlength;
        this._ylength = ylength;
        this._zlength = zlength;
    }

    private _x: number = +Infinity;
    private _y: number = +Infinity;
    private _z: number = +Infinity;

    private _xlength: number = -Infinity;
    private _ylength: number = -Infinity;
    private _zlength: number = -Infinity;

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

    public isEmpty(): boolean
    {
        return this.xend < this.x ||
            this.yend < this.y ||
            this.zend < this.z;
    }

    public transformBounds(matrix)
    {
        let x1: number = this.x;
        let y1: number = this.y;
        let z1: number = this.z;
        let x2: number = this.xend;
        let y2: number = this.yend;
        let z2: number = this.zend;
        let v;

        let points =
            [
                new THREE.Vector3(x1, y1, z1),
                new THREE.Vector3(x1, y1, z2),
                new THREE.Vector3(x1, y2, z1),
                new THREE.Vector3(x1, y2, z2),
                new THREE.Vector3(x2, y1, z1),
                new THREE.Vector3(x2, y1, z2),
                new THREE.Vector3(x2, y2, z1),
                new THREE.Vector3(x2, y2, z2)
            ];

        _.forEach(
            points,
            function (i)
            {
                i.applyMatrix4(matrix);
            });

        v = points[0];

        x1 = v.x;
        y1 = v.y;
        z1 = v.z;

        x2 = x1;
        y2 = y1;
        z2 = z1;

        for (let i = 1; i < points.length; i++)
        {
            v = points[i];

            x1 = Math.min(x1, v.x);
            y1 = Math.min(y1, v.y);
            z1 = Math.min(z1, v.z);

            x2 = Math.max(x2, v.x);
            y2 = Math.max(y2, v.y);
            z2 = Math.max(z2, v.z);
        }

        this.x = x1;
        this.y = y1;
        this.z = z1;

        this.xlength = x2 - x1;
        this.ylength = y2 - y1;
        this.zlength = z2 - z1;
    }

    public static fromBox3(box): CRect3D
    {
        let tmp: CRect3D;

        tmp = new CRect3D(
            box.min.x,
            box.min.y,
            box.min.z,
            box.max.x - box.min.x,
            box.max.y - box.min.y,
            box.max.z - box.min.z);

        return tmp;
    }
}

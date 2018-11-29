import * as Poly2Tri from "poly2tri";
import * as _ from "lodash";
import * as ClipperLib from "@jci-ahu/ui.shared.clipper";

declare const THREE: any;

export class CPolygonData
{
    private _polygon_Clipper: ClipperLib.Path = [];
    private _polygon_Poly2Tri: Poly2Tri.Point[] = [];
    private _polygon_THREE = [];

    private _openingPolygonList: CPolygonData[] = [];

    public get polygon_Clipper(): ClipperLib.Path
    {
        return this._polygon_Clipper;
    }
    public set polygon_Clipper(value: ClipperLib.Path)
    {
        this._polygon_Clipper = value;

        this._polygon_Poly2Tri = [];
        this._polygon_THREE = [];

        let _this = this;

        _.forEach(
            this._polygon_Clipper,
            function (i: ClipperLib.IntPoint)
            {
                _this._polygon_Poly2Tri.push(new Poly2Tri.Point(i.X, i.Y));
                _this._polygon_THREE.push(new THREE.Vector2(i.X, i.Y));
            });
    }

    public get polygon_Poly2Tri(): Poly2Tri.Point[]
    {
        return this._polygon_Poly2Tri;
    }
    public set polygon_Poly2Tri(value: Poly2Tri.Point[])
    {
        this._polygon_Poly2Tri = value;

        this._polygon_Clipper = [];
        this._polygon_THREE = [];

        let _this = this;

        _.forEach(
            this._polygon_Poly2Tri,
            function (i: Poly2Tri.Point)
            {
                _this.polygon_Clipper.push(new ClipperLib.IntPoint(i.x, i.y));
                _this._polygon_THREE.push(new THREE.Vector2(i.x, i.y));
            });
    }

    public get polygon_THREE()
    {
        return this._polygon_THREE;
    }
    public set polygon_THREE(value)
    {
        this._polygon_THREE = value;

        this._polygon_Clipper = [];
        this._polygon_Poly2Tri = [];

        let _this = this;

        _.forEach(
            this._polygon_THREE,
            function (i)
            {
                _this._polygon_Clipper.push(new ClipperLib.IntPoint(i.x, i.y));
                _this._polygon_Poly2Tri.push(new Poly2Tri.Point(i.x, i.y));
            });
    }

    public get openingPolygonList(): CPolygonData[]
    {
        return this._openingPolygonList;
    }
    public set openingPolygonList(value: CPolygonData[])
    {
        this._openingPolygonList = value;
    }

    public AddPoint_Clipper(pt: ClipperLib.IntPoint): void
    {
        this._polygon_Clipper.push(pt);
        this._polygon_Poly2Tri.push(new Poly2Tri.Point(pt.X, pt.Y));
        this._polygon_THREE.push(new THREE.Vector2(pt.X, pt.Y));
    }
    public AddPoint_Poly2Tri(pt: Poly2Tri.Point): void
    {
        this._polygon_Clipper.push(new ClipperLib.IntPoint(pt.x, pt.y));
        this._polygon_Poly2Tri.push(pt);
        this._polygon_THREE.push(new THREE.Vector2(pt.x, pt.y));
    }
    public AddPoint_THREE(pt): void
    {
        this._polygon_Clipper.push(new ClipperLib.IntPoint(pt.x, pt.y));
        this._polygon_Poly2Tri.push(new Poly2Tri.Point(pt.x, pt.y));
        this._polygon_THREE.push(pt);
    }
}



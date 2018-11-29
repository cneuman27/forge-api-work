import * as _ from "lodash";

import { CPolygonData } from "./CPolygonData";
import { CEdgeLine } from "./CEdgeLine";
import { CRect2D } from "./CRect2D";
import * as Enums from "../Enums";

declare const THREE: any;

export class CFacePolygonData
{
    private _face: Enums.E_Location = Enums.E_Location.Undefined;

    private _exteriorRect: CRect2D = new CRect2D();
    private _interiorRect: CRect2D = new CRect2D();

    private _exteriorPolygonList: CPolygonData[] = [];
    private _interiorPolygonList: CPolygonData[] = [];

    private _edgeList_Top: CEdgeLine[] = [];
    private _edgeList_Bottom: CEdgeLine[] = [];
    private _edgeList_Left: CEdgeLine[] = [];
    private _edgeList_Right: CEdgeLine[] = [];

    public get face(): Enums.E_Location
    {
        return this._face;
    }
    public set face(value: Enums.E_Location)
    {
        this._face = value;
    }

    public get exteriorRect(): CRect2D
    {
        return this._exteriorRect;
    }
    public set exteriorRect(value: CRect2D)
    {
        this._exteriorRect = value;
    }

    public get interiorRect(): CRect2D
    {
        return this._interiorRect;
    }
    public set interiorRect(value: CRect2D)
    {
        this._interiorRect = value;
    }

    public get exteriorPolygonList(): CPolygonData[]
    {
        return this._exteriorPolygonList;
    }
    public set exteriorPolygonList(value: CPolygonData[])
    {
        this._exteriorPolygonList = value;
    }

    public get interiorPolygonList(): CPolygonData[]
    {
        return this._interiorPolygonList;
    }
    public set interiorPolygonList(value: CPolygonData[])
    {
        this._interiorPolygonList = value;
    }

    public get edgeList_Top(): CEdgeLine[]
    {
        return this._edgeList_Top; 
    }
    public set edgeList_Top(value: CEdgeLine[])
    {
        this._edgeList_Top = value;
    }

    public get edgeList_Bottom(): CEdgeLine[]
    {
        return this._edgeList_Bottom;
    }
    public set edgeList_Bottom(value: CEdgeLine[])
    {
        this._edgeList_Bottom = value;
    }

    public get edgeList_Left(): CEdgeLine[]
    {
        return this._edgeList_Left;
    }
    public set edgeList_Left(value: CEdgeLine[])
    {
        this._edgeList_Left = value;
    }

    public get edgeList_Right(): CEdgeLine[]
    {
        return this._edgeList_Right;
    }
    public set edgeList_Right(value: CEdgeLine[])
    {
        this._edgeList_Right = value;
    }

    public normalizeEdges(): void
    {
        let tmp: number;

        // #region Top Edges

        for (let line of this.edgeList_Top)
        {
            if (line.startPoint.x > line.endPoint.x)
            {
                tmp = line.startPoint.x;

                line.startPoint.setX(line.endPoint.x);
                line.endPoint.setX(tmp);
            }
        }

        _.sortBy(
            this.edgeList_Top,
            [
                function (i: CEdgeLine)
                {
                    return i.startPoint.x;
                }
            ]);

        // #endregion

        // #region Bottom Edges

        for (let line of this.edgeList_Bottom)
        {
            if (line.startPoint.x > line.endPoint.x)
            {
                tmp = line.startPoint.x;

                line.startPoint.setX(line.endPoint.x);
                line.endPoint.setX(tmp);
            }
        }

        _.sortBy(
            this.edgeList_Bottom,
            [
                function (i: CEdgeLine)
                {
                    return i.startPoint.x;
                }
            ]);

        // #endregion

        // #region Left Edges

        for (let line of this.edgeList_Left)
        {
            if (line.startPoint.y > line.endPoint.y)
            {
                tmp = line.startPoint.y;

                line.startPoint.setY(line.endPoint.y);
                line.endPoint.setY(tmp);
            }
        }

        _.sortBy(
            this.edgeList_Left,
            [
                function (i: CEdgeLine)
                {
                    return i.startPoint.y;
                }
            ]);

        // #endregion

        // #region Right Edges

        for (let line of this.edgeList_Right)
        {
            if (line.startPoint.y > line.endPoint.y)
            {
                tmp = line.startPoint.y;

                line.startPoint.setY(line.endPoint.y);
                line.endPoint.setY(tmp);
            }
        }

        _.sortBy(
            this.edgeList_Right,
            [
                function (i: CEdgeLine)
                {
                    return i.startPoint.y;
                }
            ]);

        // #endregion
    }
}

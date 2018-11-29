import * as _ from "lodash";
import * as Enums from "../Enums";

declare const THREE: any;

export class C3DModel
{
    private _exteriorModelList_All = [];
    private _interiorModelList_All = [];
    private _edgeModelList_All = [];

    private _facesRenderedSeparately: boolean = false;

    // #region Model Maps

    private _exteriorModelListMap: Map<Enums.E_Location, any[]>;
    private _interiorModelListMap: Map<Enums.E_Location, any[]>;
    private _edgeModelListMap: Map<Enums.E_Location, any[]>;

    // #endregion

    constructor()
    {
        this._exteriorModelListMap = new Map<Enums.E_Location, any[]>();
        this._exteriorModelListMap.set(Enums.E_Location.Front, []);
        this._exteriorModelListMap.set(Enums.E_Location.Rear, []);
        this._exteriorModelListMap.set(Enums.E_Location.Bottom, []);
        this._exteriorModelListMap.set(Enums.E_Location.Top, []);
        this._exteriorModelListMap.set(Enums.E_Location.Left, []);
        this._exteriorModelListMap.set(Enums.E_Location.Right, []);

        this._interiorModelListMap = new Map<Enums.E_Location, any[]>();
        this._interiorModelListMap.set(Enums.E_Location.Front, []);
        this._interiorModelListMap.set(Enums.E_Location.Rear, []);
        this._interiorModelListMap.set(Enums.E_Location.Bottom, []);
        this._interiorModelListMap.set(Enums.E_Location.Top, []);
        this._interiorModelListMap.set(Enums.E_Location.Left, []);
        this._interiorModelListMap.set(Enums.E_Location.Right, []);

        this._edgeModelListMap = new Map<Enums.E_Location, any[]>();
        this._edgeModelListMap.set(Enums.E_Location.Front, []);
        this._edgeModelListMap.set(Enums.E_Location.Rear, []);
        this._edgeModelListMap.set(Enums.E_Location.Bottom, []);
        this._edgeModelListMap.set(Enums.E_Location.Top, []);
        this._edgeModelListMap.set(Enums.E_Location.Left, []);
        this._edgeModelListMap.set(Enums.E_Location.Right, []);
    }

    public get exteriorModelListMap(): Map<Enums.E_Location, any[]>
    {
        return this._exteriorModelListMap;
    }
    public get interiorModelListMap(): Map<Enums.E_Location, any[]>
    {
        return this._interiorModelListMap;
    }
    public get edgeModelListMap(): Map<Enums.E_Location, any[]>
    {
        return this._edgeModelListMap;
    }

    public get exteriorModelList_All(): any[]
    {
        let _this = this;

        if (this._facesRenderedSeparately === true &&
            this._exteriorModelList_All.length === 0)
        {
            this._exteriorModelListMap.forEach(
                function (value: any[], key: Enums.E_Location, map)
                {
                    _.forEach(
                        value,
                        function (i)
                        {
                            _this._exteriorModelList_All.push(i);
                        });
                });
        }

        return this._exteriorModelList_All;
    }
    public get interiorModelList_All(): any[]
    {
        let _this = this;

        if (this._facesRenderedSeparately === true &&
            this._interiorModelList_All.length === 0)
        {
            this._interiorModelListMap.forEach(
                function (value: any[], key: Enums.E_Location, map)
                {
                    _.forEach(
                        value,
                        function (i)
                        {
                            _this._interiorModelList_All.push(i);
                        });
                });
        }

        return this._interiorModelList_All;
    }
    public get edgeModelList_All(): any[]
    {
        let _this = this;

        if (this._facesRenderedSeparately === true &&
            this._edgeModelList_All.length === 0)
        {
            this._edgeModelListMap.forEach(
                function (value: any[], key: Enums.E_Location, map)
                {
                    _.forEach(
                        value,
                        function (i)
                        {
                            _this._edgeModelList_All.push(i);
                        });
                });
        }

        return this._edgeModelList_All;
    }

    public get exteriorModelList_Front(): any[]
    {
        return this._exteriorModelListMap.get(Enums.E_Location.Front);
    }
    public get interiorModelList_Front(): any[]
    {
        return this._interiorModelListMap.get(Enums.E_Location.Front);
    }
    public get edgeModelList_Front(): any[]
    {
        return this._edgeModelListMap.get(Enums.E_Location.Front);
    }

    public get exteriorModelList_Rear(): any[]
    {
        return this._exteriorModelListMap.get(Enums.E_Location.Rear);
    }
    public get interiorModelList_Rear(): any[]
    {
        return this._interiorModelListMap.get(Enums.E_Location.Rear);
    }
    public get edgeModelList_Rear(): any[]
    {
        return this._edgeModelListMap.get(Enums.E_Location.Rear);
    }

    public get exteriorModelList_Bottom(): any[]
    {
        return this._exteriorModelListMap.get(Enums.E_Location.Bottom);
    }
    public get interiorModelList_Bottom(): any[]
    {
        return this._interiorModelListMap.get(Enums.E_Location.Bottom);
    }
    public get edgeModelList_Bottom(): any[]
    {
        return this._edgeModelListMap.get(Enums.E_Location.Bottom);
    }

    public get exteriorModelList_Top(): any[]
    {
        return this._exteriorModelListMap.get(Enums.E_Location.Top);
    }
    public get interiorModelList_Top(): any[]
    {
        return this._interiorModelListMap.get(Enums.E_Location.Top);
    }
    public get edgeModelList_Top(): any[]
    {
        return this._edgeModelListMap.get(Enums.E_Location.Top);
    }

    public get exteriorModelList_Left(): any[]
    {
        return this._exteriorModelListMap.get(Enums.E_Location.Left);
    }
    public get interiorModelList_Left(): any[]
    {
        return this._interiorModelListMap.get(Enums.E_Location.Left);
    }
    public get edgeModelList_Left(): any[]
    {
        return this._edgeModelListMap.get(Enums.E_Location.Left);
    }

    public get exteriorModelList_Right(): any[]
    {
        return this._exteriorModelListMap.get(Enums.E_Location.Right);
    }
    public get interiorModelList_Right(): any[]
    {
        return this._interiorModelListMap.get(Enums.E_Location.Right);
    }
    public get edgeModelList_Right(): any[]
    {
        return this._edgeModelListMap.get(Enums.E_Location.Right);
    }

    public get facesRenderedSeparately(): boolean
    {
        return this._facesRenderedSeparately;
    }
    public set facesRenderedSeparately(value: boolean)
    {
        this._facesRenderedSeparately = value;
    }
}

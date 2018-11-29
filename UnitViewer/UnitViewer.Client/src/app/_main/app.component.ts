import { Component } from "@angular/core";
import { OnInit } from "@angular/core";
import { Inject } from "@angular/core";
import { ViewChild } from "@angular/core";
import { ElementRef } from "@angular/core";

import * as EnumsAHU from "@jci-ahu/data.ahu.enums";
import * as EnumsCoil from "@jci-ahu/data.coil.enums";
import * as EnumsCommon from "@jci-ahu/data.common.enums";

import * as DescriptorsShared from "@jci-ahu/data.shared.descriptors.interfaces";
import * as DescriptorStoreCommon from "@jci-ahu/data.common.descriptor-store.interfaces"

import * as ModelAHU from "@jci-ahu/data.ahu.model.interfaces";

import * as PolygonGenerator from "@jci-ahu/ui.shared.polygon-generator"

import { CUtility } from "@jci-ahu/shared.utility";
import { Guid } from "@jci-ahu/shared.guid";

import { GetUnitService } from "../services/get-unit.service";
import { ForgeViewerComponent } from "../forge-viewer/forge-viewer.component";
import { CConverters } from "./CConverters";

declare const THREE: any;

@Component
({
    selector: "app-root",
    templateUrl: "./app.component.html",
    styleUrls:
    [
        "./app.component.css"
    ]
})
export class AppComponent implements OnInit
{
    constructor(
        private _getUnitService: GetUnitService,
        @Inject(DescriptorsShared.TOKEN_IDescriptorSourceConnector) private _connector: DescriptorsShared.IDescriptorSourceConnector)
    {
    }

    private UNIT_CONVERSION: number = 25.4;

    @ViewChild("viewer")
    _viewer: ForgeViewerComponent = null;
       
    _ready: boolean = false;
    _modelLoaded: boolean = false;
    
    _orderNumber: string = "";
    _showContainer: boolean = true;
    _unit: ModelAHU.Configuration.Types.IUnit = null;

    public get showContainer(): boolean
    {
        return this._showContainer;
    }
    public set showContainer(value: boolean)
    {
        this._showContainer = value;
        if (this._modelLoaded)
        {
            if (this._showContainer)
            {
                this._viewer.showModel(1);
            }
            else
            {
                this._viewer.hideModel(1);
            }
        }
    }

    private _exteriorMesh_Front = null;
    private _exteriorMesh_Rear = null;
    private _exteriorMesh_Bottom = null;
    private _exteriorMesh_Top = null;
    private _exteriorMesh_Left = null;
    private _exteriorMesh_Right = null;

    private _interiorMesh_Front = null;
    private _interiorMesh_Rear = null;
    private _interiorMesh_Bottom = null;
    private _interiorMesh_Top = null;
    private _interiorMesh_Left = null;
    private _interiorMesh_Right = null;

    private _edgeMesh = null;

    // #region Rest Materials

    private _exteriorMaterial_Rest_Front = null;
    private _exteriorMaterial_Rest_Rear = null;
    private _exteriorMaterial_Rest_Bottom = null;
    private _exteriorMaterial_Rest_Top = null;
    private _exteriorMaterial_Rest_Left = null;
    private _exteriorMaterial_Rest_Right = null;

    private _interiorMaterial_Rest_Front = null;
    private _interiorMaterial_Rest_Rear = null;
    private _interiorMaterial_Rest_Bottom = null;
    private _interiorMaterial_Rest_Top = null;
    private _interiorMaterial_Rest_Left = null;
    private _interiorMaterial_Rest_Right = null;

    private _edgeMaterial_Rest = null;

    // #endregion

    public get isEnabled_LoadUnit(): boolean
    {
        if (CUtility.isNullOrWhiteSpace(this._orderNumber) ||
            this._ready === false)
        {
            return false;   
        }

        return true;
    }
    public get isEnabled_ShowContainer(): boolean
    {
        return (this._ready && this._modelLoaded);
    }

    ngOnInit()
    {
        this._connector.init(
            [
                "AHU",
                "Coil",
                "Common",
                "ISG"
            ],
            "en-US").then(() => this.afterInit());            
    }

    private async afterInit()
    {
        this._ready = true;
    }

    private loadUnitStart()
    {
        this._ready = false;

        this._viewer.loadURN(
            "urn:dXJuOmFkc2sub2JqZWN0czpvcy5vYmplY3Q6amNpX3Rlc3QzL2VtcHR5X21vZGVsLmlwdA",
            () => this.loadUnit());
    }
    private async loadUnit()
    {
        this._unit = await this._getUnitService.GetCurrentUnit(this._orderNumber);
        this.renderUnit();
        this._ready = true;
    }

    private renderUnit()
    {
        this._unit.segmentList.forEach(
            i =>
            {
                this.renderSegment(i);
            });

        if (this._showContainer)
        {
            this._viewer.showModel(1);
        }
        else
        {
            this._viewer.hideModel(1);
        }

        this._modelLoaded = true;
    }
    private renderSegment(segment: ModelAHU.Configuration.Segments.ISegment)
    {
        let generator: PolygonGenerator.HollowModel.CMOM3DGenerator_HollowModel;
        let options: PolygonGenerator.HollowModel.C3DRenderOptions_HollowModel;
        let model: PolygonGenerator.Types.C3DModel;
        let openingList: ModelAHU.Configuration.Types.IOpening[];

        generator = new PolygonGenerator.HollowModel.CMOM3DGenerator_HollowModel();

        options = new PolygonGenerator.HollowModel.C3DRenderOptions_HollowModel();

        options.renderFacesSeparately = true;
        options.renderBorder = true;
        options.borderDim = 0.125 * this.UNIT_CONVERSION;

        options.geometry = new PolygonGenerator.Types.C3DGeometry(
            segment.geometry.x * this.UNIT_CONVERSION,
            segment.geometry.y * this.UNIT_CONVERSION,
            segment.geometry.z * this.UNIT_CONVERSION,
            segment.geometry.xLength * this.UNIT_CONVERSION,
            segment.geometry.yLength * this.UNIT_CONVERSION,
            segment.geometry.zLength * this.UNIT_CONVERSION);

        // #region Wall Thicknesses

        options.wallThickness_Front = segment.wallThicknessActualValue_Front() * this.UNIT_CONVERSION;
        options.wallThickness_Rear = segment.wallThicknessActualValue_Rear() * this.UNIT_CONVERSION;
        options.wallThickness_Bottom = segment.wallThicknessActualValue_Bottom() * this.UNIT_CONVERSION;
        options.wallThickness_Top = segment.wallThicknessActualValue_Top() * this.UNIT_CONVERSION;
        options.wallThickness_Left = segment.wallThicknessActualValue_Left() * this.UNIT_CONVERSION;
        options.wallThickness_Right = segment.wallThicknessActualValue_Right() * this.UNIT_CONVERSION;

        // #endregion

        openingList = segment.getOpeningList(true, false);

        openingList.forEach(
            i =>
            {
                let opening: PolygonGenerator.Types.C3DOpening;

                opening = new PolygonGenerator.Types.C3DOpening();

                opening.geometry.x = i.geometry.x * this.UNIT_CONVERSION;
                opening.geometry.y = i.geometry.y * this.UNIT_CONVERSION;
                opening.geometry.z = i.geometry.z * this.UNIT_CONVERSION;
                opening.geometry.xlength = i.geometry.xLength * this.UNIT_CONVERSION;
                opening.geometry.ylength = i.geometry.yLength * this.UNIT_CONVERSION;
                opening.geometry.zlength = i.geometry.zLength * this.UNIT_CONVERSION;

                opening.face = CConverters.get3DLocation(i.unitSide);
                opening.shape = CConverters.get3DOpeningShape(i.openingShape);

                options.openingList.push(opening);
            });

        model = generator.generateModel(options);                

        // #region Front Side

        this._exteriorMesh_Front = [];

        model.exteriorModelList_Front.forEach(
            i =>
            {
                this._exteriorMesh_Front.push(i);
            });

        this._exteriorMaterial_Rest_Front = new THREE.MeshBasicMaterial(
            {
                color: "blue",
            });

        // #endregion

        // #region Rear Side

        this._exteriorMesh_Rear = [];

        model.exteriorModelList_Rear.forEach(
            i =>
            {
                this._exteriorMesh_Rear.push(i);
            });

        this._exteriorMaterial_Rest_Rear = new THREE.MeshBasicMaterial(
            {
                color: "blue",
            });

        // #endregion

        // #region Bottom Side

        this._exteriorMesh_Bottom = [];

        model.exteriorModelList_Bottom.forEach(
            i =>
            {
                this._exteriorMesh_Bottom.push(i);
            });

        this._exteriorMaterial_Rest_Bottom = new THREE.MeshBasicMaterial(
            {
                color: "blue",
            });
            
        // #endregion

        // #region Top Side

        this._exteriorMesh_Top = [];

        model.exteriorModelList_Top.forEach(
            i =>
            {
                this._exteriorMesh_Top.push(i);
            });
           
        this._exteriorMaterial_Rest_Top = new THREE.MeshBasicMaterial(
            {
                color: "blue",
            });

        // #endregion

        // #region Left Side

        this._exteriorMesh_Left = [];

        model.exteriorModelList_Left.forEach(
            i =>
            {
                this._exteriorMesh_Left.push(i);
            });

        this._exteriorMaterial_Rest_Left = new THREE.MeshBasicMaterial(
            {
                color: "blue",
            });
            

        // #endregion

        // #region Right Side

        this._exteriorMesh_Right = [];

        model.exteriorModelList_Right.forEach(
            i =>
            {
                this._exteriorMesh_Right.push(i);
            });

        this._exteriorMaterial_Rest_Right = new THREE.MeshBasicMaterial(
            {
                color: "blue",
            });

        // #endregion

        // #region Edge Meshes

        this._edgeMesh = [];

        model.edgeModelList_All.forEach(
            i =>
            {
                this._edgeMesh.push(i);
            });
        
        this._edgeMaterial_Rest = new THREE.MeshBasicMaterial(
            {
                color: "black"
            });

        // #endregion

        // #region Interior Meshes

        // #region Rear Side

        this._interiorMesh_Rear = [];

        model.interiorModelList_Rear.forEach(
            i =>
            {
                this._interiorMesh_Rear.push(i);
            });

        this._interiorMaterial_Rest_Rear = new THREE.MeshBasicMaterial(
            {
                color: "silver"
            });
        
        // #endregion

        // #region Front Side

        this._interiorMesh_Front = [];

        model.interiorModelList_Front.forEach(
            i =>
            {
                this._interiorMesh_Front.push(i);
            });

        this._interiorMaterial_Rest_Front = new THREE.MeshBasicMaterial(
            {
                color: "silver"
            });
        
        // #endregion

        // #region Bottom Side

        this._interiorMesh_Bottom = [];

        model.interiorModelList_Bottom.forEach(
            i =>
            {
                this._interiorMesh_Bottom.push(i);
            });

        this._interiorMaterial_Rest_Bottom = new THREE.MeshBasicMaterial(
            {
                color: "silver"
            });

        // #endregion

        // #region Top Side

        this._interiorMesh_Top = [];

        model.interiorModelList_Top.forEach(
            i =>
            {
                this._interiorMesh_Top.push(i);
            });

        this._interiorMaterial_Rest_Top = new THREE.MeshBasicMaterial(
            {
                color: "silver"
            });

        // #endregion

        // #region Left Side

        this._interiorMesh_Left = [];

        model.interiorModelList_Left.forEach(
            i =>
            {
                this._interiorMesh_Left.push(i);
            });

        this._interiorMaterial_Rest_Left = new THREE.MeshBasicMaterial(
            {
                color: "silver"
            });

        // #endregion

        // #region Right Side

        this._interiorMesh_Right = [];

        model.interiorModelList_Right.forEach(
            i =>
            {
                this._interiorMesh_Right.push(i);
            });

        this._interiorMaterial_Rest_Right = new THREE.MeshBasicMaterial(
            {
                color: "silver"
            });

        // #endregion

        // #endregion

        // #region Add Exterior Meshes

        this._exteriorMesh_Front.forEach(
            i =>
            {
                i.material = this._exteriorMaterial_Rest_Front;
                this._viewer.addMesh(i);
            });

        this._exteriorMesh_Rear.forEach(
            i =>
            {
                i.material = this._exteriorMaterial_Rest_Rear;
                this._viewer.addMesh(i);
            });

        this._exteriorMesh_Bottom.forEach(
            i =>
            {
                i.material = this._exteriorMaterial_Rest_Bottom;
                this._viewer.addMesh(i);
            });

        this._exteriorMesh_Top.forEach(
            i =>
            {
                i.material = this._exteriorMaterial_Rest_Top;
                this._viewer.addMesh(i);
            });

        this._exteriorMesh_Left.forEach(
            i =>
            {
                i.material = this._exteriorMaterial_Rest_Left;
                this._viewer.addMesh(i);
            });

        this._exteriorMesh_Right.forEach(
            i =>
            {
                i.material = this._exteriorMaterial_Rest_Right;
                this._viewer.addMesh(i);
            });

        // #endregion

        // #region Add Interior Meshes

        this._interiorMesh_Front.forEach(
            i =>
            {
                i.material = this._interiorMaterial_Rest_Front;
                this._viewer.addMesh(i);
            });

        this._interiorMesh_Rear.forEach(
            i =>
            {
                i.material = this._interiorMaterial_Rest_Rear;
                this._viewer.addMesh(i);
            });

        this._interiorMesh_Bottom.forEach(
            i =>
            {
                i.material = this._interiorMaterial_Rest_Bottom;
                this._viewer.addMesh(i);
            });

        this._interiorMesh_Top.forEach(
            i =>
            {
                i.material = this._interiorMaterial_Rest_Top;
                this._viewer.addMesh(i);
            });

        this._interiorMesh_Left.forEach(
            i =>
            {
                i.material = this._interiorMaterial_Rest_Left;
                this._viewer.addMesh(i);
            });

        this._interiorMesh_Right.forEach(
            i =>
            {
                i.material = this._interiorMaterial_Rest_Right;
                this._viewer.addMesh(i);
            });

        // #endregion

        this._edgeMesh.forEach(
            i =>
            {
                i.material = this._edgeMaterial_Rest;
                this._viewer.addMesh(i);
            });
    }

    loadUnit_OnClick(evt: Event)
    {
        this._ready = false;
        this.loadUnitStart();
    }
}


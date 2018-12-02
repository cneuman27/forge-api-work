import { Component } from "@angular/core";
import { AfterViewInit } from "@angular/core"
import { OnDestroy } from "@angular/core"
import { ViewChild } from "@angular/core";
import { ElementRef } from "@angular/core";

import { ForgeAPIService } from "../services/forge-api.service";
import { CAuthenticationToken } from "../types/CAuthenticationToken";

declare const Autodesk: any;
declare const THREE: any;

@Component
    ({
        selector: "forge-viewer",
        templateUrl: "./forge-viewer.component.html",
        styleUrls:
            [
                "./forge-viewer.component.css"
            ]
    })
export class ForgeViewerComponent implements AfterViewInit, OnDestroy
{
    constructor(
        private _forgeAPIService: ForgeAPIService)
    {
    }

    @ViewChild("viewerContainer")
    _viewerContainer: ElementRef = null;
    _viewer: any = null;
    
    ngAfterViewInit()
    {
        this.launchViewer();
    }
    ngOnDestroy()
    {
        if (this._viewer &&
            this._viewer.running)
        {
            this._viewer.tearDown();
            this._viewer.finish();
            this._viewer = null;
        }
    }

    public loadURN(
        urn: string,
        callback)
    {
        this._viewer.tearDown();
        //this._viewer.start();

        Autodesk.Viewing.Document.load(
            urn,
            (doc) =>
            {
                const geometryItems = Autodesk.Viewing.Document.getSubItemsWithProperties(
                    doc.getRootItem(),
                    {
                        type: 'geometry'
                    },
                    true);

                if (geometryItems.length === 0)
                {
                    return;
                }

                this._viewer.load(
                    doc.getViewablePath(geometryItems[0]),
                    null,
                    () =>
                    {
                        if (callback) callback();
                    });
            },
            errorMsg => console.error);
    }

    public addMesh(mesh)
    {
        this._viewer.impl.scene.add(mesh);
        this._viewer.impl.sceneUpdated(true);
    }
    public hideModel(modelID: number)
    {
        this._viewer.hideModel(modelID);
    }
    public showModel(modelID: number)
    {
        this._viewer.showModel(modelID);
    }

    private launchViewer()
    {
        if (this._viewer)
        {
            return;
        }

        const options =
        {
            env: 'AutodeskProduction',
            getAccessToken: (onSuccess) => { this.getAccessToken(onSuccess) },
        };

        // With GUI
        this._viewer = new Autodesk.Viewing.Private.GuiViewer3D(
            this._viewerContainer.nativeElement,
            {})

        // Headless (No GUI)
        //this._viewer = new Autodesk.Viewing.Viewer3D(
        //    this._viewerContainer.nativeElement,
        //    {}); 

        Autodesk.Viewing.Initializer(
            options,
            () =>
            {
                this._viewer.initialize();
            });
    }
    private async getAccessToken(onSuccess: any)
    {
        let token: CAuthenticationToken;

        token = await this._forgeAPIService.Authenticate();

        onSuccess(
            token.accessToken,
            token.expiresIn);
    }

}
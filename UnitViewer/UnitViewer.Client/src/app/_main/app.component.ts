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
        @Inject(DescriptorsShared.TOKEN_IDescriptorSourceConnector) private _connector: DescriptorsShared.IDescriptorSourceConnector)
    {
    }
    
    _ready: boolean = false;

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
}


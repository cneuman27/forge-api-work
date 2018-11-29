import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { HttpResponse } from "@angular/common/http";
import { HttpErrorResponse } from "@angular/common/http";

import { catchError } from "rxjs/operators";
import { of, Observable, empty } from "rxjs";

import * as _ from "lodash";
import * as DescriptorsShared from "@jci-ahu/data.shared.descriptors";
import * as DescriptorsSharedInterfaces from "@jci-ahu/data.shared.descriptors.interfaces";

import { Guid } from "@jci-ahu/shared.guid";
import { ConfigurationService } from "./configuration.service";

@Injectable
    ({
        providedIn: "root"
    })
export class DescriptorStoreConnectorService implements DescriptorsSharedInterfaces.IDescriptorSourceConnector
{
    constructor(
        private _http: HttpClient,
        private _config: ConfigurationService)
    {
    }

    private _descriptorData: DescriptorsSharedInterfaces.IDescriptorData[] = [];
    private _defaultLanguage: string = "";

    public async init(
        libraryList: string[],
        language: string = DescriptorsSharedInterfaces.Constants.DEFAULT_LANGUAGE): Promise<void[]>
    {
        let list: Promise<void>[] = [];

        this._defaultLanguage = language;

        for (let library of libraryList)
        {
            list.push(this._load(library, language));
        }

        return Promise.all(list);
    }
    
    public getDescriptorDataList(options: DescriptorsSharedInterfaces.IDescriptorSourceConnector_Options): DescriptorsSharedInterfaces.IDescriptorData[]
    {
        let list: DescriptorsSharedInterfaces.IDescriptorData[];

        list = this._descriptorData.filter(
            i => i.library === options.library &&
                i.namespace === options.namespace &&
                i.typeName === options.typeName);
  
        return list.sort(
            (i1, i2) =>
            {
                if (i1.namespace > i2.namespace) return 1;
                if (i1.namespace < i2.namespace) return -1;

                if (i1.typeName < i2.typeName) return 1;
                if (i1.typeName > i2.typeName) return -1;

                return 0;
            });
    }

    public get defaultLanguage(): string
    {
        return this._defaultLanguage;
    }

    private async _load(
        library: string,
        language: string): Promise<void>
    {
        _.remove(
            this._descriptorData,
            (i: DescriptorsSharedInterfaces.IDescriptorData) =>
            {
                return i.library === library;
            });
        
        let url: string;
        let res: HttpResponse<any>;

        url = this._config.APIURI_GetDescriptorData;
        url = url.replace("{language}", language);
        url = url.replace("{library}", library);

        res = await this._http.get(
            url,
            {
                observe: "response",
                headers:
                    {
                        "Content-Type": "application/json",
                        "Accept": "application/json"
                    }
            })
            .pipe(catchError((err: HttpErrorResponse) =>
            {
                console.error(`[${err.status}]`);
                console.error(err.error);

                return empty();
            }))
            .toPromise<HttpResponse<any>>();

        if (res.ok)
        {
            for (let tmp of res.body)
            {
                let descriptorData: DescriptorsShared.CDescriptorData;

                descriptorData = new DescriptorsShared.CDescriptorData();

                descriptorData.id = new Guid(tmp["ID"] as string);
                descriptorData.library = tmp["Library"] as string;
                descriptorData.namespace = tmp["Namespace"] as string;
                descriptorData.typeName = tmp["TypeName"] as string;
                descriptorData.enumeration = tmp["Enumeration"] as string;
                descriptorData.xsdEnumeration = tmp["XSDEnumeration"] as string;
                descriptorData.isActive = tmp["IsActive"];
                descriptorData.isDefault = tmp["IsDefault"];

                for (let tmp2 of tmp["MetaDataList"])
                {
                    let descriptorMetaData: DescriptorsShared.CDescriptorData_MetaData;

                    descriptorMetaData = new DescriptorsShared.CDescriptorData_MetaData();

                    descriptorMetaData.propertyName = (tmp2["PropertyName"] as string);
                    descriptorMetaData.propertyValue = (tmp2["PropertyValue"] as string);

                    descriptorData.metaDataList.push(descriptorMetaData);
                }

                this._descriptorData.push(descriptorData);
            }
        }
    }
}
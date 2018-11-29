import { Injectable } from "@angular/core";
import { Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { HttpResponse } from "@angular/common/http";
import { HttpErrorResponse } from "@angular/common/http";

import { catchError } from "rxjs/operators";
import { of, Observable, empty } from "rxjs";

import * as ModelInterfacesAHU from "@jci-ahu/data.ahu.model.interfaces";
import * as ModelInterfacesShared from "@jci-ahu/data.shared.model.interfaces";
import * as ModelShared from "@jci-ahu/data.shared.model";

import { ConfigurationService } from "./configuration.service";
import { TOKEN_IModelFactoryAHU } from "@jci-ahu/data.ahu.model-factory.interfaces";
import { IModelFactoryAHU } from "@jci-ahu/data.ahu.model-factory.interfaces";
import { Guid } from "@jci-ahu/shared.guid";

@Injectable
    ({
        providedIn: "root"
    })
export class GetUnitService
{
    constructor(
        private _configuration: ConfigurationService,
        @Inject(TOKEN_IModelFactoryAHU) private _modelFactory: IModelFactoryAHU,
        private _http: HttpClient)
    {
    }

    public async GetCurrentUnit(
        orderNumber: string,
        format: ModelInterfacesShared.E_SerializationFormat = ModelInterfacesShared.E_SerializationFormat.JSONMinified): Promise<ModelInterfacesAHU.Configuration.Types.IUnit>
    {
        let url: string;
        let res: HttpResponse<any>;
        let ctx: ModelShared.CSerializationContext;
        let unit: ModelInterfacesAHU.Configuration.Types.IUnit;

        url = this._configuration.APIURI_GetCurrentUnit.trim();
        url = url.replace("{orderNo}", orderNumber);

        ctx = new ModelShared.CSerializationContext(format);

        res = await this._http.get(
            url,
            {
                observe: "response",
                headers:
                    {
                        "Content-Type": ctx.mediaType,
                        "Accept": ctx.mediaType
                    }
            })
            .toPromise<HttpResponse<any>>();

        if (res)
        {
            unit = this._modelFactory.Configuration.Types.Unit.create();
            unit.deserialize(
                ctx,
                res.body);

            return unit;
        }

        return null;
    }
}
import { Injectable } from "@angular/core";
import { Inject } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { HttpResponse } from "@angular/common/http";
import { HttpErrorResponse } from "@angular/common/http";
import { HttpParams } from "@angular/common/http";

import { catchError } from "rxjs/operators";
import { of, Observable, empty } from "rxjs";

import { ConfigurationService } from "./configuration.service";
import { CAuthenticationToken } from "../types/CAuthenticationToken";

@Injectable
    ({
        providedIn: "root"
    })
export class ForgeAPIService
{
    constructor(
        private _configuration: ConfigurationService,
        private _http: HttpClient)
    {
    }

    public async Authenticate(): Promise<CAuthenticationToken>
    {
        let url: string;
        let res: HttpResponse<any>;

        url = this._configuration.APIURI_ForgeAuthenticate.trim();

        res = await this._http.get(
            url,
            {
                observe: "response"
            })
            .pipe(catchError((err: HttpErrorResponse) =>
            {
                console.error(`[${err.status}]`);
                console.error(err.error);

                return empty();
            }))
            .toPromise<HttpResponse<any>>();

        if (res)
        {
            let token: CAuthenticationToken;

            token = new CAuthenticationToken();
            token.accessToken = res.body["access_token"];
            token.expiresIn = res.body["expires_in"];

            return token;
        }

        return null;
    }
    public async AuthenticateLocal(): Promise<CAuthenticationToken>
    {
        let url: string;
        let res: HttpResponse<any>;
        let params: HttpParams;

        params = new HttpParams();

        params.set("client_id", this._configuration.clientID);
        params.set("client_secret", this._configuration.clientSecret);
        params.set("grant_type", "client_credentials");
        params.set("scope", "data:read viewables:read");

        url = this._configuration.APIURI_ForgeAuthenticateLocal.trim();

        res = await this._http.post(
            url,
            params.toString(),
            {
                observe: "response",
                headers: new HttpHeaders()
                    .set("Content-Type", "application/x-www-form-urlencoded")
            })
            .toPromise<HttpResponse<any>>();

        if (res)
        {
            let token: CAuthenticationToken;

            token = new CAuthenticationToken();
            token.accessToken = res.body["access_token"];
            token.expiresIn = res.body["expires_in"];

            return token;
        }

        return null;
    }
}
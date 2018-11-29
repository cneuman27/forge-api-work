import { Injectable } from "@angular/core";
import { Inject } from "@angular/core";

@Injectable
({
    providedIn: "root"
})
export class ConfigurationService 
{
    constructor()
    {
    }

    private _apiuri_GetCurrentUnit: string = "http://localhost:5000/api/unit/{orderNo}/current";
    private _apiuri_ForgeAuthenticate: string = "http://localhost:5000/api/forge/authenticate";
    private _apiuri_GetDescriptorData: string = "http://localhost:5000/api/descriptorstore/{language}/{library}";

    private _apiuri_ForgeAuthenticateLocal: string = "https://developer.api.autodesk.com/authentication/v1/authenticate";

    private _clientID: string = "S0Ur97F9zNP91dCjS7WUXJc5FnHHDT6t";
    private _clientSecret: string = "TBQwafd2sbvy60K6";

    public get APIURI_GetCurrentUnit(): string
    {
        return this._apiuri_GetCurrentUnit;
    }
    public get APIURI_ForgeAuthenticate(): string
    {
        return this._apiuri_ForgeAuthenticate;
    }
    public get APIURI_GetDescriptorData(): string
    {
        return this._apiuri_GetDescriptorData;
    }

    public get APIURI_ForgeAuthenticateLocal(): string
    {
        return this._apiuri_ForgeAuthenticateLocal;
    }

    public get clientID(): string
    {
        return this._clientID;
    }
    public get clientSecret(): string
    {
        return this._clientSecret;
    }
}
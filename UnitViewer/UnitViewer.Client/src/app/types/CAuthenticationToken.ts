import { retry } from "rxjs/operators";

export class CAuthenticationToken
{
    private _accessToken: string = "";
    private _expiresIn: number = 0;

    public get accessToken(): string
    {
        return this._accessToken;
    }
    public set accessToken(value: string)
    {
        this._accessToken = value;
    }

    public get expiresIn(): number
    {
        return this._expiresIn;
    }
    public set expiresIn(value: number)
    {
        this._expiresIn = value;
    }
}
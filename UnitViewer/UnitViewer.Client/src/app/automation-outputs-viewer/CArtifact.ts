export class CArtifact
{
    private _name: string = "";
    private _urn: string = "";
    private _urnEncoded: string = "";

    public get name(): string
    {
        return this._name;
    }
    public set name(value: string)
    {
        this._name = value;
    }

    public get urn(): string
    {
        return this._urn;
    }
    public set urn(value: string)
    {
        this._urn = value;
    }

    public get urnEncoded(): string
    {
        return this._urnEncoded;
    }
    public set urnEncoded(value: string)
    {
        this.urnEncoded = value; 
    }
}
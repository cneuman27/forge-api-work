export class CFileData
{
    private _name: string = "";
    private _data: string = null;

    public get name(): string
    {
        return this._name;
    }
    public set name(value: string)
    {
        this._name = value;
    }

    public get data(): string
    {
        return this._data;
    }
    public set data(value: string)
    {
        this._data = value;
    }
}
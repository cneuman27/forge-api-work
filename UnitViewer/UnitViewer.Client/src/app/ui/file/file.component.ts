import { Component } from "@angular/core";
import { Input, Output } from "@angular/core";
import { forwardRef } from "@angular/core";
import { EventEmitter } from "@angular/core";
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from "@angular/forms";

import { CFileData } from "./CFileData";

@Component
    ({
        selector: "ui-file",
        templateUrl: "./file.component.html",
        styleUrls:
            [
                "./file.component.css"
            ],
        providers:
            [
                {
                    provide: NG_VALUE_ACCESSOR,
                    useExisting: forwardRef(() => FileComponent),
                    multi: true
                }
            ]
    })
export class FileComponent implements ControlValueAccessor
{
    @Input("enabled")
    isEnabled: boolean = true;

    @Input("filelabel")
    fileLabel: string = "File:";

    @Input("showdownload")
    showDownload: boolean = true;

    @Input("extensionfilter")
    extensionFilter: string = "";

    @Output("fileselected")
    changed: EventEmitter<CFileData> = new EventEmitter<CFileData>();

    @Output("download")
    download: EventEmitter<void> = new EventEmitter<void>();

    public inProcess = true;

    // #region fileName

    private _fileName: string = "";

    public get fileName(): string
    {
        return this._fileName;
    }
    public set fileName(value: string)
    {
        this._fileName = value;
    }

    // #endregion
    
    onFileSelected(evt: Event)
    {
        let list: FileList;
        let input: HTMLInputElement;
        let reader: FileReader;

        input = evt.target as HTMLInputElement;

        if (!input) return;
        if (input.files.length === 0) return;
       
        this.fileName = input.files[0].name;

        reader = new FileReader();
        reader.onload = (e: any) =>
        {
            let fileData: CFileData;

            fileData = new CFileData();
            fileData.name = input.files[0].name;
            fileData.data = reader.result;

            this.changed.emit(fileData);
        }
        reader.readAsText(input.files[0]);
    }
    onDownloadClicked(evt: Event)
    {
        this.download.emit();
    }

    // #region ControlValueAccessor Implementation

    propagateChange = (value: any) => { };

    writeValue(value: any)
    {
        if (value !== undefined)
        {
            this.fileName = value + "";
        }
    }

    registerOnChange(fn)
    {
        this.propagateChange = fn;
    }
    registerOnTouched()
    {
    }

    // #endregion
}

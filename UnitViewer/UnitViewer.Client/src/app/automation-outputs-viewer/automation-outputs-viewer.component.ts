import { Component } from "@angular/core";
import { ViewChild } from "@angular/core";

import { CFileData } from "../ui/file/CFileData";
import { CArtifact } from "./CArtifact";
import { ForgeViewerComponent } from "../forge-viewer/forge-viewer.component";


@Component
    ({
        selector: "automation-outputs-viewer",
        templateUrl: "./automation-outputs-viewer.component.html",
        styleUrls:
            [
                "./automation-outputs-viewer.component.css"
            ]
    })
export class AutomationOutputViewerComponent
{
    @ViewChild("viewer")
    _viewer: ForgeViewerComponent = null;

    partNumber: string = "";
    reference: string = "";

    artifactList: CArtifact[] = [];

    private _selectedArtifact: CArtifact;

    public get selectedArtifact(): CArtifact
    {
        return this._selectedArtifact;
    }
    public set selectedArtifact(value: CArtifact)
    {
        this._selectedArtifact = value;

        if (this._selectedArtifact != null)
        {
            this._viewer.loadURN(
                `urn:${this._selectedArtifact.urnEncoded}`,
                null);
        }
    }

    onResultsFileSelected(evt: CFileData)
    {
        let results: any;

        results = JSON.parse(evt.data);

        this.partNumber = results["PartNumber"];
        this.reference = results["Reference"];

        this.artifactList = [];

        for (let tmp of results["ArtifactList"])
        {
            let artifact: CArtifact;

            artifact = new CArtifact();

            artifact.name = tmp["FileName"];
            artifact.urn = tmp["URN"];
            artifact.urnEncoded = tmp["URNEncoded"];

            this.artifactList.push(artifact);
        }

        if (this.artifactList.length > 0)
        {
            this.selectedArtifact = this.artifactList[0];
        }
    }
}

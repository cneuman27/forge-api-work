import { BrowserModule } from "@angular/platform-browser";
import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { HttpClientModule } from "@angular/common/http";
import { FormsModule } from "@angular/forms";

import { AppComponent } from "./_main/app.component";
import { ForgeViewerComponent } from "./forge-viewer/forge-viewer.component";

import { DescriptorStoreConnectorService } from "./services/descriptor-store-connector.service";
import { TOKEN_IDescriptorSourceConnector } from "@jci-ahu/data.shared.descriptors.interfaces";

import { DescriptorStoreAHUModule } from "@jci-ahu/data.ahu.descriptor-store";
import { DescriptorStoreCoilModule } from "@jci-ahu/data.coil.descriptor-store";
import { DescriptorStoreCommonModule } from "@jci-ahu/data.common.descriptor-store";

import { ModelFactoryAHUModule } from "@jci-ahu/data.ahu.model-factory";
import { ModelFactoryCoilModule } from "@jci-ahu/data.coil.model-factory";
import { ModelFactoryCommonModule } from "@jci-ahu/data.common.model-factory";

@NgModule
    ({
        declarations:
            [
                AppComponent,
                ForgeViewerComponent
            ],
        imports:
            [
                BrowserModule,
                CommonModule,
                HttpClientModule,
                FormsModule,
                DescriptorStoreAHUModule,
                DescriptorStoreCoilModule,
                DescriptorStoreCommonModule,
                ModelFactoryAHUModule,
                ModelFactoryCoilModule,
                ModelFactoryCommonModule
            ],
        providers:
            [
                {
                    provide: TOKEN_IDescriptorSourceConnector,
                    useClass: DescriptorStoreConnectorService
                }
            ],
        bootstrap:
            [
                AppComponent
            ]
    })
export class AppModule 
{
}

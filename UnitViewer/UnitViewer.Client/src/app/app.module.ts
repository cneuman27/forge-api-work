import { BrowserModule } from "@angular/platform-browser";
import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { HttpClientModule } from "@angular/common/http";
import { FormsModule } from "@angular/forms";

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from "@fortawesome/fontawesome-svg-core";
import { faDownload } from "@fortawesome/free-solid-svg-icons";
import { faUpload } from "@fortawesome/free-solid-svg-icons";

import { DescriptorStoreAHUModule } from "@jci-ahu/data.ahu.descriptor-store";
import { DescriptorStoreCoilModule } from "@jci-ahu/data.coil.descriptor-store";
import { DescriptorStoreCommonModule } from "@jci-ahu/data.common.descriptor-store";

import { ModelFactoryAHUModule } from "@jci-ahu/data.ahu.model-factory";
import { ModelFactoryCoilModule } from "@jci-ahu/data.coil.model-factory";
import { ModelFactoryCommonModule } from "@jci-ahu/data.common.model-factory";

import { GridLayoutModule } from "@jci-ahu/ui.shared.grid-layout";

import { AppComponent } from "./_main/app.component";
import { AutomationOutputViewerComponent } from "./automation-outputs-viewer/automation-outputs-viewer.component";
import { UnitViewerComponent } from "./unit-viewer/unit-viewer.component";
import { ForgeViewerComponent } from "./forge-viewer/forge-viewer.component";
import { FileComponent } from "./ui/file/file.component";

import { DescriptorStoreConnectorService } from "./services/descriptor-store-connector.service";
import { TOKEN_IDescriptorSourceConnector } from "@jci-ahu/data.shared.descriptors.interfaces";

library.add(faDownload);
library.add(faUpload);

@NgModule
    ({
        declarations:
            [
                AppComponent,
                AutomationOutputViewerComponent,
                UnitViewerComponent,
                ForgeViewerComponent,
                FileComponent
            ],
        imports:
            [
                BrowserModule,
                CommonModule,
                HttpClientModule,
                FormsModule,
                FontAwesomeModule,
                DescriptorStoreAHUModule,
                DescriptorStoreCoilModule,
                DescriptorStoreCommonModule,
                ModelFactoryAHUModule,
                ModelFactoryCoilModule,
                ModelFactoryCommonModule,
                GridLayoutModule
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

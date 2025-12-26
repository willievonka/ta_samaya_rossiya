import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MapPageBaseComponent } from '../../../components/map-page/map.page.base.component';
import { IMapLayerProperties } from '../../../components/map/interfaces/map-layer.interface';
import { PageHeaderComponent } from '../../../components/page-header/page-header.component';
import { MapZoomComponent } from '../../../components/map-zoom/map-zoom.component';
import { MapComponent } from '../../../components/map/map.component';
import { IPageHeaderOptions } from '../../../components/page-header/interfaces/page-header-options.interface';
import { IMapConfig } from '../../../components/map/interfaces/map-config.interface';
import { editMapConfig } from '../../../components/map/configs/edit-map.config';

@Component({
    selector: 'edit-analytics-map-page',
    standalone: true,
    templateUrl: './edit-analytics-map.page.html',
    styleUrl: './styles/edit-analytics-map-page.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        PageHeaderComponent,
        MapComponent,
        MapZoomComponent
    ]
})
export class EditAnalyticsMapPageComponent extends MapPageBaseComponent<IMapLayerProperties>{
    protected override readonly headerOptions: IPageHeaderOptions = {
        isDetached: true,
        adminState: {
            changeRedirect: true,
            showLogoutIcon: false
        }
    };
    protected readonly config: IMapConfig = editMapConfig;
}

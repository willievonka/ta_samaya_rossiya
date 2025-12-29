import { ChangeDetectionStrategy, Component } from '@angular/core';
import { IMapLayerProperties } from '../../../components/map/interfaces/map-layer.interface';
import { PageHeaderComponent } from '../../../components/page-header/page-header.component';
import { MapZoomComponent } from '../../../components/map-zoom/map-zoom.component';
import { MapComponent } from '../../../components/map/map.component';
import { EditAnalyticsMapModalComponent } from './components/edit-analytics-map-modal/edit-analytics-map-modal.component';
import { EditMapPageBaseComponent } from '../../../components/map-page/edit-map.page.base.component';

@Component({
    selector: 'edit-analytics-map-page',
    standalone: true,
    templateUrl: './edit-analytics-map.page.html',
    styleUrl: './styles/edit-analytics-map-page.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        PageHeaderComponent,
        MapComponent,
        MapZoomComponent,
        EditAnalyticsMapModalComponent
    ]
})
export class EditAnalyticsMapPageComponent extends EditMapPageBaseComponent<IMapLayerProperties>{}

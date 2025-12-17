import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MapComponent } from '../../components/map/map.component';
import { IMapLayerProperties } from '../../components/map/interfaces/map-layer.interface';
import { AnalyticsMapModalComponent } from './components/analytics-map-modal/analytics-map-modal.component';
import { PageHeaderComponent } from '../../components/page-header/page-header.component';
import { MapZoomComponent } from '../../components/map-zoom/map-zoom.component';
import { MapInfoComponent } from '../../components/map-info/map-info.component';
import { MapPageBaseComponent } from '../../components/map-page/map.page.base.component';

@Component({
    selector: 'analytics-map-page',
    standalone: true,
    templateUrl: './analytics-map.page.html',
    styleUrl: './styles/analytics-map-page.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        MapComponent,
        MapZoomComponent,
        MapInfoComponent,
        AnalyticsMapModalComponent,
        PageHeaderComponent
    ]
})
export class AnalyticsMapPageComponent extends MapPageBaseComponent<IMapLayerProperties> {
    /**
     * Переключает состояние модалки с переданными properties
     * @param properties
     */
    protected toggleModal(properties: IMapLayerProperties | null): void {
        if (properties) {
            this.activeItem.set(properties);
        } else {
            this.activeItem.set(null);
            this.mapInstance()?.clearRegionSelection();
        }
    }
}

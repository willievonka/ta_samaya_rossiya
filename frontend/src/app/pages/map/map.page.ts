import { ChangeDetectionStrategy, Component, Signal, viewChild } from '@angular/core';
import { MapPageBaseComponent } from '../../components/map-page/map.page.base.component';
import { PageHeaderComponent } from '../../components/page-header/page-header.component';
import { MapComponent } from '../../components/map/map.component';
import { MapZoomComponent } from '../../components/map-zoom/map-zoom.component';
import { MapInfoComponent } from '../../components/map-info/map-info.component';
import { HistoricalLineComponent } from './components/historical-line/historical-line.component';
import { IMapPoint } from '../../components/map/interfaces/map-point.interface';
import { PointInfoModalComponent } from './components/point-info-modal/point-info-modal.component';

@Component({
    selector: 'map-page',
    standalone: true,
    templateUrl: './map.page.html',
    styleUrl: './styles/map-page.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        PageHeaderComponent,
        MapComponent,
        MapZoomComponent,
        MapInfoComponent,
        HistoricalLineComponent,
        PointInfoModalComponent
    ]
})
export class MapPageComponent extends MapPageBaseComponent<IMapPoint> {
    private readonly _historicalLine: Signal<HistoricalLineComponent | undefined> = viewChild(HistoricalLineComponent);

    /**
     * Изменить состояние, активное - если есть данные, иначе неактивное
     * @param data
     */
    protected toggleState(data: IMapPoint | null): void {
        if (data) {
            this.activeItem.set(data);
            this.mapInstance()?.setPointSelection(data);
        } else {
            this.activeItem.set(null);
            this._historicalLine()?.resetActiveItem();
            this.mapInstance()?.clearPointSelection();
        }
    }
}

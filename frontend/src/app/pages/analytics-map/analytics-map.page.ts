import { ChangeDetectionStrategy, Component, inject, Signal } from '@angular/core';
import { MapComponent } from '../../components/map/map.component';
import { MapDataService } from '../../services/map-data.service';
import { IMapLayer } from '../../interfaces/map-layer.interface';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
    selector: 'analytics-map-page',
    standalone: true,
    templateUrl: './analytics-map.page.html',
    styleUrl: './styles/analytics-map.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        MapComponent
    ],
    providers: [
        MapDataService
    ]
})
export class AnalyticsMapPageComponent {
    protected readonly mapDataService: MapDataService = inject(MapDataService);
    protected readonly mapLayers: Signal<IMapLayer[] | undefined> = toSignal(this.mapDataService.getAnalyticsMapData());
}

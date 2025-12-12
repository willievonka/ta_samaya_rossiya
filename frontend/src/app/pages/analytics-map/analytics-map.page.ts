import { ChangeDetectionStrategy, Component, DestroyRef, inject, Signal, signal, viewChild, WritableSignal } from '@angular/core';
import { MapComponent } from '../../components/map/map.component';
import { MapDataService } from '../../services/map-data.service';
import { IMapLayer, IMapLayerProperties } from '../../components/map/interfaces/map-layer.interface';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { tap } from 'rxjs';
import { IMapConfig } from '../../components/map/interfaces/map-config.interface';
import { AnalyticsMapModalComponent } from './components/analytcs-map-modal/analytics-map-modal.component';
import { ActivatedRoute } from '@angular/router';
import { PageHeaderComponent } from '../../components/page-header/page-header.component';

@Component({
    selector: 'analytics-map-page',
    standalone: true,
    templateUrl: './analytics-map.page.html',
    styleUrl: './styles/analytics-map.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        MapComponent,
        AnalyticsMapModalComponent,
        PageHeaderComponent
    ]
})
export class AnalyticsMapPageComponent {
    protected readonly activeLayer: WritableSignal<IMapLayerProperties | null> = signal(null);
    protected readonly mapLayers: WritableSignal<IMapLayer[] | undefined> = signal(undefined);

    protected readonly mapConfig: IMapConfig = {
        options: {
            zoomControl: false,
            attributionControl: false,
            zoomSnap: 0.1
        },
        center: [105, 68.5],
        initZoom: 3.2,
        defaultLayerStyle: {
            fillColor: '#B4B4B4',
            fillOpacity: 1,
            color: '#FFF',
            weight: 1
        }
    };

    private readonly _mapInstance: Signal<MapComponent | undefined> = viewChild(MapComponent);
    private readonly _mapDataService: MapDataService = inject(MapDataService);
    private readonly _destroyRef: DestroyRef = inject(DestroyRef);
    private readonly _route: ActivatedRoute = inject(ActivatedRoute);

    constructor() {
        const mapId: string = this._route.snapshot.queryParamMap.get('id') ?? '';
        this.loadMapLayers(mapId);
    }

    /**
     * Переключает состояние модалки с переданными properties
     * @param properties
     */
    protected toggleModal(properties: IMapLayerProperties | null): void {
        if (properties) {
            this.activeLayer.set(properties);
        } else {
            this.activeLayer.set(null);
            this._mapInstance()?.clearRegionSelection();
        }
    }

    /** Загрузить слои для карты
     * @param mapId
     */
    private loadMapLayers(mapId: string): void {
        this._mapDataService.getAnalyticsMapData(mapId)
            .pipe(
                tap(layers => this.mapLayers.set(layers)),
                takeUntilDestroyed(this._destroyRef)
            )
            .subscribe();
    }
}

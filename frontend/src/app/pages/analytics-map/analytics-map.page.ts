import { ChangeDetectionStrategy, Component, computed, inject, Signal, signal, viewChild, WritableSignal } from '@angular/core';
import { MapComponent } from '../../components/map/map.component';
import { MapDataService } from '../../components/map/services/map-data.service';
import { IMapLayer, IMapLayerProperties } from '../../components/map/interfaces/map-layer.interface';
import { take, tap } from 'rxjs';
import { AnalyticsMapModalComponent } from './components/analytics-map-modal/analytics-map-modal.component';
import { ActivatedRoute } from '@angular/router';
import { PageHeaderComponent } from '../../components/page-header/page-header.component';
import { MapZoomComponent } from '../../components/map-zoom/map-zoom.component';
import { IMapZoomActions } from '../../components/map/interfaces/map-zoom-actions.interface';
import { MapInfoComponent } from '../../components/map-info/map-info.component';

@Component({
    selector: 'analytics-map-page',
    standalone: true,
    templateUrl: './analytics-map.page.html',
    styleUrl: './styles/analytics-map.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        MapComponent,
        MapZoomComponent,
        MapInfoComponent,
        AnalyticsMapModalComponent,
        PageHeaderComponent
    ]
})
export class AnalyticsMapPageComponent {
    protected readonly pageTitle: WritableSignal<string | undefined> = signal(undefined);
    protected readonly infoText: WritableSignal<string | undefined> = signal(undefined);
    protected readonly mapLayers: WritableSignal<IMapLayer[] | undefined> = signal(undefined);
    protected readonly zoomActions: Signal<IMapZoomActions | undefined> = computed(() => this._mapInstance()?.zoomActions());

    protected readonly activeLayer: WritableSignal<IMapLayerProperties | null> = signal(null);

    private readonly _mapInstance: Signal<MapComponent | undefined> = viewChild(MapComponent);
    private readonly _mapDataService: MapDataService = inject(MapDataService);
    private readonly _route: ActivatedRoute = inject(ActivatedRoute);

    constructor() {
        this.initMap();
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

    /** Инициализация карты */
    private initMap(): void {
        const mapId: string = this._route.snapshot.queryParamMap.get('id') ?? '';
        this._mapDataService.getMapData(mapId)
            .pipe(
                tap(data => {
                    this.pageTitle.set(data.pageTitle);
                    this.mapLayers.set(data.layers);
                    this.infoText.set(data.infoText);
                }),
                take(1)
            )
            .subscribe();
    }
}

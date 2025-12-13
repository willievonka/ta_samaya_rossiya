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
    protected readonly pageTitle: WritableSignal<string> = signal('');
    protected readonly mapLayers: WritableSignal<IMapLayer[] | undefined> = signal(undefined);
    protected readonly zoomActions: Signal<IMapZoomActions | null> = computed(() => this._mapInstance()?.zoomActions() ?? null);
    protected readonly infoText: WritableSignal<string | null> = signal(null);

    protected readonly activeLayer: WritableSignal<IMapLayerProperties | null> = signal(null);

    protected readonly pageTitleExample: string = 'Аналитическая карта России';
    protected readonly infoTextExample: string = `Карта России с участниками проекта.
    Ознакомьтесь с регионами, где наша деятельность активно развивается.
    Каждый указанный субъект РФ представлен на карте, позволяя быстро оценить географический охват и найти интересующие вас локации для получения дополнительной информации.`;

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
                tap(layers => this.mapLayers.set(layers)),
                take(1)
            )
            .subscribe();

        this.pageTitle.set(this.pageTitleExample);
        this.infoText.set(this.infoTextExample);
    }
}

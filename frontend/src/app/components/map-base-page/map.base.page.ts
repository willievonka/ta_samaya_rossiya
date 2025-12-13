import { computed, Directive, inject, Signal, signal, viewChild, WritableSignal } from '@angular/core';
import { IMapLayer } from '../map/interfaces/map-layer.interface';
import { IMapZoomActions } from '../map/interfaces/map-zoom-actions.interface';
import { MapComponent } from '../map/map.component';
import { ActivatedRoute } from '@angular/router';
import { MapDataService } from '../map/services/map-data.service';
import { take, tap } from 'rxjs';

@Directive({
    standalone: true
})
export class MapBasePage {
    protected readonly mapInstance: Signal<MapComponent | undefined> = viewChild(MapComponent);

    protected readonly pageTitle: WritableSignal<string | undefined> = signal(undefined);
    protected readonly infoText: WritableSignal<string | undefined> = signal(undefined);
    protected readonly mapLayers: WritableSignal<IMapLayer[] | undefined> = signal(undefined);
    protected readonly zoomActions: Signal<IMapZoomActions | undefined> = computed(() => this.mapInstance()?.zoomActions());

    private readonly _mapDataService: MapDataService = inject(MapDataService);
    private readonly _route: ActivatedRoute = inject(ActivatedRoute);

    constructor() {
        this.init();
    }

    /** Инициализация */
    private init(): void {
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

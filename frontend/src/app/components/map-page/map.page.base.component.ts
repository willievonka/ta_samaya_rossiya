import { computed, Directive, inject, signal, Signal, viewChild, WritableSignal } from '@angular/core';
import { IMapLayer } from '../map/interfaces/map-layer.interface';
import { IMapZoomActions } from '../map/interfaces/map-zoom-actions.interface';
import { MapComponent } from '../map/map.component';
import { ActivatedRoute } from '@angular/router';
import { MapDataService } from '../map/services/map-data.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { IMapModel } from '../map/models/map.model';

@Directive({
    standalone: true
})
export abstract class MapPageBaseComponent<TData> {
    protected readonly activeItem: WritableSignal<TData | null> = signal(null);
    protected readonly mapInstance: Signal<MapComponent | undefined> = viewChild(MapComponent);
    protected readonly zoomActions: Signal<IMapZoomActions | undefined> = computed(() => this.mapInstance()?.zoomActions());

    protected readonly pageTitle: Signal<string | undefined> = computed(() => this._mapData()?.pageTitle);
    protected readonly infoText: Signal<string | undefined> = computed(() => this._mapData()?.infoText);
    protected readonly mapLayers: Signal<IMapLayer[] | undefined> = computed(() => this._mapData()?.layers);
    protected readonly layerWithPointsColor: Signal<string | undefined> = computed(() => this._mapData()?.layerWithPointsColor);
    protected readonly pointColor: Signal<string | undefined> = computed(() => this._mapData()?.pointColor);

    private readonly _mapDataService: MapDataService = inject(MapDataService);
    private readonly _mapId: string = inject(ActivatedRoute).snapshot.queryParamMap.get('id') ?? '';
    private readonly _mapData: Signal<IMapModel | undefined> = toSignal<IMapModel>(
        this._mapDataService.getMapData(this._mapId),
        { initialValue: undefined }
    );
}

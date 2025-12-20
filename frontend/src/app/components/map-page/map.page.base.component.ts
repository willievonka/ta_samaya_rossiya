import { computed, Directive, inject, Injector, signal, Signal, viewChild, WritableSignal } from '@angular/core';
import { IMapZoomActions } from '../map/interfaces/map-zoom-actions.interface';
import { MapComponent } from '../map/map.component';
import { ActivatedRoute } from '@angular/router';
import { MapDataService } from '../map/services/map-data.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { IMapModel } from '../map/models/map.model';
import { distinctUntilChanged, map, switchMap } from 'rxjs';

@Directive({
    standalone: true
})
export abstract class MapPageBaseComponent<TData> {
    protected readonly activeItem: WritableSignal<TData | null> = signal(null);
    protected readonly mapInstance: Signal<MapComponent | undefined> = viewChild(MapComponent);
    protected readonly zoomActions: Signal<IMapZoomActions | undefined> = computed(() => this.mapInstance()?.zoomActions());
    protected readonly model: Signal<IMapModel | undefined> = computed(() => this._mapData());

    private readonly _mapDataService: MapDataService = inject(MapDataService);
    private readonly _route: ActivatedRoute = inject(ActivatedRoute);
    private readonly _injector: Injector = inject(Injector);

    private readonly _mapData: Signal<IMapModel | undefined> = toSignal(
        this._route.queryParamMap
            .pipe(
                map((paramMap) => paramMap.get('id') ?? ''),
                distinctUntilChanged(),
                switchMap((id) => this._mapDataService.getMapData(id))
            ),
        { initialValue: undefined, injector: this._injector }
    );
}
